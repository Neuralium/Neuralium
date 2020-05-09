using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {

	public class NeuraliumWalletTransactionHistoryHeaderAPI : WalletTransactionHistoryHeaderAPI {
		public decimal Amount { get; set; }
		public decimal Tip { get; set; }

		public override string ToString() {
			return base.ToString() + $", Amount: {this.Amount}, Tip: {this.Tip}";
		}
	}

	public class NeuraliumWalletTransactionHistoryDetailsAPI : WalletTransactionHistoryDetailsAPI {
		public decimal Amount { get; set; }
		public decimal Tip { get; set; }

		public override string ToString() {
			return base.ToString() + $", Amount: {this.Amount}, Tip: {this.Tip}";
		}
	}

}