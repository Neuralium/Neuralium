using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization.Cards;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards {

	public interface INeuraliumChainOptionsDigestChannelCard : IChainOptionsDigestChannelCard {
	}

	public class NeuraliumChainOptionsDigestChannelCard : ChainOptionsDigestChannelCard, INeuraliumChainOptionsDigestChannelCard {
		protected override IChainOptionsDigestChannelCard CreateCard() {
			return new NeuraliumChainOptionsDigestChannelCard();
		}
	}
}