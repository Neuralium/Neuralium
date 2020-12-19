using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal {
	public class NeuraliumBlockchainEventSerializationFal : BlockchainEventSerializationFal {

		public NeuraliumBlockchainEventSerializationFal(ChainConfigurations configurations, ICentralCoordinator centralCoordinator, BlockChannelUtils.BlockChannelTypes enabledChannels, string blocksFolderPath, string digestFolderPath, IBlockchainDigestChannelFactory blockchainDigestChannelFactory, FileSystemWrapper fileSystem) : base(configurations, centralCoordinator, enabledChannels, blocksFolderPath, digestFolderPath, blockchainDigestChannelFactory, fileSystem) {
		}
	}
}