using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1 {
	public interface INeuraliumIntermediaryElectionResultsimplementation : IJsonSerializable, ITreeHashable {
		IElectedResults CreateElectedResult();
	}

	public class NeuraliumIntermediaryElectionResultsimplementation : INeuraliumIntermediaryElectionResultsimplementation {

		public IElectedResults CreateElectedResult() {
			return new NeuraliumElectedResults();
		}

		public void JsonDehydrate(JsonDeserializer jsonDeserializer) {
		}

		public virtual HashNodeList GetStructuresArray() {
			HashNodeList nodeList = new HashNodeList();

			return nodeList;
		}
	}
}