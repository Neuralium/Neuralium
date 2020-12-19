using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainPool;
using Neuralia.Blockchains.Components.Transactions.Identifiers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool {

	public interface INeuraliumChainPoolDal : IChainPoolDal {
		List<(TransactionId transactionIds, decimal tip)> GetTransactionsAndTip();
	}

	public interface INeuraliumChainPoolDal<CHAIN_POOL_PUBLIC_TRANSACTIONS> : IChainPoolDal<CHAIN_POOL_PUBLIC_TRANSACTIONS>, INeuraliumChainPoolDal
		where CHAIN_POOL_PUBLIC_TRANSACTIONS : class, INeuraliumChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS> {
	}
}