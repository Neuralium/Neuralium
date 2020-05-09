using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1.Implementations;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Gated;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Gated {

	public interface INeuraliumThreeWayGatedTransferTransaction : IThreeWayGatedTransaction, INeuraliumTipingTransaction {
		Amount SenderVerifierBaseServiceFee { get; set; }
		Amount SenderVerifierServiceFee { get; set; }

		Amount ReceiverVerifierBaseServiceFee { get; set; }
		Amount ReceiverVerifierServiceFee { get; set; }

		Amount Amount { get; set; }
	}

	/// <summary>
	///     a special transaction which will be gated until a verifier either aprouves it or rejects. Tokens will be frozen in
	///     sender account until it is aprouved.
	/// </summary>
	public class NeuraliumThreeWayGatedTransferTransaction : ThreeWayGatedTransaction, INeuraliumThreeWayGatedTransferTransaction {

		private readonly NeuraliumTipingTransactionImplementation tipImplement = new NeuraliumTipingTransactionImplementation();

		public Amount Tip {
			get => this.tipImplement.Tip;
			set => this.tipImplement.Tip = value;
		}

		/// <summary>
		///     the service fee given to the verifier as soon as the transaction comes into action. This is given even if the
		///     transaction times out (no action is taken) and defaults
		/// </summary>
		public Amount SenderVerifierBaseServiceFee { get; set; } = new Amount();

		/// <summary>
		///     the service fee given to the verifier once a verification judgement is made.
		/// </summary>
		public Amount SenderVerifierServiceFee { get; set; } = new Amount();

		public Amount ReceiverVerifierBaseServiceFee { get; set; } = new Amount();
		public Amount ReceiverVerifierServiceFee { get; set; } = new Amount();

		/// <summary>
		///     The amount of neuraliums to transfer in the transaction
		/// </summary>
		public Amount Amount { get; set; } = new Amount();

		public override HashNodeList GetStructuresArray() {

			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.SenderVerifierBaseServiceFee);
			nodeList.Add(this.SenderVerifierServiceFee);

			nodeList.Add(this.ReceiverVerifierBaseServiceFee);
			nodeList.Add(this.ReceiverVerifierServiceFee);

			nodeList.Add(this.Amount);

			nodeList.Add(this.tipImplement);

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("SenderVerifierBaseServiceFee", this.SenderVerifierBaseServiceFee);
			jsonDeserializer.SetProperty("SenderVerifierServiceFee", this.SenderVerifierServiceFee);

			jsonDeserializer.SetProperty("ReceiverVerifierBaseServiceFee", this.ReceiverVerifierBaseServiceFee);
			jsonDeserializer.SetProperty("ReceiverVerifierServiceFee", this.ReceiverVerifierServiceFee);

			jsonDeserializer.SetProperty("Amount", this.Amount);
			this.tipImplement.JsonDehydrate(jsonDeserializer);
		}

		protected override void Sanitize() {
			base.Sanitize();

			this.SenderVerifierBaseServiceFee = NeuraliumUtilities.CapAndRound(this.SenderVerifierBaseServiceFee);
			this.SenderVerifierServiceFee = NeuraliumUtilities.CapAndRound(this.SenderVerifierServiceFee);

			this.ReceiverVerifierBaseServiceFee = NeuraliumUtilities.CapAndRound(this.ReceiverVerifierBaseServiceFee);
			this.ReceiverVerifierServiceFee = NeuraliumUtilities.CapAndRound(this.ReceiverVerifierServiceFee);

			if(this.SenderVerifierBaseServiceFee >= this.SenderVerifierServiceFee) {
				throw new ApplicationException($"Invalid transactoin arguments. {nameof(this.SenderVerifierBaseServiceFee)} must be less than {nameof(this.SenderVerifierServiceFee)}");
			}

			if((this.ReceiverVerifierBaseServiceFee != 0M) && (this.ReceiverVerifierBaseServiceFee >= this.ReceiverVerifierServiceFee)) {
				throw new ApplicationException($"Invalid transactoin arguments. {nameof(this.ReceiverVerifierBaseServiceFee)} must be less than {nameof(this.ReceiverVerifierServiceFee)}");
			}

			this.Amount = NeuraliumUtilities.CapAndRound(this.Amount);

			this.tipImplement.Sanitize();
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {
			base.RehydrateHeader(rehydrator);

			this.SenderVerifierBaseServiceFee.Rehydrate(rehydrator);
			this.SenderVerifierServiceFee.Rehydrate(rehydrator);

			this.ReceiverVerifierBaseServiceFee.Rehydrate(rehydrator);
			this.ReceiverVerifierServiceFee.Rehydrate(rehydrator);

			this.Amount.Rehydrate(rehydrator);

			this.tipImplement.Rehydrate(rehydrator);
		}

		protected override void DehydrateHeader(IDataDehydrator dehydrator) {
			base.DehydrateHeader(dehydrator);

			this.SenderVerifierBaseServiceFee.Dehydrate(dehydrator);
			this.SenderVerifierServiceFee.Dehydrate(dehydrator);

			this.ReceiverVerifierBaseServiceFee.Dehydrate(dehydrator);
			this.ReceiverVerifierServiceFee.Dehydrate(dehydrator);

			this.Amount.Dehydrate(dehydrator);

			this.tipImplement.Dehydrate(dehydrator);
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_THREE_WAY_GATED_TRANSFER_TRANSACTION, 1, 0);
		}
	}
}