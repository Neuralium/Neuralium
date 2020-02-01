using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using MoreLinq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
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

			decimal firstTierEffectiveServiceFees = 0;
			decimal secondTierEffectiveServiceFees = 0;
			decimal thirdTierEffectiveServiceFees = 0;

			if(electionContext.MaintenanceServiceFeesEnabled) {
				firstTierEffectiveServiceFees = electionContext.FirstTierBounty * electionContext.MaintenanceServiceFees.Percentage;
				secondTierEffectiveServiceFees = electionContext.SecondTierBounty * electionContext.MaintenanceServiceFees.Percentage;
				thirdTierEffectiveServiceFees = electionContext.ThirdTierBounty * electionContext.MaintenanceServiceFees.Percentage;
			}

			decimal firstTierEffectiveBounty = electionContext.FirstTierBounty - firstTierEffectiveServiceFees;
			decimal secondTierEffectiveBounty = electionContext.SecondTierBounty - secondTierEffectiveServiceFees;
			decimal thirdTierEffectiveBounty = electionContext.ThirdTierBounty - thirdTierEffectiveServiceFees;

			// assign the fees to the special network service fees account, if any and ONLY if enabled of course.
			result.InfrastructureServiceFees = 0;

			decimal effectiveServiceFees = firstTierEffectiveServiceFees + secondTierEffectiveServiceFees + thirdTierEffectiveServiceFees;

			if(electionContext.MaintenanceServiceFeesEnabled && (effectiveServiceFees > 0)) {
				result.InfrastructureServiceFees = NeuraliumUtilities.RoundNeuraliumsPrecision(effectiveServiceFees);
			}

			// create the tier buckets
			var firstTierElected = result.FirstTierElectedCandidates;
			var secondTierElected = result.SecondTierElectedCandidates;
			var thirdTierElected = result.ThirdTierElectedCandidates;

			bool hasFirstTiers = firstTierElected.Any();
			bool hasSecondTiers = secondTierElected.Any();
			bool hasThirdTiers = thirdTierElected.Any();

			if(!hasThirdTiers) {
				// ok, we have no third tier miners. the bounty will splill equallyy amongst first and second tier miners
				decimal dividedBounty = thirdTierEffectiveBounty / 2;
				firstTierEffectiveBounty += dividedBounty;
				secondTierEffectiveBounty += dividedBounty;
			}

			if(!hasSecondTiers) {
				// ok, we have no second tier miners. the bounty will splill to the first tier miners
				firstTierEffectiveBounty += secondTierEffectiveBounty;
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

			AllocateEntireBounty(hasFirstTiers, MoreEnumerable.ToHashSet(firstTierElected.Keys), firstTierEffectiveBounty, result.FirstTierElectedCandidates);
			AllocateEntireBounty(hasSecondTiers, MoreEnumerable.ToHashSet(secondTierElected.Keys), secondTierEffectiveBounty, result.SecondTierElectedCandidates);
			AllocateEntireBounty(hasThirdTiers, MoreEnumerable.ToHashSet(thirdTierElected.Keys), thirdTierEffectiveBounty, result.ThirdTierElectedCandidates);
		}
	}
}