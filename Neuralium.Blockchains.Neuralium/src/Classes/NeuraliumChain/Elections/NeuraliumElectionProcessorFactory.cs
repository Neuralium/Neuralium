using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Elections.Processors.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Elections.Processors;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Elections {
	public class NeuraliumElectionProcessorFactory : ElectionProcessorFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {

		protected override IElectionProcessor GetElectionProcessorV1(INeuraliumCentralCoordinator centralCoordinator, IEventPoolProvider chainEventPoolProvider) {
			return new NeuraliumElectionProcessor(centralCoordinator, chainEventPoolProvider);
		}
	}
}