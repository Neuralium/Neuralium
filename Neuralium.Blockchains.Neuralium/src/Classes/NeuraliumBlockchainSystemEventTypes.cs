using Neuralia.Blockchains.Core;

namespace Neuralium.Blockchains.Neuralium.Classes {
	public class NeuraliumBlockchainSystemEventTypes : BlockchainSystemEventTypes {

		public readonly BlockchainSystemEventType AccountTotalUpdated;
		public readonly BlockchainSystemEventType NeuraliumMiningBountyAllocated;
		public readonly BlockchainSystemEventType NeuraliumMiningPrimeElected;
		public readonly BlockchainSystemEventType NeuraliumTimelineUpdated;

		static NeuraliumBlockchainSystemEventTypes() {
		}

		protected NeuraliumBlockchainSystemEventTypes() {
			this.AccountTotalUpdated = this.CreateChildConstant();
			this.NeuraliumMiningBountyAllocated = this.CreateChildConstant();
			this.NeuraliumMiningPrimeElected = this.CreateChildConstant();
			this.NeuraliumTimelineUpdated = this.CreateChildConstant();

			//for debugging
			//this.PrintValues(",");
		}

		public static NeuraliumBlockchainSystemEventTypes NeuraliumInstance { get; } = new NeuraliumBlockchainSystemEventTypes();
	}
}