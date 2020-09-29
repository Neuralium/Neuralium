using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Core;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {
	public interface INeuraliumWalletTransactionHistory : IWalletTransactionHistory {
		[BsonId]
		new string TransactionId { get; set; }

		decimal Amount { get; set; }
		decimal Tip { get; set; }
		Enums.BookkeepingTypes BookkeepingType { get; set; }
	}

	public class NeuraliumWalletTransactionHistory : WalletTransactionHistory, INeuraliumWalletTransactionHistory {

		

		public decimal Amount { get; set; }
		public decimal Tip { get; set; }
		public Enums.BookkeepingTypes BookkeepingType { get; set; }
	}
}