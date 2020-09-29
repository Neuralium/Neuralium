using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Core;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletGenerationCache : IWalletGenerationCache {
		[BsonId]
		new string Key { get; set; }
		
	}

	public class NeuraliumWalletGenerationCache : WalletGenerationCache, INeuraliumWalletGenerationCache {


	}
}