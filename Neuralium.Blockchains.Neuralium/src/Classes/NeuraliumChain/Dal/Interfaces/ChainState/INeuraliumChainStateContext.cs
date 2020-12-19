using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainState;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainState {

	public interface INeuraliumChainStateContext : IChainStateContext {
	}

	public interface INeuraliumChainStateContext<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> : IChainStateContext<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>, INeuraliumChainStateContext
		where MODEL_SNAPSHOT : class, INeuraliumChainStateEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>
		where MODERATOR_KEYS_SNAPSHOT : class, INeuraliumChainStateModeratorKeysEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> {
	}
}