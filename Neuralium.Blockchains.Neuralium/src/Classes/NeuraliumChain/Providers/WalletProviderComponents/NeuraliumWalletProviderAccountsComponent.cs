using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;

namespace Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers.WalletProviderComponents {

	public interface INeuraliumWalletProviderAccountsComponentUtility : IWalletProviderAccountsComponentUtility{
		
	}
	
	public interface INeuraliumWalletProviderAccountsComponentReadonly : IWalletProviderAccountsComponentReadonly{
		
	}

	
	public interface INeuraliumWalletProviderAccountsComponentWrite : IWalletProviderAccountsComponentWrite{
		
	}
	
	public interface INeuraliumWalletProviderAccountsComponent : INeuraliumWalletProviderComponent, INeuraliumWalletProviderAccountsComponentUtility, INeuraliumWalletProviderAccountsComponentReadonly, INeuraliumWalletProviderAccountsComponentWrite{
		
	}
	
	
	public class NeuraliumWalletProviderAccountsComponent : WalletProviderAccountsComponent<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumWalletProvider>, INeuraliumWalletProviderAccountsComponent  {
		
	}
}