using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainState;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainState {

	public interface INeuraliumChainStateEntry : IChainStateEntry {
	}

	public interface INeuraliumChainStateEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> : IChainStateEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>, INeuraliumChainStateEntry
		where MODEL_SNAPSHOT : class, INeuraliumChainStateEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>
		where MODERATOR_KEYS_SNAPSHOT : class, INeuraliumChainStateModeratorKeysEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> {
	}

}