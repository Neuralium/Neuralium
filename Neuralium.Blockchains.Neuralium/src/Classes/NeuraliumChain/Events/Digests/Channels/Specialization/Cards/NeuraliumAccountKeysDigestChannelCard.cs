using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization.Cards;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards {

	public interface INeuraliumStandardAccountKeysDigestChannelCard : IStandardAccountKeysDigestChannelCard {
	}

	public class NeuraliumStandardAccountKeysDigestChannelCard : StandardAccountKeysDigestChannelCard, INeuraliumStandardAccountKeysDigestChannelCard {
		protected override IStandardAccountKeysDigestChannelCard CreateCard() {
			return new NeuraliumStandardAccountKeysDigestChannelCard();
		}
	}
}