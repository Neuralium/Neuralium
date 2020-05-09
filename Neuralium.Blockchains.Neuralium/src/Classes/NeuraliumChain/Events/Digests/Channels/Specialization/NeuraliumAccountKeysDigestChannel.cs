using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {

	public interface INeuraliumStandardAccountKeysDigestChannel : IStandardAccountKeysDigestChannel<NeuraliumStandardAccountKeysDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumStandardAccountKeysDigestChannel : StandardAccountKeysDigestChannel<NeuraliumStandardAccountKeysDigestChannelCard>, INeuraliumStandardAccountKeysDigestChannel {
		[Flags]
		public enum AccountKeysDigestChannelBands {
			Wallets
		}

		public NeuraliumStandardAccountKeysDigestChannel(int groupSize, string folder) : base(groupSize, folder) {
		}

		protected override NeuraliumStandardAccountKeysDigestChannelCard CreateNewCardInstance() {
			return new NeuraliumStandardAccountKeysDigestChannelCard();
		}

		protected override ICardUtils GetCardUtils() {
			return this.GetNeuraliumCardUtils();
		}

		protected INeuraliumCardsUtils GetNeuraliumCardUtils() {
			return NeuraliumCardsUtils.Instance;
		}
	}
}