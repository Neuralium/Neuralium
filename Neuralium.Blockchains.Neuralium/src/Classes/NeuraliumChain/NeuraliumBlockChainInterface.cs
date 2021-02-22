using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Tools.Cryptography;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Threading;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain {
	public interface INeuraliumBlockChainInterface : IBlockChainInterface<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {

		void SubmitDebugMessage(Action<(TransactionId, SafeArrayHandle)> callback);

		TaskResult<TotalAPI> QueryWalletTotal(string accountCode);

		TaskResult<bool> SendNeuraliums(AccountId targetAccountId, Amount amount, Amount tip, string note, CorrelationContext correlationContext, byte expiration = 0);

		TaskResult<TimelineHeader> QueryNeuraliumTimelineHeader(string accountCode);
		TaskResult<TimelineDay> QueryNeuraliumTimelineSection(string accountCode, DateTime day);
#if TESTNET || DEVNET
		TaskResult<bool> RefillNeuraliums(string accountCode, CorrelationContext correlationContext, byte expiration = 0);
#endif
#if COLORADO_EXCLUSION
		TaskResult<bool> BypassAppointmentVerification(string accountCode);
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

		public TaskResult<TotalAPI> QueryWalletTotal(string accountCode) {

			return this.RunTaskMethodAsync(async lc => {

				if(string.IsNullOrWhiteSpace(accountCode)) {
					accountCode = (await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountCode;
				}

				return await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountCode, true, lc).ConfigureAwait(false);
			});
		}

		public TaskResult<bool> SendNeuraliums(AccountId targetAccountId, Amount amount, Amount tip, string note, CorrelationContext correlationContext, byte expiration = 0) {

			return this.RunTaskMethodAsync(async lc => {

				string accountCode = (await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountCode;

				using(AsyncManualResetEventSlim resetEvent = new AsyncManualResetEventSlim(false)) {
					ICreateNeuraliumTransferTransactionWorkflow workflow = this.NeuraliumChainFactoryProvider.WorkflowFactory.CreateSendNeuraliumsWorkflow(accountCode, new AccountId(targetAccountId), amount, tip, note, correlationContext, expiration);

					workflow.Success += w => {
						resetEvent.Set();

						return Task.CompletedTask;
					};

					this.centralCoordinator.PostWorkflow(workflow);

					await resetEvent.WaitAsync().ConfigureAwait(false);

					return true;
				}
			});

		}

		public TaskResult<TimelineHeader> QueryNeuraliumTimelineHeader(string accountCode) {

			return this.RunTaskMethodAsync(lc => {
				return this.centralCoordinator.ChainComponentProvider.WalletProvider.GetTimelineHeader(accountCode, lc);
			});
		}

		public TaskResult<TimelineDay> QueryNeuraliumTimelineSection(string accountCode, DateTime day) {

			return this.RunTaskMethodAsync(lc => {
				
				return this.centralCoordinator.ChainComponentProvider.WalletProvider.GetTimelineSection(accountCode, day, lc);
			});
		}
#if TESTNET || DEVNET
		public TaskResult<bool> RefillNeuraliums(string accountCode, CorrelationContext correlationContext, byte expiration = 0) {

			return this.RunTaskMethodAsync(async lc => {

				if(string.IsNullOrWhiteSpace(accountCode)) {
					accountCode = (await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountCode;
				}

				using(ManualResetEventSlim resetEvent = new ManualResetEventSlim(false)) {
					ICreateNeuraliumRefillTransactionWorkflow workflow = this.NeuraliumChainFactoryProvider.WorkflowFactory.CreateRefillNeuraliumsWorkflow(accountCode, correlationContext, expiration);

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
		
#if(COLORADO_EXCLUSION)
		
		public TaskResult<bool> BypassAppointmentVerification(string accountCode) {

			
			return this.RunTaskMethodAsync(async lc => {

				if(string.IsNullOrWhiteSpace(accountCode)) {
					accountCode = (await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetActiveAccount(lc).ConfigureAwait(false)).AccountCode;
				}

				var account = await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetWalletAccount(accountCode, lc).ConfigureAwait(false);
				
				await this.centralCoordinator.ChainComponentProvider.WalletProvider.ScheduleTransaction((provider, token, lc2) => {
					account.AccountAppointment = new WalletAccount.AccountAppointmentDetails();

					account.AccountAppointment.AppointmentVerified = true;
					account.AccountAppointment.RequesterId = new TransactionId(account.GetAccountId(), GlobalRandom.GetNext()).ToGuid();
					account.AccountAppointment.AppointmentConfirmationCode = GlobalsService.TESTING_APPOINTMENT_CODE;
					account.AccountAppointment.AppointmentConfirmationCodeExpiration = DateTime.Now.AddYears(1);
					account.AccountAppointment.AppointmentStatus = Enums.AppointmentStatus.AppointmentCompleted;
					
					return Task.FromResult(true);
				}, lc).ConfigureAwait(false);

				this.centralCoordinator.ChainComponentProvider.AppointmentsProviderBase.OperatingMode = Enums.OperationStatus.Appointment;
				
				return true;
				
			});
		}

#endif

		public TaskResult<List<object>> QueryNeuraliumTransactionPool() {

			return this.RunTaskMethod(lc => {
				return this.centralCoordinator.ChainComponentProvider.BlockchainProvider.GetNeuraliumTransactionPool().Cast<object>().ToList();
			});
		}

	#endregion

	}
}