using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.DataStructures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Processors.BlockInsertionTransaction;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Processors.BlockInsertionTransaction;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumBlockchainProvider : IBlockchainProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
		List<NeuraliumTransactionPoolEntry> GetNeuraliumTransactionPool();
	}

	public class NeuraliumBlockchainProvider : BlockchainProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumBlockchainProvider {

		public NeuraliumBlockchainProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		protected INeuraliumChainPoolProvider NeuraliumChainPoolProvider => (INeuraliumChainPoolProvider) this.ChainEventPoolProvider;

		public List<NeuraliumTransactionPoolEntry> GetNeuraliumTransactionPool() {
			return this.NeuraliumChainPoolProvider.GetTransactionIdsAndTip().Select(t => new NeuraliumTransactionPoolEntry {TransactionId = t.transactionIds.ToString(), Tip = t.tip}).ToList();
		}

		protected override IBlockInsertionTransactionProcessor CreateBlockInsertionTransactionProcessor(byte moderatorKeyOrdinal) {
			return new NeuraliumBlockInsertionTransactionProcessor(this.CentralCoordinator, moderatorKeyOrdinal);
		}
	}
}