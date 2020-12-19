using System.Collections.Generic;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags {

	/// <summary>
	///     A special transaction type that is allowed to create new neuraliums
	/// </summary>
	public interface IMultiEmittingTransaction : INeuraliumModerationTransaction {
		List<RecipientSet> Recipients { get; }
	}
}