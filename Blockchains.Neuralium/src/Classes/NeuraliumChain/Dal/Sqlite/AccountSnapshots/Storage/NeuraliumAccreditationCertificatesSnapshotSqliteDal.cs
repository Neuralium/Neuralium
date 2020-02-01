using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Factories;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Tools;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {

	public interface INeuraliumAccreditationCertificatesSnapshotSqliteDal : INeuraliumAccreditationCertificatesSnapshotDal<NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, IAccreditationCertificateSnapshotSqliteDal<NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry> {
	}

	public class NeuraliumAccreditationCertificatesSnapshotSqliteDal : AccreditationCertificateSnapshotSqliteDal<NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, INeuraliumAccreditationCertificatesSnapshotSqliteDal {

		public NeuraliumAccreditationCertificatesSnapshotSqliteDal(int groupSize, string folderPath, ServiceSet serviceSet, SoftwareVersion softwareVersion, IChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(groupSize, folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {
		}
	}
}