using System.Collections.Generic;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainPool;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Components.Transactions.Identifiers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumChainPoolProvider : INeuraliumChainPoolProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumChainPoolSqliteDal, NeuraliumChainPoolSqliteContext, NeuraliumChainPoolSqlitePublicTransactions> {
	}

	/// <summary>
	///     Provide access to the event pool
	/// </summary>
	public class NeuraliumBlockchainEventPoolProvider : NeuraliumBlockchainEventPoolProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumChainPoolSqliteDal, NeuraliumChainPoolSqliteContext, NeuraliumChainPoolSqlitePublicTransactions>, INeuraliumChainPoolProvider {

		public NeuraliumBlockchainEventPoolProvider(INeuraliumCentralCoordinator centralCoordinator, IChainMiningStatusProvider miningStatusProvider) : base(centralCoordinator, miningStatusProvider) {
		}
	}

	public interface INeuraliumChainPoolProviderGenerix : IEventPoolProvider {
		List<(TransactionId transactionIds, decimal tip)> GetTransactionIdsAndTip();
	}

	public interface INeuraliumChainPoolProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_POOL_DAL, CHAIN_POOL_CONTEXT, CHAIN_POOL_PUBLIC_TRANSACTIONS> : IEventPoolProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_POOL_DAL, CHAIN_POOL_CONTEXT, CHAIN_POOL_PUBLIC_TRANSACTIONS>, INeuraliumChainPoolProviderGenerix
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_POOL_DAL : class, INeuraliumChainPoolDal<CHAIN_POOL_PUBLIC_TRANSACTIONS>
		where CHAIN_POOL_CONTEXT : class, INeuraliumChainPoolContext<CHAIN_POOL_PUBLIC_TRANSACTIONS>
		where CHAIN_POOL_PUBLIC_TRANSACTIONS : class, INeuraliumChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS> {
	}

	public abstract class NeuraliumBlockchainEventPoolProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_POOL_DAL, CHAIN_POOL_CONTEXT, CHAIN_POOL_PUBLIC_TRANSACTIONS> : BlockchainEventPoolProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_POOL_DAL, CHAIN_POOL_CONTEXT, CHAIN_POOL_PUBLIC_TRANSACTIONS>, INeuraliumChainPoolProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_POOL_DAL, CHAIN_POOL_CONTEXT, CHAIN_POOL_PUBLIC_TRANSACTIONS>
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_POOL_DAL : class, INeuraliumChainPoolDal<CHAIN_POOL_PUBLIC_TRANSACTIONS>
		where CHAIN_POOL_CONTEXT : class, INeuraliumChainPoolContext<CHAIN_POOL_PUBLIC_TRANSACTIONS>
		where CHAIN_POOL_PUBLIC_TRANSACTIONS : class, INeuraliumChainPoolPublicTransactions<CHAIN_POOL_PUBLIC_TRANSACTIONS> {

		protected NeuraliumBlockchainEventPoolProviderGenerix(CENTRAL_COORDINATOR centralCoordinator, IChainMiningStatusProvider miningStatusProvider) : base(centralCoordinator, miningStatusProvider) {
		}

		public virtual List<(TransactionId transactionIds, decimal tip)> GetTransactionIdsAndTip() {
			if(!this.EventPoolEnabled) {
				return new List<(TransactionId transactionIds, decimal tip)>(); // if disabled, we return nothing
			}

			return this.ChainPoolDal.GetTransactionsAndTip();
		}
	}
}