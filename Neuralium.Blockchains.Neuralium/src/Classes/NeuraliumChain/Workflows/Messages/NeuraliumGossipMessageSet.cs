using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.Base;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages {
	public interface INeuraliumGossipMessageSet : IBlockchainGossipMessageSet {
	}

	public interface INeuraliumGossipMessageSet<out T, EVENT_ENVELOPE_TYPE> : IBlockchainGossipMessageSet<T, EVENT_ENVELOPE_TYPE>, INeuraliumGossipMessageSet
		where T : class, IBlockchainGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>
		where EVENT_ENVELOPE_TYPE : class, IEnvelope {
	}

	public interface INeuraliumGossipMessageSet2<T, EVENT_ENVELOPE_TYPE> : IBlockchainGossipMessageSet2<T, EVENT_ENVELOPE_TYPE>
		where T : class, IBlockchainGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>
		where EVENT_ENVELOPE_TYPE : IEnvelope {
	}

	public class NeuraliumGossipMessageSet<T, EVENT_ENVELOPE_TYPE> : BlockchainGossipMessageSet<T, EVENT_ENVELOPE_TYPE>, INeuraliumGossipMessageSet<T, EVENT_ENVELOPE_TYPE>, INeuraliumGossipMessageSet2<T, EVENT_ENVELOPE_TYPE>
		where T : class, INeuraliumGossipWorkflowTriggerMessage<EVENT_ENVELOPE_TYPE>
		where EVENT_ENVELOPE_TYPE : class, IEnvelope {
	}
}