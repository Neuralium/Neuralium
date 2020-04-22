
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Tools;
using Zio;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal {
	public class NeuraliumBlockchainEventSerializationFal : BlockchainEventSerializationFal {

		public NeuraliumBlockchainEventSerializationFal(ChainConfigurations configurations, BlockChannelUtils.BlockChannelTypes enabledChannels, string blocksFolderPath, string digestFolderPath, IBlockchainDigestChannelFactory blockchainDigestChannelFactory, FileSystemWrapper fileSystem) : base(configurations, enabledChannels, blocksFolderPath, digestFolderPath, blockchainDigestChannelFactory, fileSystem) {
		}
	}
}