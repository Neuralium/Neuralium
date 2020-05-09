using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation {
	public class NeuraliumTransactionValidationException : TransactionValidationException {

		public NeuraliumTransactionValidationException(ValidationResult results) : base(results) {
		}

		public NeuraliumTransactionValidationException(TransactionValidationResult results) : base(results) {
		}

		public NeuraliumTransactionValidationException(NeuraliumTransactionValidationResult results) : base(results) {
		}
	}

}