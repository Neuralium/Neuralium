using System;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {

#if TESTNET || DEVNET
	public interface ICreateNeuraliumRefillTransactionWorkflow : IGenerateNewTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class CreateNeuraliumRefillTransactionWorkflow : GenerateNewSignedTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, ICreateNeuraliumRefillTransactionWorkflow {
		private readonly string accountCode;

		private readonly TransactionId guid;
		private readonly SafeArrayHandle hash = SafeArrayHandle.Create();

		protected INeuraliumWalletGenerationCache NeuraliumWalletGenerationCache => (INeuraliumWalletGenerationCache)this.WalletGenerationCache;
		protected INeuraliumRefillNeuraliumsTransaction NeuraliumTransaction => (INeuraliumRefillNeuraliumsTransaction)this.BlockchainEvent;
		
		public CreateNeuraliumRefillTransactionWorkflow(string accountCode, byte expiration, string note, INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base(centralCoordinator, expiration, note, correlationContext) {
			this.accountCode = accountCode;
		}

		protected override Task<ITransactionEnvelope> AssembleEvent(LockContext lockContext) {
			return this.centralCoordinator.ChainComponentProvider.AssemblyProvider.GenerateRefillNeuraliumsTransaction(this.accountCode, this.correlationContext, lockContext);
		}
		
	}
#endif
}