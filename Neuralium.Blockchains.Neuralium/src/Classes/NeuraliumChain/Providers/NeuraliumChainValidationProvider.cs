using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Validation;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Managers;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Gated;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Managers {
	public interface INeuraliumChainValidationProvider : IChainValidationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumChainValidationProvider : ChainValidationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainValidationProvider {
		public NeuraliumChainValidationProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		protected override async Task<ValidationResult> PerformBasicTransactionValidation(ITransaction transaction, ITransactionEnvelope envelope, bool? accreditationCertificateValid) {
			ValidationResult result = await base.PerformBasicTransactionValidation(transaction, envelope, accreditationCertificateValid).ConfigureAwait(false);

			if(transaction is ITipTransaction tipTransaction1) {
				if(tipTransaction1.Tip < 0) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.NEGATIVE_TIP);
				}
			}

			bool validCertificate = accreditationCertificateValid.HasValue && accreditationCertificateValid.Value;

			if(result == ValidationResult.ValidationResults.Valid) {

				if(!validCertificate && transaction is ITipTransaction tipTransaction) {

					ushort scope = transaction.TransactionId.Scope;

					// we allow one transaction for free per second
					if(scope >= 1) {
						decimal tip = tipTransaction.Tip;

						if(tip <= 0M) {
							return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.TIP_REQUIRED);
						}

						// tiered tip
						if(scope > 3) {
							//TODO: define these values
							if((scope <= 25) && (tip <= 0.0001M)) {
								return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}

							if((scope <= 100) && (tip <= 0.001M)) {
								return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}

							if((scope > byte.MaxValue) && (tip <= 0.01M)) {
								return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}

							if(tip <= 0.01M) {
								return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INSUFFICIENT_TIP);
							}
						}
					}
				}
			}

			return result;
		}

		protected override async Task<ValidationResult> ValidateTransactionTypes(ITransactionEnvelope transactionEnvelope, ITransaction transaction, bool gossipOrigin, LockContext lockContext) {
			
			ValidationResult result = new ValidationResult(ValidationResult.ValidationResults.Valid);
			
			if(result.Valid && transaction is INeuraliumTransferTransaction transferTransaction) {

				if(transferTransaction.Amount <= 0) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INVALID_AMOUNT);
				}
				if(!transferTransaction.Recipient.IsValid || transferTransaction.Recipient == transferTransaction.TransactionId.Account) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INVALID_ACCOUNT);
				}
			}
			
			if(result.Valid && transaction is INeuraliumMultiTransferTransaction multiTransferTransaction) {

				if(!multiTransferTransaction.Recipients.Any(e => e.Recipient.IsValid)) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INVALID_ACCOUNT);
				}
				if(multiTransferTransaction.Recipients.Any(e => e.Amount <= 0)) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INVALID_AMOUNT);
				}
			}

			if(result.Valid && transaction is INeuraliumSAFUContributionTransaction safuContributionTransaction) {

				if(safuContributionTransaction.NumberDays < 1 || safuContributionTransaction.DailyProtection <= 0 || !safuContributionTransaction.AcceptSAFUTermsOfService) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INVALID_ENTRY);
				}
			}
			
			if(result.Valid && transaction is INeuraliumThreeWayGatedTransferTransaction threeWayGatedTransferTransaction){

				if(threeWayGatedTransferTransaction.Amount <= 0 || threeWayGatedTransferTransaction.SenderVerifierBaseServiceFee < 0 || threeWayGatedTransferTransaction.SenderVerifierServiceFee < 0 || threeWayGatedTransferTransaction.ReceiverVerifierBaseServiceFee < 0 || threeWayGatedTransferTransaction.ReceiverVerifierServiceFee < 0) {
					return this.CreateTransactionValidationResult(ValidationResult.ValidationResults.Invalid, NeuraliumTransactionValidationErrorCodes.Instance.INVALID_AMOUNT);
				}
			}
			
			if(result.Valid && transaction is INeuraliumGatedJudgementTransaction gatedJudgementTransaction) {


			}

			return await base.ValidateTransactionTypes(transactionEnvelope, transaction, gossipOrigin, lockContext).ConfigureAwait(false);
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

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, IEventValidationErrorCodeBase errorCode) {
			return new NeuraliumBlockValidationResult(result, errorCode);
		}

		protected override BlockValidationResult CreateBlockValidationResult(ValidationResult.ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) {
			return new NeuraliumBlockValidationResult(result, errorCodes);
		}

		protected override TransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result, TransactionValidationErrorCode errorCode) {
			return new NeuraliumTransactionValidationResult(result, errorCode);
		}

		protected override TransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result, List<TransactionValidationErrorCode> errorCodes) {
			return new NeuraliumTransactionValidationResult(result, errorCodes);
		}

		protected override TransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result) {
			return new NeuraliumTransactionValidationResult(result);
		}

		protected virtual NeuraliumTransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result, NeuraliumTransactionValidationErrorCode errorCode) {
			return new NeuraliumTransactionValidationResult(result, errorCode);
		}

		protected virtual NeuraliumTransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result, List<NeuraliumTransactionValidationErrorCode> errorCodes) {
			return new NeuraliumTransactionValidationResult(result, errorCodes);
		}

		protected override TransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result, IEventValidationErrorCodeBase errorCode) {
			return new NeuraliumTransactionValidationResult(result, errorCode);
		}

		protected override TransactionValidationResult CreateTransactionValidationResult(ValidationResult.ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) {
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

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, IEventValidationErrorCodeBase errorCode) {
			return new NeuraliumMessageValidationResult(result, errorCode);
		}

		protected override MessageValidationResult CreateMessageValidationResult(ValidationResult.ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) {
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

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, IEventValidationErrorCodeBase errorCode) {
			return new NeuraliumDigestValidationResult(result, errorCode);
		}

		protected override DigestValidationResult CreateDigestValidationResult(ValidationResult.ValidationResults result, List<IEventValidationErrorCodeBase> errorCodes) {
			return new NeuraliumDigestValidationResult(result, errorCodes);
		}
	}
}