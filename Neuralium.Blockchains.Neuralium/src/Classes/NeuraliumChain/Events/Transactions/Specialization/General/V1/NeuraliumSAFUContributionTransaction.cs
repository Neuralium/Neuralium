using System;
using System.Collections.Immutable;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumSAFUContributionTransaction : INeuraliumTipingTransaction {
		AdaptiveLong1_9 NumberDays { get; set; }
		Amount DailyProtection { get; set; }
		DateTime? Start { get; set; }
		bool AcceptSAFUTermsOfService { get; set; }
	}

	public class NeuraliumSAFUContributionTransaction : NeuraliumTipingTransaction, INeuraliumSAFUContributionTransaction {

		public bool AcceptSAFUTermsOfService { get; set; }

		/// <summary>
		///     the total amount of days
		/// </summary>
		public AdaptiveLong1_9 NumberDays { get; set; } = new AdaptiveLong1_9();
		/// <summary>
		///     how many neuraliums are protected per day
		/// </summary>
		public Amount DailyProtection { get; set; } = new Amount();

		public DateTime? Start { get; set; }

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.AcceptSAFUTermsOfService);
			nodeList.Add(this.NumberDays);
			nodeList.Add(this.DailyProtection);
			nodeList.Add(this.Start);

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("AcceptSAFUTermsOfService", this.AcceptSAFUTermsOfService);
			jsonDeserializer.SetProperty("NumberDays", this.NumberDays.Value);
			jsonDeserializer.SetProperty("DailyProtection", this.DailyProtection.Value);
			jsonDeserializer.SetProperty("Start", this.Start);
		}

		public override ImmutableList<AccountId> TargetAccounts => new[] {this.TransactionId.Account}.ToImmutableList();

		protected override void Sanitize() {
			base.Sanitize();

			this.DailyProtection = NeuraliumUtilities.CapAndRound(this.DailyProtection);
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (TOKEN_TRANSFER: NeuraliumTransactionTypes.Instance.NEURALIUM_SAFU_CONTRIBUTIONS, 1, 0);
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {
			base.RehydrateHeader(rehydrator);

			this.AcceptSAFUTermsOfService = rehydrator.ReadBool();
			this.NumberDays.Rehydrate(rehydrator);
			this.DailyProtection.Rehydrate(rehydrator);
			this.Start = rehydrator.ReadNullableDateTime();

		}

		protected override void DehydrateHeader(IDataDehydrator dehydrator) {
			base.DehydrateHeader(dehydrator);

			dehydrator.Write(this.AcceptSAFUTermsOfService);
			this.NumberDays.Dehydrate(dehydrator);
			this.DailyProtection.Dehydrate(dehydrator);
			dehydrator.Write(this.Start);
		}
	}

}