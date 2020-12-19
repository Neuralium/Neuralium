using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Base {
	public interface INeuraliumClientChainWorkflow : IChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public abstract class NeuraliumClientChainWorkflow : ClientChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumClientChainWorkflow {
		public NeuraliumClientChainWorkflow(INeuraliumCentralCoordinator centralCoordinator, int port) : base(centralCoordinator) {
		}
	}
}