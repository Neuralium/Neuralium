using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Services;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumChainNetworkingProvider : IChainNetworkingProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumChainNetworkingProvider : ChainNetworkingProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainNetworkingProvider {

		public NeuraliumChainNetworkingProvider(IBlockchainNetworkingService networkingService, INeuraliumCentralCoordinator centralCoordinator) : base(networkingService, centralCoordinator) {
		}
	}
}