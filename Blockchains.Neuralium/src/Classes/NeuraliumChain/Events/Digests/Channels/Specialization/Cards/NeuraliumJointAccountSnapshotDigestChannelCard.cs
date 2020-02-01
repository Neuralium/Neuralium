using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;

using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards {

	public interface INeuraliumJointAccountSnapshotDigestChannelCard : INeuraliumAccountSnapshotDigestChannelCard, IJointAccountSnapshotDigestChannelCard {
	}

	public class NeuraliumJointAccountSnapshotDigestChannelCard : JointAccountSnapshotDigestChannelCard, INeuraliumJointAccountSnapshotDigestChannelCard {


		public Amount Balance { get; set; } = new Amount();

		public override void Rehydrate(IDataRehydrator rehydrator) {
			base.Rehydrate(rehydrator);

			this.Balance.Rehydrate(rehydrator);
		}

		public override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

			this.Balance.Dehydrate(dehydrator);
		}

		protected override IAccountAttribute CreateAccountFeature() {
			return new NeuraliumAccountAttribute();
		}

		protected override IJointMemberAccount CreateJointMemberAccount() {
			return new NeuraliumJointMemberAccount();
		}

		protected override IAccountSnapshotDigestChannelCard CreateCard() {
			return new NeuraliumJointAccountSnapshotDigestChannelCard();
		}
	}
}