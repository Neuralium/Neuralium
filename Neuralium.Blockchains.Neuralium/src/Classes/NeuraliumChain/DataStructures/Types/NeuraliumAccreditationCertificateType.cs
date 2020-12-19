using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Types;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Types {

	public class NeuraliumAccreditationCertificateTypes : AccreditationCertificateTypes {

		static NeuraliumAccreditationCertificateTypes() {
		}

		protected NeuraliumAccreditationCertificateTypes() {

			//this.PrintValues(";");

		}

		public static new NeuraliumAccreditationCertificateTypes Instance { get; } = new NeuraliumAccreditationCertificateTypes();
	}
}