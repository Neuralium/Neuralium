using System;
using System.Threading.Tasks;
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

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {
	public interface ICreateNeuraliumTransferTransactionWorkflow : IGenerateNewTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class CreateNeuraliumTransferTransactionWorkflow : GenerateNewTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumAssemblyProvider>, ICreateNeuraliumTransferTransactionWorkflow {
		private readonly Guid accountUuid;
		private readonly Amount amount;
		private readonly TransactionId guid;
		private readonly SafeArrayHandle hash = SafeArrayHandle.Create();
		private readonly AccountId targetAccountId;
		private readonly Amount tip;

		public CreateNeuraliumTransferTransactionWorkflow(Guid accountUuid, AccountId targetAccountId, Amount amount, Amount tip, byte expiration, string note, INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base(centralCoordinator, expiration, note, correlationContext) {
			this.accountUuid = accountUuid;
			this.targetAccountId = targetAccountId;
			this.amount = amount;
			this.tip = tip;
		}

		protected override ValidationResult ValidateContents(ITransactionEnvelope envelope) {
			ValidationResult result = base.ValidateContents(envelope);

			if(result.Invalid) {
				return result;
			}

			return NeuraliumTransactionCreationUtils.ValidateTransaction(envelope.Contents.RehydratedTransaction);
		}

		protected override Task<ITransactionEnvelope> AssembleEvent(LockContext lockContext) {
			return this.centralCoordinator.ChainComponentProvider.AssemblyProvider.GenerateNeuraliumTransferTransaction(this.accountUuid, this.targetAccountId, this.amount, this.tip, this.correlationContext, lockContext);
		}

		protected override async Task PerformSanityChecks(LockContext lockContext) {
			await base.PreTransaction(lockContext).ConfigureAwait(false);

			if(this.targetAccountId == default(AccountId)) {
				throw new EventGenerationException("A valid target account Id must be set.");
			}

			AccountId accountId = await this.centralCoordinator.ChainComponentProvider.WalletProvider.GetPublicAccountId(this.accountUuid, lockContext).ConfigureAwait(false);

			if(this.targetAccountId == accountId) {
				throw new EventGenerationException("We cannot send a transaction to the same account Id.");
			}

			var availableBalance = await centralCoordinator.ChainComponentProvider.WalletProvider.GetUsableAccountBalance(accountUuid, lockContext).ConfigureAwait(false);
			
			if(availableBalance < (this.amount + this.tip)) {
				throw new EventGenerationException($"We dont have enough tokens. Our usable total is {availableBalance}, so we can not set {(this.amount + this.tip)} tokens.");
			}
		}
	}
}