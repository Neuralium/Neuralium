using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Cryptography.TLS;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Tools;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.General;
using Neuralium.Core.Classes.Runtime;
using Neuralium.Core.Controllers;
using Serilog;

namespace Neuralium.Core.Classes.Services {

	public interface IRpcService : IDisposableExtended {

		INeuraliumBlockChainInterface NeuraliumBlockChainInterface { get; }

		IBlockChainInterface this[ushort i] { get; set; }

		IBlockChainInterface this[BlockchainType i] { get; set; }

		IRpcProvider RpcProvider { get; }

		Func<Task<bool>> ShutdownRequested { get; set; }

		bool IsStarted { get; }
		void Start();
		void Stop();
	}

	public interface IRpcService<RPC_HUB, RCP_CLIENT> : IRpcService
		where RPC_HUB : RpcHub<RCP_CLIENT>
		where RCP_CLIENT : class, IRpcClient {

		IHubContext<RPC_HUB, RCP_CLIENT> hubContext { get; }
	}

	public class RpcService<RPC_HUB, RCP_CLIENT> : IRpcService<RPC_HUB, RCP_CLIENT>
		where RPC_HUB : RpcHub<RCP_CLIENT>
		where RCP_CLIENT : class, IRpcClient {
		private readonly IOptions<AppSettings> appsettings;

		private readonly Dictionary<BlockchainType, IBlockChainInterface> chains = new Dictionary<BlockchainType, IBlockChainInterface>();
		protected readonly IRpcProvider<RPC_HUB, RCP_CLIENT> rpcProvider;

		private CancellationTokenSource cancellationToken;
		private Task rpcTask;

		private IHost rpcWebHost;
		private IServiceProvider serviceProvider;

		public RpcService(IRpcProvider rpcProvider, IServiceProvider serviceProvider, IOptions<AppSettings> appsettings) {
			this.rpcProvider = (IRpcProvider<RPC_HUB, RCP_CLIENT>) rpcProvider;
			this.rpcProvider.RpcService = this;
			this.serviceProvider = serviceProvider;
			this.appsettings = appsettings;
		}

		public Func<Task<bool>> ShutdownRequested { get; set; }
		public bool IsStarted { get; private set; }

		public IRpcProvider RpcProvider => this.rpcProvider;

		public IHubContext<RPC_HUB, RCP_CLIENT> hubContext { get; private set; }

		public void Start() {
			if(GlobalSettings.ApplicationSettings.RpcMode != AppSettingsBase.RpcModes.None) {
				try {
					if(this.rpcWebHost != null) {
						throw new ApplicationException("Rpc webhost is already running");
					}

					if(GlobalSettings.ApplicationSettings.RpcMode != AppSettingsBase.RpcModes.None) {
						this.rpcWebHost = this.BuildRpcHost(new string[0]);

						// get the signalr hub
						this.hubContext = this.rpcWebHost.Services.GetService<IHubContext<RPC_HUB, RCP_CLIENT>>();
						this.rpcProvider.HubContext = this.hubContext;

						this.cancellationToken = new CancellationTokenSource();
						this.rpcTask = this.rpcWebHost.RunAsync(this.cancellationToken.Token);
					}

					this.IsStarted = true;

				} catch(Exception ex) {
					throw new ApplicationException("Failed to start RPC Server", ex);
				}
			}
		}

		public void Stop() {
			try {
				this.cancellationToken?.Cancel();
				this.rpcWebHost?.StopAsync(TimeSpan.FromSeconds(5));
			} catch(Exception ex) {
				//TODO: im disabling this for the demo. restore error handling!!
			} finally {
				this.rpcWebHost?.Dispose();
				this.rpcWebHost = null;
				this.IsStarted = false;
				this.cancellationToken?.Dispose();
				this.cancellationToken = null;
			}
		}

		public INeuraliumBlockChainInterface NeuraliumBlockChainInterface => (INeuraliumBlockChainInterface) this[NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium];

		public IBlockChainInterface this[ushort i] {
			get => this[(BlockchainType) i];
			set => this[(BlockchainType) i] = value;
		}

		public IBlockChainInterface this[BlockchainType i] {
			get => this.chains[i];
			set {
				value.ChainEventRaised += this.rpcProvider.ValueOnChainEventRaised;
				this.chains.Add(i, value);
			}
		}

		protected IConfigurationRoot GetAspnetCoreConfiguration(string[] args) {
			IConfigurationRoot config = new ConfigurationBuilder().AddCommandLine(args).Build();

			//TODO: set this correctly
			int serverport = config.GetValue<int?>("port") ?? GlobalSettings.ApplicationSettings.RpcPort;
			string serverurls = config.GetValue<string>("server.urls") ?? $"http://*:{serverport}";

			Dictionary<string, string> configDictionary = new Dictionary<string, string> {{"server.urls", serverurls}, {"port", serverport.ToString()}};

			return new ConfigurationBuilder().AddCommandLine(args).AddInMemoryCollection(configDictionary).Build();
		}

		protected IHost BuildRpcHost(string[] args) {
			IConfigurationRoot config = this.GetAspnetCoreConfiguration(args);
			int port = config.GetValue<int?>("port") ?? GlobalsService.DEFAULT_RPC_PORT;

			IHostBuilder builder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => {
				webBuilder.UseConfiguration(config).UseContentRoot(Directory.GetCurrentDirectory()).UseKestrel(options => {

					options.AddServerHeader = false;
					IPAddress listenAddress = IPAddress.Loopback;
					
					if(GlobalSettings.ApplicationSettings.RpcBindMode == AppSettingsBase.RpcBindModes.Any) {
						listenAddress = IPAddress.Any;
					}
					else if(GlobalSettings.ApplicationSettings.RpcBindMode == AppSettingsBase.RpcBindModes.Custom && !string.IsNullOrWhiteSpace(GlobalSettings.ApplicationSettings.RpcBindingAddress)) {
						listenAddress = IPAddress.Parse(GlobalSettings.ApplicationSettings.RpcBindingAddress);
					}

					options.Listen(listenAddress, port, listenOptions => {
						if(GlobalSettings.ApplicationSettings.RpcTransport == AppSettingsBase.RpcTransports.Secured) {

							X509Certificate2 rpcCertificate = null;
							string directory = Path.GetDirectoryName(GlobalSettings.ApplicationSettings.TlsCertificate);
							string file = Path.GetFileName(GlobalSettings.ApplicationSettings.TlsCertificate);

							if(string.IsNullOrWhiteSpace(directory)) {
								directory = Bootstrap.GetExecutingDirectoryName();
							}
							string certfile = Path.Combine(directory, file);
							
							if(string.IsNullOrWhiteSpace(GlobalSettings.ApplicationSettings.TlsCertificate)) {
								// generate a certificate file
								NLog.Default.Information($"Generating a {GlobalSettings.ApplicationSettings.TlsCertificateStrength} bits TLS certificate...");
								var certificates = new TlsProvider(GlobalSettings.ApplicationSettings.TlsCertificateStrength, TlsProvider.HashStrength.Sha256).Build();
								
								rpcCertificate = certificates.localCertificate;
								File.WriteAllBytes(certfile, rpcCertificate.RawData);
								File.WriteAllBytes(certfile+".root", certificates.rootCertificate.RawData);
								
								NLog.Default.Information($"TLS certificate generated successfully and saved at {certfile}.");
							} 
							
							// Ok, load our certificate file

							if(!File.Exists(certfile)) {
								throw new ApplicationException($"The TLS certificate file path did not exist: '{certfile}'");
							}

							rpcCertificate = new X509Certificate2(File.ReadAllBytes(certfile), "");
							
							listenOptions.UseHttps(rpcCertificate);
							NLog.Default.Information($"RPC service configured in secure TLS mode.");

						}

					});

					options.Limits.MaxConcurrentConnections = 3;

				});

				this.ConfigureWebHost(webBuilder);
			});

			return builder.Build();
		}

