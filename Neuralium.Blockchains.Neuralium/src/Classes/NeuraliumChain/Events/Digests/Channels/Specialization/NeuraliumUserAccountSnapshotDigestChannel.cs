using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumUserAccountSnapshotDigestChannel : IUserAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumUserAccountSnapshotDigestChannel : UserAccountSnapshotDigestChannel<NeuraliumStandardAccountSnapshotDigestChannelCard>, INeuraliumUserAccountSnapshotDigestChannel {
		[Flags]
		public enum AccountSnapshotDigestChannelBands {
			Wallets
		}

		public NeuraliumUserAccountSnapshotDigestChannel(int groupSize, string folder) : base(groupSize, folder) {
		}

		protected override NeuraliumStandardAccountSnapshotDigestChannelCard CreateNewCardInstance() {
			return new NeuraliumStandardAccountSnapshotDigestChannelCard();
		}
	}
}