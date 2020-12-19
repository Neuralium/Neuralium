using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {
	public interface INeuraliumChainOptionsSnapshotSqliteContext : INeuraliumChainOptionsSnapshotContext<NeuraliumChainOptionsSnapshotSqliteEntry>, IChainOptionsSnapshotSqliteContext<NeuraliumChainOptionsSnapshotSqliteEntry> {
	}

	public class NeuraliumChainOptionsSnapshotSqliteContext : ChainOptionsSnapshotSqliteContext<NeuraliumChainOptionsSnapshotSqliteEntry>, INeuraliumChainOptionsSnapshotSqliteContext {
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);

		}
	}
}