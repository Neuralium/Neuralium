using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests {
	public class NeuraliumBlockchainDigest : BlockchainDigest, INeuraliumDigest {

		protected override IAccountSnapshotDigestChannel CreateAccountSnapshotDigestChannel(int groupSize) {
			//return new NeuraliumAccountSnapshotDigestChannel(groupSize);
			return null;
		}
	}
}