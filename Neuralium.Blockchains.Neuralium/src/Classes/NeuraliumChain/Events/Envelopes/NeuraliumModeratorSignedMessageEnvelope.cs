using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes {

	public interface INeuraliumModeratorSignedMessageEnvelope : IModeratorSignedMessageEnvelope, INeuraliumMessageEnvelope {
		
	}
	public class NeuraliumModeratorSignedMessageEnvelope : ModeratorSignedMessageEnvelope, INeuraliumModeratorSignedMessageEnvelope {
		
	}
}