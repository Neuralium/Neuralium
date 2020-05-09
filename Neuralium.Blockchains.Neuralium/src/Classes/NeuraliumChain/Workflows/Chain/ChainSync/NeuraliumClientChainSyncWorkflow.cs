using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Block;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Digest;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.ChainSync;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync {
	//CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, GOSSIP_WORKFLOW_TRIGGER_MESSAGE,                                                                              EVENT_ENVELOPE_TYPE, TRANSACTION_DETAILS, TRANSACTION_BLOCK_FACTORY, BLOCKCHAIN_MODEL, WORKFLOW_MESSAGE_FACTORY, CHAIN_SYNC_TRIGGER, SERVER_TRIGGER_REPLY, CLOSE_CONNECTION, HOURLY_HASHES, HOURLY_HASHES_MATCHES, HOURLY_TRANSACTIONS, TRANSACTIONS_DELTA, COMPLETE_TRANSACTION_REQUEST, COMPLETE_TRANSACTION_SET_RESPONSE
	public class NeuraliumClientChainSyncWorkflow : ClientChainSyncWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumChainSyncTrigger, NeuraliumServerTriggerReply, NeuraliumFinishSync, NeuraliumClientRequestBlock, NeuraliumClientRequestDigest, NeuraliumServerSendBlock, NeuraliumServerSendDigest, NeuraliumClientRequestBlockInfo, NeuraliumServerSendBlockInfo, NeuraliumClientRequestDigestFile, NeuraliumServerSendDigestFile, NeuraliumClientRequestDigestInfo, NeuraliumServerSendDigestInfo, NeuraliumClientRequestBlockSliceHashes, NeuraliumServerRequestBlockSliceHashes> {

		public NeuraliumClientChainSyncWorkflow(INeuraliumCentralCoordinator centralCoordinator, FileSystemWrapper fileSystem) : base(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, centralCoordinator, fileSystem) {

		}
	}
}