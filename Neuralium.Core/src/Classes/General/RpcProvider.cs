using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Blockchains.Neuralium.Classes;
using Blockchains.Neuralium.Classes.NeuraliumChain;
using Microsoft.AspNetCore.SignalR;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Tools.Cryptography;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Data.Arrays;
using Neuralium.Api.Common;
using Neuralium.Core.Classes.Services;
using Neuralium.Core.Controllers;
using Serilog;

namespace Neuralium.Core.Classes.General {

	public interface IRpcClientMethods {
	}

	public interface IRpcServerMethods : INeuraliumApiMethods {
	}

	public interface IRpcClientEvents {
		Task ReturnClientLongRunningEvent(int correlationId, int result, string error);
		Task LongRunningStatusUpdate(int correlationId, ushort eventId, byte eventType, object message);

		Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, int attempt);
		Task EnterKeysPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, Guid accountID, string keyname, int attempt);
		Task RequestCopyWalletKeyFile(int correlationId, ushort chainType, int keyCorrelationCode, Guid accountID, string keyname, int attempt);

		Task AccountTotalUpdated(string accountId, object total);
		Task RequestCopyWallet(int correlationId, ushort chainType);
		Task PeerTotalUpdated(int total);

		Task BlockchainSyncStatusChanged(ushort chainType, Enums.ChainSyncState syncStatus);
		Task WalletSyncStatusChanged(ushort chainType, Enums.ChainSyncState syncStatus);

		Task MiningStatusChanged(ushort chainType, bool isMining);

		Task WalletCreationStarted(int correlationId);
		Task WalletCreationEnded(int correlationId);

		Task AccountCreationStarted(int correlationId);
		Task AccountCreationEnded(int correlationId);
		Task AccountCreationMessage(int correlationId, string message);
		Task AccountCreationStep(int correlationId, string stepName, int stepIndex, int stepTotal);
		Task AccountCreationError(int correlationId, string error);

		Task KeyGenerationStarted(int correlationId, string keyName, int keyIndex, int keyTotal);
		Task KeyGenerationEnded(int correlationId, string keyName, int keyIndex, int keyTotal);
		Task KeyGenerationMessage(int correlationId, string keyName, string message, int keyIndex, int keyTotal);
		Task KeyGenerationError(int correlationId, string keyName, string error);
		Task KeyGenerationPercentageUpdate(int correlationId, string keyName, int percentage);

		Task AccountPublicationStarted(int correlationId);
		Task AccountPublicationEnded(int correlationId);
		Task RequireNodeUpdate(ushort chainType, string chainName);

		Task AccountPublicationPOWNonceIteration(int correlationId, int nonce, int difficulty);
		Task AccountPublicationPOWNonceFound(int correlationId, int nonce, int difficulty, List<int> powSolutions);

		Task AccountPublicationMessage(int correlationId, string message);
		Task AccountPublicationStep(int correlationId, string stepName, int stepIndex, int stepTotal);
		Task AccountPublicationError(int correlationId, string error);

		Task WalletSyncStarted(ushort chainType, long currentBlockId, long blockHeight, decimal percentage);
		Task WalletSyncEnded(ushort chainType, long currentBlockId, long blockHeight, decimal percentage);
		Task WalletSyncUpdate(ushort chainType, long currentBlockId, long blockHeight, decimal percentage, string estimatedTimeRemaining);
		Task WalletSyncError(ushort chainType, string error);

		Task BlockchainSyncStarted(ushort chainType, long currentBlockId, long publicBlockHeight, decimal percentage);
		Task BlockchainSyncEnded(ushort chainType, long currentBlockId, long publicBlockHeight, decimal percentage);
		Task BlockchainSyncUpdate(ushort chainType, long currentBlockId, long publicBlockHeight, decimal percentage, string estimatedTimeRemaining);
		Task BlockchainSyncError(ushort chainType, string error);

		Task TransactionSent(int correlationId, string transactionId);
		Task TransactionConfirmed(string transactionId, object transaction);
		Task TransactionReceived(string transactionId);
		Task TransactionMessage(string transactionId, string message);
		Task TransactionRefused(string transactionId, string reason);
		Task TransactionError(int correlationId, string transactionId, List<ushort> errorCodes);

		Task MiningStarted(ushort chainType);
		Task MiningEnded(ushort chainType, int status);
		Task MiningElected(ushort chainType, long electionBlockId, byte level);
		Task MiningPrimeElected(ushort chainType, long electionBlockId, byte level);
		Task MiningPrimeElectedMissed(ushort chainType, long publicationBlockId, long electionBlockId, byte level);

		Task NeuraliumMiningBountyAllocated(ushort chainType, long blockId, decimal bounty, decimal transactionTip, string delegateAccountId);
		Task NeuraliumMiningPrimeElected(ushort chainType, long electionBlockId, decimal bounty, decimal transactionTip, string delegateAccountId, byte level);

		Task BlockInserted(ushort chainType, long blockId, DateTime timestamp, string hash, long publicBlockId, int lifespan);
		Task BlockInterpreted(ushort chainType, long blockId);

		Task DigestInserted(ushort chainType, int digestId, DateTime timestamp, string hash);

		Task Message(string message, DateTime timestamp, string level, object[] properties);
		Task Error(int correlationId, string error);

		Task Alert(int messageCode);

		Task ConnectableStatusChanged(bool connectable);

		Task ShutdownCompleted();
		Task ShutdownStarted();
	}

	public interface IRpcClientEventsExtended : IRpcClientEvents {
	}

	public interface IRpcProvider : IRpcServerMethods, IRpcClientMethods {

		Task ValueOnChainEventRaised(CorrelationContext? correlationContext, BlockchainSystemEventType eventType, BlockchainType chainType, object[] extraParameters);
		void TotalPeersUpdated(int count);
		void ShutdownCompleted();
		void ShutdownStarted();
		void LogMessage(string message, DateTime timestamp, string level, object[] properties);
		bool ConsoleMessagesEnabled { get; }
	}

	public interface IRpcProvider<RPC_HUB, RCP_CLIENT> : IRpcProvider
		where RPC_HUB : RpcHub<RCP_CLIENT>
		where RCP_CLIENT : class, IRpcClient {

		IRpcService<RPC_HUB, RCP_CLIENT> RpcService { get; set; }

		IHubContext<RPC_HUB, RCP_CLIENT> HubContext { get; set; }
	}

	public class RpcProvider<RPC_HUB, RCP_CLIENT> : IRpcProvider<RPC_HUB, RCP_CLIENT>
		where RPC_HUB : RpcHub<RCP_CLIENT>
		where RCP_CLIENT : class, IRpcClient {

		private readonly object locker = new object();
		private readonly Dictionary<int, LongRunningEvents> longRunningEvents = new Dictionary<int, LongRunningEvents>();

		public bool ConsoleMessagesEnabled { get; private set; } = false;

		//private Timer maintenanceTimer;

		public RpcProvider() {
			//this.maintenanceTimer = new Timer(this.TimerCallback, this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
		}

		public IHubContext<RPC_HUB, RCP_CLIENT> HubContext { get; set; }

		public IRpcService<RPC_HUB, RCP_CLIENT> RpcService { get; set; }

		public Task<bool> ToggleServerMessages(bool enable) {

			return new TaskFactory().StartNew(() => {
				this.ConsoleMessagesEnabled = enable;

				return this.ConsoleMessagesEnabled;
			});
		}

		public Task<object> QuerySystemInfo() {
			try {

				int systemMode = 0;

#if TESTNET
				systemMode = 1;
#elif DEVNET
			systemMode = 2;
#endif

				return Task.FromResult((object) new SystemInfoAPI() {Version = GlobalSettings.SoftwareVersion.ToString(), Mode = systemMode, ConsoleEnabled = this.ConsoleMessagesEnabled});
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query system version");

				throw new HubException("Failed to query system version");
			}
		}

		public Task<List<object>> QuerySupportedChains() {
			try {
				IGlobalsService globalsService = DIService.Instance.GetService<IGlobalsService>();

				return Task.FromResult(globalsService.SupportedChains.Select(c => new SupportedChainsAPI() {Id = c.Key.Value, Name = c.Value.Name, Enabled = c.Value.Enabled, Started = c.Value.Started}).Cast<object>().ToList());
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query supported chains API");

				throw new HubException("Failed to query block heights");
			}
		}

		public Task<string> Ping() {
			return Task.FromResult("pong");
		}

		public Task<int> QueryTotalConnectedPeersCount() {
			try {
				INetworkingService networkingService = DIService.Instance.GetService<INetworkingService>();

				int count = 0;

				if(networkingService?.IsStarted ?? false) {
					count = networkingService.CurrentPeerCount;
				}

				return Task.FromResult(count);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query total peers count API");

				throw new HubException("Failed to Query total peers count");
			}
		}

		public Task<bool> QueryMiningPortConnectable() {
			try {
				INetworkingService networkingService = DIService.Instance.GetService<INetworkingService>();

				bool isConnectable = false;

				if(networkingService?.IsStarted ?? false) {
					isConnectable = networkingService.ConnectionStore.IsConnectable;
				}

				return Task.FromResult(isConnectable);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query node connectible");

				throw new HubException("Failed to Query node connectible");
			}
		}

		public Task<bool> Shutdown() {

			try {

				return new TaskFactory<bool>().StartNew(() => {
					// start a substask so it runs async from the call
					new TaskFactory<bool>().StartNew(() => this.RpcService.ShutdownRequested?.Invoke() ?? false);

					return true;
				});

			} catch(Exception ex) {
				Log.Error(ex, "Failed to request system shutdown");

				throw new HubException("Failed to request system shutdown");
			}

		}

	#region events

		/// <summary>
		///     Receive chain events and propagate them to the clients
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="chainType"></param>
		/// <param name="extraParameters"></param>
		/// <returns></returns>
		public Task ValueOnChainEventRaised(CorrelationContext? correlationContext, BlockchainSystemEventType eventType, BlockchainType chainType, object[] parameters) {

			(Task<Task> task, CorrelationContext correlationContext) result;
			var extraParameters = parameters?.ToArray() ?? new object[0];


			// this is an instant call
			if(BlockchainSystemEventTypes.Instance.IsValueBaseset(eventType)) {
				if(eventType == BlockchainSystemEventTypes.Instance.WalletSyncStarted) {
					// alert the client of the event
					return this.HubContext?.Clients?.All?.WalletSyncStarted(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.WalletSyncEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.WalletSyncEnded(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.WalletSyncUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.WalletSyncUpdate(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.BlockchainSyncStarted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockchainSyncStarted(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.BlockchainSyncEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockchainSyncEnded(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.BlockchainSyncUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockchainSyncUpdate(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.BlockInserted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockInserted(chainType.Value, (long) extraParameters[0], (DateTime) extraParameters[1], (string) extraParameters[2], (long) extraParameters[3], (int) extraParameters[4]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.BlockInterpreted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockInterpreted(chainType.Value, (long) extraParameters[0]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.DigestInserted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.DigestInserted(chainType.Value, (int) extraParameters[0], (DateTime) extraParameters[1], (string) extraParameters[2]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.Alert) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.Alert((int) extraParameters[0]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.ConnectableStatusChanged) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.ConnectableStatusChanged((bool) extraParameters[0]);

				} else if(eventType == BlockchainSystemEventTypes.Instance.TransactionReceived) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.TransactionReceived((string) extraParameters[0]);
				} else if(eventType == BlockchainSystemEventTypes.Instance.MiningStarted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningStarted(chainType.Value);
				} else if(eventType == BlockchainSystemEventTypes.Instance.MiningEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningEnded(chainType.Value, (int) extraParameters[0]);
				} else if(eventType == BlockchainSystemEventTypes.Instance.MiningElected) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningElected(chainType.Value, (long) extraParameters[0], (byte) extraParameters[1]);
				} else if(eventType == BlockchainSystemEventTypes.Instance.MiningPrimeElected) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningPrimeElected(chainType.Value, (long) extraParameters[0], (byte) extraParameters[1]);
				} else if(eventType == BlockchainSystemEventTypes.Instance.MiningPrimeElectedMissed) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningPrimeElectedMissed(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (byte) extraParameters[2]);
				} else if(eventType == BlockchainSystemEventTypes.Instance.MiningStatusChanged) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningStatusChanged(chainType.Value, (bool) extraParameters[0]);
				} else if(eventType == BlockchainSystemEventTypes.Instance.RequestWalletPassphrase) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.EnterWalletPassphrase(correlationContext?.CorrelationId??0, chainType.Value, (int) extraParameters[0], (int) extraParameters[1]);
						
				} else if(eventType == BlockchainSystemEventTypes.Instance.RequestKeyPassphrase) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.EnterKeysPassphrase(correlationContext?.CorrelationId??0, chainType.Value, (int) extraParameters[0], (Guid) extraParameters[1], extraParameters[2].ToString(), (int) extraParameters[3]);
					
				}else if(eventType == BlockchainSystemEventTypes.Instance.RequestCopyWallet) {
					// alert the client of the event
					return this.HubContext?.Clients?.All?.RequestCopyWallet(correlationContext?.CorrelationId??0, chainType.Value);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.RequestCopyKeyFile) {
						// alert the client of the event
						return this.HubContext?.Clients?.All?.RequestCopyWalletKeyFile(correlationContext?.CorrelationId??0, chainType.Value, (int) extraParameters[0], (Guid) extraParameters[1], extraParameters[2].ToString(), (int) extraParameters[3]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.AccountPublicationPOWNonceIteration) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.AccountPublicationPOWNonceIteration(correlationContext?.CorrelationId??0, (int) extraParameters[0], (int) extraParameters[1]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.AccountPublicationPOWNonceFound) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.AccountPublicationPOWNonceFound(correlationContext?.CorrelationId??0, (int) extraParameters[0], (int) extraParameters[1], (List<int>) extraParameters[2]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.TransactionError) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.TransactionError(correlationContext?.CorrelationId??0, (string) extraParameters[0], (List<ushort>) extraParameters[1]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.TransactionSent) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.TransactionSent(correlationContext?.CorrelationId??0, (string) extraParameters[0]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.KeyGenerationStarted) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.KeyGenerationStarted(correlationContext?.CorrelationId??0, (string) extraParameters[0], (int) extraParameters[1], (int) extraParameters[2]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.KeyGenerationEnded) {

						// alert the client of the event
						return this.HubContext?.Clients?.All?.KeyGenerationEnded(correlationContext?.CorrelationId??0, (string) extraParameters[0], (int) extraParameters[1], (int) extraParameters[2]);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.KeyGenerationPercentageUpdate) {
						// alert the client of the event
						return this.HubContext?.Clients?.All?.KeyGenerationPercentageUpdate(correlationContext?.CorrelationId??0, (string) extraParameters[0], (int) extraParameters[1]);

					
				} else if(eventType == BlockchainSystemEventTypes.Instance.AccountPublicationEnded) {

						// alert the client of the event
						//await this.HubContext?.Clients?.All?.AccountPublicationCompleted(sessionCorrelationId, (Guid)parameters[0], (bool)parameters[1], (long)parameters[2]);
						return this.HubContext?.Clients?.All?.AccountPublicationEnded(correlationContext?.CorrelationId??0);
					
				} else if(eventType == BlockchainSystemEventTypes.Instance.RequireNodeUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.RequireNodeUpdate((ushort) extraParameters[0], (string) extraParameters[1]);
					
				}
				else {
					Log.Debug($"Event {eventType.Value} was not handled");

					if(correlationContext.HasValue) {
						//action = (sessionCorrelationId, resetEvent) => {

							object parameter = null;

							if(extraParameters.Any()) {
								parameter = extraParameters[0];
							}

							// alert the client of the event
							return this.HubContext?.Clients?.All?.LongRunningStatusUpdate(correlationContext?.CorrelationId??0, eventType.Value, 1, parameter);
						//};
					}
				}

			} else if(BlockchainSystemEventTypes.Instance.IsValueChildset(eventType)) {
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningPrimeElected) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.NeuraliumMiningPrimeElected(chainType.Value, (long) extraParameters[0], (decimal) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3], (byte) extraParameters[4]);
				} else if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningBountyAllocated) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.NeuraliumMiningBountyAllocated(chainType.Value, (long) extraParameters[0], (decimal) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3]);
				}

				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.AccountTotalUpdated) {

					string accountId = this.GetParameterField<string>("AccountId", extraParameters[0]);
					object total = this.GetParameterField<object>("Total", extraParameters[0]);

					return this.HubContext?.Clients?.All?.AccountTotalUpdated(accountId, total);

				} else {
					Log.Debug($"Event {eventType.Value} was not handled");

					if(correlationContext.HasValue) {
					//	action = (sessionCorrelationId, resetEvent) => {

							object parameter = null;

							if((extraParameters != null) && extraParameters.Any()) {
								parameter = extraParameters[0];
							}

							// alert the client of the event
							this.HubContext?.Clients?.All?.LongRunningStatusUpdate(correlationContext?.CorrelationId??0, eventType.Value, 2, parameter);

							return default;
						//};
					}
				}
			}

			// if(action != null) {
			//
			// 	if(!correlationContext.HasValue || correlationContext.Value.IsNew) {
			// 		// if we had no previous correlation id, then its an uncorrelated event, so we give it one
			// 		result = this.CreateServerLongRunningEvent(action);
			//
			// 		return result.task;
			// 	} else {
			// 		LongRunningEvents autoEvent = null;
			//
			// 		lock(this.locker) {
			// 			autoEvent = this.longRunningEvents.ContainsKey(correlationContext.Value.CorrelationId) ? this.longRunningEvents[correlationContext.Value.CorrelationId] : null;
			// 		}
			//
			// 		return action?.Invoke(correlationContext.Value, autoEvent);
			// 	}
			// }

			return Task.FromResult(true);
		}

		public void TotalPeersUpdated(int count) {
			this.HubContext?.Clients?.All?.PeerTotalUpdated(count);
		}

		public void ShutdownStarted() {
			this.HubContext?.Clients?.All?.ShutdownStarted();
		}

		public void ShutdownCompleted() {
			this.HubContext?.Clients?.All?.ShutdownCompleted();
		}

		public void LogMessage(string message, DateTime timestamp, string level, object[] properties) {
			if(this.ConsoleMessagesEnabled) {
				this.HubContext?.Clients?.All?.Message(message, timestamp, level, properties);
			}
		}

	#endregion

	#region common chain queries

		public async Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase) {
			try {
				await this.GetChainInterface(chainType).SetWalletPassphrase(correlationId, keyCorrelationCode, passphrase).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to provide wallet passphrase");

				throw new HubException("Failed to provide wallet passphrase");
			}
		}

		public async Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase) {
			try {
				await this.GetChainInterface(chainType).SetWalletKeyPassphrase(correlationId, keyCorrelationCode, passphrase).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to provide wallet key passphrase");

				throw new HubException("Failed to provide wallet key passphrase");
			}
		}

		public async Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode) {
			try {
				await this.GetChainInterface(chainType).WalletKeyFileCopied(correlationId, keyCorrelationCode).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to alert that key file was copied");

				throw new HubException("Failed to alert that key file was copied");
			}
		}

		public async Task<bool> IsBlockchainSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).IsBlockchainSynced().awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to check if blockchains is synced");

				throw new HubException("Failed to check if blockchains is synced");
			}
		}

		public async Task<bool> IsWalletSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).IsWalletSynced().awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to check if wallet is synced");

				throw new HubException("Failed to check if wallet is synced");
			}
		}

		public async Task<bool> SyncBlockchain(ushort chainType, bool force) {
			try {
				return await this.GetChainInterface(chainType).SyncBlockchain(force).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to sync blockchain");

				throw new HubException("Failed to sync blockchain");
			}
		}

		public async Task<object> BackupWallet(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).BackupWallet().awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to backup wallet");

				throw new HubException("Failed to backup wallet");
			}
		}

		public async Task<long> QueryBlockHeight(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryBlockHeight().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query block height API");

				throw new HubException("Failed to query block heights");
			}
		}

		public async Task<object> QueryChainStatus(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryChainStatus().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query chain status API");

				throw new HubException("Failed to query chain status");
			}
		}

		public async Task<object> QueryWalletInfo(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletInfo().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query wallet info API");

				throw new HubException("Failed to query wallet info");
			}
		}

		public async Task<object> QueryBlockChainInfo(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryBlockChainInfo().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query blockchain info");

				throw new HubException("Failed to query blockchain info");
			}
		}

		public async Task<bool> IsWalletLoaded(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).IsWalletLoaded().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query if wallet is loaded API");

				throw new HubException("Failed to query if wallet is loaded");
			}
		}

		public async Task<bool> WalletExists(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).WalletExists().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query if wallet exists API");

				throw new HubException("Failed to query if wallet exists");
			}
		}

		public async Task<int> LoadWallet(ushort chainType, string passphrase = null) {
			return await this.CreateClientLongRunningEvent(async (correlationContext, resetEvent) => await this.GetChainInterface(chainType).LoadWallet(correlationContext, passphrase).awaitableTask);
		}

		public async Task<int> CreateNewWallet(ushort chainType, string accountName, bool encryptWallet, bool encryptKey, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases, bool publishAccount) {
			try {
				return await this.CreateClientLongRunningEvent(async (correlationContext, resetEvent) => await this.GetChainInterface(chainType).CreateNewWallet(correlationContext, accountName, encryptWallet, encryptKey, encryptKeysIndividually, passphrases?.ToImmutableDictionary(e => int.Parse(e.Key), e => e.Value), publishAccount).awaitableTask);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to load wallet");

				throw new HubException("Failed to load wallet");
			}
		}

		public async Task<List<object>> QueryWalletTransactionHistory(ushort chainType, Guid accountUuid) {
			try {
				var result = await this.GetChainInterface(chainType).QueryWalletTransactionHistory(accountUuid).awaitableTask;

				return result.Cast<object>().ToList();
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet transaction history");

				throw new HubException("Failed to query wallet transaction history");
			}
		}

		public async Task<object> QueryWalletTransationHistoryDetails(ushort chainType, Guid accountUuid, string transactionId) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletTransationHistoryDetails(accountUuid, transactionId).awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet transaction history details");

				throw new HubException("Failed to query wallet transaction history details");
			}
		}

		public async Task<List<object>> QueryWalletAccounts(ushort chainType) {
			try {
				var result = await this.GetChainInterface(chainType).QueryWalletAccounts().awaitableTask;

				return result.Cast<object>().ToList();
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<object> QueryWalletAccountDetails(ushort chainType, Guid accountUuid) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletAccountDetails(accountUuid).awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet account details");

				throw new HubException("Failed to load wallet account details");
			}
		}

		public async Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, Guid accountUuid) {
			try {
				TransactionId transactionId = await this.GetChainInterface(chainType).QueryWalletAccountPresentationTransactionId(accountUuid).awaitableTask;

				return transactionId.ToString();
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query account presentation transaction Id");

				throw new HubException("Failed to query account presentation transaction Id");
			}
		}

		public async Task<int> CreateAccount(ushort chainType, string accountName, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases) {
			try {
				return await this.CreateClientLongRunningEvent(async (correlationContext, resetEvent) => await this.GetChainInterface(chainType).CreateAccount(correlationContext, accountName, publishAccount, encryptKeys, encryptKeysIndividually, passphrases?.ToImmutableDictionary(e => int.Parse(e.Key), e => e.Value)).awaitableTask);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to create account");

				throw new HubException("Failed to create account");
			}
		}

		public async Task<bool> SetActiveAccount(ushort chainType, Guid accountUuid) {
			try {
				return await this.GetChainInterface(chainType).SetActiveAccount(accountUuid).awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<bool> SetWalletPassphrase(int correlationId, string passphrase) {
			try {
				await this.FullfillLongRunningEvent(correlationId, passphrase);

				return true;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<bool> SetKeysPassphrase(int correlationId, string passphrase) {
			try {
				await this.FullfillLongRunningEvent(correlationId, passphrase);

				return true;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<int> PublishAccount(ushort chainType, Guid? accountUuId) {
			return await this.CreateClientLongRunningEvent(async (correlationId, resetEvent) => await this.GetChainInterface(chainType).PresentAccountPublicly(correlationId, accountUuId).awaitableTask);
		}

		public Task StartMining(ushort chainType, string delegateAccountId) {

			var tf = new TaskFactory();

			return tf.StartNew(() => {

				// start a second task that is independent of the return of this call. this is because missing key passphrase requests can block the call, and anyways an event will be sent of mining is stated or stopped.
				tf.StartNew(() => {
					try {
						AccountId delegateId = null;

						if(!string.IsNullOrWhiteSpace(delegateAccountId)) {
							delegateId = AccountId.FromString(delegateAccountId);
						}

						this.GetChainInterface(chainType).EnableMining(delegateId);

					} catch(Exception ex) {
						Log.Error(ex, "Failed to enable mining");

						throw new HubException("Failed to enable mining");
					}
				});
			});
		}

		public Task StopMining(ushort chainType) {

			return new TaskFactory().StartNew(() => {
				try {
					this.GetChainInterface(chainType).DisableMining();

				} catch(Exception ex) {
					Log.Error(ex, "Failed to disable mining");

					throw new HubException("Failed to disable mining");
				}
			});
		}

		public Task<bool> IsMiningAllowed(ushort chainType) {
			try {
				return Task.FromResult(this.GetChainInterface(chainType).IsMiningAllowed);

			} catch(Exception ex) {
				Log.Error(ex, "Failed to verify if mining is allowed");

				throw new HubException("Failed to verify if mining is allowed");
			}
		}

		public Task<bool> IsMiningEnabled(ushort chainType) {
			try {
				return Task.FromResult(this.GetChainInterface(chainType).IsMiningEnabled);

			} catch(Exception ex) {
				Log.Error(ex, "Failed to verify if mining is enabled");

				throw new HubException("Failed to verify if mining is enabled");
			}
		}

		public async Task<bool> QueryBlockchainSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryBlockchainSynced().awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query blockchain sync status");

				throw new HubException("Failed to query blockchain sync status");
			}
		}

		public async Task<bool> QueryWalletSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletSynced().awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query wallet sync status");

				throw new HubException("Failed to query wallet sync status");
			}
		}

		public async Task<string> QueryBlock(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryBlock(blockId).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}

		public async Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryCompressedBlock(blockId).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}

		public async Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId) {
			try {
				var result = await this.GetChainInterface(chainType).QueryBlockBinaryTransactions(blockId).awaitableTask;

				return result.Select(e => new {TransactionId = e.Key.ToString(), Data = e.Value}).Cast<object>().ToList();

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query block binary transactions");

				throw new HubException("Failed to query block binary transactions");
			}
		}

		public async Task<object> QueryElectionContext(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryElectionContext(blockId).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query block election details");

				throw new HubException("Failed to query block election details");
			}
		}

		public async Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel) {
			
			try {
				var result = await this.GetChainInterface(chainType).QueryMiningHistory(page, pageSize, maxLevel).awaitableTask;
				
				return result.Cast<object>().ToList();

			} catch(Exception ex) {
				Log.Error(ex, "Failed to query block mining history");

				throw new HubException("Failed to query mining history");
			}
		}

		public async Task<bool> CreateNextXmssKey(ushort chainType, Guid accountUuid, byte ordinal) {
			try {
				return await this.GetChainInterface(chainType).CreateNextXmssKey(accountUuid, ordinal).awaitableTask;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to create next xmss key");

				throw new HubException("Failed to create next xmss key");
			}
		}

		public async Task<byte[]> SignXmssMessage(ushort chainType, Guid accountUuid, byte[] message) {
			try {
				SafeArrayHandle signature = await this.GetChainInterface(chainType).SignXmssMessage(accountUuid, ByteArray.WrapAndOwn(message)).awaitableTask;

				var result = signature.ToExactByteArrayCopy();

				signature.Return();

				return result;

			} catch(Exception ex) {
				Log.Error(ex, "Failed to sign XMSS message");

				throw new HubException("Failed to sign XMSS message");
			}
		}

	#endregion

	#region neuralium chain queries

		public async Task<object> QueryAccountTotalNeuraliums(Guid accountId) {
			try {
				return await this.GetNeuraliumChainInterface().QueryWalletTotal(accountId).awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to Query wallet total");

				throw new HubException("Failed to query wallet total");
			}
		}

		public async Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal tip, string note) {

			try {
				return await this.CreateClientLongRunningEvent(async (correlationContext, resetEvent) => await this.GetNeuraliumChainInterface().SendNeuraliums(AccountId.FromString(targetAccountId), amount, tip, note, correlationContext).awaitableTask);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to send neuraliums");

				throw new HubException("Failed to send neuraliums");
			}
		}

		public async Task<object> QueryNeuraliumTimelineHeader(Guid accountUuid) {

			try {
				return await this.GetNeuraliumChainInterface().QueryNeuraliumTimelineHeader(accountUuid);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query neuralium timeline header");

				throw new HubException("Failed to query neuralium timeline header");
			}
		}

		public async Task<List<object>> QueryNeuraliumTimelineSection(Guid accountUuid, DateTime firstday, int skip, int take) {

			try {
				var results = await this.GetNeuraliumChainInterface().QueryNeuraliumTimelineSection(accountUuid, firstday, skip, take);

				return results.Cast<object>().ToList();
			} catch(Exception ex) {
				Log.Error(ex, "Failed to query neuralium timeline header");

				throw new HubException("Failed to query neuralium timeline header");
			}
		}

