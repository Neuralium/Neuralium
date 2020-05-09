using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation {
	public class NeuraliumBlockValidationException : BlockValidationException {

		public NeuraliumBlockValidationException(ValidationResult results) : base(results) {
		}

		public NeuraliumBlockValidationException(BlockValidationResult results) : base(results) {
		}

		public NeuraliumBlockValidationException(NeuraliumBlockValidationResult results) : base(results) {
		}
	}
}