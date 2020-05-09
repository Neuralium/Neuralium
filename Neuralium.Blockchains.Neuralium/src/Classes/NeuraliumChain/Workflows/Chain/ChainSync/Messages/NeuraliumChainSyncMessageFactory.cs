using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Block;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Digest;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.ChainSync.Messages;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Common.Classes.Tools;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages {
	public class NeuraliumChainSyncMessageFactory : ChainSyncMessageFactory<NeuraliumChainSyncTrigger, NeuraliumServerTriggerReply, NeuraliumFinishSync, NeuraliumClientRequestBlock, NeuraliumServerSendBlock, NeuraliumClientRequestBlockInfo, NeuraliumServerSendBlockInfo, NeuraliumClientRequestDigest, NeuraliumServerSendDigest, NeuraliumClientRequestDigestFile, NeuraliumServerSendDigestFile, NeuraliumClientRequestDigestInfo, NeuraliumServerSendDigestInfo, NeuraliumClientRequestBlockSliceHashes, NeuraliumServerRequestBlockSliceHashes>, IChainSyncMessageFactory {

		public NeuraliumChainSyncMessageFactory(IMainChainMessageFactory mainChainMessageFactory, BlockchainServiceSet serviceSet) : base(mainChainMessageFactory, serviceSet) {
		}
	}
}