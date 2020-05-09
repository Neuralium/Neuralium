using Neuralia.Blockchains.Core;

namespace Neuralium.Blockchains.Neuralium.Classes {
	public class NeuraliumBlockchainTypes : BlockchainTypes {

		public readonly BlockchainType Neuralium;

		static NeuraliumBlockchainTypes() {
		}

		protected NeuraliumBlockchainTypes() {
			this.Neuralium = this.CreateChildConstant();
		}

		public static NeuraliumBlockchainTypes NeuraliumInstance { get; } = new NeuraliumBlockchainTypes();
	}

	public class NeuraliumBlockchainTypeNameProvider : IBlockchainTypeNameProvider {

		public string GetBlockchainTypeName(BlockchainType chainType) {
			if(chainType == NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium) {
				return "Neuralium";
			}

			return "";
		}

		public bool MatchesType(BlockchainType chainType) {
			return chainType == NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium;
		}
	}
}