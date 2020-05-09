using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys {

	public interface INeuraliumSecretPentaWalletKey : ISecretPentaWalletKey, INeuraliumSecretDoubleWalletKey {
	}

	public class NeuraliumSecretPentaWalletKey : SecretPentaWalletKey, INeuraliumSecretPentaWalletKey {
		protected override WalletKeyHelper CreateWalletKeyHelper() {
			return new NeuraliumWalletKeyHelper();
		}
	}
}