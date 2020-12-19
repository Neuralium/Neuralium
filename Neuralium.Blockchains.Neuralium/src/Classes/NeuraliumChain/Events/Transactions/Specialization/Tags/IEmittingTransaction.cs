using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags {

	/// <summary>
	///     A special transaction type that is allowed to create new neuraliums
	/// </summary>
	public interface IEmittingTransaction : INeuraliumModerationTransaction {
		Amount Amount { get; set; }
		AccountId Recipient { get; set; }
	}
}