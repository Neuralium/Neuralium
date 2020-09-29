using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys {

	public interface INeuraliumNTRUWalletKey : INTRUWalletKey {
	}

	public class NeuraliumNTRUWalletKey : NTRUWalletKey, INeuraliumNTRUWalletKey {
	}
}