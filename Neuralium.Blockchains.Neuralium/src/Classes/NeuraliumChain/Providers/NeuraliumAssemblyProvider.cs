using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
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
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumAssemblyProvider : IAssemblyProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
		Task<ITransactionEnvelope> GenerateNeuraliumTransferTransaction(Guid accountUuid, AccountId recipient, Amount amount, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0);

		Task<ITransactionEnvelope> GenerateNeuraliumMultiTransferTransaction(Guid accountUuid, List<RecipientSet> recipients, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0);

		Task<ITransactionEnvelope> GenerateRefillNeuraliumsTransaction(Guid accountUuid, CorrelationContext correlationContext, LockContext lockContext);
	}

	public class NeuraliumAssemblyProvider : AssemblyProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumAssemblyProvider {
		public NeuraliumAssemblyProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override async Task<ITransactionEnvelope> GenerateDebugTransaction() {

			try {
				ITransaction transaction = this.CreateNewDebugTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(transaction, GlobalsService.TRANSACTION_KEY_NAME, null, null).ConfigureAwait(false);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium debug transaction", ex);
			}
		}

		public override async Task<IMessageEnvelope> GenerateDebugMessage() {
			try {
				INeuraliumDebugMessage message = new NeuraliumDebugMessage();
				message.Message = "allo :)";
				IMessageEnvelope envelope = await this.GenerateBlockchainMessage(message, null).ConfigureAwait(false);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium debug message", ex);
			}
		}

		public virtual async Task<ITransactionEnvelope> GenerateNeuraliumTransferTransaction(Guid accountUuid, AccountId recipient, Amount amount, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0) {
			try {

				if(recipient.IsPresentation) {
					throw new ApplicationException("Cannot send tokens to a presentation account");
				}
				INeuraliumTransferTransaction transferTransaction = new NeuraliumTransferTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(transferTransaction, GlobalsService.TRANSACTION_KEY_NAME, EnvelopeSignatureTypes.Instance.Published, lockContext, expiration, async lc => {

					transferTransaction.Recipient = recipient;
					transferTransaction.Amount = amount;
					transferTransaction.Tip = tip;

					// let's ensure we have the balance
					Amount balance = (await this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountUuid, false, lc).ConfigureAwait(false)).Total;

					// make sure that the amount spent and tip are less than what we have in total
					if((balance - (amount + tip)) < 0) {
						//TODO: what to do here?
						throw new InvalidOperationException("We don't have enough to transfer");
					}
				}).ConfigureAwait(false);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium key change transaction", ex);
			}
		}

		public virtual async Task<ITransactionEnvelope> GenerateNeuraliumMultiTransferTransaction(Guid accountUuid, List<RecipientSet> recipients, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0) {
			try {
				
				if(recipients.Any(a => a.Recipient.IsPresentation)) {
					throw new ApplicationException($"Cannot send tokens to a presentation account");
				}
				
				INeuraliumMultiTransferTransaction multiTransferTransaction = new NeuraliumMultiTransferTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(multiTransferTransaction, GlobalsService.TRANSACTION_KEY_NAME, EnvelopeSignatureTypes.Instance.Published, lockContext, expiration, async lc => {

					multiTransferTransaction.Recipients.AddRange(recipients.Where(e => e.Amount > 0));
					multiTransferTransaction.Tip = tip;

					// let's ensure we have the balance
					Amount balance = (await this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountUuid, false, lc).ConfigureAwait(false)).Total;

					// make sure that the amount spent and tip are less than what we have in total

					if((balance - (multiTransferTransaction.Amount + tip)) < 0) {
						//TODO: what to do here?
						throw new InvalidOperationException("We don't have enough to transfer");
					}

				}).ConfigureAwait(false);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium key change transaction", ex);
			}
		}

		public virtual async Task<ITransactionEnvelope> GenerateRefillNeuraliumsTransaction(Guid accountUuid, CorrelationContext correlationContext, LockContext lockContext) {
			try {
				INeuraliumRefillNeuraliumsTransaction refillTransaction = new NeuraliumRefillNeuraliumsTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(refillTransaction, GlobalsService.TRANSACTION_KEY_NAME, EnvelopeSignatureTypes.Instance.Published, lockContext).ConfigureAwait(false);

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