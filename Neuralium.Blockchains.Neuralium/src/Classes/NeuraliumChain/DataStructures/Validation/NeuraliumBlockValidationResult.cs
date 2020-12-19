using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.Exceptions.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.Exceptions.Validation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation {
	public class NeuraliumBlockValidationResult : BlockValidationResult {

		public NeuraliumBlockValidationResult(ValidationResults result) : base(result) {
		}

		public NeuraliumBlockValidationResult(ValidationResults result, IEventValidationErrorCodeBase errorCode) : base(result, errorCode) {
		}

		public NeuraliumBlockValidationResult(ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumBlockValidationResult(ValidationResults result, BlockValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumBlockValidationResult(ValidationResults result, List<BlockValidationErrorCode> errorCodes) : base(result, errorCodes) {
		}

		public NeuraliumBlockValidationResult(ValidationResults result, NeuraliumBlockValidationErrorCode errorCode) : base(result, errorCode) {
		}

		public NeuraliumBlockValidationResult(ValidationResults result, List<NeuraliumBlockValidationErrorCode> errorCodes) : base(result, errorCodes?.Cast<IEventValidationErrorCodeBase>().ToList()) {
		}

		public override EventValidationException GenerateException() {
			return new NeuraliumBlockValidationException(this);
		}
	}

	public class NeuraliumBlockValidationErrorCodes : BlockValidationErrorCodes {
		public readonly NeuraliumBlockValidationErrorCode INSUFFICIENT_TIP;

		public readonly NeuraliumBlockValidationErrorCode TIP_REQUIRED;

		static NeuraliumBlockValidationErrorCodes() {
		}

		protected NeuraliumBlockValidationErrorCodes() {

			this.SetOffset(10_000);

			this.TIP_REQUIRED = this.CreateChildConstant();
			this.INSUFFICIENT_TIP = this.CreateChildConstant();

			//this.PrintValues(";");		
		}

		public static new NeuraliumBlockValidationErrorCodes Instance { get; } = new NeuraliumBlockValidationErrorCodes();

		protected NeuraliumBlockValidationErrorCode CreateChildConstant(NeuraliumBlockValidationErrorCode offset = default) {
			return new NeuraliumBlockValidationErrorCode(base.CreateChildConstant(offset).Value);
		}
	}

	public class NeuraliumBlockValidationErrorCode : BlockValidationErrorCode {

		public NeuraliumBlockValidationErrorCode() {
		}

		public NeuraliumBlockValidationErrorCode(ushort value) : base(value) {
		}

		public static implicit operator NeuraliumBlockValidationErrorCode(ushort d) {
			return new NeuraliumBlockValidationErrorCode(d);
		}

		public static bool operator ==(NeuraliumBlockValidationErrorCode a, NeuraliumBlockValidationErrorCode b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(NeuraliumBlockValidationErrorCode a, NeuraliumBlockValidationErrorCode b) {
			return !(a == b);
		}
	}
}