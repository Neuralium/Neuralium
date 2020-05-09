using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Extensions;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.Network.Exceptions;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Core.Types;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Data.Arrays;
using Neuralia.Blockchains.Tools.Locking;
using Neuralia.Blockchains.Tools.Serialization;
using Neuralia.Blockchains.Tools.Threading;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.Services;
using Serilog;

namespace Neuralium.Core.Classes.Runtime {

	public interface INeuraliumApp : ILoopThread, IAppRemote {
	}

	public class NeuraliumApp : NeuraliumApp<AppSettings> {

		public NeuraliumApp(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, IRpcService rpcService, IOptions<AppSettings> appSettings, NeuraliumOptions options, IBlockchainTimeService timeService, IBlockchainNetworkingService networkingService, IGlobalsService globalService) : base(serviceProvider, applicationLifetime, rpcService, appSettings, options, timeService, networkingService, globalService) {
		}
	}

	public class NeuraliumApp<APP_SETTINGS> : LoopThread<NeuraliumApp<APP_SETTINGS>>, INeuraliumApp
		where APP_SETTINGS : AppSettings, new() {

		protected readonly IHostApplicationLifetime applicationLifetime;

		protected readonly APP_SETTINGS appSettings;

		protected readonly NeuraliumOptions CmdOptions;

		private readonly DelayedTriggerComponent delayedTriggerComponent = new DelayedTriggerComponent();
		protected readonly IGlobalsService globalService;

		protected readonly IBlockchainNetworkingService networkingService;

		protected readonly IRpcService rpcService;
		protected readonly IServiceProvider serviceProvider;
		protected readonly IBlockchainTimeService timeService;
		protected INeuraliumBlockChainInterface neuraliumBlockChainInterface;

		public NeuraliumApp(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, IRpcService rpcService, IOptions<APP_SETTINGS> appSettings, NeuraliumOptions options, IBlockchainTimeService timeService, IBlockchainNetworkingService networkingService, IGlobalsService globalService) : base(200) {

			GlobalsService.AppRemote = this;

			this.appSettings = appSettings.Value;
			this.CmdOptions = options;
			this.networkingService = networkingService;

			this.timeService = timeService;
			this.rpcService = rpcService;

			this.rpcService.ShutdownRequested += async () => {
				try {
					await this.Shutdown().ConfigureAwait(false);

					return true;
				} catch(Exception ex) {
					NLog.Default.Error(ex, "Failed to shutdown system");
				}

				return false;
			};

			//TODO: remove this
#if DEBUG
			TransactionType trxTypes = NeuraliumTransactionTypes.Instance.NEURALIUM_REFILL_NEURLIUMS;
			BlockchainSystemEventType ff = NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.AccountTotalUpdated;
#endif

			this.serviceProvider = serviceProvider;
			this.applicationLifetime = applicationLifetime;

			this.globalService = globalService;

			// register the default DI Service
			DIService.Instance.AddServiceProvider(BlockchainTypes.Instance.None, serviceProvider);

			this.delayedTriggerComponent.TriggerAchived += this.InitLateComponents;
		}

		public virtual async Task Shutdown() {

			LockContext lockContext = null;

			this.applicationLifetime.ApplicationStopping.Register(() => {

				// alert everyone shutdown has completed
				this.RunRpcCommand(() => this.rpcService.RpcProvider.ShutdownStarted());
			});

			this.applicationLifetime.ApplicationStopped.Register(() => {

				// alert everyone shutdown has completed
				this.RunRpcCommand(() => this.rpcService.RpcProvider.ShutdownCompleted());
			});

			await this.StopChains(lockContext).ConfigureAwait(false);

			this.applicationLifetime.StopApplication();
		}

		public override async Task Stop() {
			await base.Stop().ConfigureAwait(false);

			await this.Shutdown().ConfigureAwait(false);
		}

		protected virtual async Task CreateChains(LockContext lockContext) {

			if(this.appSettings.NeuraliumChainConfiguration.Enabled) {

				ChainRuntimeConfiguration chainRuntimeConfiguration = new ChainRuntimeConfiguration();

				//DEBUG
				// // mobile will work differently:
				// chainRuntimeConfiguration.ServiceExecutionTypes.Add(Enums.SERIALIZATION_SERVICE, Enums.ServiceExecutionTypes.Synchronous);
				// chainRuntimeConfiguration.ServiceExecutionTypes.Add(Enums.VALIDATION_SERVICE, Enums.ServiceExecutionTypes.Synchronous);
				// chainRuntimeConfiguration.ServiceExecutionTypes.Add(Enums.BLOCKCHAIN_SERVICE, Enums.ServiceExecutionTypes.Synchronous);
				// chainRuntimeConfiguration.ServiceExecutionTypes.Add(Enums.WALLET_SERVICE, Enums.ServiceExecutionTypes.Synchronous);
				// chainRuntimeConfiguration.ServiceExecutionTypes.Add(Enums.ASSEMBLY_SERVICE, Enums.ServiceExecutionTypes.Synchronous);
				// chainRuntimeConfiguration.ServiceExecutionTypes.Add(Enums.INTERPRETATION_SERVICE, Enums.ServiceExecutionTypes.Synchronous);

				this.globalService.AddSupportedChain(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, "Neuralium", true);
				this.neuraliumBlockChainInterface = await NeuraliumChainInstantiationFactory.Instance.CreateNewChain(this.serviceProvider, lockContext, chainRuntimeConfiguration).ConfigureAwait(false);

				this.delayedTriggerComponent.IncrementTotal();
				this.neuraliumBlockChainInterface.BlockchainStarted += this.delayedTriggerComponent.IncrementInitedComponetsCount;
				this.neuraliumBlockChainInterface.Shutdown += () => this.applicationLifetime.StopApplication();

				// register the chain to the rpc Services
				this.RunRpcCommand(() => this.rpcService[NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium] = this.neuraliumBlockChainInterface);
			}
		}

		protected virtual void InitializeChains() {

			if(this.appSettings.NeuraliumChainConfiguration.Enabled) {

			}

		}

		protected virtual async Task StartChains(LockContext lockContext) {

			if((this.neuraliumBlockChainInterface != null) && this.appSettings.NeuraliumChainConfiguration.Enabled) {
				try {

					await this.neuraliumBlockChainInterface.StartChain(lockContext).ConfigureAwait(false);
					this.globalService.SupportedChains[NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium].Started = true;
				} catch(Exception ex) {
					throw new ApplicationException("Failed to start neuralium chain", ex);
				}
			}
		}

		protected virtual async Task StopChains(LockContext lockContext) {

			if((this.neuraliumBlockChainInterface != null) && this.appSettings.NeuraliumChainConfiguration.Enabled) {
				try {

					await this.neuraliumBlockChainInterface.StopChain(lockContext).ConfigureAwait(false);
					this.globalService.SupportedChains[NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium].Started = false;
				} catch(Exception ex) {
					throw new ApplicationException("Failed to stop neuralium chain", ex);
				}
			}
		}

		/// <summary>
		///     Ensure we set the appsettings and any command line overrides
		/// </summary>
		protected virtual void SetAppSettings() {

			GlobalSettings.Instance.SetValues<NeuraliumOptionsSetter>(this.PrepareSettings());

			NLog.Default.Information($"Current software version: {GlobalSettings.SoftwareVersion}");
		}

		protected virtual GlobalSettings.GlobalSettingsParameters PrepareSettings() {
			GlobalSettings.GlobalSettingsParameters parameters = new GlobalSettings.GlobalSettingsParameters();

			// thats our current version. manually set for now.

			//FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(NeuraliumApp)).Location);
			parameters.softwareVersion = new SoftwareVersion(0, 0, 1, 5, "TESTNET trial run V", this.VersionValidationCallback);
			parameters.appSettings = this.appSettings;
			parameters.cmdOptions = this.CmdOptions;
			parameters.nodeInfo = new NodeInfo(Enums.GossipSupportTypes.Full, Enums.PeerTypes.FullNode);

#if TESTNET
			parameters.networkId = NeuraliumConstants.TEST_NETWORK_ID;
#elif DEVNET
			parameters.networkId = NeuraliumConstants.DEV_NETWORK_ID;
#else
			parameters.networkId = NeuraliumConstants.MAIN_NETWORK_ID;
#endif

			return parameters;
		}

		/// <summary>
		///     this is where we decide which versions are acceptable to us
		/// </summary>
		/// <param name="localVersion"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		private bool VersionValidationCallback(SoftwareVersion localVersion, SoftwareVersion other) {
			SoftwareVersion minimumAcceptable = new SoftwareVersion(0, 0, 1, 5);

			return (other <= localVersion) && (other >= minimumAcceptable);
		}

