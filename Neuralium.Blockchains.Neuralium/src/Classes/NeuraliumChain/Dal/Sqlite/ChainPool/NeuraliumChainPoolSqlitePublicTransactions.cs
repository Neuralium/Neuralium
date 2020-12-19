using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.ChainPool;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainPool {

	public interface INeuraliumChainPoolSqlitePublicTransactions : INeuraliumChainPoolPublicTransactions<NeuraliumChainPoolSqlitePublicTransactions>, IChainPoolSqlitePublicTransactions<NeuraliumChainPoolSqlitePublicTransactions> {
	}

	public class NeuraliumChainPoolSqlitePublicTransactions : ChainPoolSqlitePublicTransactions<NeuraliumChainPoolSqlitePublicTransactions>, INeuraliumChainPoolSqlitePublicTransactions {
		public decimal Tip { get; set; }
	}
}