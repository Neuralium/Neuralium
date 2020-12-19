using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Base {
	public interface INeuraliumChainWorkflow : IChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public abstract class NeuraliumChainWorkflow : ChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainWorkflow {
		public NeuraliumChainWorkflow(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}