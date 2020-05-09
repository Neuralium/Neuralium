using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.WalletSync;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.ChainSync;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.WalletSync;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Factories;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Factories {
	public interface INeuraliumClientWorkflowFactory : IClientChainWorkflowFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumClientWorkflowFactory : ClientChainWorkflowFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumClientWorkflowFactory {
		public NeuraliumClientWorkflowFactory(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override IClientChainSyncWorkflow CreateChainSynchWorkflow(FileSystemWrapper fileSystem) {
			return new NeuraliumClientChainSyncWorkflow(this.centralCoordinator, fileSystem);
		}

		public override ISyncWalletWorkflow CreateSyncWalletWorkflow() {
			return new NeuraliumSyncWalletWorkflow(this.centralCoordinator);
		}

		public NeuraliumClientChainSyncWorkflow CreateNeuraliumChainSynchWorkflow(FileSystemWrapper fileSystem) {
			return (NeuraliumClientChainSyncWorkflow) this.CreateChainSynchWorkflow(fileSystem);
		}

		public NeuraliumSyncWalletWorkflow CreateNeuraliumSyncWalletWorkflow() {
			return (NeuraliumSyncWalletWorkflow) this.CreateSyncWalletWorkflow();
		}
	}
}