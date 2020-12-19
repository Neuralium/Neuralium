using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation {
	public class NeuraliumMessageValidationResult : MessageValidationResult {

		public NeuraliumMessageValidationResult(ValidationResults result) : base(result) {
		}

		public NeuraliumMessageValidationResult(ValidationResults result, IEventValidationErrorCodeBase errorCode) : base(result, errorCode) {
		}

		public NeuraliumMessageValidationResult(ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumMessageValidationResult(ValidationResults result, MessageValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumMessageValidationResult(ValidationResults result, List<MessageValidationErrorCode> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumMessageValidationResult(ValidationResults result, NeuraliumMessageValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumMessageValidationResult(ValidationResults result, List<NeuraliumMessageValidationErrorCode> errorCodes) : base(result, errorCodes?.Cast<IEventValidationErrorCodeBase>().ToList()) {
		}

		public override EventValidationException GenerateException() {
			return new NeuraliumMessageValidationException(this);
		}
	}

	public class NeuraliumMessageValidationErrorCodes : MessageValidationErrorCodes {
		public readonly NeuraliumMessageValidationErrorCode INSUFFICIENT_TIP;

		public readonly NeuraliumMessageValidationErrorCode TIP_REQUIRED;

		static NeuraliumMessageValidationErrorCodes() {
		}

		protected NeuraliumMessageValidationErrorCodes() {

			this.SetOffset(10_000);

			this.TIP_REQUIRED = this.CreateChildConstant();
			this.INSUFFICIENT_TIP = this.CreateChildConstant();

			//this.PrintValues(";");		
		}

		public static new NeuraliumMessageValidationErrorCodes Instance { get; } = new NeuraliumMessageValidationErrorCodes();

		protected NeuraliumMessageValidationErrorCode CreateChildConstant(NeuraliumMessageValidationErrorCode offset = default) {
			return new NeuraliumMessageValidationErrorCode(base.CreateChildConstant(offset).Value);
		}
	}

	public class NeuraliumMessageValidationErrorCode : MessageValidationErrorCode {

		public NeuraliumMessageValidationErrorCode() {
		}

		public NeuraliumMessageValidationErrorCode(ushort value) : base(value) {
		}

		public static implicit operator NeuraliumMessageValidationErrorCode(ushort d) {
			return new NeuraliumMessageValidationErrorCode(d);
		}

		public static bool operator ==(NeuraliumMessageValidationErrorCode a, NeuraliumMessageValidationErrorCode b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(NeuraliumMessageValidationErrorCode a, NeuraliumMessageValidationErrorCode b) {
			return !(a == b);
		}
	}
}