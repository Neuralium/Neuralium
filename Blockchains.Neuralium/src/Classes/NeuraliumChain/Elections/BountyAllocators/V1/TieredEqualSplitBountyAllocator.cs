using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Serilog;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Elections.BountyAllocators.V1 {
	public class TieredEqualSplitBountyAllocator : BountyAllocator {

		public const decimal FULL_SHARE_PEER_ALLOCATION = 1; // 100%
		public const decimal SIMPLE_SHARE_PEER_ALLOCATION = 0.66M; // 100%
		public const decimal NO_SHARE_PEER_ALLOCATION = 0.33M; // 33%

		public TieredEqualSplitBountyAllocator(IBountyAllocationMethod bountyAllocationMethod) : base(bountyAllocationMethod) {
		}

		public TieredEqualSplitBountyAllocator() {
		}

		public override void AllocateBounty(INeuraliumFinalElectionResults result, INeuraliumElectionContext electionContext, Dictionary<AccountId, (Enums.MiningTiers electedTier, AccountId delegateAccountId)> electedPeers, Dictionary<AccountId, (decimal delegateBountyShare, decimal InfrastructureServiceFees)> delegateAllocations) {

			int electedCount = result.ElectedCandidates.Count;

			if(electedCount == 0) {
				// if there are no elected, then nothing is allocated; nothing at all and the bounty is lost to everyone
				return;
			}

			var tierEffectiveServiceFees = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, 0M);
			
			if(electionContext.MaintenanceServiceFeesEnabled) {

				foreach(var tier in tierEffectiveServiceFees.ToArray()) {
					if(electionContext.MiningTierBounties.ContainsKey(tier.Key)) {
						tierEffectiveServiceFees[tier.Key] = electionContext.MiningTierBounties[tier.Key] * electionContext.MaintenanceServiceFees.Percentage;
					}
				}
			}

			var tierEffectiveBounties = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, 0M);
			
			foreach(var tier in tierEffectiveBounties.ToArray()) {
				if(electionContext.MiningTierBounties.ContainsKey(tier.Key)) {
					tierEffectiveBounties[tier.Key] = electionContext.MiningTierBounties[tier.Key];
				}

				if(tierEffectiveServiceFees.ContainsKey(tier.Key)) {
					tierEffectiveBounties[tier.Key] -= tierEffectiveServiceFees[tier.Key];
				}
			}

			// assign the fees to the special network service fees account, if any and ONLY if enabled of course.
			result.InfrastructureServiceFees = 0;

			decimal effectiveServiceFees = 0;

			if(tierEffectiveServiceFees.Any()) {
				effectiveServiceFees = tierEffectiveServiceFees.Values.Sum();
			}

			if(electionContext.MaintenanceServiceFeesEnabled && (effectiveServiceFees > 0)) {
				result.InfrastructureServiceFees = NeuraliumUtilities.RoundNeuraliumsPrecision(effectiveServiceFees);
			}

			// create the tier buckets
			
			var tierElected = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, (Dictionary<AccountId, IElectedResults>)null);

			foreach(var tier in tierElected.Keys.ToArray()) {
				tierElected[tier] = result.GetTierElectedCandidates(tier);
			}
			
			
			var tierHasElected = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, false);
			foreach(var tier in tierHasElected.Keys.ToArray()) {
				if(tierElected.ContainsKey(tier)) {
					tierHasElected[tier] = tierElected[tier].Any();
				}
			}

			bool firstTierEnabled = MiningTierUtils.HasTier(electionContext.MiningTiers, Enums.MiningTiers.FirstTier);
			bool secondTierEnabled = MiningTierUtils.HasTier(electionContext.MiningTiers, Enums.MiningTiers.SecondTier);
			bool thirdTierEnabled = MiningTierUtils.HasTier(electionContext.MiningTiers, Enums.MiningTiers.ThirdTier);
			
			// the first 3 tiers have special spill over logics, so we apply them ehre
			bool hasFirstTiers = firstTierEnabled && tierHasElected[Enums.MiningTiers.FirstTier];
			bool hasSecondTiers = secondTierEnabled && tierHasElected[Enums.MiningTiers.SecondTier];
			bool hasThirdTiers = thirdTierEnabled && tierHasElected[Enums.MiningTiers.ThirdTier];

			if(thirdTierEnabled && !hasThirdTiers && (firstTierEnabled || secondTierEnabled)) {
				// ok, we have no third tier miners. the bounty will spill over equally amongst first and second tier miners
				decimal spilloverBounty = tierEffectiveBounties[Enums.MiningTiers.ThirdTier];

				if(firstTierEnabled && secondTierEnabled) {
					decimal dividedBounty = spilloverBounty / 2;
					tierEffectiveBounties[Enums.MiningTiers.FirstTier] += dividedBounty;
					tierEffectiveBounties[Enums.MiningTiers.SecondTier] += dividedBounty;
				}
				else if(secondTierEnabled) {
					tierEffectiveBounties[Enums.MiningTiers.SecondTier] += spilloverBounty;
				}
				else if(firstTierEnabled) {
					tierEffectiveBounties[Enums.MiningTiers.FirstTier] += spilloverBounty;
				}
			}

			if(secondTierEnabled && !hasSecondTiers && firstTierEnabled) {
				// ok, we have no second tier miners. the bounty will splill to the first tier miners
				tierEffectiveBounties[Enums.MiningTiers.FirstTier] += tierEffectiveBounties[Enums.MiningTiers.SecondTier];
			}

			// now allocate weaker peers with their lesser part of the bounty

			void AllocateBounty(KeyValuePair<AccountId, IElectedResults> elected, decimal adjustedBounty, Dictionary<AccountId, IElectedResults> electedResults) {
				
				decimal allocatedBounty = 0;
				
				INeuraliumElectedResults electedResultsEntry = (INeuraliumElectedResults) electedResults[elected.Key];
				try {

					if((electedResultsEntry.DelegateAccountId == null) || !delegateAllocations.ContainsKey(electedResultsEntry.DelegateAccountId)) {
						allocatedBounty = adjustedBounty;
					} else {
						// assign it to the delegate account
						if(!result.DelegateAccounts.ContainsKey(electedResultsEntry.DelegateAccountId)) {
							result.DelegateAccounts.Add(electedResultsEntry.DelegateAccountId, result.CreateDelegateResult());
						}

						decimal infrastructureFees = delegateAllocations[electedResultsEntry.DelegateAccountId].InfrastructureServiceFees;
						decimal allocation = delegateAllocations[electedResultsEntry.DelegateAccountId].delegateBountyShare;
						
						decimal delegateBountyShare = adjustedBounty * allocation;
						((INeuraliumDelegateResults) result.DelegateAccounts[electedResultsEntry.DelegateAccountId]).BountyShare += NeuraliumUtilities.RoundNeuraliumsPrecision(delegateBountyShare);

						// the account gets the rest
						allocatedBounty = adjustedBounty - delegateBountyShare;
					}
				} catch(Exception ex) {
					Log.Error(ex, $"Failed to allocate bounty for account {elected.Key}");
				}
				
				electedResultsEntry.BountyShare = NeuraliumUtilities.RoundNeuraliumsPrecision(allocatedBounty);
			}

			void AllocateEntireBounty(bool hasAny, HashSet<AccountId> electedAccountIds, decimal effectiveBounty, Dictionary<AccountId, IElectedResults> electedResults) {
				if(!hasAny) {
					return;
				}

				decimal adjustedBounty = effectiveBounty / electedAccountIds.Count;

				foreach(var entry in electedResults.Where(e => electedAccountIds.Contains(e.Key))) {
					AllocateBounty(entry, adjustedBounty, electedResults);
				}
			}

			foreach(var tier in electionContext.MiningTiers) {
				AllocateEntireBounty(tierHasElected[tier], MoreEnumerable.ToHashSet(tierElected[tier].Keys), tierEffectiveBounties[tier], tierElected[tier]);
			}
		}
	}
}