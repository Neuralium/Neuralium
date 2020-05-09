using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation {
	public class NeuraliumMessageValidationException : MessageValidationException {

		public NeuraliumMessageValidationException(ValidationResult results) : base(results) {
		}

		public NeuraliumMessageValidationException(MessageValidationResult results) : base(results) {
		}

		public NeuraliumMessageValidationException(NeuraliumMessageValidationResult results) : base(results) {
		}
	}
}