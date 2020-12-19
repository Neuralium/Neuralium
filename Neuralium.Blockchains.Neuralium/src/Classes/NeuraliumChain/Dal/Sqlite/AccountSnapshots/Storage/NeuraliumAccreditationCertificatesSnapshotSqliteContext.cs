using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {
	public interface INeuraliumAccreditationCertificatesSnapshotSqliteContext : INeuraliumAccreditationCertificatesSnapshotContext<NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, IAccreditationCertificateSnapshotSqliteContext<NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry> {
	}

	public class NeuraliumAccreditationCertificatesSnapshotSqliteContext : AccreditationCertificateSnapshotSqliteContext<NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, INeuraliumAccreditationCertificatesSnapshotSqliteContext {
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<NeuraliumAccreditationCertificateSnapshotSqliteEntry>(eb => {

				eb.Property(b => b.ProviderBountyshare).HasConversion(v => v.Value, v => (Amount) v);

				eb.Property(b => b.InfrastructureServiceFees).HasConversion(v => v.Value, v => (Amount) v);

			});
		}
	}
}