		protected virtual async Task InitializeApp(LockContext lockContext) {

			try {

				if(!this.appSettings.DisableTimeServer) {
					// update time
					try {
						this.timeService.InitTime();
					} catch(Exception ex) {
						NLog.Default.Error(ex, "Failed to contact time servers and initialize local time. This is critical and we cannot continue. stopping the application.");

						throw;
					}
				}

#if TESTNET
				this.DeleteObsoleteWallets();

#endif
				this.InitRpc();

				if(this.appSettings.P2PEnabled) {
					await this.InitNetworking().ConfigureAwait(false);
				}

				this.RunPreLaunchCode();

				await this.CreateChains(lockContext).ConfigureAwait(false);

				this.InitializeChains();

				this.InitEnvironment();

				await this.StartChains(lockContext).ConfigureAwait(false);

				if(this.appSettings.P2PEnabled) {
					this.delayedTriggerComponent.IncrementTotal();
					await this.StartNetworking(this.delayedTriggerComponent.IncrementInitedComponetsCount).ConfigureAwait(false);
				}

				this.delayedTriggerComponent.Start();
			} catch(P2pException p2pEx) {
				if(p2pEx.InnerException is SocketException socketException) {
					if(socketException.ErrorCode == 98) {
						// address already in use
						throw new ApplicationException("Another process is already using our port. We can not share the same port.", p2pEx);
					}
				}

				throw new ApplicationException("Failed to initialize daemon", p2pEx);

			} catch(Exception ex) {
				throw new ApplicationException("Failed to initialize daemon", ex);
			}
		}

