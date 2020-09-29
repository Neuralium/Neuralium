using System;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Managers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Workflows.Base;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Managers {
	public interface INeuraliumBlockchainManager : IBlockchainManager<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumBlockchainManager : BlockchainManager<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumBlockchainManager {

		public NeuraliumBlockchainManager(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		protected override IWorkflow CreateTransactionWorkflow(ComponentVersion<TransactionType> version) {
			
			var factory = this.CentralCoordinator.ChainComponentProvider.ChainFactoryProvider.WorkflowFactory;

			if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_TRANSFER) {
				if(version == (1, 0)) {
					return factory.CreateSendNeuraliumsWorkflow("", new AccountId(), 0, 0,"", new CorrelationContext());
				}
			}
#if DEVNET || TESTNET
			if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_REFILL_NEURLIUMS) {
				if(version == (1, 0)) {
					return factory.CreateRefillNeuraliumsWorkflow("", new CorrelationContext());
				}
			}
#endif
			return base.CreateTransactionWorkflow(version);
		}
	}
}