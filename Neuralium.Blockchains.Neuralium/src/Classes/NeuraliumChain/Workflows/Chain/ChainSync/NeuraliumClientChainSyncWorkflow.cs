using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Block;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Digest;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.ChainSync;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync {
	public class NeuraliumClientChainSyncWorkflow : ClientChainSyncWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumChainSyncTrigger, NeuraliumServerTriggerReply, NeuraliumFinishSync, NeuraliumClientRequestBlock, NeuraliumClientRequestDigest, NeuraliumServerSendBlock, NeuraliumServerSendDigest, NeuraliumClientRequestBlockInfo, NeuraliumServerSendBlockInfo, NeuraliumClientRequestDigestFile, NeuraliumServerSendDigestFile, NeuraliumClientRequestDigestInfo, NeuraliumServerSendDigestInfo, NeuraliumClientRequestBlockSliceHashes, NeuraliumServerRequestBlockSliceHashes> {

		public NeuraliumClientChainSyncWorkflow(INeuraliumCentralCoordinator centralCoordinator, FileSystemWrapper fileSystem) : base(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, centralCoordinator, fileSystem) {

		}
	}
}