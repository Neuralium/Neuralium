using System.Diagnostics;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Genesis;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization {

	public interface INeuraliumGenesisBlock : IGenesisBlock, INeuraliumBlock {
	}

	[DebuggerDisplay("BlockId: {BlockId}")]
	public class NeuraliumGenesisBlock : GenesisBlock, INeuraliumGenesisBlock {
	}
}