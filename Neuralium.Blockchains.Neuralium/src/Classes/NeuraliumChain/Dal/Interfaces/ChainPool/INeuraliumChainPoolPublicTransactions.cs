using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainPool;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool {

	public interface INeuraliumChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS> : IChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS>
		where CHAIN_POOL_PUBLIC_TRANSACTIONS : class, INeuraliumChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS> {

		decimal Tip { get; set; }
	}

}