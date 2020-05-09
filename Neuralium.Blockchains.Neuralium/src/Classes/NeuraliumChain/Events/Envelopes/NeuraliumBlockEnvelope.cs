using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes {
	public interface INeuraliumBlockEnvelope : IBlockEnvelope {
	}

	public class NeuraliumBlockEnvelope : BlockEnvelope<INeuraliumBlock>, INeuraliumBlockEnvelope {
	}
}