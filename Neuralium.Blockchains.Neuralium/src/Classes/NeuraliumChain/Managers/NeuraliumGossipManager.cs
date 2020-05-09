using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Managers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Managers {
	public interface INeuraliumGossipManager : IGossipManager<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumGossipManager : GossipManager<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumGossipManager {

		public NeuraliumGossipManager(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}