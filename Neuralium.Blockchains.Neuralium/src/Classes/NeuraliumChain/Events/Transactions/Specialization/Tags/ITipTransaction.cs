using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags {

	public interface ITip {
		Amount Tip { get; set; }
	}

	/// <summary>
	///     if transactions support transaction fees, this is the interface to go to
	/// </summary>
	public interface ITipTransaction : INeuraliumTransaction, ITip {
	}
}