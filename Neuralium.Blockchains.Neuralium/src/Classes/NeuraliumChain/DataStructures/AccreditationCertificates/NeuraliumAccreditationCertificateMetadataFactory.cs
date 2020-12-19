using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.AccreditationCertificates.MiningClusters;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.AccreditationCertificates.SdkProviders;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.AccreditationCertificates;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.AccreditationCertificates.MiningClusters;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.AccreditationCertificates.SdkProviders;
using Neuralia.Blockchains.Core.General.Versions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.AccreditationCertificates {
	public class NeuraliumAccreditationCertificateMetadataFactory : AccreditationCertificateMetadataFactory {

		protected override SdkProviderAccreditationCertificateMetadata CreateSdkProviderAccreditationCertificateMetadata(ComponentVersion version) {
			if(version == (1, 0)) {
				return new NeuraliumSdkProviderAccreditationCertificateMetadata();
			}

			return null;
		}

		protected override MiningClusterAccreditationCertificateMetadata CreateMiningClusterAccreditationCertificateMetadata(ComponentVersion version) {
			if(version == (1, 0)) {
				return new NeuraliumMiningClusterAccreditationCertificateMetadata();
			}

			return null;
		}
	}
}