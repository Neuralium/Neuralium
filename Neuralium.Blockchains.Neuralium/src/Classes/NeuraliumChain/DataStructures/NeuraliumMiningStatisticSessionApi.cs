using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {
	public class NeuraliumMiningStatisticSessionApi : MiningStatisticSessionAPI{
		public double AverageBountyPerBlock { get; set; }
		public double TotalBounties { get; set; }
		public double TotalTips { get; set; }
	}
	public class NeuraliumMiningStatisticAggregateApi : MiningStatisticAggregateAPI{
		public double AverageBountyPerBlock { get; set; }
		public double TotalBounties { get; set; }
		public double TotalTips { get; set; }
	}
	
}