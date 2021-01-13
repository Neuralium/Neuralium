using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.Configuration;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.P2p.Connections;
using Neuralia.Blockchains.Core.Services;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.General;
using Neuralium.Core.Classes.Services;
using Neuralium.Core.Controllers;
using Neuralium.Core.Resources;
using Serilog;
using Serilog.Configuration;
using Serilog.Enrichers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Extensions;
using Neuralia.Blockchains.Tools;
using Serilog.Core;
using Serilog.Settings.Configuration;

namespace Neuralium.Core.Classes.Runtime {
	public class Bootstrap {

		protected const string prefix = "NEURALIUM_";
		protected const string logging = "logging.json";
		protected const string appsettings = "config/config.json";

		protected const string docker_base_path = "/home/data/config.json";
		protected const string docker_appsettings = "config/docker.config.json";
		protected const string ConfigSectionName = "AppSettings";

		protected const string LOGS_FOLDER = "logs";
		protected const string hostsettings = "hostsettings.json";
		protected NeuraliumOptions cmdOptions;

		protected string configSectionName = ConfigSectionName;

		public IServiceProvider ServiceProvider { get; private set; }

		protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) {

			services.AddSingleton<AppSettingsBase>(x => x.GetService<IOptions<AppSettings>>().Value);

			services.Configure<NeuraliumBlockChainConfigurations>(configuration.GetSection("NeuraliumBlockChainConfigurations"));

			services.AddSingleton<IFileFetchService, FileFetchService>();
			services.AddSingleton<IHttpService, HttpService>();
			
			services.AddSingleton<IPortMappingService, PortMappingService>();

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

#if DEBUG && (DEVNET || TESTNET) || COLORADO_EXCLUSION

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

			return FileUtilities.GetExecutingDirectory();

			throw new ApplicationException("Invalid execution directory");
		}

		protected virtual string GetConfigurationFilePath(bool verbose = false) {


			if(!string.IsNullOrWhiteSpace(this.cmdOptions?.ConfigFile)) {
				if(verbose)
					Console.WriteLine($"Loading user provided config file {this.cmdOptions.ConfigFile}");
				return this.cmdOptions.ConfigFile;
			}

			if (this.cmdOptions.RuntimeMode.ToUpper() == "DOCKER")
			{
				if (verbose)
					Console.WriteLine("Docker mode.");

				if (File.Exists(docker_base_path))
				{
					if (verbose)
						Console.WriteLine($"Loading config file {docker_base_path}");
					return docker_base_path;
				}
				
				if (verbose)
					Console.WriteLine($"Default docker config not found at {docker_base_path}. Loading config file {docker_appsettings}");
				return docker_appsettings;
			}

			if(verbose)
				Console.WriteLine($"Loading config file {appsettings}");
			
			return appsettings;

		}
		
