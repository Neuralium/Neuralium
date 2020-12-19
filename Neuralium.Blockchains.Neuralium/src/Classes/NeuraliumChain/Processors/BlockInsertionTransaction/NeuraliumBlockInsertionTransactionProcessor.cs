using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Processors.BlockInsertionTransaction;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Processors.BlockInsertionTransaction {
	public class NeuraliumBlockInsertionTransactionProcessor : BlockInsertionTransactionProcessor<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {

		public NeuraliumBlockInsertionTransactionProcessor(INeuraliumCentralCoordinator centralCoordinator, byte moderatorKeyOrdinal) : base(centralCoordinator, moderatorKeyOrdinal) {
		}
	}
}