		protected virtual void ConfigureWebHost(IWebHostBuilder builder) {
			AppSettings appsettings = this.appsettings.Value;

			builder.ConfigureServices(services => {
				//webapi RPC:

				services.AddControllers();

				//.AddApiExplorer()     // Optional (Microsoft.AspNetCore.Mvc.ApiExplorer)
				//.AddAuthorization()   // Optional if no authentication
				//.AddFormatterMappings()

				//.AddDataAnnotations() // Optional if no validation using attributes (Microsoft.AspNetCore.Mvc.DataAnnotations)
				//.AddJsonFormatters();

				//.AddCors()            // Optional (Microsoft.AspNetCore.Mvc.Cors)

				services.AddSignalR(hubOptions => {
					//hubOptions.SupportedProtocols.Clear();
#if TESTNET || DEVNET
					hubOptions.EnableDetailedErrors = true;
#endif
					hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
					hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(30);

				}).AddJsonProtocol(options => {
					options.PayloadSerializerOptions.WriteIndented = false;
				});

			}).Configure(app => {
				app.UseRouting();

				app.UseEndpoints(endpoints => {

					endpoints.MapHub<RPC_HUB>("/signal", option => {
						option.ApplicationMaxBufferSize = 0;
						option.TransportMaxBufferSize = 0;
						option.WebSockets.CloseTimeout = TimeSpan.FromDays(7);
						option.Transports = HttpTransportType.WebSockets;
					});

					endpoints.MapControllers();
				});

				if(appsettings.RpcTransport == AppSettingsBase.RpcTransports.Secured) {
					app.UseHttpsRedirection();
				}

			}).ConfigureLogging((context, logging) => {
				// remove all logging providers
				logging.ClearProviders();
			}).SuppressStatusMessages(true);
		}

	#region dispose

		protected virtual void Dispose(bool disposing) {
			if(disposing && !this.IsDisposed) {

				try {
					this.Stop();
				} catch(Exception ex) {
					NLog.Default.Error(ex, "Failed to stop");
				}
			}

			this.IsDisposed = true;
		}

		~RpcService() {
			this.Dispose(false);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool IsDisposed { get; private set; }

	#endregion

	}
}