#if TESTNET || DEVNET
		public async Task<int> RefillNeuraliums(Guid accountUuid) {

			try {
				return await this.CreateClientLongRunningEvent(async (correlationContext, resetEvent) => await this.GetNeuraliumChainInterface().RefillNeuraliums(accountUuid, correlationContext).awaitableTask);
			} catch(Exception ex) {
				Log.Error(ex, "Failed to refill neuraliums");

				throw new HubException("Failed to refill neuraliums");
			}
		}
#endif

		public async Task<List<object>> QueryNeuraliumTransactionPool() {
			try {
				return await this.GetNeuraliumChainInterface().QueryNeuraliumTransactionPool().awaitableTask;
			} catch(Exception ex) {
				Log.Error(ex, "Failed to neuralium transaction pool");

				throw new HubException("Failed to query neuralium transaction pool");
			}
		}

		public Task<bool> RestoreWalletBackup(string source, string dest) {
			return Task.Run(() => {

				Narballer nar = new Narballer("", new FileSystem());

				nar.Restore(dest, source, null, true);

				return true;
			});
		}

	#endregion

	#region Utils

		public T ConvertParameterType<T>(object parameter)
			where T : class, new() {

			T result = new T();

			Type resultType = typeof(T);

			foreach(PropertyInfo property in parameter.GetType().GetProperties()) {

				PropertyInfo mainProp = resultType.GetProperty(property.Name);

				if(mainProp != null) {

					mainProp.SetValue(result, property.GetValue(parameter));
				}
			}

			return result;
		}

		public T GetParameterField<T>(string fieldName, object parameter) {
			PropertyInfo mainProp = parameter.GetType().GetProperty(fieldName);

			if(mainProp != null) {

				return (T) mainProp.GetValue(parameter);
			}

			return default;
		}

		protected IBlockChainInterface GetChainInterface(ushort chainType) {
			return this.GetChainInterface<IBlockChainInterface>(chainType);
		}

		protected INeuraliumBlockChainInterface GetNeuraliumChainInterface() {
			return this.GetChainInterface<INeuraliumBlockChainInterface>(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium.Value);
		}

		protected T GetChainInterface<T>(ushort chainType)
			where T : class, IBlockChainInterface {
			BlockchainType castedChainType = chainType;

			T chainInterface = null;

			if(castedChainType.Value == NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium.Value) {
				chainInterface = (T) this.RpcService[NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium];

			} else {
				throw new HubException("Unsupported chain type");
			}

			if(chainInterface == null) {
				throw new HubException("Chain is invalid");
			}

			return chainInterface;
		}

		/// <summary>
		///     Create a long running event that may take a while to return
		/// </summary>
		/// <param name="action"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public (Task<Task> task, CorrelationContext correlationContext) CreateServerLongRunningEvent(Func<CorrelationContext, LongRunningEvents, Task> action, int timeout = 60 * 3) {
			CorrelationContext correlationContext = new CorrelationContext(GlobalRandom.GetNext());

			LongRunningEvents longRunningEvent = new LongRunningEvents(TimeSpan.FromMinutes(timeout));

			lock(this.locker) {
				this.longRunningEvents.Add(correlationContext.CorrelationId, longRunningEvent);
			}

			var task = Task.Run(async () => {

				try {
					var resultTask = action(correlationContext, longRunningEvent);

					return resultTask;
				} catch(Exception ex) {
					//TODO: what to do here?
				} finally {
					// clean up the event
					await this.CompleteLongRunningEvent(correlationContext.CorrelationId, null);
				}

				return default;
			});

			return (task, correlationContext);
		}

		/// <summary>
		///     Ensure a long running thread is provided it's value
		/// </summary>
		/// <param name="correlationId"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public Task<bool> FullfillLongRunningEvent(int correlationId, object data) {

			lock(this.locker) {
				if(!this.longRunningEvents.ContainsKey(correlationId)) {
					return Task.FromResult(true);
				}

				using(LongRunningEvents longRunningEvent = this.longRunningEvents[correlationId]) {

					longRunningEvent.State = data;

					//longRunningEvent.AutoResetEvent.Set();
				}
			}

			return Task.FromResult(true);
		}

		/// <summary>
		///     Clean the long running event from cache
		/// </summary>
		/// <param name="correlationId"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public Task<bool> CompleteLongRunningEvent(int correlationId, object data) {

			lock(this.locker) {

				var result = this.FullfillLongRunningEvent(correlationId, data);

				if(this.longRunningEvents.ContainsKey(correlationId)) {
					this.longRunningEvents.Remove(correlationId);
				}

				return result;
			}
		}

		/// <summary>
		///     If the long running event was triggered by a client, then we need to ensure that we return.
		/// </summary>
		/// <param name="action"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private Task<int> CreateClientLongRunningEvent<T>(Func<CorrelationContext, LongRunningEvents, Task<T>> action) {
			var result = this.CreateServerLongRunningEvent(async (correlationId, longRunningEvent) => {

				try {
					await action(correlationId, longRunningEvent);

					// alert clients that this correlated method has returned
					this.HubContext?.Clients?.All?.ReturnClientLongRunningEvent(correlationId.CorrelationId, 0, "");

				} catch(Exception ex) {
					this.HubContext?.Clients?.All?.ReturnClientLongRunningEvent(correlationId.CorrelationId, 1, ex.Message);

					throw ex;
				}
			});

			return Task.FromResult(result.correlationContext.CorrelationId);
		}

		/// <summary>
		///     Allow the client to inform us that we can slide the timeout. the client is still there and the wait is justified.
		/// </summary>
		/// <param name="correlationId"></param>
		/// <returns></returns>
		public Task<bool> RenewLongRunningEvent(int correlationId) {

			LongRunningEvents longRunningEvent = null;

			lock(this.locker) {
				if(this.longRunningEvents.ContainsKey(correlationId)) {
					longRunningEvent = this.longRunningEvents[correlationId];
				}
			}

			longRunningEvent?.SlideTimeout();

			return Task.FromResult(true);
		}

		public class LongRunningEvents : IDisposable {
			//public readonly ManualResetEventSlim AutoResetEvent;

			private readonly TimeSpan spanTimeout;
			public DateTime Timeout;

			public LongRunningEvents(TimeSpan timeout) {
				this.spanTimeout = this.spanTimeout;

				//this.AutoResetEvent = new ManualResetEventSlim(false);
				this.SlideTimeout();
			}

			public object State { get; set; }

			public void SlideTimeout() {
				this.Timeout = DateTime.UtcNow + this.spanTimeout;
			}

			public void Dispose() {
				//this.AutoResetEvent?.Dispose();
			}
		}

	#endregion

	}
}