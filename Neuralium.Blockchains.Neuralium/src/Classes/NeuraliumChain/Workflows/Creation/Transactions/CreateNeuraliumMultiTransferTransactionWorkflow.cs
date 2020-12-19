using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {
	public interface ICreateNeuraliumMultiTransferTransactionWorkflow : IGenerateNewTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class CreateNeuraliumMultiTransferTransactionWorkflow : GenerateNewSignedTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, ICreateNeuraliumMultiTransferTransactionWorkflow {
		private readonly string accountCode;
		private readonly Amount amount;
		private readonly TransactionId guid;
		private readonly SafeArrayHandle hash = SafeArrayHandle.Create();

		private readonly List<RecipientSet> recipients;
		private readonly long targetAccountId;
		private readonly Amount tip;

		protected INeuraliumWalletGenerationCache NeuraliumWalletGenerationCache => (INeuraliumWalletGenerationCache)this.WalletGenerationCache;
		protected INeuraliumMultiTransferTransaction NeuraliumTransaction => (INeuraliumMultiTransferTransaction)this.BlockchainEvent;

		
		public CreateNeuraliumMultiTransferTransactionWorkflow(string accountCode, List<RecipientSet> recipients, Amount tip, byte expiration, string note, INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base(centralCoordinator, expiration, note, correlationContext) {
			this.accountCode = accountCode;
			this.recipients = recipients;
			this.tip = tip;
		}

		protected override async Task ValidateContents(LockContext lockContext) {
			await base.PreValidateContents(lockContext).ConfigureAwait(false);
			
			var result = NeuraliumTransactionCreationUtils.ValidateTransaction(envelope.Contents.RehydratedEvent);

			if(result.Invalid) {
				result.GenerateException();
			}
		}

		protected override Task<ITransactionEnvelope> AssembleEvent(LockContext lockContext) {
			return this.centralCoordinator.ChainComponentProvider.AssemblyProvider.GenerateNeuraliumMultiTransferTransaction(this.accountCode, this.recipients, this.tip, this.correlationContext, lockContext);
		}
		
		protected override async Task PerformSanityChecks(LockContext lockContext) {

			if(!this.recipients.Any()) {
				throw new EventGenerationException("Recipients must be set.");
			}

			AccountId accountId = await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetPublicAccountId(this.accountCode, lockContext).ConfigureAwait(false);

			var availableBalance = await centralCoordinator.ChainComponentProvider.WalletProvider.GetUsableAccountBalance(accountCode, lockContext).ConfigureAwait(false);
			
			if(availableBalance < (this.amount + this.tip)) {
				throw new EventGenerationException($"We dont have enough tokens. Our usable total is {availableBalance}, so we can not set {(this.amount + this.tip)} tokens.");
			}
		}
	}
}