using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {
	public interface INeuraliumJointMemberAccountSqliteEntry : INeuraliumJointMemberAccountEntry, IJointMemberAccountSqliteEntry {
	}

	public class NeuraliumJointMemberAccountSqliteEntry : JointMemberAccountSqliteEntry, INeuraliumJointMemberAccountSqliteEntry {
	}
}