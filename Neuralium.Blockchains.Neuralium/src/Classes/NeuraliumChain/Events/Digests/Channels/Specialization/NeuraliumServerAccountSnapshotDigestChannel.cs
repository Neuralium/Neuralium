using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumServerAccountSnapshotDigestChannel : IServerAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumServerAccountSnapshotDigestChannel : ServerAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumServerAccountSnapshotDigestChannel {
		[Flags]
		public enum AccountSnapshotDigestChannelBands {
			Wallets
		}

		public NeuraliumServerAccountSnapshotDigestChannel(int groupSize, string folder) : base(groupSize, folder) {
		}

		protected override NeuraliumStandardAccountSnapshotDigestChannelCard CreateNewCardInstance() {
			return new NeuraliumStandardAccountSnapshotDigestChannelCard();
		}
	}
}