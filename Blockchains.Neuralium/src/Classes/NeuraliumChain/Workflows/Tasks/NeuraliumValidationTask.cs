// using Blockchains.Neuralium.Classes.NeuraliumChain.Managers;
// using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
// using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Tasks.Base;
// using Neuralia.Blockchains.Core.Workflows.Tasks.Routing;
//
// namespace Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Tasks {
// 	public interface INeuraliumValidationTask : IValidationTask {
// 	}
//
// 	public interface INeuraliumValidationTask<K> : INeuraliumValidationTask, IValidationTask<K> {
// 	}
//
// 	public interface INeuraliumValidationTask<T, K> : INeuraliumValidationTask<K>, IValidationTask<T, K>
// 		where T : IRoutedTaskRoutingHandler {
// 	}
//
// 	public class NeuraliumValidationTask<K> : NeuraliumValidationTaskGenerix<INeuraliumValidationManager, K> {
// 	}
//
// 	public class NeuraliumValidationTaskGenerix<T, K> : ValidationTask<T, K, INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumValidationTask<T, K>
// 		where T : IRoutedTaskRoutingHandler {
// 	}
// }