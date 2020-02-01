using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes.Signatures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Services;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumAssemblyProvider : IAssemblyProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
		ITransactionEnvelope GenerateNeuraliumTransferTransaction(Guid accountUuid, AccountId recipient, Amount amount, Amount tip, CorrelationContext correlationContext, byte expiration = 0);

		ITransactionEnvelope GenerateNeuraliumMultiTransferTransaction(Guid accountUuid, List<RecipientSet> recipients, Amount tip, CorrelationContext correlationContext, byte expiration = 0);

		ITransactionEnvelope GenerateRefillNeuraliumsTransaction(Guid accountUuid, CorrelationContext correlationContext);
	}

	public class NeuraliumAssemblyProvider : AssemblyProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumAssemblyProvider {
		public NeuraliumAssemblyProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override ITransactionEnvelope GenerateDebugTransaction() {

			try {
				ITransaction transaction = this.CreateNewDebugTransaction();

				ITransactionEnvelope envelope = this.GenerateTransaction(transaction, GlobalsService.TRANSACTION_KEY_NAME, null);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium debug transaction", ex);
			}
		}

		public override IMessageEnvelope GenerateDebugMessage() {
			try {
				INeuraliumDebugMessage message = new NeuraliumDebugMessage();
				message.Message = "allo :)";
				IMessageEnvelope envelope = this.GenerateBlockchainMessage(message);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium debug message", ex);
			}
		}

		public virtual ITransactionEnvelope GenerateNeuraliumTransferTransaction(Guid accountUuid, AccountId recipient, Amount amount, Amount tip, CorrelationContext correlationContext, byte expiration = 0) {
			try {
				INeuraliumTransferTransaction transferTransaction = new NeuraliumTransferTransaction();

				ITransactionEnvelope envelope = this.GenerateTransaction(transferTransaction, GlobalsService.TRANSACTION_KEY_NAME, EnvelopeSignatureTypes.Instance.Published, expiration, () => {

					transferTransaction.Recipient = recipient;
					transferTransaction.Amount = amount;
					transferTransaction.Tip = tip;

					// let's ensure we have the balance
					Amount balance = this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountUuid, false).Total;

					// make sure that the amount spent and tip are less than what we have in total
					if((balance - (amount + tip)) < 0) {
						//TODO: what to do here?
						throw new InvalidOperationException("We don't have enough to transfer"); 
					}
				});

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium key change transaction", ex);
			}
		}

		public virtual ITransactionEnvelope GenerateNeuraliumMultiTransferTransaction(Guid accountUuid, List<RecipientSet> recipients, Amount tip, CorrelationContext correlationContext, byte expiration = 0) {
			try {
				INeuraliumMultiTransferTransaction multiTransferTransaction = new NeuraliumMultiTransferTransaction();

				ITransactionEnvelope envelope = this.GenerateTransaction(multiTransferTransaction, GlobalsService.TRANSACTION_KEY_NAME, EnvelopeSignatureTypes.Instance.Published, expiration, () => {

					multiTransferTransaction.Recipients.AddRange(recipients);
					multiTransferTransaction.Tip = tip;

					// let's ensure we have the balance
					Amount balance = this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountUuid, false).Total;

					// make sure that the amount spent and tip are less than what we have in total

					if((balance - (multiTransferTransaction.Total + tip)) < 0) {
						//TODO: what to do here?
						throw new InvalidOperationException("We don't have enough to transfer");
					}
				});

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium key change transaction", ex);
			}
		}

		public virtual ITransactionEnvelope GenerateRefillNeuraliumsTransaction(Guid accountUuid, CorrelationContext correlationContext) {
			try {
				INeuraliumRefillNeuraliumsTransaction refillTransaction = new NeuraliumRefillNeuraliumsTransaction();

				ITransactionEnvelope envelope = this.GenerateTransaction(refillTransaction, GlobalsService.TRANSACTION_KEY_NAME, EnvelopeSignatureTypes.Instance.Published);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium key change transaction", ex);
			}
		}

		protected override IStandardPresentationTransaction CreateNewPresentationTransaction() {
			return new NeuraliumStandardPresentationTransaction();
		}

		protected override IStandardAccountKeyChangeTransaction CreateNewKeyChangeTransaction(byte ordinalId) {
			//TODO: fix this
			return new NeuraliumStandardAccountKeyChangeTransaction(ordinalId);
		}

		protected override ITransaction CreateNewDebugTransaction() {
			return new NeuraliumDebugTransaction();
		}

		protected override ITransactionEnvelope CreateNewTransactionEnvelope() {
			return new NeuraliumTransactionEnvelope();
		}

		protected override IMessageEnvelope CreateNewMessageEnvelope() {
			return new NeuraliumMessageEnvelope();
		}

		protected override IElectionsRegistrationMessage CreateNewMinerRegistrationMessage() {
			return new NeuraliumElectionsRegistrationMessage();
		}

		//		protected override IKeyChangeTransaction CreateNewChangeKeyTransactionBloc() {
		//			return new NeuraliumKeyChangeTransaction();
		//		}
	}
}