using System;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization {
	public interface INeuraliumJointAccountSnapshotDigestChannel : IJointAccountSnapshotDigestChannel<NeuraliumJointAccountSnapshotDigestChannelCard>, INeuraliumDigestChannel {
	}

	public class NeuraliumJointAccountSnapshotDigestChannel : JointAccountSnapshotDigestChannel<NeuraliumJointAccountSnapshotDigestChannelCard>, INeuraliumJointAccountSnapshotDigestChannel {
		[Flags]
		public enum AccountSnapshotDigestChannelBands {
			Wallets
		}

		public NeuraliumJointAccountSnapshotDigestChannel(int groupSize, string folder) : base(groupSize, folder) {
		}

		protected override NeuraliumJointAccountSnapshotDigestChannelCard CreateNewCardInstance() {
			return new NeuraliumJointAccountSnapshotDigestChannelCard();
		}
	}
}