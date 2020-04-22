using System;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Tools;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Resources;
using Serilog;

namespace Neuralium.Core.Classes.Runtime {
	public interface INeuraliumService : IHostedService, IDisposableExtended {
	}

	public class NeuraliumService : INeuraliumService {
		public const string SLA_ACCEPTANCE = "YES";

		protected readonly IHostApplicationLifetime applicationLifetime;

		protected readonly INeuraliumApp neuraliumApp;
		protected readonly NeuraliumOptions options;

		public NeuraliumService(IHostApplicationLifetime ApplicationLifetime, INeuraliumApp neuraliumApp, NeuraliumOptions options) {

			this.applicationLifetime = ApplicationLifetime;
			this.neuraliumApp = neuraliumApp;
			this.options = options;
		}

		public async Task StartAsync(CancellationToken cancellationNeuralium) {

			try {
#if TESTNET

				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Currently in TESTNET mode");
				Console.ResetColor();
				
				this.CheckTestnetDelay();

				TimeSpan waitTime = TimeSpan.FromHours(1);

				this.pollingTimer = new Timer(state => {

					try{
						this.CheckTestnetDelay();
					}
					catch(Exception ex){
						//TODO: do something?
						Log.Error(ex, "Timer exception");
					}

				}, this, waitTime, waitTime);

#elif NET
#elif DEVNET
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Currently in DEVNET mode");
			Console.ResetColor();

			this.CheckDevnetDelay();

			TimeSpan waitTime = TimeSpan.FromHours(1);

			this.pollingTimer = new Timer(state => {

try{
				this.CheckDevnetDelay();
}
				catch(Exception ex){
					//TODO: do something?
Log.Error(ex, "Timer exception");
				}

			}, this, waitTime, waitTime);

#endif

				// lets do the legal stuff. sucks but thats how it is now...
				this.VerifySoftwareLicenseAgreement();

				Log.Information("Daemon is starting....");

				this.applicationLifetime.ApplicationStarted.Register(this.OnStarted);
				this.applicationLifetime.ApplicationStopping.Register(this.OnStopping);
				this.applicationLifetime.ApplicationStopped.Register(this.OnStopped);
				AppDomain.CurrentDomain.ProcessExit += this.CurrentDomainOnProcessExit;
				this.neuraliumApp.Error += (app, exception) => {
					Log.Error(exception, "Failed to run neuralium app. exception occured");

					Log.Information("Hit any key to exit....");

					Task task = Task.Run(() => {
						Console.ReadKey();
					}, cancellationNeuralium);

					// auto shutdown after a few seconds.
					// ReSharper disable once AsyncConverter.AsyncWait
					task.Wait(1000 * 10);

					this.applicationLifetime.StopApplication();

					return Task.CompletedTask;
				};

				await this.neuraliumApp.Start().ConfigureAwait(false);

			} catch(Exception ex) {
				
				this.applicationLifetime.StopApplication();
				
				Log.Warning($"Application service failed to start.");

			}
		}

		private void CurrentDomainOnProcessExit(object sender, EventArgs e) {
			Log.Information("Daemon shutdown requested...");
			this.applicationLifetime.StopApplication();
		}
		
		public async Task StopAsync(CancellationToken cancellationNeuralium) {

			Log.Information("Daemon shutdown in progress...");
#if TESTNET || DEVNET
			this.pollingTimer?.Dispose();
			this.autoResetEvent.Set();
#endif

			await this.neuraliumApp.Stop().ConfigureAwait(false);
			this.neuraliumApp.WaitStop(TimeSpan.FromSeconds(30));

			this.neuraliumApp.Dispose();
		}

		/// <summary>
		///     Verify and confirm the users has accepted the software license agreement
		/// </summary>
		protected virtual void VerifySoftwareLicenseAgreement() {

			// Display the TOS
			Log.Information(NeuraliumAppTranslationsManager.Instance.TOSPresentation);

			// now we check if the license has been accepted
			string slaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".neuralium-sla");

