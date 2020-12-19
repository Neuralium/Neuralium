using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Core.P2p.Connections;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Base {
	public interface INeuraliumServerChainWorkflow : IServerChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public abstract class NeuraliumServerChainWorkflow : ServerChainWorkflow<NeuraliumWorkflowTriggerMessage, INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumServerChainWorkflow {
		public NeuraliumServerChainWorkflow(INeuraliumCentralCoordinator centralCoordinator, BlockchainTriggerMessageSet<NeuraliumWorkflowTriggerMessage> triggerMessage, PeerConnection peerConnectionn) : base(centralCoordinator, triggerMessage, peerConnectionn) {
		}
	}
}