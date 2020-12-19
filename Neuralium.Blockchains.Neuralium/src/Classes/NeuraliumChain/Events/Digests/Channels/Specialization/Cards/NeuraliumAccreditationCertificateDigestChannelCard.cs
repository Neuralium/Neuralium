using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization.Cards;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards {
	public interface INeuraliumAccreditationCertificateDigestChannelCard : IAccreditationCertificateDigestChannelCard {
	}

	public class NeuraliumAccreditationCertificateDigestChannelCard : AccreditationCertificateDigestChannelCard, INeuraliumAccreditationCertificateDigestChannelCard {

		protected override IAccreditationCertificateSnapshotAccount CreateAccreditationCertificateSnapshotAccount() {
			return new NeuraliumAccreditationCertificateSnapshotAccount();
		}

		protected override IAccreditationCertificateDigestChannelCard CreateCard() {
			return new NeuraliumAccreditationCertificateDigestChannelCard();
		}
	}
}