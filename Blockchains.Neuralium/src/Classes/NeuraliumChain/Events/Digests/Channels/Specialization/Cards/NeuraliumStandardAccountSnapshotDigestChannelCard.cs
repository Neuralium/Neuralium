using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards {

	public interface INeuraliumStandardAccountSnapshotDigestChannelCard : INeuraliumAccountSnapshotDigestChannelCard, IStandardAccountSnapshotDigestChannelCard {
	}

	public class NeuraliumStandardAccountSnapshotDigestChannelCard : StandardAccountSnapshotDigestChannelCard, INeuraliumStandardAccountSnapshotDigestChannelCard {
		
		public decimal Balance { get; set; } = new Amount();

		public override void Rehydrate(IDataRehydrator rehydrator) {
			base.Rehydrate(rehydrator);

			Amount amount = new Amount();
			amount.Rehydrate(rehydrator); 
			this.Balance = amount.Value;
		}

		public override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

			Amount amount = new Amount();
			amount = this.Balance;
			amount.Dehydrate(dehydrator);
		}

		protected override IAccountAttribute CreateAccountFeature() {
			return new NeuraliumAccountAttribute();
		}

		protected override IAccountSnapshotDigestChannelCard CreateCard() {
			return new NeuraliumStandardAccountSnapshotDigestChannelCard();
		}
	}
}