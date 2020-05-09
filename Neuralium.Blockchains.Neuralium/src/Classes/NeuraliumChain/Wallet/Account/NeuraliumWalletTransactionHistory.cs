using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletTransactionHistory : IWalletTransactionHistory {
		[BsonId]
		new string TransactionId { get; set; }

		decimal Amount { get; set; }
		decimal Tip { get; set; }
		NeuraliumWalletTransactionHistory.MoneratyTransactionTypes MoneratyTransactionType { get; set; }
	}

	public class NeuraliumWalletTransactionHistory : WalletTransactionHistory, INeuraliumWalletTransactionHistory {

		public enum MoneratyTransactionTypes {
			Debit,
			Credit
		}

		public decimal Amount { get; set; }
		public decimal Tip { get; set; }
		public MoneratyTransactionTypes MoneratyTransactionType { get; set; }
	}
}