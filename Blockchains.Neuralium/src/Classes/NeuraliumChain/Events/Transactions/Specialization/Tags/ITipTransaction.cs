using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags {

	public interface ITip {
		Amount Tip { get; set; }
	}
	/// <summary>
	///     if transactions support transaction fees, this is the interface to go to
	/// </summary>
	public interface ITipTransaction: INeuraliumTransaction, ITip {
		
	}
}