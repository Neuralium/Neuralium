using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletKeyHistory : IWalletKeyHistory {
	}

	public class NeuraliumWalletKeyHistory : WalletKeyHistory, INeuraliumWalletKeyHistory {
	}
}