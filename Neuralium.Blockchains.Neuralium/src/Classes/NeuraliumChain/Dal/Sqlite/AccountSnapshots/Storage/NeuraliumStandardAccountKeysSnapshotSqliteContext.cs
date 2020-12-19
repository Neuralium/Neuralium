using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage.Base;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {
	public interface INeuraliumStandardAccountKeysSnapshotSqliteContext : INeuraliumStandardAccountKeysSnapshotContext<NeuraliumStandardAccountKeysSnapshotSqliteEntry>, IAccountKeysSnapshotSqliteContext<NeuraliumStandardAccountKeysSnapshotSqliteEntry> {
	}

	public class NeuraliumStandardAccountKeysSnapshotSqliteContext : StandardAccountKeysSnapshotSqliteContext<NeuraliumStandardAccountKeysSnapshotSqliteEntry>, INeuraliumStandardAccountKeysSnapshotSqliteContext {
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);

		}
	}
}