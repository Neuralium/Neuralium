using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {

	public interface INeuraliumAccreditationCertificateSnapshotAccountSqliteEntry : INeuraliumAccreditationCertificateSnapshotAccountEntry {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry : AccreditationCertificateSnapshotAccountSqliteEntry, INeuraliumAccreditationCertificateSnapshotAccountSqliteEntry {
	}
}