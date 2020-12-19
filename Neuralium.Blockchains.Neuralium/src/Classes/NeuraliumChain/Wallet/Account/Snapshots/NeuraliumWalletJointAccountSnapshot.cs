using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots {

	public interface INeuraliumWalletJointAccountSnapshot<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE> : IWalletJointAccountSnapshot<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE>, INeuraliumWalletAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumJointAccountSnapshot<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE>
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute, new()
		where JOINT_MEMBER_FEATURE : INeuraliumJointMemberAccount {
	}

	public interface INeuraliumWalletJointAccountSnapshot : INeuraliumWalletJointAccountSnapshot<NeuraliumAccountAttribute, NeuraliumJointMemberAccount>, INeuraliumWalletAccountSnapshot {
	}

	public class NeuraliumWalletJointAccountSnapshot : WalletJointAccountSnapshot<NeuraliumAccountAttribute, NeuraliumJointMemberAccount>, INeuraliumWalletJointAccountSnapshot {
		public byte? FreezeDataVersion { get; set; }
		public byte[] FreezeData { get; set; }

		public decimal Balance { get; set; } = new Amount();
	}
}