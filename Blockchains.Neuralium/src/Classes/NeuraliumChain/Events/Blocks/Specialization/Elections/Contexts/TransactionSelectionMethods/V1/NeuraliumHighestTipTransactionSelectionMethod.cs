using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.Configuration.TransactionSelectionStrategies;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using MoreLinq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Versions;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods.V1 {

	/// <summary>
	///     select transactions by tip amount. the highest paying transactions are chosen first. Then we go by age
	/// </summary>
	public class NeuraliumHighestTipTransactionSelectionMethod : TransactionSelectionMethod {
		private readonly HighestTipTransactionSelectionStrategySettings highestTipTransactionSelectionStrategySettings;
		private readonly IBlockchainTimeService timeService;

		private readonly ITransactionTipsAllocationMethod transactionSelectionMethod;

		public NeuraliumHighestTipTransactionSelectionMethod(long blockId, ITransactionTipsAllocationMethod transactionSelectionMethod, IChainStateProvider chainStateProvider, INeuraliumWalletProviderProxy walletProvider, IElectionContext electionContext, HighestTipTransactionSelectionStrategySettings highestTipTransactionSelectionStrategySettings, IBlockchainTimeService timeService, Enums.ChainSharingTypes blockSavingModes) : base(blockId, chainStateProvider, walletProvider,  electionContext, blockSavingModes) {
			this.transactionSelectionMethod = transactionSelectionMethod;
			this.highestTipTransactionSelectionStrategySettings = highestTipTransactionSelectionStrategySettings;
			this.timeService = timeService;
		}

		protected override ComponentVersion<TransactionSelectionMethodType> SetIdentity() {
			return (NeuraliumTransactionSelectionMethodTypes.Instance.HighestTips, 1, 0);
		}

		public override List<TransactionId> PerformTransactionSelection(IEventPoolProvider chainEventPoolProvider, List<TransactionId> existingTransactions) {

			var poolTransactions = ((INeuraliumChainPoolProviderGenerix) chainEventPoolProvider).GetTransactionIdsAndTip();

			// exclude the transactions that should not be selected
			var availableTransactions = poolTransactions.Where(p => !existingTransactions.Contains(p.transactionIds)).ToList();

			if(!availableTransactions.Any()) {
				return new List<TransactionId>();
			}

			// remove transactions with no tip, we so desired
			var tipAvailableTransactions = availableTransactions.Where(t => t.tip != 0).ToList();
			var notipAvailableTransactions = availableTransactions.Where(t => t.tip == 0).ToList();

			var finalTransactions = new List<(TransactionId transactionIds, decimal tip)>();

			if(this.highestTipTransactionSelectionStrategySettings.TimeSortingMethod == HighestTipTransactionSelectionStrategySettings.TimeSortingMethods.Random) {

				if(this.highestTipTransactionSelectionStrategySettings.TipSortingMethod == HighestTipTransactionSelectionStrategySettings.TipSortingMethods.MostToLess) {
					finalTransactions.AddRange(tipAvailableTransactions.OrderByDescending(t => t.tip));
					finalTransactions.AddRange(notipAvailableTransactions);
				} else if(this.highestTipTransactionSelectionStrategySettings.TipSortingMethod == HighestTipTransactionSelectionStrategySettings.TipSortingMethods.LessToMost) {
					finalTransactions.AddRange(tipAvailableTransactions.OrderBy(t => t.tip));
					finalTransactions.AddRange(notipAvailableTransactions);
				} else {

					finalTransactions = availableTransactions.Shuffle().ToList();
				}
			}

			if(this.highestTipTransactionSelectionStrategySettings.TimeSortingMethod != HighestTipTransactionSelectionStrategySettings.TimeSortingMethods.Random) {

				// lets group the times by hour (the inception is irrelevant since its all relative for grouping, so we set a dummy date)

				var group = tipAvailableTransactions.GroupBy(t => {

					DateTime timestamp = this.timeService.GetTransactionDateTime(t.transactionIds, DateTime.UtcNow.AddYears(-1));

					// remove the minutes and seconds so we can group by hour
					return new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, 0, 0);
				});

				if(this.highestTipTransactionSelectionStrategySettings.TimeSortingMethod == HighestTipTransactionSelectionStrategySettings.TimeSortingMethods.NewerToOlder) {
					group = group.OrderByDescending(g => g.Key);
				} else if(this.highestTipTransactionSelectionStrategySettings.TimeSortingMethod == HighestTipTransactionSelectionStrategySettings.TimeSortingMethods.OlderToNewer) {
					group = group.OrderBy(g => g.Key);
				}

				//TODO: perhaps we could make this more elaborate?

				// and now, recombine them to have our prefered order
				finalTransactions.AddRange(group.SelectMany(g => {

					if(this.highestTipTransactionSelectionStrategySettings.TipSortingMethod == HighestTipTransactionSelectionStrategySettings.TipSortingMethods.MostToLess) {
						return g.OrderByDescending(t => t.tip).ToList();
					}

					if(this.highestTipTransactionSelectionStrategySettings.TipSortingMethod == HighestTipTransactionSelectionStrategySettings.TipSortingMethods.LessToMost) {
						return g.OrderBy(t => t.tip).ToList();
					}

					return g.ToList();
				}).ToList());

				finalTransactions.AddRange(notipAvailableTransactions);
			}

			return this.SelectSelection(finalTransactions);
		}

		protected override List<TransactionId> SelectSelection(List<TransactionId> transactionIds) {
			throw new NotImplementedException();
		}

		protected List<TransactionId> SelectSelection(List<(TransactionId transactionId, decimal tip)> transactionIds) {

			return transactionIds.OrderByDescending(t => t.tip).ThenBy(t => t.transactionId.Timestamp).Take(this.MaximumTransactionCount).Select(t => t.transactionId).ToList();
		}
	}
}