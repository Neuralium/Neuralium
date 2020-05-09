using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainState;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainState {

	public interface INeuraliumChainStateDal : IChainStateDal {
	}

	public interface INeuraliumChainStateDal<CHAIN_STATE_CONTEXT, MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> : IChainStateDal<CHAIN_STATE_CONTEXT, MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>, INeuraliumChainStateDal
		where CHAIN_STATE_CONTEXT : INeuraliumChainStateContext
		where MODEL_SNAPSHOT : class, INeuraliumChainStateEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>
		where MODERATOR_KEYS_SNAPSHOT : class, INeuraliumChainStateModeratorKeysEntry<MODEL_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> {
	}
}