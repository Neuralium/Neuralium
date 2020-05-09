using Neuralium.Blockchains.Neuralium.Classes.Configuration;

namespace Neuralium.Core.Classes.Configuration {

	public interface IAppSettings : INeuraliumBlockchainAppSettings {
	}

	public class AppSettings : NeuraliumBlockchainAppSettings, IAppSettings {
	}

}