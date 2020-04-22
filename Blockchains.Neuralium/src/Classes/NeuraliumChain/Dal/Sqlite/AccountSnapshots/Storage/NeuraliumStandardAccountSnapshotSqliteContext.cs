using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {
	public interface INeuraliumStandardAccountSnapshotSqliteContext : INeuraliumStandardAccountSnapshotContext<NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry>, IStandardAccountSnapshotSqliteContext<NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry> {
	}

	public class NeuraliumStandardAccountSnapshotSqliteContext : StandardAccountSnapshotSqliteContext<NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry>, INeuraliumStandardAccountSnapshotSqliteContext {
		
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<NeuraliumStandardAccountSnapshotSqliteEntry>(eb => {

				eb.Property(b => b.Balance).IsRequired(true);
			});
			
			modelBuilder.Entity<NeuraliumStandardAccountAttributeSqliteEntry>(eb => {

			});
		}
	}
}