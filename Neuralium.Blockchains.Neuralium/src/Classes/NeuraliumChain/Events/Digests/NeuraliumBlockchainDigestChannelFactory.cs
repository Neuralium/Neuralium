using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests {

	public interface INeuraliumBlockchainDigestChannelFactory : IBlockchainDigestChannelFactory {
		INeuraliumStandardAccountKeysDigestChannel CreateNeuraliumAccountKeysDigestChannel(int groupSize, string folder);
		INeuraliumStandardAccountSnapshotDigestChannel CreateNeuraliumStandardAccountSnapshotDigestChannel(int groupSize, string folder);
		INeuraliumJointAccountSnapshotDigestChannel CreateNeuraliumJointAccountSnapshotDigestChannel(int groupSize, string folder);
		INeuraliumAccreditationCertificateDigestChannel CreateNeuraliumAccreditationCertificateDigestChannel(string folder);
		INeuraliumChainOptionsDigestChannel CreateNeuraliumChainOptionsDigestChannel(string folder);
	}

	public class NeuraliumBlockchainDigestChannelFactory : BlockchainDigestChannelFactory, INeuraliumBlockchainDigestChannelFactory {
		public override IStandardAccountKeysDigestChannel CreateStandardAccountKeysDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumAccountKeysDigestChannel(groupSize, folder);
		}

		public override IStandardAccountSnapshotDigestChannel CreateStandardAccountSnapshotDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumStandardAccountSnapshotDigestChannel(groupSize, folder);
		}

		public override IJointAccountSnapshotDigestChannel CreateJointAccountSnapshotDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumJointAccountSnapshotDigestChannel(groupSize, folder);
		}

		public INeuraliumStandardAccountKeysDigestChannel CreateNeuraliumAccountKeysDigestChannel(int groupSize, string folder) {
			return new NeuraliumStandardAccountKeysDigestChannel(groupSize, folder);
		}

		public INeuraliumStandardAccountSnapshotDigestChannel CreateNeuraliumStandardAccountSnapshotDigestChannel(int groupSize, string folder) {
			return new NeuraliumStandardAccountSnapshotDigestChannel(groupSize, folder);
		}

		public INeuraliumJointAccountSnapshotDigestChannel CreateNeuraliumJointAccountSnapshotDigestChannel(int groupSize, string folder) {
			return new NeuraliumJointAccountSnapshotDigestChannel(groupSize, folder);
		}

		public override IAccreditationCertificateDigestChannel CreateAccreditationCertificateDigestChannel(string folder) {
			return this.CreateNeuraliumAccreditationCertificateDigestChannel(folder);
		}

		public INeuraliumAccreditationCertificateDigestChannel CreateNeuraliumAccreditationCertificateDigestChannel(string folder) {
			return new NeuraliumAccreditationCertificateDigestChannel(folder);
		}

		public override IChainOptionsDigestChannel CreateChainOptionsDigestChannel(string folder) {
			return this.CreateNeuraliumChainOptionsDigestChannel(folder);
		}

		public INeuraliumChainOptionsDigestChannel CreateNeuraliumChainOptionsDigestChannel(string folder) {
			return new NeuraliumChainOptionsDigestChannel(folder);
		}
	}
}