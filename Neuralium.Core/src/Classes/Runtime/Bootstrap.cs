using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blockchains.Neuralium.Classes.Configuration;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Services;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.General;
using Neuralium.Core.Classes.Services;
using Neuralium.Core.Controllers;
using Neuralium.Core.Resources;

using Serilog;
using Serilog.Enrichers;

namespace Neuralium.Core.Classes.Runtime {
	public class Bootstrap {

		protected const string prefix = "NEURALIUM_";
		protected const string appsettings = "config/config.json";
		
		protected const string docker_base_path = "/home/data/config.json";
		protected const string docker_appsettings = "config/docker.config.json";
		protected const string ConfigSectionName = "AppSettings";
		
		protected const string hostsettings = "hostsettings.json";
		protected NeuraliumOptions cmdOptions;

		protected string configSectionName = ConfigSectionName;
		
		public IServiceProvider ServiceProvider { get; private set; }

		static Bootstrap() {


		}

		protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) {

			services.AddSingleton<AppSettingsBase>(x => x.GetService<IOptions<AppSettings>>().Value);

			services.Configure<NeuraliumBlockChainConfigurations>(configuration.GetSection("NeuraliumBlockChainConfigurations"));

			services.AddSingleton<IFileFetchService, FileFetchService>();
			services.AddSingleton<IHttpService, HttpService>();

			services.AddSingleton<IBlockchainTimeService, BlockchainTimeService>();
			services.AddSingleton<ITimeService>(x => x.GetService<IBlockchainTimeService>());

			services.AddSingleton<IBlockchainGuidService, BlockchainGuidService>();
			services.AddSingleton<IGuidService>(x => x.GetService<IBlockchainGuidService>());

			services.AddSingleton<IGlobalsService, GlobalsService>();

			services.AddSingleton<IRpcService, RpcService<RpcHub<IRpcClient>, IRpcClient>>();
			services.AddSingleton<IRpcProvider>(x => new RpcProvider<RpcHub<IRpcClient>, IRpcClient>());

			services.AddSingleton<NeuraliumOptions, NeuraliumOptions>(x => this.cmdOptions);

			services.AddSingleton<IBlockchainNetworkingService, BlockchainNetworkingService>();
			services.AddSingleton<INetworkingService>(x => x.GetService<IBlockchainNetworkingService>());

			services.AddSingleton<IDataAccessService, DataAccessService>();

			services.AddSingleton<IBlockchainInstantiationService, BlockchainInstantiationService>();
			services.AddSingleton<IInstantiationService>(x => x.GetService<IBlockchainInstantiationService>());
		}

		protected virtual void ConfigureExtraServices(IServiceCollection services, IConfiguration configuration) {
			
			
			
#if DEBUG
			//services.AddSingleton<INeuraliumApp, NeuraliumApp>();
			services.AddSingleton<INeuraliumApp, NeuraliumAppConsole>();
#else
			services.AddSingleton<INeuraliumApp, NeuraliumApp>();
#endif
		}

		protected virtual void ConfigureInitComponents() {
			LiteDBMappers.RegisterBasics();
			NeuraliumLiteDBMappers.RegisterBasics();
		}

		protected virtual void AddHostedService(IServiceCollection services, IConfiguration configuration) {
			services.AddHostedService<NeuraliumService>();
		}

		public void SetCmdOptions(NeuraliumOptions cmdOptions) {
			this.cmdOptions = cmdOptions;
		}

		public static string GetExecutingDirectoryName() {

			return AppDomain.CurrentDomain.BaseDirectory;

			throw new ApplicationException("Invalid execution directory");
		}

