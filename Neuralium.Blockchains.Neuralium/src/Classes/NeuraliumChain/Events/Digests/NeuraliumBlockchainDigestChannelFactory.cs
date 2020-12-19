using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests {

	public interface INeuraliumBlockchainDigestChannelFactory : IBlockchainDigestChannelFactory {
		INeuraliumUserAccountKeysDigestChannel CreateNeuraliumUserAccountKeysDigestChannel(int groupSize, string folder);
		INeuraliumUserAccountSnapshotDigestChannel CreateNeuraliumUserAccountSnapshotDigestChannel(int groupSize, string folder);
		INeuraliumServerAccountKeysDigestChannel CreateNeuraliumServerAccountKeysDigestChannel(int groupSize, string folder);
		INeuraliumServerAccountSnapshotDigestChannel CreateNeuraliumServerAccountSnapshotDigestChannel(int groupSize, string folder);
		INeuraliumModeratorAccountKeysDigestChannel CreateNeuraliumModeratorAccountKeysDigestChannel(int groupSize, string folder);
		INeuraliumModeratorAccountSnapshotDigestChannel CreateNeuraliumModeratorAccountSnapshotDigestChannel(int groupSize, string folder);
		INeuraliumJointAccountSnapshotDigestChannel CreateNeuraliumJointAccountSnapshotDigestChannel(int groupSize, string folder);
		INeuraliumAccreditationCertificateDigestChannel CreateNeuraliumAccreditationCertificateDigestChannel(string folder);
		INeuraliumChainOptionsDigestChannel CreateNeuraliumChainOptionsDigestChannel(string folder);
	}

	public class NeuraliumBlockchainDigestChannelFactory : BlockchainDigestChannelFactory, INeuraliumBlockchainDigestChannelFactory {
		
		public override IStandardAccountKeysDigestChannel CreateUserAccountKeysDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumUserAccountKeysDigestChannel(groupSize, folder);
		}

		public override IUserAccountSnapshotDigestChannel CreateUserAccountSnapshotDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumUserAccountSnapshotDigestChannel(groupSize, folder);
		}

		public override IStandardAccountKeysDigestChannel CreateServerAccountKeysDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumUserAccountKeysDigestChannel(groupSize, folder);
		}

		public override IServerAccountSnapshotDigestChannel CreateServerAccountSnapshotDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumServerAccountSnapshotDigestChannel(groupSize, folder);
		}
		
		public override IStandardAccountKeysDigestChannel CreateModeratorAccountKeysDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumUserAccountKeysDigestChannel(groupSize, folder);
		}

		public override IModeratorAccountSnapshotDigestChannel CreateModeratorAccountSnapshotDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumModeratorAccountSnapshotDigestChannel(groupSize, folder);
		}
		
		public override IJointAccountSnapshotDigestChannel CreateJointAccountSnapshotDigestChannel(int groupSize, string folder) {
			return this.CreateNeuraliumJointAccountSnapshotDigestChannel(groupSize, folder);
		}

		public INeuraliumJointAccountSnapshotDigestChannel CreateNeuraliumJointAccountSnapshotDigestChannel(int groupSize, string folder) {
			return new NeuraliumJointAccountSnapshotDigestChannel(groupSize, folder);
		}
		
		public INeuraliumUserAccountKeysDigestChannel CreateNeuraliumUserAccountKeysDigestChannel(int groupSize, string folder) {
			return new NeuraliumUserAccountKeysDigestChannel(groupSize, folder);
		}

		public INeuraliumUserAccountSnapshotDigestChannel CreateNeuraliumUserAccountSnapshotDigestChannel(int groupSize, string folder) {
			return new NeuraliumUserAccountSnapshotDigestChannel(groupSize, folder);
		}

		public INeuraliumServerAccountKeysDigestChannel CreateNeuraliumServerAccountKeysDigestChannel(int groupSize, string folder) {
			return new NeuraliumServerAccountKeysDigestChannel(groupSize, folder);
		}

		public INeuraliumServerAccountSnapshotDigestChannel CreateNeuraliumServerAccountSnapshotDigestChannel(int groupSize, string folder) {
			return new NeuraliumServerAccountSnapshotDigestChannel(groupSize, folder);
		}

		
		public INeuraliumModeratorAccountKeysDigestChannel CreateNeuraliumModeratorAccountKeysDigestChannel(int groupSize, string folder) {
			return new NeuraliumModeratorAccountKeysDigestChannel(groupSize, folder);
		}

		public INeuraliumModeratorAccountSnapshotDigestChannel CreateNeuraliumModeratorAccountSnapshotDigestChannel(int groupSize, string folder) {
			return new NeuraliumModeratorAccountSnapshotDigestChannel(groupSize, folder);
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