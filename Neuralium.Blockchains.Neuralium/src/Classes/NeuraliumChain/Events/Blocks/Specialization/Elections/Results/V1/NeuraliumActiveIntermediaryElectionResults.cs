using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1 {

	public interface INeuraliumActiveIntermediaryElectionResults : IActiveIntermediaryElectionResults, INeuraliumIntermediaryElectionResults {
	}

	public class NeuraliumActiveIntermediaryElectionResults : ActiveIntermediaryElectionResults, INeuraliumActiveIntermediaryElectionResults {
		private readonly NeuraliumIntermediaryElectionResultsimplementation neuraliumIntermediaryElectionResultsimplementation;

		public NeuraliumActiveIntermediaryElectionResults() {
			this.neuraliumIntermediaryElectionResultsimplementation = new NeuraliumIntermediaryElectionResultsimplementation();
		}

		public override IElectedResults CreateElectedResult() {
			return this.neuraliumIntermediaryElectionResultsimplementation.CreateElectedResult();
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			base.JsonDehydrate(jsonDeserializer);
			this.neuraliumIntermediaryElectionResultsimplementation.JsonDehydrate(jsonDeserializer);
		}

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.neuraliumIntermediaryElectionResultsimplementation);

			return nodeList;
		}
	}
}