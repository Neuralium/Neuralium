using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General {

	public interface INeuraliumDebugMessage : IDebugMessage, INeuraliumBlockchainMessage {
	}

	public class NeuraliumDebugMessage : DebugMessage, INeuraliumDebugMessage {
	}
}