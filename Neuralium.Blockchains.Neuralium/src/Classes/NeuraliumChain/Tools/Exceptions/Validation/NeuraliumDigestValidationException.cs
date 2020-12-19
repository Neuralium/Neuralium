using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation {
	public class NeuraliumDigestValidationException : DigestValidationException {

		public NeuraliumDigestValidationException(ValidationResult results) : base(results) {
		}

		public NeuraliumDigestValidationException(DigestValidationResult results) : base(results) {
		}

		public NeuraliumDigestValidationException(NeuraliumDigestValidationResult results) : base(results) {
		}
	}
}