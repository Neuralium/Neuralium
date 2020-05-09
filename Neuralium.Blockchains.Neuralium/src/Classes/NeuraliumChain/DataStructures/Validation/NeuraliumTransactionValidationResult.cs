using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation {
	public class NeuraliumTransactionValidationResult : TransactionValidationResult {

		public NeuraliumTransactionValidationResult(ValidationResults result) : base(result) {
		}

		public NeuraliumTransactionValidationResult(ValidationResults result, IEventValidationErrorCodeBase errorCode) : base(result, errorCode) {
		}

		public NeuraliumTransactionValidationResult(ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumTransactionValidationResult(ValidationResults result, TransactionValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumTransactionValidationResult(ValidationResults result, List<TransactionValidationErrorCode> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumTransactionValidationResult(ValidationResults result, NeuraliumTransactionValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumTransactionValidationResult(ValidationResults result, List<NeuraliumTransactionValidationErrorCode> errorCodes) : base(result, errorCodes?.Cast<IEventValidationErrorCodeBase>().ToList()) {
		}

		public override EventValidationException GenerateException() {
			return new NeuraliumTransactionValidationException(this);
		}
	}

	public class NeuraliumTransactionValidationErrorCodes : TransactionValidationErrorCodes {
		public readonly NeuraliumTransactionValidationErrorCode INSUFFICIENT_TIP;
		public readonly TransactionValidationErrorCode INVALID_AMOUNT;
		public readonly TransactionValidationErrorCode NEGATIVE_TIP;

		public readonly NeuraliumTransactionValidationErrorCode TIP_REQUIRED;

		static NeuraliumTransactionValidationErrorCodes() {
		}

		protected NeuraliumTransactionValidationErrorCodes() {

			this.SetOffset(10_000);

			this.TIP_REQUIRED = this.CreateChildConstant();
			this.INSUFFICIENT_TIP = this.CreateChildConstant();

			this.NEGATIVE_TIP = this.CreateChildConstant();
			this.INVALID_AMOUNT = this.CreateChildConstant();

			//this.PrintValues(";");		
		}

		public static new NeuraliumTransactionValidationErrorCodes Instance { get; } = new NeuraliumTransactionValidationErrorCodes();

		protected NeuraliumTransactionValidationErrorCode CreateChildConstant(NeuraliumTransactionValidationErrorCode offset = default) {
			return new NeuraliumTransactionValidationErrorCode(base.CreateChildConstant(offset).Value);
		}
	}

	public class NeuraliumTransactionValidationErrorCode : TransactionValidationErrorCode {

		public NeuraliumTransactionValidationErrorCode() {
		}

		public NeuraliumTransactionValidationErrorCode(ushort value) : base(value) {
		}

		public static implicit operator NeuraliumTransactionValidationErrorCode(ushort d) {
			return new NeuraliumTransactionValidationErrorCode(d);
		}

		public static bool operator ==(NeuraliumTransactionValidationErrorCode a, NeuraliumTransactionValidationErrorCode b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(NeuraliumTransactionValidationErrorCode a, NeuraliumTransactionValidationErrorCode b) {
			return !(a == b);
		}
	}
}