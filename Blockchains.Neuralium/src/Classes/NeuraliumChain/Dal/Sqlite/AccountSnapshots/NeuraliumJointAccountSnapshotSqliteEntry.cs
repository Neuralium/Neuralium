using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {
	public interface INeuraliumJointAccountSnapshotSqliteEntry : INeuraliumJointAccountSnapshotEntry<NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, IJointAccountSnapshotSqliteEntry<NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, INeuraliumAccountSnapshotSqliteEntry<NeuraliumJointAccountAttributeSqliteEntry> {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumJointAccountSnapshotSqliteEntry : JointAccountSnapshotSqliteEntry<NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, INeuraliumJointAccountSnapshotSqliteEntry {

		public Amount Balance { get; set; } = new Amount();
	}
}