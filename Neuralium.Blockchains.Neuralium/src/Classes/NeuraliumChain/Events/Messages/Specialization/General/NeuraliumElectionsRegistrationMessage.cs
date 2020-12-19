using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.AccreditationCertificates;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.AccreditationCertificates;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Elections;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General {

	public interface INeuraliumElectionsRegistrationMessage : IElectionsRegistrationMessage, INeuraliumBlockchainMessage {
	}

	public class NeuraliumElectionsRegistrationMessage : ElectionsRegistrationMessage, INeuraliumElectionsRegistrationMessage {
		protected override AccreditationCertificateMetadataFactory CreateAccreditationCertificateMetadataFactory() {
			return new NeuraliumAccreditationCertificateMetadataFactory();
		}
	}

}