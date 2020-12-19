using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures {
	public struct SAFUContributerSet {
		public AccountId Contributer { get; set; }
		public Amount Amount { get; set; }
	}

}