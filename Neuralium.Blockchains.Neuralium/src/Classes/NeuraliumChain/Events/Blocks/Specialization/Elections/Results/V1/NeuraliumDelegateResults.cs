using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1 {

	public interface INeuraliumDelegateResults : IDelegateResults {

		Amount BountyShare { get; set; }
	}

	public class NeuraliumDelegateResults : DelegateResults, INeuraliumDelegateResults {

		public Amount BountyShare { get; set; } = new Amount();

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			base.JsonDehydrate(jsonDeserializer);

			jsonDeserializer.SetProperty("BountyShare", this.BountyShare.Value);
		}

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.BountyShare);

			return nodeList;
		}
	}
}