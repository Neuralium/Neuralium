using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Serialization;
using Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Factories;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumChainFactoryProvider : IChainFactoryProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumBlockchainEventsRehydrationFactory, INeuraliumMainChainMessageFactory, INeuraliumWorkflowTaskFactory, INeuraliumChainTypeCreationFactory, INeuraliumChainWorkflowFactory, INeuraliumClientWorkflowFactory, INeuraliumServerWorkflowFactory, INeuraliumGossipWorkflowFactory, INeuraliumChainDalCreationFactory> {
	}

	public class NeuraliumChainFactoryProvider : ChainFactoryProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumBlockchainEventsRehydrationFactory, INeuraliumMainChainMessageFactory, INeuraliumWorkflowTaskFactory, INeuraliumChainTypeCreationFactory, INeuraliumChainWorkflowFactory, INeuraliumClientWorkflowFactory, INeuraliumServerWorkflowFactory, INeuraliumGossipWorkflowFactory, INeuraliumChainDalCreationFactory>, INeuraliumChainFactoryProvider {

		public NeuraliumChainFactoryProvider(INeuraliumChainDalCreationFactory chainDalCreationFactory, INeuraliumChainTypeCreationFactory chainTypeCreationFactory, INeuraliumChainWorkflowFactory workflowFactory, INeuraliumMainChainMessageFactory messageFactory, INeuraliumClientWorkflowFactory clientWorkflowFactory, INeuraliumServerWorkflowFactory serverWorkflowFactory, INeuraliumGossipWorkflowFactory gossipWorkflowFactory, INeuraliumWorkflowTaskFactory taskFactory, INeuraliumBlockchainEventsRehydrationFactory blockchainEventsRehydrationFactory) : base(chainDalCreationFactory, chainTypeCreationFactory, workflowFactory, messageFactory, clientWorkflowFactory, serverWorkflowFactory, gossipWorkflowFactory, taskFactory, blockchainEventsRehydrationFactory) {
		}
	}
}