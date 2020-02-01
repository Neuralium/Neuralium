using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {
	public interface INeuraliumJointAccountSnapshotSqliteContext : INeuraliumJointAccountSnapshotContext<NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, IJointAccountSnapshotSqliteContext<NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry> {
	}

	public class NeuraliumJointAccountSnapshotSqliteContext : JointAccountSnapshotSqliteContext<NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, INeuraliumJointAccountSnapshotSqliteContext {
		
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<NeuraliumJointAccountSnapshotSqliteEntry>(eb => {

				eb.Property(b => b.Balance).HasConversion(v => v.Value, v => (Amount) v);
				eb.Property(b => b.Balance).IsRequired(true);
			});
			
			modelBuilder.Entity<NeuraliumJointAccountAttributeSqliteEntry>(eb => {
				
			});
		}
	}
}