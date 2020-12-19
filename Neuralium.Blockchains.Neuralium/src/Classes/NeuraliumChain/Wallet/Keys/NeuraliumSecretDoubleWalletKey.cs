using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys {

	public interface INeuraliumSecretDoubleWalletKey : ISecretDoubleWalletKey, INeuraliumSecretComboWalletKey {
	}

	public class NeuraliumSecretDoubleWalletKey : SecretDoubleWalletKey, INeuraliumSecretDoubleWalletKey {
		protected override WalletKeyHelper CreateWalletKeyHelper() {
			return new NeuraliumWalletKeyHelper();
		}
	}
}