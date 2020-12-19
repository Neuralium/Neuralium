using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Keys {
	public class NeuraliumWalletKeyHelper : WalletKeyHelper {

		protected override IChainTypeCreationFactory CreateChainTypeCreationFactory() {
			return new NeuraliumChainTypeCreationFactory(null);
		}
	}
}