using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys {

	public interface INeuraliumSecretWalletKey : ISecretWalletKey, INeuraliumXmssWalletKey {
	}

	public class NeuraliumSecretWalletKey : SecretWalletKey, INeuraliumSecretWalletKey {
	}
}