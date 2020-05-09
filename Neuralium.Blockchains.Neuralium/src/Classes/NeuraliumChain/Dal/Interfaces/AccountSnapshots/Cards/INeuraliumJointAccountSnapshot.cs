using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards {

	public interface INeuraliumJointAccountSnapshot : IJointAccountSnapshot, INeuraliumAccountSnapshot {
	}

	public interface INeuraliumJointAccountSnapshot<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE> : IJointAccountSnapshot<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE>, INeuraliumAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumJointAccountSnapshot
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute
		where JOINT_MEMBER_FEATURE : IJointMemberAccount {
	}
}