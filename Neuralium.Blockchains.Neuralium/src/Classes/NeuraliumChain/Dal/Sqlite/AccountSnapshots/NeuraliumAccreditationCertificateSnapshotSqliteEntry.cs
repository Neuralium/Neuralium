using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {
	public interface INeuraliumAccreditationCertificateSnapshotSqliteEntry : INeuraliumAccreditationCertificateSnapshotEntry<NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, IAccreditationCertificateSnapshotSqliteEntry<NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry> {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumAccreditationCertificateSnapshotSqliteEntry : AccreditationCertificateSnapshotSqliteEntry<NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, INeuraliumAccreditationCertificateSnapshotSqliteEntry {

		public Amount ProviderBountyshare { get; set; } = new Amount();
		public Amount InfrastructureServiceFees { get; set; } = new Amount();
	}

}