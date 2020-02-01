using System;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Validation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Data;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {
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

		protected override ITransactionEnvelope AssembleEvent() {
			return this.centralCoordinator.ChainComponentProvider.AssemblyProvider.GenerateNeuraliumTransferTransaction(this.accountUuid, this.targetAccountId, this.amount, this.tip, this.correlationContext);
		}

		protected override void PerformSanityChecks() {
			base.PreTransaction();

			if(this.targetAccountId == null) {
				throw new EventGenerationException("A valid target account Id must be set.");
			}

			var accountId = this.centralCoordinator.ChainComponentProvider.WalletProvider.GetPublicAccountId(this.accountUuid);

			if(this.targetAccountId == accountId) {
				throw new EventGenerationException("We cannot send a transaction to the same account Id.");
			}
		}
	}
}