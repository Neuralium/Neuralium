using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Core;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions {

	public interface INeuraliumCreateChangeKeyTransactionWorkflow : ICreateChangeKeyTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumCreateChangeKeyTransactionWorkflow : CreateChangeKeyTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumAssemblyProvider>, INeuraliumCreateChangeKeyTransactionWorkflow {

		public NeuraliumCreateChangeKeyTransactionWorkflow(INeuraliumCentralCoordinator centralCoordinator, byte expiration, string note, byte changingKeyOrdinal, CorrelationContext correlationContext) : base(centralCoordinator, expiration, note, changingKeyOrdinal, correlationContext) {

		}
	}
}