			if(this.options.AcceptSoftwareLicenseAgreement == SLA_ACCEPTANCE) {
				try {
					File.WriteAllText(slaFilePath, this.options.AcceptSoftwareLicenseAgreement);
				} catch(Exception ex) {
					Log.Error(ex, $"Failed to write software license agreement file to {slaFilePath}");

					// we can keep going, this is not critical
				}
			} else {
				bool accepted = false;

				try {
					if(File.Exists(slaFilePath)) {
						if(File.ReadAllText(slaFilePath) == SLA_ACCEPTANCE) {
							accepted = true;
						}
					}
				} catch {
				}

				if(!accepted) {
					Log.Warning("Confirm your acceptance of the terms of the software license agreement. Type \"YES\" to accept.");

					string value = Console.ReadLine();

					if(value == SLA_ACCEPTANCE) {
						File.WriteAllText(slaFilePath, value);
						accepted = true;
					}
				}

				if(!accepted) {
					Log.Fatal("The Software License Agreement has not been accepted. We can not continue.");

					throw new SLARejectedException();
				}
			}

			Log.Information("The software license agreement has been accepted.");
		}

		protected virtual void OnStarted() {
			Log.Information("Daemon is successfully started.");

			// Post-startup code goes here
		}

		protected virtual void OnStopping() {
			Log.Information("Daemon shutdown requested.");
		}

		protected virtual void OnStopped() {
			Log.Information("Daemon successfully stopped");
		}
#if TESTNET
		private Timer pollingTimer;

		private readonly ManualResetEventSlim autoResetEvent = new ManualResetEventSlim(false);
		protected virtual void CheckTestnetDelay() {
			
			//TODO: this needs review
			//TimeSpan allowDelay = TimeSpan.FromDays(5);
			//DateTime fileBuildTime = AssemblyUtils.GetBuildTimestamp(typeof(NeuraliumService));
			
			//TimeSpan allowDelay =  - fileBuildTime;
			
			//TimeSpan elapsed = DateTime.UtcNow - ;

			var limit = new DateTime(2020, 4, 26, 23, 0, 0, DateTimeKind.Utc);
			if(DateTime.UtcNow > limit) {
			
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Red;
				Log.Fatal("This TESTNET release has expired! It can not be used anymore. Please download a more recent version from https://www.neuralium.com. [EXPIRED TESTNET].");

				throw new TrialTimeoutException();
			} else {
				TimeSpan remaining = limit - DateTime.UtcNow;
			
				Log.Warning($"This TESTNET release is still valid for {remaining.Days} days and {remaining.Hours} hours.");
			}
		}
#elif DEVNET
		private Timer pollingTimer;

		private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
		protected virtual void CheckDevnetDelay() {
			//TimeSpan allowDelay = TimeSpan.FromDays(5);
			TimeSpan allowDelay = new DateTime(2019, 9, 4, 23, 0, 0, DateTimeKind.Utc) - DateTime.UtcNow;

			var limit = new DateTime(2019, 10, 14, 23, 0, 0, DateTimeKind.Utc);
			if(DateTime.UtcNow > limit) {
			
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Red;
				Log.Fatal("This TESTNET release has expired! It can not be used anymore. Please download a more recent version from https://www.neuralium.com. [EXPIRED DEVNET].");
			
				throw new TrialTimeoutException();
			} else {
				TimeSpan remaining = limit - DateTime.UtcNow;
			
				Log.Warning($"This TESTNET release is still valid for {remaining.Days} days and {remaining.Hours} hours.");
			}
		}
#endif

	#region Dispose

		public bool IsDisposed { get; private set; }

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {

			if(disposing && !this.IsDisposed) {

				try {
					try {
					this.autoResetEvent?.Dispose();
					} catch(Exception ex) {
						Log.Verbose("error occured", ex);
					}

				} catch(Exception ex) {
					Log.Error(ex, "failed to dispose of Neuralium service");
				} 
			}
			this.IsDisposed = true;
		}

		~NeuraliumService() {
			this.Dispose(false);
		}

	#endregion

	}
}