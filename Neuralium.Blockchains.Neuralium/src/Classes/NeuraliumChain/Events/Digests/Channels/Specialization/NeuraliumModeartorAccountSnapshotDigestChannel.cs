using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumModeratorAccountSnapshotDigestChannel : IModeratorAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumModeratorAccountSnapshotDigestChannel : ModeratorAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumModeratorAccountSnapshotDigestChannel {
		[Flags]
		public enum AccountSnapshotDigestChannelBands {
			Wallets
		}

		public NeuraliumModeratorAccountSnapshotDigestChannel(int groupSize, string folder) : base(groupSize, folder) {
		}

		protected override NeuraliumStandardAccountSnapshotDigestChannelCard CreateNewCardInstance() {
			return new NeuraliumStandardAccountSnapshotDigestChannelCard();
		}
	}
}