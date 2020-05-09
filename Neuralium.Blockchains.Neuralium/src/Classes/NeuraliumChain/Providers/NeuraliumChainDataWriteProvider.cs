using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumChainDataLoadProvider : IChainDataLoadProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public interface INeuraliumChainDataWriteProvider : IChainDataWriteProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainDataLoadProvider {
	}

	public class NeuraliumChainDataWriteProvider : ChainDataWriteProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainDataWriteProvider {

		public NeuraliumChainDataWriteProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}