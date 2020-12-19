using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Core.P2p.Messages.Base;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.Base {
	public interface INeuraliumGossipWorkflowTriggerMessage : IGossipWorkflowTriggerMessage {
	}

	public interface INeuraliumGossipWorkflowTriggerMessage<out EVENT_ENVELOPE_TYPE> : IBlockchainGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>, INeuraliumGossipWorkflowTriggerMessage
		where EVENT_ENVELOPE_TYPE : class, IEnvelope {
	}

	public abstract class NeuraliumGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE> : BlockchainGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>, INeuraliumGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>
		where EVENT_ENVELOPE_TYPE : class, IEnvelope {
	}
}