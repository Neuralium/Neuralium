using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags {
	/// <summary>
	/// a transaction that sends tokens
	/// </summary>
	public interface ISendTransaction : INeuraliumTransaction {
		Amount Amount { get; set; }
	}
}