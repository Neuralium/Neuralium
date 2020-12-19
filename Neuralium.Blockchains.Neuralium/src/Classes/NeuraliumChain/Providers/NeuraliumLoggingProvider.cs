using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	
	public interface INeuraliumLoggingProvider : ILoggingProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	/// <summary>
	///     Provide access to the chain state that is saved in the DB
	/// </summary>
	public class NeuraliumLoggingProvider : LoggingProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumLoggingProvider {

		public NeuraliumLoggingProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}