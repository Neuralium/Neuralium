using System.Collections.Immutable;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumTransferTransaction : INeuraliumTipingTransaction, ISendTransaction {
		AccountId Recipient { get; set; }
	}

	public class NeuraliumTransferTransaction : NeuraliumTipingTransaction, INeuraliumTransferTransaction {

		public AccountId Recipient { get; set; } = new AccountId();
		public Amount Amount { get; set; } = new Amount();
		
		public override HashNodeList GetStructuresArray(Enums.MutableStructureTypes types) {
			HashNodeList nodeList = base.GetStructuresArray(types);

			nodeList.Add(this.Recipient);
			nodeList.Add(this.Amount);

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("Recipient", this.Recipient);
			jsonDeserializer.SetProperty("Amount", this.Amount);
		}

		public override Enums.TransactionTargetTypes TargetType => Enums.TransactionTargetTypes.Range;
		public override AccountId[] ImpactedAccounts => this.TargetAccountsAndSender();
		public override AccountId[] TargetAccounts => this.GetAccountIds(this.Recipient);

		protected override void Sanitize() {
			base.Sanitize();

			this.Amount = NeuraliumUtilities.CapAndRound(this.Amount);
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (TOKEN_TRANSFER: NeuraliumTransactionTypes.Instance.NEURALIUM_TRANSFER, 1, 0);
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {
			base.RehydrateHeader(rehydrator);

			this.Recipient.Rehydrate(rehydrator);
			this.Amount.Rehydrate(rehydrator);
		}

		protected override void DehydrateHeader(IDataDehydrator dehydrator) {
			base.DehydrateHeader(dehydrator);

			this.Recipient.Dehydrate(dehydrator);

			this.Amount.Dehydrate(dehydrator);
		}
	}

}