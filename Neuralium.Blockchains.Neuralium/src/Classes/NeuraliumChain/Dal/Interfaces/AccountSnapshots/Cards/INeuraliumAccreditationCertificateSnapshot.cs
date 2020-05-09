using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards {

	public interface INeuraliumAccreditationCertificateSnapshot : INeuraliumSnapshot, IAccreditationCertificateSnapshot {

		Amount ProviderBountyshare { get; set; }
		Amount InfrastructureServiceFees { get; set; }
	}

	public interface INeuraliumAccreditationCertificateSnapshot<ACCREDITATION_CERTIFICATE_SNAPSHOT_ACCOUNT> : IAccreditationCertificateSnapshot<ACCREDITATION_CERTIFICATE_SNAPSHOT_ACCOUNT>, INeuraliumAccreditationCertificateSnapshot
		where ACCREDITATION_CERTIFICATE_SNAPSHOT_ACCOUNT : INeuraliumAccreditationCertificateSnapshotAccount {
	}
}