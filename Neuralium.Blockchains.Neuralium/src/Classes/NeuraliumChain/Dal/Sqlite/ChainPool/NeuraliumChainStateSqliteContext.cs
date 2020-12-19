using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.ChainPool;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainPool {
	public interface INeuraliumChainPoolSqliteContext : INeuraliumChainPoolContext<NeuraliumChainPoolSqlitePublicTransactions>, IChainPoolSqliteContext<NeuraliumChainPoolSqlitePublicTransactions> {
	}

	public class NeuraliumChainPoolSqliteContext : ChainPoolSqliteContext<NeuraliumChainPoolSqlitePublicTransactions>, INeuraliumChainPoolSqliteContext {
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);
		}
	}
}