using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumChainDataLoadProvider : IChainDataLoadProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public interface INeuraliumChainDataWriteProvider : IChainDataWriteProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainDataLoadProvider {
	}

	public class NeuraliumChainDataWriteProvider : ChainDataWriteProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainDataWriteProvider {

		public NeuraliumChainDataWriteProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		protected override DecomposedBlockAPI CreateDecomposedBlockAPI() {
			return new NeuraliumDecomposedBlockAPI();
		}
	}
}