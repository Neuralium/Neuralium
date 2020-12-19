using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.Configuration;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.DataAccess.Dal;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {

	public interface INeuraliumUserWalletFileInfo : IUserWalletFileInfo {
	}

	public class NeuraliumUserWalletFileInfo : UserWalletFileInfo<NeuraliumUserWallet>, INeuraliumUserWalletFileInfo {

		public NeuraliumUserWalletFileInfo(string filename, NeuraliumBlockChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
		}
	}
}