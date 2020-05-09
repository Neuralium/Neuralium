using System;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {

#if TESTNET || DEVNET
	public interface ICreateNeuraliumRefillTransactionWorkflow : IGenerateNewTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class CreateNeuraliumRefillTransactionWorkflow : GenerateNewTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumAssemblyProvider>, ICreateNeuraliumRefillTransactionWorkflow {
		private readonly Guid accountUuid;

		private readonly TransactionId guid;
		private readonly SafeArrayHandle hash = SafeArrayHandle.Create();

		public CreateNeuraliumRefillTransactionWorkflow(Guid accountUuid, byte expiration, string note, INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base(centralCoordinator, expiration, note, correlationContext) {
			this.accountUuid = accountUuid;
		}

		protected override Task<ITransactionEnvelope> AssembleEvent(LockContext lockContext) {
			return this.centralCoordinator.ChainComponentProvider.AssemblyProvider.GenerateRefillNeuraliumsTransaction(this.accountUuid, this.correlationContext, lockContext);
		}
	}
#endif
}