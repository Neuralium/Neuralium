using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Blockchains.Neuralium.Classes.NeuraliumChain.Managers;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Threading;

namespace Blockchains.Neuralium.Classes.NeuraliumChain {
	public interface INeuraliumBlockChainInterface : IBlockChainInterface<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {

		void SubmitDebugMessage(Action<(TransactionId, SafeArrayHandle)> callback);

		TaskResult<TotalAPI> QueryWalletTotal(Guid accountUuid);

		TaskResult<bool> SendNeuraliums(AccountId targetAccountId, Amount amount, Amount tip, string note, CorrelationContext correlationContext, byte expiration = 0);

		TaskResult<TimelineHeader> QueryNeuraliumTimelineHeader(Guid accountUuid);
		TaskResult<List<TimelineDay>> QueryNeuraliumTimelineSection(Guid accountUuid, DateTime firstday, int skip, int take);
#if TESTNET || DEVNET
		TaskResult<bool> RefillNeuraliums(Guid accountUuid, CorrelationContext correlationContext, byte expiration = 0);
#endif

		TaskResult<List<object>> QueryNeuraliumTransactionPool();
	}

	public class NeuraliumBlockChainInterface : BlockChainInterface<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumBlockChainInterface {
		public NeuraliumBlockChainInterface(INeuraliumCentralCoordinator coordinator, TimeSpan? taskCheckSpan = null) : base(coordinator, taskCheckSpan) {
		}

		protected INeuraliumChainFactoryProvider NeuraliumChainFactoryProvider => (INeuraliumChainFactoryProvider) this.centralCoordinator.ChainComponentProvider.ChainFactoryProviderBase;

		// demo method
		// public (Task<int> task, CancellationTokenSource ctSource) GetBlockCount () {
		//     LoopbackTask message = new LoopbackTask (Task.Addresses.BlockChainController, Task.Addresses.Interface);
		//     return RunMethod<int> (message, (result) => {
		//         // do something with the results and return something
		//         return 1;
		//     });
		// }

		// start our pletora of threads

		public override void PrintChainDebug(string item) {
			if(item == "print") {
				// var transactionTask = new NeuraliumInterpretationTask<int>();
				//
				// transactionTask.SetAction((service, taskRoutingContext) => {
				// 	//service.BlockChainModel.PrintChainDebug();
				//
				// });
				//
				// this.centralCoordinator.RouteTask(transactionTask);
			}
		}

		public void SubmitDebugMessage(Action<(TransactionId, SafeArrayHandle)> callback) {
			IInsertDebugMessageWorkflow workflow = this.NeuraliumChainFactoryProvider.WorkflowFactory.CreateDebugMessageWorkflow();

			workflow.Success += w => {
				// this.TriggerBlockchainLoaded();
				// this.coordinator.BlockChainModelController.BlockChainModel.SetSynced();
				// this.TriggerBlockChainSynced();

				return Task.CompletedTask;
			};

			this.centralCoordinator.PostWorkflow(workflow);
		}

		public void SubmitDebugConfirm(TransactionId uid, SafeArrayHandle hash) {
			IInsertDebugConfirmWorkflow workflow = this.NeuraliumChainFactoryProvider.WorkflowFactory.CreateDebugConfirmChainWorkflow(uid, hash);

			workflow.Success += w => {
				return Task.CompletedTask;
			};

			this.centralCoordinator.PostWorkflow(workflow);
		}

	#region system methods

	#endregion

	#region API methods

		public TaskResult<TotalAPI> QueryWalletTotal(Guid accountUuid) {

			return this.RunTaskMethodAsync(async (lc) => {

				if(accountUuid == Guid.Empty) {
					accountUuid = (await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountUuid;
				}

				return await centralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountUuid, true, lc).ConfigureAwait(false);
			});
		}

		public TaskResult<bool> SendNeuraliums(AccountId targetAccountId, Amount amount, Amount tip, string note, CorrelationContext correlationContext, byte expiration = 0) {

			

			return this.RunTaskMethodAsync(async (lc) => {

				Guid accountUuid = (await centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountUuid;
				
				using(ManualResetEventSlim resetEvent = new ManualResetEventSlim(false)) {
					ICreateNeuraliumTransferTransactionWorkflow workflow = this.NeuraliumChainFactoryProvider.WorkflowFactory.CreateSendNeuraliumsWorkflow(accountUuid, new AccountId(targetAccountId), amount, tip, note, correlationContext, expiration);

					workflow.Success += w => {
						resetEvent.Set();
						
						return Task.CompletedTask;
					};

					this.centralCoordinator.PostWorkflow(workflow);

					resetEvent.Wait();

					return true;
				}
			});

		}

		public TaskResult<TimelineHeader> QueryNeuraliumTimelineHeader(Guid accountUuid) {
			
			return this.RunTaskMethodAsync((lc) => {
				return this.centralCoordinator.ChainComponentProvider.WalletProvider.GetTimelineHeader(accountUuid, lc);
			});
		}

		public TaskResult<List<TimelineDay>> QueryNeuraliumTimelineSection(Guid accountUuid, DateTime firstday, int skip, int take) {
			
			return this.RunTaskMethodAsync((lc) => {
				return this.centralCoordinator.ChainComponentProvider.WalletProvider.GetTimelineSection(accountUuid, firstday, lc, skip, take);
			});
		}
#if TESTNET || DEVNET
		public TaskResult<bool> RefillNeuraliums(Guid accountUuid, CorrelationContext correlationContext, byte expiration = 0) {


			return this.RunTaskMethodAsync(async (lc) => {

				if(accountUuid == Guid.Empty) {
					accountUuid = (await centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountUuid;
				}

				using(ManualResetEventSlim resetEvent = new ManualResetEventSlim(false)) {
					ICreateNeuraliumRefillTransactionWorkflow workflow = this.NeuraliumChainFactoryProvider.WorkflowFactory.CreateRefillNeuraliumsWorkflow(accountUuid, correlationContext, expiration);

					workflow.Success += w => {
						resetEvent.Set();
						
						return Task.CompletedTask;
					};

					this.centralCoordinator.PostWorkflow(workflow);

					resetEvent.Wait();

					return true;
				}
			});
		}
#endif

		public TaskResult<List<object>> QueryNeuraliumTransactionPool() {

			return this.RunTaskMethod((lc) => {
				return this.centralCoordinator.ChainComponentProvider.BlockchainProvider.GetNeuraliumTransactionPool().Cast<object>().ToList();
			});
		}

	#endregion

	}
}