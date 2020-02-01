using System;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Core;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {
	public interface INeuraliumCreatePresentationTransactionWorkflow : ICreatePresentationTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumCreatePresentationTransactionWorkflow : CreatePresentationTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumAssemblyProvider>, INeuraliumCreatePresentationTransactionWorkflow {

		public NeuraliumCreatePresentationTransactionWorkflow(INeuraliumCentralCoordinator centralCoordinator, byte expiration, CorrelationContext correlationContext, Guid? accountUuId) : base(centralCoordinator, expiration, correlationContext, accountUuId) {
		}
	}
}