using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes {
	public interface INeuraliumSignedMessageEnvelope : ISignedMessageEnvelope, INeuraliumMessageEnvelope {
	}

	public class NeuraliumSignedMessageEnvelope : SignedMessageEnvelope, INeuraliumSignedMessageEnvelope {
	}
}