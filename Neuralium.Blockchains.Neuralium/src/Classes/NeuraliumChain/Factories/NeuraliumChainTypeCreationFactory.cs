using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories {

	public interface INeuraliumChainTypeCreationFactory : IChainTypeCreationFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumChainTypeCreationFactory : ChainTypeCreationFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainTypeCreationFactory {

		public NeuraliumChainTypeCreationFactory(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override IWalletAccount CreateNewWalletAccount() {
			return new NeuraliumWalletAccount();
		}

		public override IXmssWalletKey CreateNewXmssWalletKey() {
			return new NeuraliumXmssWalletKey();
		}

		public override IXmssMTWalletKey CreateNewXmssMTWalletKey() {
			return new NeuraliumXmssMTWalletKey();
		}

		// public override IQTeslaWalletKey CreateNewQTeslaWalletKey() {
		// 	return new NeuraliumQTeslaWalletKey();
		// }

		public override ISecretWalletKey CreateNewSecretWalletKey() {
			return new NeuraliumSecretWalletKey();
		}

		public override ISecretComboWalletKey CreateNewSecretComboWalletKey() {
			return new NeuraliumSecretComboWalletKey();
		}

		public override ISecretDoubleWalletKey CreateNewSecretDoubleWalletKey() {
			return new NeuraliumSecretDoubleWalletKey();
		}

		public override ISecretPentaWalletKey CreateNewSecretPentaWalletKey() {
			return new NeuraliumSecretPentaWalletKey();
		}

		public override ITripleXmssWalletKey CreateNewTripleXmssWalletKey() {
			return new NeuraliumTripleXmssWalletKey();
		}

		public override INTRUPrimeWalletKey CreateNewNTRUPrimeWalletKey() {
			return new NeuraliumNTRUPrimeWalletKey();
		}

		public override IMcElieceWalletKey CreateNewMcElieceWalletKey() {
			return new NeuraliumMcElieceWalletKey();
		}

		public override IUserWallet CreateNewUserWallet() {
			return new NeuraliumUserWallet();
		}

		public override WalletKeyHistory CreateNewWalletKeyHistory() {
			return new NeuraliumWalletKeyHistory();
		}

		public override WalletAccountKeyLog CreateNewWalletAccountKeyLog() {
			return new NeuraliumWalletAccountKeyLog();
		}
		
		public override WalletAccountKeyLogMetadata CreateNewWalletAccountKeyLogMetadata() {
			return new NeuraliumWalletAccountKeyLogMetadata();
		}

		public override IWalletGenerationCache CreateNewWalletAccountGenerationCache() {
			return new NeuraliumWalletGenerationCache();
		}

		public override IWalletTransactionHistory CreateNewWalletAccountTransactionHistory() {
			return new NeuraliumWalletTransactionHistory();
		}

		public override IWalletElectionsHistory CreateNewWalletElectionsHistoryEntry() {
			return new NeuraliumWalletElectionsHistory();
		}

		public override WalletElectionCache CreateNewWalletAccountElectionCache() {
			return new NeuraliumWalletElectionCache();
		}

		public override WalletAccountChainState CreateNewWalletAccountChainState() {
			return new NeuraliumWalletAccountChainState();
		}

		public override IWalletAccountChainStateKey CreateNewWalletAccountChainStateKey() {
			return new NeuraliumWalletAccountChainStateKey();
		}

		public override IWalletStandardAccountSnapshot CreateNewWalletAccountSnapshot() {
			return new NeuraliumWalletStandardAccountSnapshot();
		}

		public override IWalletJointAccountSnapshot CreateNewWalletJointAccountSnapshot() {
			return new NeuraliumWalletJointAccountSnapshot();
		}

		public override IEventPoolProvider CreateBlockchainEventPoolProvider(IChainMiningStatusProvider miningStatusProvider) {
			return new NeuraliumBlockchainEventPoolProvider(this.CentralCoordinator, miningStatusProvider);
		}

		public override IChainDataWriteProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateChainDataWriteProvider() {
			return new NeuraliumChainDataWriteProvider(this.CentralCoordinator);
		}
	}
}