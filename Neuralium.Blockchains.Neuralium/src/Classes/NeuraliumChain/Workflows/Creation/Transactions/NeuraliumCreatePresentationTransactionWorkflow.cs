using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Core;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {
	public interface INeuraliumCreatePresentationTransactionWorkflow : ICreatePresentationTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumCreatePresentationTransactionWorkflow : CreatePresentationTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumCreatePresentationTransactionWorkflow {

		public NeuraliumCreatePresentationTransactionWorkflow(INeuraliumCentralCoordinator centralCoordinator, byte expiration, CorrelationContext correlationContext, string accountCode) : base(centralCoordinator, expiration, correlationContext, accountCode) {
		}
	}
}