		protected virtual void BuildConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder configApp) {

			IConfigurationBuilder entry = configApp.SetBasePath(GetExecutingDirectoryName());

			string path = GetConfigurationFilePath(true);
			entry = entry.AddJsonFile(path, false, false);
			
			entry.AddEnvironmentVariables().AddEnvironmentVariables(prefix);
		}
		
		protected virtual void BuildLogConfiguration(IConfigurationBuilder configApp) {

			IConfigurationBuilder entry = configApp.SetBasePath(GetExecutingDirectoryName());

			string path = GetConfigurationFilePath(false);

			string directoryName = Path.GetDirectoryName(path);
			
			entry = entry.AddJsonFile(Path.Join(directoryName, logging), false, false);
			
			entry.AddEnvironmentVariables().AddEnvironmentVariables(prefix); //not sure this is needed
		}
		
		protected virtual IHostBuilder BuildHost() {

			this.ConfigureInitComponents();

			return new HostBuilder().ConfigureHostConfiguration(configHost => {
				//					configHost.SetBasePath(GetExecutingDirectoryName());
				//					configHost.AddJsonFile(_hostsettings, optional: true);
				//					configHost.AddEnvironmentVariables(prefix: _prefix);
			}).ConfigureAppConfiguration((hostingContext, configApp) => {

				this.BuildConfiguration(hostingContext, configApp);
				this.BuildLogConfiguration(configApp);
				
			}).ConfigureServices((hostContext, services) => {

				services.AddOptions<HostOptions>().Configure(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(10));

				if(!string.IsNullOrWhiteSpace(this.cmdOptions?.ConfigSection)) {
					this.configSectionName = this.cmdOptions.ConfigSection;
				}
				
				Console.WriteLine($"Loading config section {this.configSectionName}");

				this.ConfigureAppSettings(this.configSectionName, services, hostContext.Configuration);

				this.ConfigureServices(services, hostContext.Configuration);

				// allow children to add their own overridable services
				this.ConfigureExtraServices(services, hostContext.Configuration);

				this.AddHostedService(services, hostContext.Configuration);

				services.Configure<HostOptions>(option => {
					option.ShutdownTimeout = TimeSpan.FromSeconds(20);
				});
			}).ConfigureLogging((hostingContext, logging) =>
			{
				Console.WriteLine($"logging section: {hostingContext.Configuration.GetSection("Logging")}");
				
				logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				logging.AddConsole();

			}).UseSerilog((hostingContext, loggerConfiguration) => {
				
				IConfigurationSection section = hostingContext.Configuration.GetSection(this.configSectionName);
				AppSettings appSettingsTemplate = new AppSettings();

				var logging = hostingContext.Configuration.GetSection("Serilog").GetSection("WriteTo").GetChildren();
				
				foreach(var writeTo in logging)
				{
					// here we replace the path to the logs to the base folder
					if(writeTo.GetSection("Name").Value == "File") {
						
						var path = writeTo.GetSection("Args:path");
						Console.WriteLine($"[Serilog-Debug] 'File' Logging section found, specified path is {path}.");
						if(path != null && path.Value.ToLower() == "auto") {
							
							string systemFilesPath = GlobalsService.GetGeneralSystemFilesDirectoryPath(() => section.GetValue<string>(nameof(appSettingsTemplate.SystemFilesPath)));
				
							string logsPath = Path.Combine(systemFilesPath, LOGS_FOLDER);
				
							FileExtensions.EnsureDirectoryStructure(logsPath);
							
							Console.WriteLine($"[Serilog-Debug] 'auto' path was used, logs will be written to in folder {logsPath}.");
							
							path.Value = Path.Combine(logsPath, "log-.txt");
						}
					}
				}
				// ConfigurationAssemblySource.AlwaysScanDllFiles is required because of a bug in serilog. it should be fixed version 3.2 and over and can be removed.
				// https://github.com/serilog/serilog-settings-configuration/issues/239
				loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration, ConfigurationAssemblySource.AlwaysScanDllFiles);
				
				if(!this.cmdOptions.NoRPC) {


					AppSettingsBase.RpcModes rpcMode = appSettingsTemplate.RpcMode;

					string rpcModeName = nameof(appSettingsTemplate.RpcMode);

					if(section.GetSection(rpcModeName).Exists()) {
						rpcMode = section.GetValue<AppSettingsBase.RpcModes>(rpcModeName);
					}

					if(rpcMode != AppSettingsBase.RpcModes.None) {
						// now we configure the RPC logger

						//TODO: use a different logger for RPC than console
						LoggerMinimumLevelConfiguration entry = loggerConfiguration.MinimumLevel;

						AppSettingsBase.RpcLoggingLevels loggingLevel = section.GetValue<AppSettingsBase.RpcLoggingLevels>(nameof(appSettingsTemplate.RpcLoggingLevel));

						// if(loggingLevel == AppSettingsBase.RpcLoggingLevels.Verbose) {
						// 	loggerConfiguration = entry.Verbose();
						// } else {
						// 	loggerConfiguration = entry.Information();
						// }

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

			IHost host = hostBuilder.Build();
			this.ServiceProvider = host.Services;

			try {
				NLog.Default.Information("Starting host");
				await host.RunAsync().ConfigureAwait(false);

			} catch(OperationCanceledException) {
				// thats fine, lets just exit
			} catch(Exception ex) {
				string message = NeuraliumAppTranslationsManager.Instance.Bootstrap_Run_Host_terminated_unexpectedly;

				// here we write it twice, just in case the log provider is not initialize here
				Console.WriteLine(message + "-" + ex);
				NLog.Default.Fatal(ex, message);

				return 1;
			} finally {
				Log.CloseAndFlush();
			}

			return 0;
		}
	}

}