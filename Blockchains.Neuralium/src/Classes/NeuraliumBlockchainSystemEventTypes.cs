using Neuralia.Blockchains.Core;

namespace Blockchains.Neuralium.Classes {
	public class NeuraliumBlockchainSystemEventTypes : BlockchainSystemEventTypes {

		public readonly BlockchainSystemEventType AccountTotalUpdated;
		public readonly BlockchainSystemEventType NeuraliumMiningBountyAllocated;
		public readonly BlockchainSystemEventType NeuraliumMiningPrimeElected;

		static NeuraliumBlockchainSystemEventTypes() {
		}

		protected NeuraliumBlockchainSystemEventTypes() {
			this.AccountTotalUpdated = this.CreateChildConstant();
			this.NeuraliumMiningBountyAllocated = this.CreateChildConstant();
			this.NeuraliumMiningPrimeElected = this.CreateChildConstant();

			//for debugging
			//this.PrintValues(",");
		}

		public static NeuraliumBlockchainSystemEventTypes NeuraliumInstance { get; } = new NeuraliumBlockchainSystemEventTypes();
	}
}