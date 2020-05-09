using System.Diagnostics;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization {

	public interface INeuraliumElectionBlock : IElectionBlock, INeuraliumBlock {
	}

	[DebuggerDisplay("BlockId: {BlockId}")]
	public class NeuraliumElectionBlock : ElectionBlock, INeuraliumElectionBlock {
	}
}