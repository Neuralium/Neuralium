using Neuralium.Blockchains.Neuralium.Classes.Configuration;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {

	
	public class NeuraliumWalletElectionsStatisticsFileInfo : WalletElectionsStatisticsFileInfo<NeuraliumWalletElectionsMiningSessionStatistics, NeuraliumWalletElectionsMiningAggregateStatistics> {

		public NeuraliumWalletElectionsStatisticsFileInfo(IWalletAccount account, string filename, NeuraliumBlockChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(account, filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
		}

		protected override NeuraliumWalletElectionsMiningAggregateStatistics CreateEntryType() {
			//we dont use this
			return default;
		}
	}
}