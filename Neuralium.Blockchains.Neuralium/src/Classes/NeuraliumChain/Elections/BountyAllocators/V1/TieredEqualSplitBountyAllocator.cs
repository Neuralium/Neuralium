using System;
using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using MoreLinq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.Logging;
using Serilog;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Elections.BountyAllocators.V1 {
	public class TieredEqualSplitBountyAllocator : BountyAllocator {
		
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

			Dictionary<Enums.MiningTiers, decimal> tierEffectiveServiceFees = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, 0M);

			if(electionContext.MaintenanceServiceFeesEnabled) {

				foreach((Enums.MiningTiers key, var _) in tierEffectiveServiceFees.ToArray()) {
					
					tierEffectiveServiceFees.SetTierValue(key, electionContext.MiningTierBounties.GetTierValue(key) * electionContext.MaintenanceServiceFees.Percentage);
				}
			}

			Dictionary<Enums.MiningTiers, decimal> tierEffectiveBounties = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, 0M);

			foreach((Enums.MiningTiers key, var _) in tierEffectiveBounties.ToArray()) {
				tierEffectiveBounties.SetTierValue(key, electionContext.MiningTierBounties.GetTierValue(key));
				
				if(tierEffectiveServiceFees.ContainsKey(key)) {
					tierEffectiveBounties[key] -= tierEffectiveServiceFees[key];
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

			Dictionary<Enums.MiningTiers, Dictionary<AccountId, IElectedResults>> tierElected = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, (Dictionary<AccountId, IElectedResults>) null);

			foreach(Enums.MiningTiers tier in tierElected.Keys.ToArray()) {
				tierElected[tier] = result.GetTierElectedCandidates(tier);
			}

			Dictionary<Enums.MiningTiers, bool> tierHasElected = MiningTierUtils.FillMiningTierSet(electionContext.MiningTiers, false);

			foreach(Enums.MiningTiers tier in tierHasElected.Keys.ToArray()) {
				var tierEntry = tierElected.GetTierValue(tier);

				if(MiningTierUtils.IsFirstOrSecondTier(tier) && tierEntry != null) {
					// make sure we have ONLY server entries in the first 2 tiers
					tierEntry = tierEntry.Where(e => e.Key.IsServer).ToDictionary(e => e.Key, e => e.Value);
				}
				tierHasElected.SetTierValue(tier, tierEntry?.Any()??false);
			}

			bool firstTierEnabled = MiningTierUtils.HasTier(electionContext.MiningTiers, Enums.MiningTiers.FirstTier);
			bool secondTierEnabled = MiningTierUtils.HasTier(electionContext.MiningTiers, Enums.MiningTiers.SecondTier);
			bool thirdTierEnabled = MiningTierUtils.HasTier(electionContext.MiningTiers, Enums.MiningTiers.ThirdTier);

			// the first 3 tiers have special spill over logics, so we apply them here
			bool hasFirstTiers = firstTierEnabled && tierHasElected.GetTierValue(Enums.MiningTiers.FirstTier);
			bool hasSecondTiers = secondTierEnabled && tierHasElected.GetTierValue(Enums.MiningTiers.SecondTier);
			bool hasThirdTiers = thirdTierEnabled && tierHasElected.GetTierValue(Enums.MiningTiers.ThirdTier);

			if(thirdTierEnabled && !hasThirdTiers && (firstTierEnabled || secondTierEnabled)) {
				// ok, we have no third tier miners. the bounty will spill over equally amongst first and second tier miners
				decimal spilloverBounty = tierEffectiveBounties.GetTierValue(Enums.MiningTiers.ThirdTier);

				if(firstTierEnabled && secondTierEnabled) {
					decimal dividedBounty = spilloverBounty / 2;
					tierEffectiveBounties[Enums.MiningTiers.FirstTier] += dividedBounty;
					tierEffectiveBounties[Enums.MiningTiers.SecondTier] += dividedBounty;
				} else if(secondTierEnabled) {
					tierEffectiveBounties[Enums.MiningTiers.SecondTier] += spilloverBounty;
				} else if(firstTierEnabled) {
					tierEffectiveBounties[Enums.MiningTiers.FirstTier] += spilloverBounty;
				}
			}

			if(secondTierEnabled && !hasSecondTiers && firstTierEnabled && hasFirstTiers) {
				// ok, we have no second tier miners. the bounty will splill to the first tier miners
				tierEffectiveBounties[Enums.MiningTiers.FirstTier] += tierEffectiveBounties.GetTierValue(Enums.MiningTiers.SecondTier);
			}

			// now allocate weaker peers with their lesser part of the bounty

			void AllocateBounty(KeyValuePair<AccountId, IElectedResults> elected, decimal adjustedBounty, Dictionary<AccountId, IElectedResults> electedResults) {

				decimal allocatedBounty = 0;

				INeuraliumElectedResults electedResultsEntry = (INeuraliumElectedResults) electedResults[elected.Key];

				try {

					if((electedResultsEntry.DelegateAccountId == default(AccountId)) || !delegateAllocations.ContainsKey(electedResultsEntry.DelegateAccountId)) {
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
					NLog.Default.Error(ex, $"Failed to allocate bounty for account {elected.Key}");
				}

				electedResultsEntry.BountyShare = NeuraliumUtilities.RoundNeuraliumsPrecision(allocatedBounty);
			}

			void AllocateEntireBounty(bool hasAny, HashSet<AccountId> electedAccountIds, decimal effectiveBounty, Dictionary<AccountId, IElectedResults> electedResults) {
				if(!hasAny) {
					return;
				}

				decimal adjustedBounty = effectiveBounty / electedAccountIds.Count;

				foreach(KeyValuePair<AccountId, IElectedResults> entry in electedResults.Where(e => electedAccountIds.Contains(e.Key))) {
					AllocateBounty(entry, adjustedBounty, electedResults);
				}
			}

			foreach(Enums.MiningTiers tier in electionContext.MiningTiers) {
				AllocateEntireBounty(tierHasElected[tier], MoreEnumerable.ToHashSet(tierElected[tier].Keys), tierEffectiveBounties[tier], tierElected[tier]);
			}
		}
	}
}