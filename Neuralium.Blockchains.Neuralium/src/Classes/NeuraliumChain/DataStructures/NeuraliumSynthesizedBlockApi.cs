using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {

	public class NeuraliumSynthesizedBlockApi : SynthesizedBlockAPI<NeuraliumSynthesizedBlockApi.NeuraliumSynthesizedElectionResultAPI, NeuraliumSynthesizedBlockApi.NeuraliumSynthesizedGenesisBlockAPI> {

		public class NeuraliumSynthesizedElectionResultAPI : SynthesizedElectionResultAPI {

			public decimal BountyShare { get; set; }
			public decimal Tips { get; set; }
		}

		public class NeuraliumSynthesizedGenesisBlockAPI : SynthesizedGenesisBlockAPI {
		}
	}
}