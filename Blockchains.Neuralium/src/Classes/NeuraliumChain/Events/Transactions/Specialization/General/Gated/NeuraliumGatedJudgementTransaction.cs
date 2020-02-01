using System;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1.Implementations;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Gated;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Gated {
	
	public interface INeuraliumGatedJudgementTransaction : IGatedJudgementTransaction, INeuraliumTipingTransaction{}
	
	
	public class NeuraliumGatedJudgementTransaction : GatedJudgementTransaction, INeuraliumGatedJudgementTransaction {
		private readonly NeuraliumTipingTransactionImplementation tipImplement = new NeuraliumTipingTransactionImplementation();

		public Amount Tip {
			get => this.tipImplement.Tip;
			set => this.tipImplement.Tip = value;
		}
		
		public override HashNodeList GetStructuresArray() {

			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.tipImplement);

			return nodeList;
		}

		protected override void Sanitize() {
			base.Sanitize();

			this.tipImplement.Sanitize();
		}
		
		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			this.tipImplement.JsonDehydrate(jsonDeserializer);
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {
			base.RehydrateHeader(rehydrator);

			this.tipImplement.Rehydrate(rehydrator);
		}

		protected override void DehydrateHeader(IDataDehydrator dehydrator) {
			base.DehydrateHeader(dehydrator);

			this.tipImplement.Dehydrate(dehydrator);
		}
		
	}
}