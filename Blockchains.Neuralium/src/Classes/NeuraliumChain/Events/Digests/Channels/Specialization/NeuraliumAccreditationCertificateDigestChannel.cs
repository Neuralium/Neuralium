using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumAccreditationCertificateDigestChannel : IAccreditationCertificateDigestChannel<NeuraliumAccreditationCertificateDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumAccreditationCertificateDigestChannel : AccreditationCertificateDigestChannel<NeuraliumAccreditationCertificateDigestChannelCard>, INeuraliumAccreditationCertificateDigestChannel {

		public NeuraliumAccreditationCertificateDigestChannel(string folder) : base(folder) {
		}
	}
}