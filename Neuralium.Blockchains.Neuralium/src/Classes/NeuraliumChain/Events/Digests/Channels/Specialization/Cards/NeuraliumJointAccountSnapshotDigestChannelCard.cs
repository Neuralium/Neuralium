using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Channels.Specialization.Cards;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests.Channels.Specialization.Cards {

	public interface INeuraliumJointAccountSnapshotDigestChannelCard : INeuraliumAccountSnapshotDigestChannelCard, IJointAccountSnapshotDigestChannelCard {
	}

	public class NeuraliumJointAccountSnapshotDigestChannelCard : JointAccountSnapshotDigestChannelCard, INeuraliumJointAccountSnapshotDigestChannelCard {

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

		protected override IJointMemberAccount CreateJointMemberAccount() {
			return new NeuraliumJointMemberAccount();
		}

		protected override IAccountSnapshotDigestChannelCard CreateCard() {
			return new NeuraliumJointAccountSnapshotDigestChannelCard();
		}
	}
}