		protected virtual void InitEnvironment() {
			if(!GlobalSettings.ApplicationSettings.SynclessMode) {
				ThreadPool.SetMaxThreads(100, 200);
			}
		}

#if TESTNET
		protected virtual void DeleteObsoleteWallets() {

			string testnetFlagFilePath = Path.Combine(GlobalsService.GetGeneralSystemFilesDirectoryPath(), ".testnet-wallet-version-flag");

			bool deleteObsoleteFolder = false;

			try {
				if(File.Exists(testnetFlagFilePath)) {
					using(SafeArrayHandle data = ByteArray.WrapAndOwn(File.ReadAllBytes(testnetFlagFilePath))) {

						using(IDataRehydrator rehydrator = DataSerializationFactory.CreateRehydrator(data)) {

							SoftwareVersion version = new SoftwareVersion();
							version.Rehydrate(rehydrator);

							if(version < GlobalSettings.SoftwareVersion) {
								deleteObsoleteFolder = true;
							}
						}
					}

				} else {
					deleteObsoleteFolder = true;
				}
			} catch {
			}

			if(deleteObsoleteFolder) {

				try {
					string neuraliumfolder = Path.Combine(GlobalsService.GetGeneralSystemFilesDirectoryPath(), GlobalsService.TOKEN_CHAIN_NAME);
					string neuraliumfolderOld = Path.Combine(GlobalsService.GetGeneralSystemFilesDirectoryPath(), "neuraliums");

					if(Directory.Exists(neuraliumfolder)) {

						NLog.Default.Warning("You have a wallet from an older TESTNET version. we are deleting it for you.");

						Directory.Delete(neuraliumfolder, true);
					}

					if(Directory.Exists(neuraliumfolderOld)) {

						NLog.Default.Warning("You have a wallet from an older TESTNET version. we are deleting it for you.");

						Directory.Delete(neuraliumfolderOld, true);
					}

					if(File.Exists(testnetFlagFilePath)) {
						File.Delete(testnetFlagFilePath);
					}
				} catch {

				}
			}

			try {
				if(!File.Exists(testnetFlagFilePath)) {
					using IDataDehydrator dehydrator = DataSerializationFactory.CreateDehydrator();
					GlobalSettings.SoftwareVersion.Dehydrate(dehydrator);

					SafeArrayHandle resultBytes = dehydrator.ToArray();

					FileExtensions.EnsureDirectoryStructure(GlobalsService.GetGeneralSystemFilesDirectoryPath());

					FileExtensions.WriteAllBytes(testnetFlagFilePath, resultBytes);
					resultBytes.Return();
				}
			} catch {

			}
		}
#endif
		/// <summary>
		///     these items will be called only when the set of dependent components will be initialized
		/// </summary>
		protected virtual void InitLateComponents() {
			this.StartRpc();
		}

		protected void RunRpcCommand(Action action) {
			if(this.appSettings.RpcMode != AppSettingsBase.RpcModes.None) {
				if(action != null) {
					action();
				}
			}
		}

		protected override sealed async Task Initialize(LockContext lockContext) {

			try {
				await base.Initialize(lockContext).ConfigureAwait(false);

				// set the important global settings
				this.SetAppSettings();

				// and now the actual app initialization
				await this.InitializeApp(lockContext).ConfigureAwait(false);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Failed to initialize app.");

				throw;
			}
		}

		protected virtual void RunPreLaunchCode() {

		}

		protected virtual void InitRpc() {

			this.RunRpcCommand(() => {
				this.networkingService.PeerConnectionsCountUpdated += this.rpcService.RpcProvider.TotalPeersUpdated;
			});
		}

		protected virtual void StartRpc() {

			this.RunRpcCommand(() => this.rpcService.Start());
		}

		protected virtual Task InitNetworking() {
			return this.networkingService.Initialize();
		}

		protected virtual Task StartNetworking(Action startedCallback) {

			this.networkingService.Started += startedCallback;

			return this.networkingService.Start();
		}

		protected override Task ProcessLoop(LockContext lockContext) {
			this.CheckShouldCancel();

			return this.RunLoop();
		}

		protected virtual Task RunLoop() {

			return Task.CompletedTask;
		}

		protected override async Task DisposeAllAsync() {

			LockContext lockContext = null;
			await base.DisposeAllAsync().ConfigureAwait(false);

			try {

				this.rpcService?.Stop();

				try {
					this.neuraliumBlockChainInterface?.StopChain(lockContext);
				} finally {
					this.neuraliumBlockChainInterface = null;
				}

				try {
					await this.networkingService.Stop().ConfigureAwait(false);
					this.networkingService.Dispose();
				} catch(Exception ex) {
					NLog.Default.Verbose("error occured", ex);
				}
			} catch(Exception ex) {
				NLog.Default.Error(ex, "failed to dispose of app server");
			}

		}
	}
}