using Blockchains.Neuralium.Classes.Configuration;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {
	public class NeuraliumWalletAccountSnapshotFileInfo : WalletAccountSnapshotFileInfo<NeuraliumWalletStandardAccountSnapshot> {

		public NeuraliumWalletAccountSnapshotFileInfo(IWalletAccount account, string filename, NeuraliumBlockChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(account, filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
		}
	}
}