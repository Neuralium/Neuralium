using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.Base;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.P2p.Connections;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Base {
	public interface INeuraliumGossipChainWorkflow : IGossipChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public abstract class NeuraliumGossipChainWorkflow<EVENT_ENVELOPE_TYPE> : GossipChainWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumGossipMessageSet<NeuraliumGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>, EVENT_ENVELOPE_TYPE>, NeuraliumGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>, EVENT_ENVELOPE_TYPE>, INeuraliumGossipChainWorkflow
		where EVENT_ENVELOPE_TYPE : class, IEnvelope {
		protected NeuraliumGossipChainWorkflow(BlockchainType chainType, INeuraliumCentralCoordinator centralCoordinator, NeuraliumGossipMessageSet<NeuraliumGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>, EVENT_ENVELOPE_TYPE> triggerMessage, PeerConnection peerConnectionn) : base(chainType, centralCoordinator, triggerMessage, peerConnectionn) {
		}
	}
}