		protected virtual void BuildConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder configApp) {

			IConfigurationBuilder entry = configApp.SetBasePath(GetExecutingDirectoryName());

			if(this.cmdOptions.RuntimeMode.ToUpper() == "DOCKER") {
				Console.WriteLine($"Docker mode.");
				if(File.Exists(docker_base_path)) {
					Console.WriteLine($"Loading config file {docker_base_path}");
					entry = entry.AddJsonFile(docker_base_path, false, false);
				} else {
					Console.WriteLine($"Default docker config not found. Loading config file {docker_appsettings}");
					entry = entry.AddJsonFile(docker_appsettings, false, false);
				}
			} else {
				entry = entry.AddJsonFile(appsettings, false, false);
			}

			entry.AddEnvironmentVariables().AddEnvironmentVariables(prefix);

		}

		protected virtual IHostBuilder BuildHost() {

			this.ConfigureInitComponents();

			return new HostBuilder().ConfigureHostConfiguration(configHost => {
				//					configHost.SetBasePath(GetExecutingDirectoryName());
				//					configHost.AddJsonFile(_hostsettings, optional: true);
				//					configHost.AddEnvironmentVariables(prefix: _prefix);
			}).ConfigureAppConfiguration((hostingContext, configApp) => {

				this.BuildConfiguration(hostingContext, configApp);
			}).ConfigureServices((hostContext, services) => {
				
				services.AddOptions<HostOptions>().Configure(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(10));

				
				if(!string.IsNullOrWhiteSpace(this.cmdOptions?.ConfigSection)) {
					this.configSectionName = this.cmdOptions.ConfigSection;
				}

				Log.Verbose($"Loading config section {this.configSectionName}");

				this.ConfigureAppSettings(this.configSectionName, services, hostContext.Configuration);
				
				this.ConfigureServices(services, hostContext.Configuration);

				// allow children to add their own overridable services
				this.ConfigureExtraServices(services, hostContext.Configuration);

				this.AddHostedService(services, hostContext.Configuration);

				services.Configure<HostOptions>(option => {
					option.ShutdownTimeout = TimeSpan.FromSeconds(20);
				});
			}).ConfigureLogging((hostingContext, logging) => {

				logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				logging.AddConsole();
				

			}).UseSerilog((hostingContext, loggerConfiguration) => {
				loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);

				if(!this.cmdOptions.NoRPC) {

					var section = hostingContext.Configuration.GetSection(this.configSectionName);

					var appSettingsTemplate = new AppSettings();
					AppSettingsBase.RpcModes rpcMode = appSettingsTemplate.RpcMode;

					string rpcModeName = nameof(appSettingsTemplate.RpcMode);
					if(section.GetSection(rpcModeName).Exists()) {
						rpcMode = section.GetValue<AppSettingsBase.RpcModes>(rpcModeName);
					}
					
					if(rpcMode != AppSettingsBase.RpcModes.None) {
						// now we configure the RPC logger
						
						var entry = loggerConfiguration.MinimumLevel;
						
						var loggingLevel = section.GetValue<AppSettingsBase.RpcLoggingLevels>(nameof(appSettingsTemplate.RpcLoggingLevel));
						
						if(loggingLevel == AppSettingsBase.RpcLoggingLevels.Verbose) {
							loggerConfiguration = entry.Verbose();
						} else {
							loggerConfiguration = entry.Information();
						}

						loggerConfiguration.Enrich.With(new ThreadIdEnricher()).WriteTo.RpcEventLogSink(this);
					}
				}

			}).UseConsoleLifetime();
		}

		protected virtual void ConfigureAppSettings(string configSection, IServiceCollection services, IConfiguration configuration) {
			services.Configure<AppSettings>(configuration.GetSection(configSection));
		}

		public virtual async Task<int> Run() {

			IHostBuilder hostBuilder = this.BuildHost().UseConsoleLifetime();

			var host = hostBuilder.Build();
			this.ServiceProvider = host.Services;

			try {
				Log.Information("Starting host");
				await host.RunAsync().ConfigureAwait(false);

			} catch(OperationCanceledException) {
				// thats fine, lets just exit
			} catch(Exception ex) {
				string message = NeuraliumAppTranslationsManager.Instance.Bootstrap_Run_Host_terminated_unexpectedly;

				// here we write it twice, just in case the log provider is not initialize here
				Console.WriteLine(message + "-" + ex);
				Log.Fatal(ex, message);

				return 1;
			} finally {
				Log.CloseAndFlush();
			}

			return 0;
		}
	}

}