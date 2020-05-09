using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {

	public class NeuraliumMiningHistory : MiningHistory {
		public decimal BountyShare { get; set; }
		public decimal TransactionTips { get; set; }

		public override string ToString() {
			return base.ToString() + $" BountyShare: {this.BountyShare}, TransactionTips: {this.TransactionTips}";
		}
	}

}