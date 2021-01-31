using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Microsoft.AspNetCore.SignalR;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Cryptography.THS.V1;
using Neuralia.Blockchains.Core.Exceptions;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.Network;
using Neuralia.Blockchains.Core.P2p.Connections;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Tools;
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

	

	public interface IRpcClientEventsExtended : INeuraliumApiEvents {
	}

	public interface IRpcProvider : IRpcServerMethods, IRpcClientMethods {
		public bool ConsoleMessagesEnabled { get; }

		public Task ValueOnChainEventRaised(CorrelationContext correlationContext, BlockchainSystemEventType eventType, BlockchainType chainType, object[] extraParameters);
		public void TotalPeersUpdated(int count);
		public void ShutdownCompleted();
		public void ShutdownStarted();
		public void LogMessage(string message, DateTime timestamp, string level, object[] properties);
	}

	public interface IRpcProvider<RPC_HUB, RCP_CLIENT> : IRpcProvider
		where RPC_HUB : RpcHub<RCP_CLIENT>
		where RCP_CLIENT : class, IRpcClient {

		public IRpcService<RPC_HUB, RCP_CLIENT> RpcService { get; set; }

		public IHubContext<RPC_HUB, RCP_CLIENT> HubContext { get; set; }
	}

	public class RpcProvider<RPC_HUB, RCP_CLIENT> : IRpcProvider<RPC_HUB, RCP_CLIENT>
		where RPC_HUB : RpcHub<RCP_CLIENT>
		where RCP_CLIENT : class, IRpcClient {

		private readonly object locker = new object();
		private readonly Dictionary<int, LongRunningEvents> longRunningEvents = new Dictionary<int, LongRunningEvents>();

		//private Timer maintenanceTimer;

		public bool ConsoleMessagesEnabled { get; private set; }

		public IHubContext<RPC_HUB, RCP_CLIENT> HubContext { get; set; }

		public IRpcService<RPC_HUB, RCP_CLIENT> RpcService { get; set; }

		public Task<bool> ToggleServerMessages(bool enable) {

			return Task.Run(() => {
				this.ConsoleMessagesEnabled = enable;

				return this.ConsoleMessagesEnabled;
			});
		}

		public async Task<int> TestP2pPort(int testPort, bool callback) {
			try {
				INetworkingService networkingService = DIService.Instance.GetService<INetworkingService>();
				
				return (int) await networkingService.TestP2pPort((PortTester.TcpTestPorts)testPort, callback).ConfigureAwait(false);
					
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to test for p2p port");

				throw new HubException("Failed to test for p2p port");
			}
		}

		public Task<object> QuerySystemInfo() {
			try {

				int systemMode = 0;

#if TESTNET
				systemMode = 1;
#elif DEVNET
			systemMode = 2;
#endif

				return Task.FromResult((object) new SystemInfoAPI {ReleaseVersion = GlobalSettings.SoftwareReleaseVersion.ToString(), BlockchainVersion = GlobalSettings.BlockchainCompatibilityVersion.ToString(), Mode = systemMode, ConsoleEnabled = this.ConsoleMessagesEnabled});
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query system version");

				throw new HubException("Failed to query system version");
			}
		}

		public Task<List<object>> QuerySupportedChains() {
			try {
				IGlobalsService globalsService = DIService.Instance.GetService<IGlobalsService>();

				return Task.FromResult(globalsService.SupportedChains.Select(c => new SupportedChainsAPI {Id = c.Key.Value, Name = c.Value.Name, Enabled = c.Value.Enabled, Started = c.Value.Started}).Cast<object>().ToList());
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query supported chains API");

				throw new HubException("Failed to query block heights");
			}
		}

		public Task<string> Ping() {
			return Task.FromResult("pong");
		}

		public async Task<object> GetPortMappingStatus()
		{
			try {
				IPortMappingService portMappingService = DIService.Instance.GetService<IPortMappingService>();
				return await portMappingService.GetPortMappingStatus().ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed tto get port mappings");

				throw new HubException("Failed to get port mappings");
			}
		}

		public Task<bool> ConfigurePortMappingMode(bool useUPnP, bool usePmP, int natDeviceIndex)
		{
			try {
				IPortMappingService portMappingService = DIService.Instance.GetService<IPortMappingService>();
				return portMappingService.ConfigurePortMappingMode(useUPnP, usePmP, natDeviceIndex);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to get port mappings");

				throw new HubException("Failed to get port mappings");
			}
		}

		public Task<byte> GetPublicIPMode() {
			try {
				INetworkingService networkingService = DIService.Instance.GetService<INetworkingService>();

				IPMode mode = IPMode.Unknown;

				if(networkingService?.IsStarted ?? false) {
					mode = networkingService.ConnectionStore.PublicIpMode;
				}

				return Task.FromResult((byte)mode);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query public IP Mode");

				throw new HubException("Failed to Query public IP Mode");
			}
		}

		public Task SetUILocale(string locale) {
			try {
				GlobalSettings.Instance.SetLocale(locale);
				NLog.Default.Information($"Locale changed to {locale}");
				
				return Task.CompletedTask;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to set locale");

				throw new HubException("Failed to set locale");
			}
		}

		public async Task<byte> GetMiningRegistrationIpMode(ushort chainType) {
			try {
				IPMode mode = await this.GetChainInterface(chainType).GetMiningRegistrationIpMode().awaitableTask.ConfigureAwait(false);

				return (byte) mode;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query chain status API");

				throw new HubException("Failed to query chain status");
			}
		}
		public Task<List<object>> QueryPeerConnectionDetails() {
			try {
				INetworkingService networkingService = DIService.Instance.GetService<INetworkingService>();

				if(networkingService?.IsStarted ?? false)
					return Task.FromResult(networkingService.ConnectionStore.PeerConnectionsDetails.ToArray().ToList<object>());

				return Task.FromResult(new List<object>());
			} catch(Exception ex)
			{
				var message = $"{nameof(QueryPeerConnectionDetails)} Failed";
				NLog.Default.Error(ex, message);
				throw new HubException(message);
			}
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
				NLog.Default.Error(ex, "Failed to Query total peers count API");

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
				NLog.Default.Error(ex, "Failed to Query node connectible");

				throw new HubException("Failed to Query node connectible");
			}
		}

		public Task<bool> Shutdown() {

			try {

				return Task.Run(async () => {
					// start a substask so it runs async from the call
					await Task.Run(async () => {

						if(this.RpcService.ShutdownRequested == null) {
							return false;
						}

						await this.RpcService.ShutdownRequested().ConfigureAwait(false);

						return true;
					}).ConfigureAwait(false);

					return true;
				});

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to request system shutdown");

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
		public Task ValueOnChainEventRaised(CorrelationContext correlationContext, BlockchainSystemEventType eventType, BlockchainType chainType, object[] parameters) {

			object[] extraParameters = parameters?.ToArray() ?? Array.Empty<object>();
			
			// this is an instant call
			if(BlockchainSystemEventTypes.Instance.IsValueBaseset(eventType)) {
				if(eventType == BlockchainSystemEventTypes.Instance.WalletSyncStarted) {
					// alert the client of the event
					return this.HubContext?.Clients?.All?.WalletSyncStarted(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.WalletSyncEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.WalletSyncEnded(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.WalletSyncUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.WalletSyncUpdate(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.BlockchainSyncStarted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockchainSyncStarted(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.BlockchainSyncEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockchainSyncEnded(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.BlockchainSyncUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockchainSyncUpdate(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.BlockInserted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockInserted(chainType.Value, (long) extraParameters[0], (DateTime) extraParameters[1], (string) extraParameters[2], (long) extraParameters[3], (int) extraParameters[4]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.BlockInterpreted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.BlockInterpreted(chainType.Value, (long) extraParameters[0]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.DigestInserted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.DigestInserted(chainType.Value, (int) extraParameters[0], (DateTime) extraParameters[1], (string) extraParameters[2]);

				}
				
				if(eventType == BlockchainSystemEventTypes.Instance.Message) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.Message(chainType.Value, ((ReportableMessageType)extraParameters[0]).Value, extraParameters[1].ToString(), null);
				}
				if(eventType == BlockchainSystemEventTypes.Instance.Alert) {

					// alert the client of the event
					var exception = (ReportableException) extraParameters[0];
					return this.HubContext?.Clients?.All?.Alert(chainType.Value, exception.ErrorType.Value, exception.Message, (int)exception.PriorityLevel, (int)exception.ReportLevel, exception.Parameters);
				}
				if(eventType == BlockchainSystemEventTypes.Instance.ImportantWalletUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.ImportantWalletUpdate(chainType.Value);
				}
				
				if(eventType == BlockchainSystemEventTypes.Instance.ConnectableStatusChanged) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.ConnectableStatusChanged((bool) extraParameters[0]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.TransactionReceived) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.TransactionReceived(chainType.Value, (string) extraParameters[0]);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.MiningStarted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningStarted(chainType.Value);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.MiningEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningEnded(chainType.Value, (int) extraParameters[0]);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.MiningElected) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningElected(chainType.Value, (long) extraParameters[0], (byte) extraParameters[1]);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.MiningPrimeElected) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningPrimeElected(chainType.Value, (long) extraParameters[0], (byte) extraParameters[1]);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.MiningPrimeElectedMissed) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningPrimeElectedMissed(chainType.Value, (long) extraParameters[0], (long) extraParameters[1], (byte) extraParameters[2]);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.MiningStatusChanged) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.MiningStatusChanged(chainType.Value, (bool) extraParameters[0]);
				}

				if(eventType == BlockchainSystemEventTypes.Instance.RequestWalletPassphrase) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.EnterWalletPassphrase(correlationContext?.CorrelationId ?? 0, chainType.Value, (int) extraParameters[0], (int) extraParameters[1]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.RequestKeyPassphrase) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.EnterKeysPassphrase(correlationContext?.CorrelationId ?? 0, chainType.Value, (int) extraParameters[0], (string) extraParameters[1], extraParameters[2].ToString(), (int) extraParameters[3]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.RequestCopyWallet) {
					// alert the client of the event
					return this.HubContext?.Clients?.All?.RequestCopyWallet(correlationContext?.CorrelationId ?? 0, chainType.Value);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.RequestCopyKeyFile) {
					// alert the client of the event
					return this.HubContext?.Clients?.All?.RequestCopyWalletKeyFile(correlationContext?.CorrelationId ?? 0, chainType.Value, (int) extraParameters[0], (string) extraParameters[1], extraParameters[2].ToString(), (int) extraParameters[3]);

				}
				//
				// if(eventType == BlockchainSystemEventTypes.Instance.AccountPublicationTHSNonceIteration) {
				//
				// 	// alert the client of the event
				// 	return this.HubContext?.Clients?.All?.AccountPublicationTHSNonceIteration(correlationContext?.CorrelationId ?? 0, (int) extraParameters[0], (long) extraParameters[1]);
				//
				// }
				//
				// if(eventType == BlockchainSystemEventTypes.Instance.AccountPublicationTHSNonceFound) {
				//
				// 	// alert the client of the event
				// 	return this.HubContext?.Clients?.All?.AccountPublicationTHSNonceFound(correlationContext?.CorrelationId ?? 0, (int) extraParameters[0], (long) extraParameters[1], (List<int>) extraParameters[2]);
				//
				// }

				if(eventType == BlockchainSystemEventTypes.Instance.TransactionError) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.TransactionError(correlationContext?.CorrelationId ?? 0,chainType.Value,  (extraParameters[0]?.ToString()??""), (List<ushort>) extraParameters[1]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.TransactionSent) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.TransactionSent(correlationContext?.CorrelationId ?? 0, chainType.Value, (string) extraParameters[0]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.KeyGenerationStarted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.KeyGenerationStarted(correlationContext?.CorrelationId ?? 0,chainType.Value,  (string) extraParameters[0], (int) extraParameters[1], (int) extraParameters[2]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.KeyGenerationEnded) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.KeyGenerationEnded(correlationContext?.CorrelationId ?? 0,chainType.Value,  (string) extraParameters[0], (int) extraParameters[1], (int) extraParameters[2]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.KeyGenerationPercentageUpdate) {
					// alert the client of the event
					return this.HubContext?.Clients?.All?.KeyGenerationPercentageUpdate(correlationContext?.CorrelationId ?? 0, chainType.Value, (string) extraParameters[0], (int) extraParameters[1]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.AccountPublicationEnded) {

					// alert the client of the event
					//await this.HubContext?.Clients?.All?.AccountPublicationCompleted(sessionCorrelationId, (Guid)parameters[0], (bool)parameters[1], (long)parameters[2]);
					return this.HubContext?.Clients?.All?.AccountPublicationEnded(correlationContext?.CorrelationId ?? 0, chainType.Value);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.RequireNodeUpdate) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.RequireNodeUpdate((ushort) extraParameters[0], (string) extraParameters[1]);

				}

				if(eventType == BlockchainSystemEventTypes.Instance.TransactionHistoryUpdated) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.TransactionHistoryUpdated((ushort) extraParameters[0]);
				}
				
				if(eventType == BlockchainSystemEventTypes.Instance.ElectionContextCached) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.ElectionContextCached((ushort) extraParameters[0], (long) extraParameters[1], (long) extraParameters[2], (long) extraParameters[3]);
				}
				
				if(eventType == BlockchainSystemEventTypes.Instance.ElectionProcessingCompleted) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.ElectionProcessingCompleted((ushort) extraParameters[0], (long) extraParameters[1], (int) extraParameters[2]);
				}
				
				if(eventType == BlockchainSystemEventTypes.Instance.Error) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.Error(correlationContext?.CorrelationId ?? 0, (ushort) extraParameters[0], extraParameters[1]?.ToString()??"");
				}
				if(eventType == BlockchainSystemEventTypes.Instance.AppointmentPuzzleBegin) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.AppointmentPuzzleBegin(chainType.Value, (int) extraParameters[0], ((List<string>) extraParameters[1]), ((List<string>) extraParameters[2]));
				}
				if(eventType == BlockchainSystemEventTypes.Instance.AppointmentVerificationCompleted) {

					// alert the client of the event

					return this.HubContext?.Clients?.All?.AppointmentVerificationCompleted(chainType.Value, (bool) extraParameters[0], ((long?) extraParameters[1])?.ToString()??"");
				}
				if(eventType == BlockchainSystemEventTypes.Instance.InvalidPuzzleEngineVersion) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.InvalidPuzzleEngineVersion(chainType.Value, (int) extraParameters[0], (int) extraParameters[1], (int) extraParameters[2]);
				}
				
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.THSTrigger) {
					return this.HubContext?.Clients?.All?.THSTrigger(chainType.Value);
				}
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.THSBegin) {

					return this.HubContext?.Clients?.All?.THSBegin(chainType.Value, (long) extraParameters[0], (int) extraParameters[1], (long) extraParameters[2], (long) extraParameters[3], (long) extraParameters[4], (long) extraParameters[5], (int) extraParameters[6], (long) extraParameters[7], (long[]) extraParameters[8], (int[]) extraParameters[9]);
				}
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.THSRound) {
					return this.HubContext?.Clients?.All?.THSRound(chainType.Value, (int) extraParameters[0], (long) extraParameters[1], (int) extraParameters[2]);
				}
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.THSIteration) {
					return this.HubContext?.Clients?.All?.THSIteration(chainType.Value, (long[]) extraParameters[0], (long)extraParameters[1], (long) extraParameters[2], (long) extraParameters[3], (double) extraParameters[4]);
				}
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.THSSolution) {
					var solutionSet = (THSSolutionSet) extraParameters[0];
					return this.HubContext?.Clients?.All?.THSSolution(chainType.Value,solutionSet.Solutions.Select(e => e.nonce).ToList(), solutionSet.Solutions.Select(e => e.solution).ToList(), (long) extraParameters[1]);
				}
				
				NLog.Default.Debug($"Event {eventType.Value} was not explicitly handled");

				if(correlationContext != null) {
					//action = (sessionCorrelationId, resetEvent) => {

					object parameter = null;

					if(extraParameters.Any()) {
						parameter = extraParameters[0];
					}

					// alert the client of the event
					return this.HubContext?.Clients?.All?.LongRunningStatusUpdate(correlationContext?.CorrelationId ?? 0, eventType.Value, 1, chainType.Value, parameter);

					//};
				}

			} else if(BlockchainSystemEventTypes.Instance.IsValueChildset(eventType)) {
				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningPrimeElected) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.NeuraliumMiningPrimeElected(chainType.Value, (long) extraParameters[0], (decimal) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3], (byte) extraParameters[4]);
				}

				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningBountyAllocated) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.NeuraliumMiningBountyAllocated(chainType.Value, (long) extraParameters[0], (decimal) extraParameters[1], (decimal) extraParameters[2], (string) extraParameters[3]);
				}

				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumTimelineUpdated) {

					// alert the client of the event
					return this.HubContext?.Clients?.All?.NeuraliumTimelineUpdated();
				}

				if(eventType == NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.AccountTotalUpdated) {

					string accountId = this.GetParameterField<string>("AccountId", extraParameters[0]);
					object total = this.GetParameterField<object>("Total", extraParameters[0]);

					return this.HubContext?.Clients?.All?.AccountTotalUpdated(accountId, total);
				}

				NLog.Default.Debug($"Event {eventType.Value} was not handled");

				if(correlationContext != null) {
					//	action = (sessionCorrelationId, resetEvent) => {

					object parameter = null;

					if((extraParameters != null) && extraParameters.Any()) {
						parameter = extraParameters[0];
					}

					// alert the client of the event
					this.HubContext?.Clients?.All?.LongRunningStatusUpdate(correlationContext?.CorrelationId ?? 0, eventType.Value, 2, chainType.Value, parameter);

					return default;

					//};
				}
			}

			// if(action != null) {
			//
			// 	if(!correlationContext.HasValue || correlationContext.Value.IsNew) {
			// 		// if we had no previous correlation id, then its an unVerified event, so we give it one
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
				this.HubContext?.Clients?.All?.ConsoleMessage(message, timestamp, level, properties);
			}
		}

	#endregion

	#region common chain queries

		public async Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase, bool setKeysToo = false) {
			try {
				await this.GetChainInterface(chainType).SetWalletPassphrase(correlationId, keyCorrelationCode, passphrase).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to provide wallet passphrase");

				throw new HubException("Failed to provide wallet passphrase");
			}
		}

		public async Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase) {
			try {
				await this.GetChainInterface(chainType).SetWalletKeyPassphrase(correlationId, keyCorrelationCode, passphrase).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to provide wallet key passphrase");

				throw new HubException("Failed to provide wallet key passphrase");
			}
		}

		public async Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode) {
			try {
				await this.GetChainInterface(chainType).WalletKeyFileCopied(correlationId, keyCorrelationCode).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to alert that key file was copied");

				throw new HubException("Failed to alert that key file was copied");
			}
		}

		public async Task<bool> IsBlockchainSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).IsBlockchainSynced().awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to check if blockchains is synced");

				throw new HubException("Failed to check if blockchains is synced");
			}
		}

		public Task<bool> IsWalletSynced(ushort chainType) {
			try {
				return this.GetChainInterface(chainType).IsWalletSynced().awaitableTask;

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to check if wallet is synced");

				throw new HubException("Failed to check if wallet is synced");
			}
		}

		public async Task<int> GetCurrentOperatingMode(ushort chainType) {
			try {
				var operatingMode = await this.GetChainInterface(chainType).GetCurrentOperatingMode().awaitableTask.ConfigureAwait(false);

				return (int)operatingMode;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to get operating mode");

				throw new HubException("Failed to get operating mode");
			}
		}

		public async Task<bool> SyncBlockchain(ushort chainType, bool force) {
			try {
				return await this.GetChainInterface(chainType).SyncBlockchain(force).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to sync blockchain");

				throw new HubException("Failed to sync blockchain");
			}
		}

		public async Task<object> BackupWallet(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).BackupWallet().awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to backup wallet");

				throw new HubException("Failed to backup wallet");
			}
		}

		public Task<bool> RestoreWalletFromBackup(ushort chainType, string backupsPath, string passphrase, string salt, string nonce, int iterations) {
			try {
				return this.GetChainInterface(chainType).RestoreWalletFromBackup(backupsPath, passphrase, salt, nonce, iterations).awaitableTask;

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to restore wallet");

				throw new ApplicationException("Failed to restore wallet");
			}
		}
		
		public Task<bool> AttemptWalletRescue(ushort chainType) {
			try {
				return this.GetChainInterface(chainType).AttemptWalletRescue().awaitableTask;

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to attempt wallet rescue");

				throw new ApplicationException("Failed to attempt wallet rescue");
			}
		}

		public async Task<long> QueryBlockHeight(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryBlockHeight().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query block height API");

				throw new HubException("Failed to query block heights");
			}
		}
		public async Task<int> QueryDigestHeight(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryDigestHeight().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query block height API");

				throw new HubException("Failed to query block heights");
			}
		}
		

		public async Task<long> QueryLowestAccountBlockSyncHeight(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryLowestAccountBlockSyncHeight().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query Lowest Account Block Sync Height");

				throw new HubException("Failed to Query Lowest Account Block Sync Height");
			}
		}

		public async Task<object> QueryChainStatus(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryChainStatus().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query chain status API");

				throw new HubException("Failed to query chain status");
			}
		}

		public async Task<object> QueryWalletInfo(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletInfo().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query wallet info API");

				throw new HubException("Failed to query wallet info");
			}
		}

		public async Task<object> QueryBlockChainInfo(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryBlockChainInfo().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query blockchain info");

				throw new HubException("Failed to query blockchain info");
			}
		}

		public async Task<bool> IsWalletLoaded(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).IsWalletLoaded().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query if wallet is loaded API");

				throw new HubException("Failed to query if wallet is loaded");
			}
		}

		public async Task<bool> WalletExists(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).WalletExists().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query if wallet exists API");

				throw new HubException("Failed to query if wallet exists");
			}
		}

		public Task<int> LoadWallet(ushort chainType, string passphrase = null) {
			return this.CreateClientLongRunningEvent((correlationContext, resetEvent) => this.GetChainInterface(chainType).LoadWallet(correlationContext, passphrase).awaitableTask);
		}

		public async Task<object> QueryAppointmentConfirmationResult(ushort chainType, string accountCode) {
			try {
				return await this.GetChainInterface(chainType).QueryAppointmentConfirmationResult(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to check if account can be published wallet");

				throw new HubException("Failed to load wallet");
			}
		}

		public async Task<bool> ClearAppointment(ushort chainType, string accountCode) {
			try {
				return await this.GetChainInterface(chainType).ClearAppointment(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to clear appointment");

				throw new HubException("Failed to clear appointment");
			}
		}
		
		public async Task<object> CanPublishAccount(ushort chainType, string accountCode) {
			try {
				return await this.GetChainInterface(chainType).CanPublishAccount(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to check if account can be published wallet");

				throw new HubException("Failed to load wallet");
			}
		}

		public Task SetSMSConfirmationCode(ushort chainType, string accountCode, long confirmationCode) {
			try {
				return this.GetChainInterface(chainType).SetSMSConfirmationCode(accountCode, confirmationCode).awaitableTask;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to set SMS confirmation code");

				throw new HubException("Failed to set SMS confirmation code");
			}
		}
		
		public Task SetSMSConfirmationCodeString(ushort chainType, string accountCode, string confirmationCode) {
			if(long.TryParse(confirmationCode, out long code)) {
				return this.SetSMSConfirmationCode(chainType, accountCode, code);
			}
			
			throw new HubException("Invalid confirmation code");
		}

		public async Task GenerateXmssKeyIndexNodeCache(ushort chainType, string accountCode, byte ordinal, long index) {
			try {
				await this.GetChainInterface(chainType).GenerateXmssKeyIndexNodeCache(accountCode, ordinal, index).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to GenerateXmssKeyIndexNodeCache");

				throw new HubException("Failed to GenerateXmssKeyIndexNodeCache");
			}
		}

		public async Task<int> CreateNewWallet(ushort chainType, string accountName, int accountType, bool encryptWallet, bool encryptKey, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases, bool publishAccount) {
			try {
				return await this.CreateClientLongRunningEvent((correlationContext, resetEvent) => this.GetChainInterface(chainType).CreateNewWallet(correlationContext, accountName, (Enums.AccountTypes)accountType, encryptWallet, encryptKey, encryptKeysIndividually, passphrases?.ToImmutableDictionary(e => int.Parse(e.Key), e => e.Value), publishAccount).awaitableTask).ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to load wallet");

				throw new HubException("Failed to load wallet");
			}
		}

		public async Task<List<object>> QueryWalletTransactionHistory(ushort chainType, string accountCode) {
			try {
				List<WalletTransactionHistoryHeaderAPI> result = await this.GetChainInterface(chainType).QueryWalletTransactionHistory(accountCode).awaitableTask.ConfigureAwait(false);

				return result.Cast<object>().ToList();
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet transaction history");

				throw new HubException("Failed to query wallet transaction history");
			}
		}

		public async Task<object> QueryWalletTransactionHistoryDetails(ushort chainType, string accountCode, string transactionId) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletTransactionHistoryDetails(accountCode, transactionId).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet transaction history details");

				throw new HubException("Failed to query wallet transaction history details");
			}
		}

		public async Task<List<object>> QueryWalletAccounts(ushort chainType) {
			try {
				List<WalletAccountAPI> result = await this.GetChainInterface(chainType).QueryWalletAccounts().awaitableTask.ConfigureAwait(false);

				return result.Cast<object>().ToList();
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<string> QueryDefaultWalletAccountId(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryDefaultWalletAccountId().awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet default account id");

				throw new HubException("Failed to query wallet default account id");
			}
		}

		public async Task<string> QueryDefaultWalletAccountCode(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryDefaultWalletAccountCode().awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet default account uuid");

				throw new HubException("Failed to query wallet default account uuid");
			}
		}

		public async Task<object> QueryWalletAccountDetails(ushort chainType, string accountCode) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletAccountDetails(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet account details");

				throw new HubException("Failed to load wallet account details");
			}
		}
		
		public async Task<object> QueryWalletAccountAppointmentDetails(ushort chainType, string accountCode) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletAccountAppointmentDetails(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet account appointment details");

				throw new HubException("Failed to load wallet account appointment details");
			}
		}
		

		public async Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, string accountCode) {
			try {
				TransactionId transactionId = await this.GetChainInterface(chainType).QueryWalletAccountPresentationTransactionId(accountCode).awaitableTask.ConfigureAwait(false);

				return transactionId.ToString();
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query account presentation transaction Id");

				throw new HubException("Failed to query account presentation transaction Id");
			}
		}

		public Task<string> Test(string data) {
			try {
				return this.GetNeuraliumChainInterface().Test(data).awaitableTask;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to test");

				throw new HubException("Failed to test");
			}
		}

		public Task<int> RequestAppointment(ushort chainType, string accountCode, int preferredRegion) {
			try {
				return this.GetChainInterface(chainType).RequestAppointment(accountCode, preferredRegion).awaitableTask;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to request appointment");

				throw new HubException("Failed to request appointment");
			}
		}

		public async Task<int> CreateStandardAccount(ushort chainType, string accountName, int accountType, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases) {
			try {
				return await this.CreateClientLongRunningEvent((correlationContext, resetEvent) => this.GetChainInterface(chainType).CreateStandardAccount(correlationContext, accountName, (Enums.AccountTypes)accountType, publishAccount, encryptKeys, encryptKeysIndividually, passphrases?.ToImmutableDictionary(e => int.Parse(e.Key), e => e.Value)).awaitableTask).ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to create account");

				throw new HubException("Failed to create account");
			}
		}

		public async Task<bool> SetActiveAccount(ushort chainType, string accountCode) {
			try {
				return await this.GetChainInterface(chainType).SetActiveAccount(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<bool> SetWalletPassphrase(int correlationId, string passphrase, bool setKeysToo = false) {
			try {
				await this.FullfillLongRunningEvent(correlationId, (passphrase, setKeysToo)).ConfigureAwait(false);

				return true;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public async Task<bool> SetKeysPassphrase(int correlationId, string passphrase) {
			try {
				await this.FullfillLongRunningEvent(correlationId, passphrase).ConfigureAwait(false);

				return true;
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet accounts");

				throw new HubException("Failed to load wallet accounts");
			}
		}

		public Task<int> PublishAccount(ushort chainType, string accountCode) {
			return this.CreateClientLongRunningEvent((correlationId, resetEvent) => this.GetChainInterface(chainType).PresentAccountPublicly(correlationId, accountCode).awaitableTask);
		}

		public Task StartMining(ushort chainType, string delegateAccountId, int tier = 0) {

			return Task.Run(() => {

				// start a second task that is independent of the return of this call. this is because missing key passphrase requests can block the call, and anyways an event will be sent of mining is stated or stopped.
				Task task = Task.Run(async () => {
					try {
						AccountId delegateId = null;

						if(!string.IsNullOrWhiteSpace(delegateAccountId)) {
							delegateId = AccountId.FromString(delegateAccountId);
						}

						await this.GetChainInterface(chainType).EnableMining(null, tier, delegateId).ConfigureAwait(false);

					} catch(Exception ex) {
						NLog.Default.Error(ex, "Failed to enable mining");

						throw new HubException("Failed to enable mining");
					}
				});
			});
		}

		public Task StopMining(ushort chainType) {

			return Task.Run(async () => {
				try {
					await this.GetChainInterface(chainType).DisableMining(null).ConfigureAwait(false);

				} catch(Exception ex) {
					NLog.Default.Error(ex, "Failed to disable mining");

					throw new HubException("Failed to disable mining");
				}
			});
		}

		public Task<bool> IsMiningAllowed(ushort chainType) {
			try {
				return Task.FromResult(this.GetChainInterface(chainType).IsMiningAllowed);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to verify if mining is allowed");

				throw new HubException("Failed to verify if mining is allowed");
			}
		}

		public Task<bool> IsMiningEnabled(ushort chainType) {
			try {
				return Task.FromResult(this.GetChainInterface(chainType).IsMiningEnabled);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to verify if mining is enabled");

				throw new HubException("Failed to verify if mining is enabled");
			}
		}

		public async Task<bool> QueryBlockchainSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryBlockchainSynced().awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query blockchain sync status");

				throw new HubException("Failed to query blockchain sync status");
			}
		}

		public async Task<bool> QueryWalletSynced(ushort chainType) {
			try {
				return await this.GetChainInterface(chainType).QueryWalletSynced().awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query wallet sync status");

				throw new HubException("Failed to query wallet sync status");
			}
		}
		
		public async Task<string> QueryDecomposedBlockJson(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryDecomposedBlockJson(blockId).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}
		
		public async Task<object> QueryDecomposedBlock(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryDecomposedBlock(blockId).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}

		public async Task<string> QueryBlock(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryBlock(blockId).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}

		public async Task<byte[]> QueryBlockBytes(ushort chainType, long blockId) {
			try {
				var data = await this.GetChainInterface(chainType).LoadBlockBytes(blockId).awaitableTask.ConfigureAwait(false);

				return data?.ToExactByteArrayCopy();

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}
		
		public async Task<string> GetBlockSizeAndHash(ushort chainType, long blockId) {
			try {
				var result = await this.GetChainInterface(chainType).GetBlockSizeAndHash(blockId).awaitableTask.ConfigureAwait(false);

				if(result == null) {
					return null;
				}

				return JsonSerializer.Serialize(new GetBlockSizeAndHashAPI{Hash = result.Value.hash.ToExactByteArrayCopy(), Channels = result.Value.sizes.Entries.Select(e => e.Key.ToString()).ToArray(), Sizes = result.Value.sizes.Entries.Select(e => e.Value).ToArray()});

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}
		
		public async Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryCompressedBlock(blockId).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block");

				throw new HubException("Failed to query block");
			}
		}

		public async Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId) {
			try {
				Dictionary<TransactionId, byte[]> result = await this.GetChainInterface(chainType).QueryBlockBinaryTransactions(blockId).awaitableTask.ConfigureAwait(false);

				return result.Select(e => new {TransactionId = e.Key.ToString(), Data = e.Value}).Cast<object>().ToList();

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block binary transactions");

				throw new HubException("Failed to query block binary transactions");
			}
		}
		

		public async Task<object> QueryElectionContext(ushort chainType, long blockId) {
			try {
				return await this.GetChainInterface(chainType).QueryElectionContext(blockId).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block election details");

				throw new HubException("Failed to query block election details");
			}
		}

		public async Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel) {

			try {
				List<MiningHistory> result = await this.GetChainInterface(chainType).QueryMiningHistory(page, pageSize, maxLevel).awaitableTask.ConfigureAwait(false);

				return result.Cast<object>().ToList();

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block mining history");

				throw new HubException("Failed to query mining history");
			}
		}

		public async Task<object> QueryMiningStatistics(ushort chainType) {
			try {
				var result = (await this.GetChainInterface(chainType).QueryMiningStatistics(null).awaitableTask.ConfigureAwait(false));
				return (object)result;
				
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block mining statistics");

				throw new HubException("Failed to query mining statistics");
			}
		}

		public async Task<bool> ClearCachedCredentials(ushort chainType) {
			try {
				return (await this.GetChainInterface(chainType).ClearCachedCredentials().awaitableTask.ConfigureAwait(false));

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to clear cached mining credentials");

				throw new HubException("Failed to clear cached mining credentials");
			}
		}

		public async Task<long> QueryCurrentDifficulty(ushort chainType) {
			try {
				return (await this.GetChainInterface(chainType).QueryCurrentDifficulty().awaitableTask.ConfigureAwait(false));
				
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query block mining difficulty");

				throw new HubException("Failed to query mining difficulty");
			}
		}

		public async Task<bool> CreateNextXmssKey(ushort chainType, string accountCode, byte ordinal) {
			try {
				return await this.GetChainInterface(chainType).CreateNextXmssKey(accountCode, ordinal).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to create next xmss key");

				throw new HubException("Failed to create next xmss key");
			}
		}
		
		public async Task<byte[]> SignXmssMessage(ushort chainType, string accountCode, byte[] message) {
			try {
				SafeArrayHandle signature = await this.GetChainInterface(chainType).SignXmssMessage(accountCode, SafeArrayHandle.WrapAndOwn(message)).awaitableTask.ConfigureAwait(false);

				byte[] result = signature.ToExactByteArrayCopy();

				signature.Return();

				return result;

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to sign XMSS message");

				throw new HubException("Failed to sign XMSS message");
			}
		}

		public async Task SetPuzzleAnswers(ushort chainType, List<int> answers) {
			try {
				await this.GetChainInterface(chainType).SetPuzzleAnswers(answers).awaitableTask.ConfigureAwait(false);
				
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to set puzzle answer");

				throw new HubException("Failed to set puzzle answer");
			}
		}

	#endregion

	#region neuralium chain queries

		public async Task<object> QueryAccountTotalNeuraliums(string accountCode) {
			try {
				return await this.GetNeuraliumChainInterface().QueryWalletTotal(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to Query wallet total");

				throw new HubException("Failed to query wallet total");
			}
		}

		public async Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal tip, string note) {

			try {
				return await this.CreateClientLongRunningEvent((correlationContext, resetEvent) => this.GetNeuraliumChainInterface().SendNeuraliums(AccountId.FromString(targetAccountId), amount, tip, note, correlationContext).awaitableTask).ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to send neuraliums");

				throw new HubException("Failed to send neuraliums");
			}
		}

		public async Task<object> QueryNeuraliumTimelineHeader(string accountCode) {

			try {
				return await this.GetNeuraliumChainInterface().QueryNeuraliumTimelineHeader(accountCode).awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query neuralium timeline header");

				throw new HubException("Failed to query neuralium timeline header");
			}
		}

		public async Task<object> QueryNeuraliumTimelineSection(string accountCode, DateTime day) {

			try {
				return await this.GetNeuraliumChainInterface().QueryNeuraliumTimelineSection(accountCode, day).awaitableTask.ConfigureAwait(false);

			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to query neuralium timeline header");

				throw new HubException("Failed to query neuralium timeline header");
			}
		}

#if TESTNET || DEVNET
		public async Task<int> RefillNeuraliums(string accountCode) {

			try {
				return await this.CreateClientLongRunningEvent((correlationContext, resetEvent) => this.GetNeuraliumChainInterface().RefillNeuraliums(accountCode, correlationContext).awaitableTask).ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to refill neuraliums");

				throw new HubException("Failed to refill neuraliums");
			}
		}
#endif
#if COLORADO_EXCLUSION
		public Task<bool> BypassAppointmentVerification(string accountCode) {
			return this.GetNeuraliumChainInterface().BypassAppointmentVerification(accountCode).awaitableTask;
		}
#endif

		public async Task<List<object>> QueryNeuraliumTransactionPool() {
			try {
				return await this.GetNeuraliumChainInterface().QueryNeuraliumTransactionPool().awaitableTask.ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to neuralium transaction pool");

				throw new HubException("Failed to query neuralium transaction pool");
			}
		}

		public Task<bool> RestoreWalletNarballBackup(string source, string dest) {
			return Task.Run(() => {

				using FileSystemWrapper fileSystem = FileSystemWrapper.CreatePhysical();
				Narballer nar = new Narballer("", fileSystem);

				nar.Restore(dest, source, null);

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
		public (Task task, CorrelationContext correlationContext) CreateServerLongRunningEvent(Func<CorrelationContext, LongRunningEvents, Task> action, int timeout = 60 * 3) {
			CorrelationContext correlationContext = new CorrelationContext(GlobalRandom.GetNext());

			LongRunningEvents longRunningEvent = new LongRunningEvents(TimeSpan.FromMinutes(timeout));

			lock(this.locker) {
				this.longRunningEvents.Add(correlationContext.CorrelationId, longRunningEvent);
			}

			Task startedTask = Task<Task>.Factory.StartNew(async () => {
				try {
					await action(correlationContext, longRunningEvent).ConfigureAwait(false);
				} finally {
					// clean up the event
					await this.CompleteLongRunningEvent(correlationContext.CorrelationId, null).ConfigureAwait(false);
				}
			}, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

			return (startedTask, correlationContext);
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

				Task<bool> result = this.FullfillLongRunningEvent(correlationId, data);

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
			(Task task, CorrelationContext correlationContext) result = this.CreateServerLongRunningEvent(async (correlationId, longRunningEvent) => {

				try {
					await action(correlationId, longRunningEvent).ConfigureAwait(false);

					// alert clients that this Verified method has returned
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

			public void Dispose() {
				//this.AutoResetEvent?.Dispose();
			}

			public void SlideTimeout() {
				this.Timeout = DateTimeEx.CurrentTime + this.spanTimeout;
			}
		}

	#endregion

	}
}