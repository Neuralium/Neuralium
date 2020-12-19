using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes {
	public interface INeuraliumPresentationTransactionEnvelope : IPresentationTransactionEnvelope, INeuraliumTransactionEnvelope {
	}
	
	public class NeuraliumPresentationTransactionEnvelope : PresentationTransactionEnvelope, INeuraliumPresentationTransactionEnvelope {
		
	}
}