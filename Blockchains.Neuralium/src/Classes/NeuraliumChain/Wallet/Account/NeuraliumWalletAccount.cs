using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Cryptography.Encryption.Symetrical;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Tools.Cryptography;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Data.Arrays;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletAccount : IWalletAccount {
	}

	public class NeuraliumWalletAccount : WalletAccount, INeuraliumWalletAccount {

		public IEncryptorParameters NeuraliumTimelineFileEncryptionParameters { get; set; }
		public ByteArray NeuraliumTimelineFileSecret { get; set; }

		public override void InitializeNewEncryptionParameters(BlockchainServiceSet serviceSet, ChainConfigurations chainConfiguration) {
			base.InitializeNewEncryptionParameters(serviceSet, chainConfiguration);

			this.InitializeNewNeuraliumTimelineEncryptionParameters(serviceSet, chainConfiguration);
		}

		public override void ClearEncryptionParameters() {
			base.ClearEncryptionParameters();

			this.ClearNeuraliumTimelineEncryptionParameters();
		}

		public void ClearNeuraliumTimelineEncryptionParameters() {
			this.NeuraliumTimelineFileEncryptionParameters = null;
			this.NeuraliumTimelineFileSecret = null;
		}

		public virtual void InitializeNewNeuraliumTimelineEncryptionParameters(BlockchainServiceSet serviceSet, ChainConfigurations chainConfiguration) {
			// create those no matter what
			if(this.NeuraliumTimelineFileEncryptionParameters == null) {
				this.NeuraliumTimelineFileEncryptionParameters = FileEncryptorUtils.GenerateEncryptionParameters(chainConfiguration);
				var secretKey = new byte[333];
				GlobalRandom.GetNextBytes(secretKey);
				this.NeuraliumTimelineFileSecret = ByteArray.WrapAndOwn(secretKey);
			}
		}
	}
}