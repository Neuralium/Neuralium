using System;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumStandardAccountSnapshotDigestChannel : IStandardAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumStandardAccountSnapshotDigestChannel : StandardAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumStandardAccountSnapshotDigestChannel {
		[Flags]
		public enum AccountSnapshotDigestChannelBands {
			Wallets
		}

		public NeuraliumStandardAccountSnapshotDigestChannel(int groupSize, string folder) : base(groupSize, folder) {
		}

		protected override NeuraliumStandardAccountSnapshotDigestChannelCard CreateNewCardInstance() {
			return new NeuraliumStandardAccountSnapshotDigestChannelCard();
		}
	}
}