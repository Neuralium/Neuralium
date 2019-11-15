using Blockchains.Neuralium.Classes.Configuration;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {

	public interface INeuraliumWalletTransactionHistoryFileInfo : IWalletTransactionHistoryFileInfo {
	}

	public class NeuraliumWalletTransactionHistoryFileInfo : WalletTransactionHistoryFileInfo<NeuraliumWalletTransactionHistory>, INeuraliumWalletTransactionHistoryFileInfo {

		public NeuraliumWalletTransactionHistoryFileInfo(IWalletAccount account, string filename, NeuraliumBlockChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(account, filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
		}
	}
}