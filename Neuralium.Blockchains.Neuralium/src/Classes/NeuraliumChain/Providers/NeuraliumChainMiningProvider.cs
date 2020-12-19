using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Elections;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Network.AppointmentValidatorProtocol;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumChainMiningProvider : IChainMiningProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	/// <summary>
	///     Provide access to the chain state that is saved in the DB
	/// </summary>
	public class NeuraliumChainMiningProvider : ChainMiningProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainMiningProvider {

		public NeuraliumChainMiningProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
		
		public override bool MiningAllowed => true;
		protected override IAppointmentValidatorDelegate CreateAppointmentValidatorDelegate() {
			return new NeuraliumAppointmentValidatorDelegate(this.centralCoordinator);
		}

		public override async Task<BlockElectionDistillate> PrepareBlockElectionContext(IBlock currentBlock, AccountId miningAccountId, LockContext lockContext) {
			NeuraliumBlockElectionDistillate blockElectionDistillate = (NeuraliumBlockElectionDistillate) await base.PrepareBlockElectionContext(currentBlock, miningAccountId, lockContext).ConfigureAwait(false);

			if(currentBlock is INeuraliumBlock neuraliumBlock) {

				if(neuraliumBlock is INeuraliumElectionBlock neuraliumElectionBlock) {

					if(neuraliumElectionBlock.ElectionContext is INeuraliumElectionContext neuraliumElectionContext) {

					}
				}

			}

			return blockElectionDistillate;
		}

		protected override IElectionProcessorFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> GetElectionProcessorFactory() {
			return new NeuraliumElectionProcessorFactory();
		}

		protected override BlockElectionDistillate CreateBlockElectionContext() {
			return new NeuraliumBlockElectionDistillate();
		}

		protected override async Task PreparePassiveElectionContext(long currentBlockId, AccountId miningAccountId, PassiveElectionContextDistillate intermediaryResultEntry, IPassiveIntermediaryElectionResults passiveIntermediaryElectionResults, IBlock currentBlock, LockContext lockContext) {
			await base.PreparePassiveElectionContext(currentBlockId, miningAccountId, intermediaryResultEntry, passiveIntermediaryElectionResults, currentBlock, lockContext).ConfigureAwait(false);

			if(passiveIntermediaryElectionResults is INeuraliumIntermediaryElectionResults neuraliumSimpleIntermediaryElectionResults && intermediaryResultEntry is NeuraliumPassiveElectionContextDistillate neuraliumIntermediaryElectionContext) {
				
			}
		}

		protected override async Task PrepareFinalElectionContext(long currentBlockId, AccountId miningAccountId, FinalElectionResultDistillate finalResultDistillateEntry, IFinalElectionResults finalElectionResult, IBlock currentBlock, LockContext lockContext) {
			await base.PrepareFinalElectionContext(currentBlockId, miningAccountId, finalResultDistillateEntry, finalElectionResult, currentBlock, lockContext).ConfigureAwait(false);

			if(finalElectionResult is INeuraliumFinalElectionResults neuraliumSimpleFinalElectionResults && finalResultDistillateEntry is NeuraliumFinalElectionResultDistillate neuraliumFinalElectionContext) {
				if(neuraliumSimpleFinalElectionResults.ElectedCandidates[miningAccountId] is INeuraliumElectedResults neuraliumElectedResults) {

					bool activeElection = neuraliumSimpleFinalElectionResults is INeuraliumActiveFinalElectionResults;
					
					neuraliumFinalElectionContext.BountyShare = neuraliumElectedResults.BountyShare;

					List<Amount> tippingTransactions = currentBlock.GetAllConfirmedTransactions().Where(t => neuraliumElectedResults.Transactions.Contains(t.Key)).Select(t => t.Value).OfType<ITipTransaction>().Select(t => t.Tip).ToList();

					if(tippingTransactions.Any()) {
						neuraliumFinalElectionContext.TransactionTips = tippingTransactions.Sum(t => t);
					}
					
					await this.SetNeuraliumFinalElectedContextStatistics(currentBlockId, neuraliumFinalElectionContext, lockContext).ConfigureAwait(false);
				}
			}
		}
		
		protected override async Task SetFinalElectedContextStatistics(FinalElectionResultDistillate finalResultDistillateEntry, long currentBlockId, LockContext lockContext){
			
			// do nothing, we will override the whole thing
		}
		
		protected override void UpdateWalletSessionStatistics(WalletElectionsMiningSessionStatistics miningTotalStatistics, FinalElectionResultDistillate finalResultDistillateEntry, long currentBlockId, LockContext lockContext) {
			base.UpdateWalletSessionStatistics(miningTotalStatistics, finalResultDistillateEntry, currentBlockId, lockContext);
			
			if(miningTotalStatistics is NeuraliumWalletElectionsMiningSessionStatistics neuraliumWalletElectionsMiningStatistics) {

				if(finalResultDistillateEntry is NeuraliumFinalElectionResultDistillate neuraliumFinalElectionContext) {
					neuraliumWalletElectionsMiningStatistics.TotalBounty += neuraliumFinalElectionContext.BountyShare;
					neuraliumWalletElectionsMiningStatistics.TotalTips += neuraliumFinalElectionContext.TransactionTips;
				}
			}
		}
		
		protected override void UpdateWalletTotalStatistics(WalletElectionsMiningAggregateStatistics miningAggregateStatistics, FinalElectionResultDistillate finalResultDistillateEntry, long currentBlockId, LockContext lockContext) {
			base.UpdateWalletTotalStatistics(miningAggregateStatistics, finalResultDistillateEntry, currentBlockId, lockContext);
			
			if(miningAggregateStatistics is NeuraliumWalletElectionsMiningAggregateStatistics neuraliumWalletElectionsMiningStatistics) {

				if(finalResultDistillateEntry is NeuraliumFinalElectionResultDistillate neuraliumFinalElectionContext) {
					neuraliumWalletElectionsMiningStatistics.TotalBounty += neuraliumFinalElectionContext.BountyShare;
					neuraliumWalletElectionsMiningStatistics.TotalTips += neuraliumFinalElectionContext.TransactionTips;
				}
			}
		}
		
		protected virtual async Task SetNeuraliumFinalElectedContextStatistics(long currentBlockId, NeuraliumFinalElectionResultDistillate neuraliumFinalElectionContext, LockContext lockContext){
			
			await this.WalletProvider.UpdateMiningStatistics(this.MiningAccountId, this.MiningTier, s => {
				this.UpdateWalletSessionStatistics(s, neuraliumFinalElectionContext, currentBlockId, lockContext);
			}, t => {
				this.UpdateWalletTotalStatistics(t, neuraliumFinalElectionContext, currentBlockId, lockContext);
			}, lockContext).ConfigureAwait(false);
			
		}
		
		protected override ElectedCandidateResultDistillate CreateElectedCandidateResult() {
			return new NeuraliumElectedCandidateResultDistillate();
		}

		protected override async Task ConfirmedPrimeElected(BlockElectionDistillate blockElectionDistillate, FinalElectionResultDistillate finalElectionResultDistillate) {

			await base.ConfirmedPrimeElected(blockElectionDistillate, finalElectionResultDistillate).ConfigureAwait(false);

			NeuraliumBlockElectionDistillate neuraliumBlockElectionDistillate = (NeuraliumBlockElectionDistillate) blockElectionDistillate;

			NeuraliumFinalElectionResultDistillate neuraliumFinalElectionContext = (NeuraliumFinalElectionResultDistillate) finalElectionResultDistillate;
			
			this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumMiningPrimeElected(blockElectionDistillate.electionBockId, neuraliumFinalElectionContext.BountyShare, neuraliumFinalElectionContext.TransactionTips, AccountId.FromString(neuraliumFinalElectionContext.DelegateAccountId)));

			MiningHistoryEntry.MiningHistoryParameters parameters = new MiningHistoryEntry.MiningHistoryParameters();
			parameters.blockElectionDistillate = blockElectionDistillate;
			parameters.finalElectionResultDistillate = finalElectionResultDistillate;
			parameters.Message = NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningPrimeElected;
			parameters.Level = ChainMiningProvider.MiningEventLevel.Level1;
			parameters.Parameters = new object[] {blockElectionDistillate.electionBockId, neuraliumFinalElectionContext.BountyShare, neuraliumFinalElectionContext.TransactionTips, AccountId.FromString(neuraliumFinalElectionContext.DelegateAccountId)};
			this.AddMiningHistoryEntry(parameters);
		}

		protected override MiningHistoryEntry CreateMiningHistoryEntry() {
			return new NeuraliumMiningHistoryEntry();
		}

		protected override void PrepareMiningHistoryEntry(MiningHistoryEntry entry, MiningHistoryEntry.MiningHistoryParameters parameters) {
			base.PrepareMiningHistoryEntry(entry, parameters);

			if(entry.Message == BlockchainSystemEventTypes.Instance.MiningPrimeElected) {
				entry.Message = NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningPrimeElected;

				if(entry is NeuraliumMiningHistoryEntry neuraliumMiningHistoryEntry && parameters.finalElectionResultDistillate is NeuraliumFinalElectionResultDistillate neuraliumFinalElectionContext) {
					neuraliumMiningHistoryEntry.BountyShare = neuraliumFinalElectionContext.BountyShare;
					neuraliumMiningHistoryEntry.TransactionTips = neuraliumFinalElectionContext.TransactionTips;
				}
			}
		}
	}

	public class NeuraliumMiningHistoryEntry : MiningHistoryEntry {
		public decimal BountyShare { get; set; }
		public decimal TransactionTips { get; set; }

		public override MiningHistory ToApiHistory() {
			MiningHistory entry = base.ToApiHistory();

			if(entry is NeuraliumMiningHistory neuraliumMiningHistory) {
				neuraliumMiningHistory.BountyShare = this.BountyShare;
				neuraliumMiningHistory.TransactionTips = this.TransactionTips;
			}

			return entry;
		}

		public override MiningHistory CreateApiMiningHistory() {
			return new NeuraliumMiningHistory();
		}

	}
}