using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Block;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Digest;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.ChainSync;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Core.P2p.Connections;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync {
	public class NeuraliumServerChainSyncWorkflow : ServerChainSyncWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumChainSyncTrigger, NeuraliumServerTriggerReply, NeuraliumFinishSync, NeuraliumClientRequestBlock, NeuraliumClientRequestDigest, NeuraliumServerSendBlock, NeuraliumServerSendDigest, NeuraliumClientRequestBlockInfo, NeuraliumServerSendBlockInfo, NeuraliumClientRequestDigestFile, NeuraliumServerSendDigestFile, NeuraliumClientRequestDigestInfo, NeuraliumServerSendDigestInfo, NeuraliumClientRequestBlockSliceHashes, NeuraliumServerRequestBlockSliceHashes> {

		public NeuraliumServerChainSyncWorkflow(BlockchainTriggerMessageSet<NeuraliumChainSyncTrigger> triggerMessage, PeerConnection peerConnectionn, INeuraliumCentralCoordinator centralCoordinator) : base(triggerMessage, peerConnectionn, centralCoordinator) {
		}
	}
}