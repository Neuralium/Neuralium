using Neuralium.Blockchains.Neuralium.Classes.Configuration;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Configuration;
using Neuralia.Blockchains.Core.Configuration;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumChainConfigurationProvider : IChainConfigurationProvider {
		NeuraliumBlockChainConfigurations NeuraliumBlockChainConfiguration { get; }
		NeuraliumBlockChainConfigurations GetNeuraliumChainConfiguration();
	}

	/// <summary>
	///     Provide access to the chain state that is saved in the DB
	/// </summary>
	public class NeuraliumChainConfigurationProvider : ChainConfigurationProvider, INeuraliumChainConfigurationProvider {

		public override BlockChainConfigurations GetChainConfiguration() {
			return this.GetNeuraliumChainConfiguration();
		}

		public NeuraliumBlockChainConfigurations GetNeuraliumChainConfiguration() {
			return (NeuraliumBlockChainConfigurations) GlobalSettings.ApplicationSettings.GetChainConfiguration(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium);
		}

		public NeuraliumBlockChainConfigurations NeuraliumBlockChainConfiguration => this.GetNeuraliumChainConfiguration();
	}

}