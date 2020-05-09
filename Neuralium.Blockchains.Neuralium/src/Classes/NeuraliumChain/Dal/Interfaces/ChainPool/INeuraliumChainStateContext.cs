using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainPool;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool {

	public interface INeuraliumChainPoolContext : IChainPoolContext {
	}

	public interface INeuraliumChainPoolContext<CHAIN_POOL_PUBLIC_TRANSACTIONS> : IChainPoolContext<CHAIN_POOL_PUBLIC_TRANSACTIONS>, INeuraliumChainPoolContext
		where CHAIN_POOL_PUBLIC_TRANSACTIONS : class, INeuraliumChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS> {
	}
}