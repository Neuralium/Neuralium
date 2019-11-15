using System;
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

namespace Neuralium.Core.Classes.Runtime {
	public class Bootstrap {

		protected const string prefix = "NEURALIUM_";
		protected const string appsettings = "config/config.json";
		protected const string docker_appsettings = "config/docker.config.json";

		protected const string hostsettings = "hostsettings.json";
		public static RpcProvider<RpcHub<IRpcClient>, IRpcClient> RpcProvider;
		protected NeuraliumOptions cmdOptions;

		static Bootstrap() {

			RpcProvider = new RpcProvider<RpcHub<IRpcClient>, IRpcClient>();
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
			services.AddSingleton<IRpcProvider>(x => RpcProvider);

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
				entry = entry.AddJsonFile(docker_appsettings, false, false);
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

				string configSection = "AppSettings";

				if(!string.IsNullOrWhiteSpace(this.cmdOptions?.ConfigSection)) {
					configSection = this.cmdOptions.ConfigSection;
				}

				Log.Verbose($"Loading config section {configSection}");

				this.ConfigureAppSettings(configSection, services, hostContext.Configuration);

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
			}).UseConsoleLifetime();
		}

		protected virtual void ConfigureAppSettings(string configSection, IServiceCollection services, IConfiguration configuration) {
			services.Configure<AppSettings>(configuration.GetSection(configSection));
		}

		public virtual async Task<int> Run() {

			IHostBuilder host = this.BuildHost().UseConsoleLifetime();

			try {
				Log.Information("Starting host");
				await host.RunConsoleAsync();

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