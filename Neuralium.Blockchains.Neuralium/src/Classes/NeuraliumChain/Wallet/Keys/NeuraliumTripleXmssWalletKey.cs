using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys {

	public interface INeuraliumTripleXmssWalletKey : ITripleXmssWalletKey {
	}

	public class NeuraliumTripleXmssWalletKey : TripleXmssWalletKey, INeuraliumTripleXmssWalletKey {
		protected override WalletKeyHelper CreateWalletKeyHelper() {
			return new NeuraliumWalletKeyHelper();
		}
	}
}