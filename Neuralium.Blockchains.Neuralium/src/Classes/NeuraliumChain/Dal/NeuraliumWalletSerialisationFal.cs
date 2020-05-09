using System;
using System.IO;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal {

	public interface INeuraliumWalletSerialisationFal : IWalletSerialisationFal {
		INeuraliumWalletTimelineFileInfo CreateNeuraliumWalletTimelineFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails);
	}

	public class NeuraliumWalletSerialisationFal : WalletSerialisationFal<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumWalletSerialisationFal {

		public const string NEURALIUM_WALLET_TIMELINE_FILE_NAME = "NeuraliumTimeline.neuralium";

		public NeuraliumWalletSerialisationFal(INeuraliumCentralCoordinator centralCoordinator, string chainFilesDirectoryPath, FileSystemWrapper fileSystem) : base(centralCoordinator, chainFilesDirectoryPath, fileSystem) {
		}

		public override IWalletAccountSnapshotFileInfo CreateWalletSnapshotFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails) {
			return new NeuraliumWalletAccountSnapshotFileInfo(account, this.GetWalletAccountSnapshotPath(account.AccountUuid), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, walletPassphraseDetails);

		}

		public override IUserWalletFileInfo CreateWalletFileInfo() {
			return new NeuraliumUserWalletFileInfo(this.GetWalletFilePath(), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, new WalletPassphraseDetails(false, this.centralCoordinator.ChainComponentProvider.ChainConfigurationProviderBase.ChainConfiguration.DefaultWalletPassphraseTimeout));

		}

		public override IWalletTransactionCacheFileInfo CreateWalletTransactionCacheFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails) {

			return new NeuraliumWalletTransactionCacheFileInfo(account, this.GetWalletTransactionCachePath(account.AccountUuid), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, walletPassphraseDetails);
		}

		public override IWalletTransactionHistoryFileInfo CreateWalletTransactionHistoryFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails) {

			return new NeuraliumWalletTransactionHistoryFileInfo(account, this.GetWalletTransactionHistoryPath(account.AccountUuid), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, walletPassphraseDetails);
		}

		public override IWalletElectionsHistoryFileInfo CreateWalletElectionsHistoryFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails) {
			return new NeuraliumWalletElectionsHistoryFileInfo(account, this.GetWalletElectionsHistoryPath(account.AccountUuid), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, walletPassphraseDetails);
		}

		public override IWalletElectionsStatisticsFileInfo CreateWalletElectionsStatisticsFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails) {
			return new NeuraliumWalletElectionsStatisticsFileInfo(account, this.GetWalletElectionsStatisticsPath(account.AccountUuid), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, walletPassphraseDetails);
		}

		public INeuraliumWalletTimelineFileInfo CreateNeuraliumWalletTimelineFileInfo(IWalletAccount account, WalletPassphraseDetails walletPassphraseDetails) {
			return new NeuraliumWalletTimelineFileInfo(account, this.GetNeuraliumWalletTimelinePath(account.AccountUuid), this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.ChainConfiguration, this.centralCoordinator.BlockchainServiceSet, this, walletPassphraseDetails);
		}

		public virtual string GetNeuraliumWalletTimelinePath(Guid accountUuid) {
			return Path.Combine(this.GetWalletAccountsContentsFolderPath(accountUuid), NEURALIUM_WALLET_TIMELINE_FILE_NAME);
		}
	}
}