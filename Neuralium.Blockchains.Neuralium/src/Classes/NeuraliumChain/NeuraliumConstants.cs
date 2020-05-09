using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain {
	public static class NeuraliumConstants {
		public const int DEFAULT_MODERATOR_ACCOUNT_ID = Constants.DEFAULT_MODERATOR_ACCOUNT_ID;
		public const int DEFAULT_MODERATOR_DESTRUCTION_ACCOUNT_ID = 2;
		public const int DEFAULT_NETWORK_MAINTENANCE_SERVICE_FEES_ACCOUNT_ID = 3;
		public const int DEFAULT_NEURALIUM_ELECTION_POOL_ACCOUNT_ID = 4;
		public const int DEFAULT_NEURALIUM_SAFU_ACCOUNT_ID = 5;

		public const int DEFAULT_NEURALIUM_GATED_VERIFIER_ACCOUNT_ID = 6;
		public const int DEFAULT_NEURALIUM_ESCROW_ACCOUNT_ID = 7;

		/// <summary>
		///     the id of the first account that will be publicly assigned.
		/// </summary>
		public const int FIRST_PUBLIC_ACCOUNT_NUMBER = Constants.FIRST_PUBLIC_ACCOUNT_NUMBER;

		public const BlockChannelUtils.BlockChannelTypes ActiveBlockchainChannels = BlockChannelUtils.BlockChannelTypes.HighHeader | BlockChannelUtils.BlockChannelTypes.LowHeader | BlockChannelUtils.BlockChannelTypes.Contents;

		public const BlockChannelUtils.BlockChannelTypes CompressedBlockchainChannels = BlockChannelUtils.BlockChannelTypes.LowHeader | BlockChannelUtils.BlockChannelTypes.Contents;

		public const int MAIN_NETWORK_ID = 0x51A21052;
		public const int TEST_NETWORK_ID = 0x10B62364;
		public const int DEV_NETWORK_ID = 0x10A61253;

		public const int ELECTION_POOL_ACCREDITATION_CERTIFICATE_ID = 6;
		public const int ESCROW_ACCREDITATION_CERTIFICATE_ID = 7;
		public const int GATED_VERIFIER_ACCREDITATION_CERTIFICATE_ID = 8;
		public const int SAFU_ACCREDITATION_CERTIFICATE_ID = 9;
	}
}