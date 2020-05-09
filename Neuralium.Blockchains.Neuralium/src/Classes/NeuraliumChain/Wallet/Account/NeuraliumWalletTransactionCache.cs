using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletTransactionCache : IWalletTransactionCache {
		[BsonId]
		new string TransactionId { get; set; }

		decimal Amount { get; set; }
		decimal Tip { get; set; }
		NeuraliumWalletTransactionCache.MoneratyTransactionTypes MoneratyTransactionType { get; set; }
	}

	public class NeuraliumWalletTransactionCache : WalletTransactionCache, INeuraliumWalletTransactionCache {

		public enum MoneratyTransactionTypes {
			None,
			Debit,
			Credit
		}

		public decimal Amount { get; set; }
		public decimal Tip { get; set; }
		public MoneratyTransactionTypes MoneratyTransactionType { get; set; } = MoneratyTransactionTypes.None;
	}
}