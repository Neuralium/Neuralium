using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.Base;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Gossip.TransactionReceivedGossip.Messages.V1;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.TransactionReceivedGossip.Messages.V1 {
	public class NeuraliumTransactionCreatedGossipMessage : TransactionCreatedGossipMessage<INeuraliumTransactionEnvelope>, INeuraliumGossipWorkflowTriggerMessage<INeuraliumTransactionEnvelope> {

		public override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

		}
	}
}