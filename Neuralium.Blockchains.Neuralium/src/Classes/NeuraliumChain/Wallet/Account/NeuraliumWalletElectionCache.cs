using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletElectionCache : IWalletElectionCache {
	}

	public class NeuraliumWalletElectionCache : WalletElectionCache, INeuraliumWalletElectionCache {
	}
}