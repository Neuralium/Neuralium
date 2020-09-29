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
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General.Appointments;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumAssemblyProvider : IAssemblyProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
		Task<ITransactionEnvelope> GenerateNeuraliumTransferTransaction(string accountCode, AccountId recipient, Amount amount, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0);

		Task<ITransactionEnvelope> GenerateNeuraliumMultiTransferTransaction(string accountCode, List<RecipientSet> recipients, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0);
#if TESTNET || DEVNET
		Task<ITransactionEnvelope> GenerateRefillNeuraliumsTransaction(string accountCode, CorrelationContext correlationContext, LockContext lockContext);
#endif
	}

	public class NeuraliumAssemblyProvider : AssemblyProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumAssemblyProvider {
		public NeuraliumAssemblyProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public virtual async Task<ITransactionEnvelope> GenerateNeuraliumTransferTransaction(string accountCode, AccountId recipient, Amount amount, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0) {
			try {

				if(recipient.IsPresentation) {
					throw new ApplicationException("Cannot send tokens to a presentation account");
				}
				INeuraliumTransferTransaction transferTransaction = new NeuraliumTransferTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(transferTransaction, lockContext, async lc => {

					transferTransaction.Recipient = recipient;
					transferTransaction.Amount = amount;
					transferTransaction.Tip = tip;

					// let's ensure we have the balance
					Amount balance = (await this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountCode, false, lc).ConfigureAwait(false)).Total;

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

		public virtual async Task<ITransactionEnvelope> GenerateNeuraliumMultiTransferTransaction(string accountCode, List<RecipientSet> recipients, Amount tip, CorrelationContext correlationContext, LockContext lockContext, byte expiration = 0) {
			try {
				
				if(recipients.Any(a => a.Recipient.IsPresentation)) {
					throw new ApplicationException($"Cannot send tokens to a presentation account");
				}
				
				INeuraliumMultiTransferTransaction multiTransferTransaction = new NeuraliumMultiTransferTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(multiTransferTransaction, lockContext, async lc => {

					multiTransferTransaction.Recipients.AddRange(recipients.Where(e => e.Amount > 0));
					multiTransferTransaction.Tip = tip;

					// let's ensure we have the balance
					Amount balance = (await this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountCode, false, lc).ConfigureAwait(false)).Total;

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

#if TESTNET || DEVNET
		public virtual async Task<ITransactionEnvelope> GenerateRefillNeuraliumsTransaction(string accountCode, CorrelationContext correlationContext, LockContext lockContext) {
			try {
				INeuraliumRefillNeuraliumsTransaction refillTransaction = new NeuraliumRefillNeuraliumsTransaction();

				ITransactionEnvelope envelope = await this.GenerateTransaction(refillTransaction, lockContext).ConfigureAwait(false);

				return envelope;
			} catch(Exception ex) {
				throw new ApplicationException("failed to generate neuralium key change transaction", ex);
			}
		}
#endif

		protected override IStandardPresentationTransaction CreateNewPresentationTransaction() {
			return new NeuraliumStandardPresentationTransaction();
		}

		protected override IStandardAccountKeyChangeTransaction CreateNewKeyChangeTransaction(byte ordinalId) {
			//TODO: fix this
			return new NeuraliumStandardAccountKeyChangeTransaction(ordinalId);
		}

		protected override ITransactionEnvelope CreateNewTransactionEnvelope() {
			return new NeuraliumTransactionEnvelope();
		}

		protected override IPresentationTransactionEnvelope CreateNewPresentationTransactionEnvelope() {
			return new NeuraliumPresentationTransactionEnvelope();
		}

		protected override ISignedMessageEnvelope CreateNewSignedMessageEnvelope() {
			return new NeuraliumSignedMessageEnvelope();
		}

		protected override IInitiationAppointmentMessageEnvelope CreateNewInitiationAppointmentMessageEnvelope() {
			return new NeuraliumInitiationAppointmentMessageEnvelope();
		}

		protected override IElectionsRegistrationMessage CreateNewMinerRegistrationMessage() {
			return new NeuraliumElectionsRegistrationMessage();
		}

		protected override InitiationAppointmentRequestMessage CreateNewInitiationAppointmentRequestMessage() {
			return new NeuraliumInitiationAppointmentRequestMessage();
		}

		protected override AppointmentRequestMessage CreateNewAppointmentRequestMessage() {
			return new NeuraliumAppointmentRequestMessage();
		}

		protected override AppointmentVerificationResultsMessage CreateNewAppointmentVerificationResultsMessage() {
			return new NeuraliumAppointmentVerificationResultsMessage();
		}

		//		protected override IKeyChangeTransaction CreateNewChangeKeyTransactionBloc() {
		//			return new NeuraliumKeyChangeTransaction();
		//		}
	}
}