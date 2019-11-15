using System.Collections.Generic;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Managers;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Managers {
	public interface INeuraliumChainValidationProvider : IChainValidationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumChainValidationProvider : ChainValidationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainValidationProvider {
		public NeuraliumChainValidationProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		protected override ValidationResult PerformBasicTransactionValidation(ITransaction transaction, ITransactionEnvelope envelope, bool? accreditationCertificateValid) {
			ValidationResult result = base.PerformBasicTransactionValidation(transaction, envelope, accreditationCertificateValid);

			bool validCertificate = accreditationCertificateValid.HasValue && accreditationCertificateValid.Value;

			if(result == ValidationResult.ValidationResults.Valid) {

				if(!validCertificate && transaction is ITipTransaction tipTransaction) {

					byte scope = transaction.TransactionId.Scope;

					// we allow one transaction for free per second
					if(scope >= 1) {
						decimal tip = tipTransaction.Tip;

						if(tip <= 0M) {
							return this.CreateTrasactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.TIP_REQUIRED);
						}

						// tiered tip
						if(scope > 3) {
							//TODO: define these values
							if((scope <= 25) && (tip <= 0.0001M)) {
								return this.CreateTrasactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}

							if((scope <= 100) && (tip <= 0.001M)) {
								return this.CreateTrasactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}

							if(tip <= 0.01M) {
								return this.CreateTrasactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}
						}
					}
				}
			}

			return result;
		}

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, BlockValidationErrorCode errorCode) {
			return new NeuraliumBlockValidationResult(result, errorCode);
		}

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, List<BlockValidationErrorCode> errorCodes) {
			return new NeuraliumBlockValidationResult(result, errorCodes);
		}

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result) {
			return new NeuraliumBlockValidationResult(result);
		}

		protected virtual NeuraliumBlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, NeuraliumBlockValidationErrorCode errorCode) {
			return new NeuraliumBlockValidationResult(result, errorCode);
		}

		protected virtual NeuraliumBlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, List<NeuraliumBlockValidationErrorCode> errorCodes) {
			return new NeuraliumBlockValidationResult(result, errorCodes);
		}

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, EventValidationErrorCode errorCode) {
			return new NeuraliumBlockValidationResult(result, errorCode);
		}

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, List<EventValidationErrorCode> errorCodes) {
			return new NeuraliumBlockValidationResult(result, errorCodes);
		}

		protected override TransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result, TransactionValidationErrorCode errorCode) {
			return new NeuraliumTransactionValidationResult(result, errorCode);
		}

		protected override TransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result, List<TransactionValidationErrorCode> errorCodes) {
			return new NeuraliumTransactionValidationResult(result, errorCodes);
		}

		protected override TransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result) {
			return new NeuraliumTransactionValidationResult(result);
		}

		protected virtual NeuraliumTransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result, NeuraliumTransactionValidationErrorCode errorCode) {
			return new NeuraliumTransactionValidationResult(result, errorCode);
		}

		protected virtual NeuraliumTransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result, List<NeuraliumTransactionValidationErrorCode> errorCodes) {
			return new NeuraliumTransactionValidationResult(result, errorCodes);
		}

		protected override TransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result, EventValidationErrorCode errorCode) {
			return new NeuraliumTransactionValidationResult(result, errorCode);
		}

		protected override TransactionValidationResult CreateTrasactionValidationResult(ValidationResult.ValidationResults result, List<EventValidationErrorCode> errorCodes) {
			return new NeuraliumTransactionValidationResult(result, errorCodes);
		}

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, MessageValidationErrorCode errorCode) {
			return new NeuraliumMessageValidationResult(result, errorCode);
		}

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, List<MessageValidationErrorCode> errorCodes) {
			return new NeuraliumMessageValidationResult(result, errorCodes);
		}

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result) {
			return new NeuraliumMessageValidationResult(result);
		}

		protected virtual NeuraliumMessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, NeuraliumMessageValidationErrorCode errorCode) {
			return new NeuraliumMessageValidationResult(result, errorCode);
		}

		protected virtual NeuraliumMessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, List<NeuraliumMessageValidationErrorCode> errorCodes) {
			return new NeuraliumMessageValidationResult(result, errorCodes);
		}

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, EventValidationErrorCode errorCode) {
			return new NeuraliumMessageValidationResult(result, errorCode);
		}

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, List<EventValidationErrorCode> errorCodes) {
			return new NeuraliumMessageValidationResult(result, errorCodes);
		}

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, DigestValidationErrorCode errorCode) {
			return new NeuraliumDigestValidationResult(result, errorCode);
		}

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, List<DigestValidationErrorCode> errorCodes) {
			return new NeuraliumDigestValidationResult(result, errorCodes);
		}

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result) {
			return new NeuraliumDigestValidationResult(result);
		}

		protected virtual NeuraliumDigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, NeuraliumDigestValidationErrorCode errorCode) {
			return new NeuraliumDigestValidationResult(result, errorCode);
		}

		protected virtual NeuraliumDigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, List<NeuraliumDigestValidationErrorCode> errorCodes) {
			return new NeuraliumDigestValidationResult(result, errorCodes);
		}

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, EventValidationErrorCode errorCode) {
			return new NeuraliumDigestValidationResult(result, errorCode);
		}

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, List<EventValidationErrorCode> errorCodes) {
			return new NeuraliumDigestValidationResult(result, errorCodes);
		}
	}
}