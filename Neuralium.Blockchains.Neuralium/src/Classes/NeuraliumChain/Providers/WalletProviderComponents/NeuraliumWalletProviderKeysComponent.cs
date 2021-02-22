using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;

namespace Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers.WalletProviderComponents {


	public interface INeuraliumWalletProviderKeysComponentUtility : IWalletProviderKeysComponentUtility{
		
	}
	
	public interface INeuraliumWalletProviderKeysComponentReadonly : IWalletProviderKeysComponentReadonly{
		
	}

	
	public interface INeuraliumWalletProviderKeysComponentWrite : IWalletProviderKeysComponentWrite{
		
	}
	
	public interface INeuraliumWalletProviderKeysComponent : INeuraliumWalletProviderComponent, INeuraliumWalletProviderKeysComponentUtility, INeuraliumWalletProviderKeysComponentReadonly, INeuraliumWalletProviderKeysComponentWrite{
		
	}
	public class NeuraliumWalletProviderKeysComponent : WalletProviderKeysComponent<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumWalletProvider>, INeuraliumWalletProviderKeysComponent {
		
	}
}