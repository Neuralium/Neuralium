using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods.V1 {
	/// <summary>
	///     A simple but fair allocation method where any shared transactions between more than one elected is redistributed by
	///     selecting the smallest hash first who
	///     gets the first allocation (highest fees), then we move to the second lowest all the way up to the highest, where we
	///     then loop back
	///     to the lowest until no more transactions are remaining. of course, any transaction unique to the elected will be
	///     allocated to this one only.
	/// </summary>
	public class LowestToHighestTransactionTipsAllocationMethod : TransactionTipsAllocationMethod {

		protected override ComponentVersion<TransactionTipsAllocationMethodType> SetIdentity() {
			return (LowestToLargest: TransactionTipsAllocationMethodTypes.Instance.LowestToHighest, 1, 0);
		}

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			jsonDeserializer.SetProperty("Name", "LowestToHighest");
		}
	}
}