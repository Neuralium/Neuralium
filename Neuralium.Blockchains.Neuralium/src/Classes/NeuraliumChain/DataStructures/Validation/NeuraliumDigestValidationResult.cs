using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation {
	public class NeuraliumDigestValidationResult : DigestValidationResult {

		public NeuraliumDigestValidationResult(ValidationResults result) : base(result) {
		}

		public NeuraliumDigestValidationResult(ValidationResults result, IEventValidationErrorCodeBase errorCode) : base(result, errorCode) {
		}

		public NeuraliumDigestValidationResult(ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumDigestValidationResult(ValidationResults result, DigestValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumDigestValidationResult(ValidationResults result, List<DigestValidationErrorCode> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumDigestValidationResult(ValidationResults result, NeuraliumDigestValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumDigestValidationResult(ValidationResults result, List<NeuraliumDigestValidationErrorCode> errorCodes) : base(result, errorCodes?.Cast<IEventValidationErrorCodeBase>().ToList()) {
		}

		public override EventValidationException GenerateException() {
			return new NeuraliumDigestValidationException(this);
		}
	}

	public class NeuraliumDigestValidationErrorCodes : DigestValidationErrorCodes {
		public readonly NeuraliumDigestValidationErrorCode INSUFFICIENT_TIP;

		public readonly NeuraliumDigestValidationErrorCode TIP_REQUIRED;

		static NeuraliumDigestValidationErrorCodes() {
		}

		protected NeuraliumDigestValidationErrorCodes() {

			this.SetOffset(10_000);

			this.TIP_REQUIRED = this.CreateChildConstant();
			this.INSUFFICIENT_TIP = this.CreateChildConstant();

			//this.PrintValues(";");		
		}

		public static new NeuraliumDigestValidationErrorCodes Instance { get; } = new NeuraliumDigestValidationErrorCodes();

		protected NeuraliumDigestValidationErrorCode CreateChildConstant(NeuraliumDigestValidationErrorCode offset = default) {
			return new NeuraliumDigestValidationErrorCode(base.CreateChildConstant(offset).Value);
		}
	}

	public class NeuraliumDigestValidationErrorCode : DigestValidationErrorCode {

		public NeuraliumDigestValidationErrorCode() {
		}

		public NeuraliumDigestValidationErrorCode(ushort value) : base(value) {
		}

		public static implicit operator NeuraliumDigestValidationErrorCode(ushort d) {
			return new NeuraliumDigestValidationErrorCode(d);
		}

		public static bool operator ==(NeuraliumDigestValidationErrorCode a, NeuraliumDigestValidationErrorCode b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(NeuraliumDigestValidationErrorCode a, NeuraliumDigestValidationErrorCode b) {
			return !(a == b);
		}
	}
}