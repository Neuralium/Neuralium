using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures {
	public struct RecipientSet {
		public RecipientSet(AccountId recipient, Amount amount) {
			this.Recipient = recipient;
			this.Amount = amount;
		}
		
		public RecipientSet(AccountId recipient) {
			this.Recipient = recipient;
			this.Amount = new Amount();
		}

		public AccountId Recipient { get; set; }
		public Amount Amount { get; set; }
	}

}