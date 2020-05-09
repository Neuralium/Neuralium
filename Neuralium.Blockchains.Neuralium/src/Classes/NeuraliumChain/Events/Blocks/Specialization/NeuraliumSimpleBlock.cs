using System.Diagnostics;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Simple;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization {

	public interface INeuraliumSimpleBlock : ISimpleBlock, INeuraliumBlock {
	}

	[DebuggerDisplay("BlockId: {BlockId}")]
	public class NeuraliumSimpleBlock : SimpleBlock, INeuraliumSimpleBlock {
	}
}