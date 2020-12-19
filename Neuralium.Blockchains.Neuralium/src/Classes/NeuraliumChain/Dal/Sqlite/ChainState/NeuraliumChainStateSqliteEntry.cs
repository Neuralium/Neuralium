using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainState;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.ChainState;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainState {

	public interface INeuraliumChainStateSqliteEntry : INeuraliumChainStateEntry<NeuraliumChainStateSqliteEntry, NeuraliumChainStateSqliteModeratorKeysEntry>, IChainStateSqliteEntry<NeuraliumChainStateSqliteEntry, NeuraliumChainStateSqliteModeratorKeysEntry> {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumChainStateSqliteEntry : ChainStateSqliteEntry<NeuraliumChainStateSqliteEntry, NeuraliumChainStateSqliteModeratorKeysEntry>, INeuraliumChainStateSqliteEntry {
	}
}