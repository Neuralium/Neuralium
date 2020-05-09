using Neuralium.Blockchains.Neuralium.Classes;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Tools.Data.Arrays;

namespace Neuralium.Core.Classes.Configuration {
	public class NeuraliumOptionsSetter<A, O> : OptionsSetter<A, O>
		where A : AppSettings
		where O : NeuraliumOptions {

		public override void SetOptions(A appSettings, O cmdOptions) {
			base.SetOptions(appSettings, cmdOptions);

			// make sure we enable desired loggers
			NLog.EnableLoggers(appSettings);
			
			if(cmdOptions == null) {
				return;
			}

			if(cmdOptions.NoP2p) {
				appSettings.DisableP2P = cmdOptions.NoP2p;
			}

			if(cmdOptions.NoTimeServer) {
				appSettings.DisableTimeServer = cmdOptions.NoTimeServer;
			}

			if(cmdOptions.NoRPC) {
				appSettings.RpcMode = AppSettingsBase.RpcModes.None;
			}

			if(cmdOptions.SkipGenesisHashVerification) {
				appSettings.GetChainConfiguration(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium).SkipGenesisHashVerification = cmdOptions.SkipGenesisHashVerification;
			}

			if(cmdOptions.SkipDigestHashVerification) {
				appSettings.GetChainConfiguration(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium).SkipDigestHashVerification = cmdOptions.SkipDigestHashVerification;
			}

			if(cmdOptions.NoNeuraliumsChain) {
				appSettings.NeuraliumChainConfiguration.Enabled = cmdOptions.NoNeuraliumsChain;
			}

			if(cmdOptions.Port.HasValue) {
				appSettings.Port = cmdOptions.Port.Value;
			}

			if(!string.IsNullOrWhiteSpace(cmdOptions.SerializationType)) {
				if(cmdOptions.SerializationType.ToUpper() == "MASTER") {
					appSettings.SerializationType = AppSettingsBase.SerializationTypes.Master;
				} else if(cmdOptions.SerializationType.ToUpper() == "FEEDER") {
					appSettings.SerializationType = AppSettingsBase.SerializationTypes.Feeder;
				}
			}

			ByteArray.RENT_LARGE_BUFFERS = appSettings.UseArrayPools;

			if(cmdOptions.UseArrayPools.HasValue) {
				ByteArray.RENT_LARGE_BUFFERS = cmdOptions.UseArrayPools.Value;
			}
		}
	}

	public class NeuraliumOptionsSetter : NeuraliumOptionsSetter<AppSettings, NeuraliumOptions> {
	}
}