using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {
	public interface INeuraliumJointAccountSnapshotSqliteEntry : INeuraliumJointAccountSnapshotEntry<NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, IJointAccountSnapshotSqliteEntry<NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, INeuraliumAccountSnapshotSqliteEntry<NeuraliumJointAccountAttributeSqliteEntry> {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumJointAccountSnapshotSqliteEntry : JointAccountSnapshotSqliteEntry<NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, INeuraliumJointAccountSnapshotSqliteEntry {

		public decimal Balance { get; set; } = new Amount();
	}
}