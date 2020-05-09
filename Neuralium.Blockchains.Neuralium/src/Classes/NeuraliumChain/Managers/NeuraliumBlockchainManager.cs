using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Managers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Managers {
	public interface INeuraliumBlockchainManager : IBlockchainManager<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumBlockchainManager : BlockchainManager<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumBlockchainManager {

		public NeuraliumBlockchainManager(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}