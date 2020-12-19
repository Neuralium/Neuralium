using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Managers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Tasks.Base;
using Neuralia.Blockchains.Core.Workflows.Tasks.Routing;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Tasks {
	public interface INeuraliumBlockchainTask : IBlockchainTask {
	}

	public interface INeuraliumBlockchainTask<K> : INeuraliumBlockchainTask, IBlockchainTask<K> {
	}

	public interface INeuraliumBlockchainTask<T, K> : INeuraliumBlockchainTask<K>, IBlockchainTask<T, K>
		where T : IRoutedTaskRoutingHandler {
	}

	public class NeuraliumBlockchainTask<K> : NeuraliumBlockchainTaskGenerix<INeuraliumBlockchainManager, K> {
	}

	public class NeuraliumBlockchainTaskGenerix<T, K> : BlockchainTask<T, K, INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumBlockchainTask<T, K>
		where T : IRoutedTaskRoutingHandler {
	}
}