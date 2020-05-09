using System.Collections.Generic;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.P2p.Connections;
using Neuralia.Blockchains.Core.Types;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools {
	public static class NeuraliumNodeInfo {

		public static readonly NodeInfo Full = new NodeInfo(Enums.GossipSupportTypes.Full, Enums.PeerTypes.FullNode, new Dictionary<BlockchainType, ChainSettings>(new[] {new KeyValuePair<BlockchainType, ChainSettings>(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, new ChainSettings(NodeShareType.Full))}));
		public static readonly NodeInfo Hub = new NodeInfo(Enums.GossipSupportTypes.None, Enums.PeerTypes.Hub, new Dictionary<BlockchainType, ChainSettings>(new[] {new KeyValuePair<BlockchainType, ChainSettings>(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, new ChainSettings(NodeShareType.None))}));
		public static readonly NodeInfo Unknown = new NodeInfo(Enums.GossipSupportTypes.None, Enums.PeerTypes.Unknown, new Dictionary<BlockchainType, ChainSettings>(new[] {new KeyValuePair<BlockchainType, ChainSettings>(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, new ChainSettings(NodeShareType.None))}));
	}
}