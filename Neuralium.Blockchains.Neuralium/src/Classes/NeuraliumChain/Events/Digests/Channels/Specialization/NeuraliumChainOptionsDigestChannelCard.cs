using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumChainOptionsDigestChannel : IChainOptionsDigestChannel<NeuraliumChainOptionsDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumChainOptionsDigestChannel : ChainOptionsDigestChannel<NeuraliumChainOptionsDigestChannelCard>, INeuraliumChainOptionsDigestChannel {

		public NeuraliumChainOptionsDigestChannel(string folder) : base(folder) {
		}
	}
}