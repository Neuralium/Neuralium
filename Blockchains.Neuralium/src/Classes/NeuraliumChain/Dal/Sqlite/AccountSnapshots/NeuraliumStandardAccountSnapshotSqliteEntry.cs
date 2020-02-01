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

	public interface INeuraliumStandardAccountSnapshotSqliteEntry : INeuraliumStandardAccountSnapshotEntry<NeuraliumStandardAccountAttributeSqliteEntry>, IStandardAccountSnapshotSqliteEntry<NeuraliumStandardAccountAttributeSqliteEntry>, INeuraliumAccountSnapshotSqliteEntry<NeuraliumStandardAccountAttributeSqliteEntry> {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumStandardAccountSnapshotSqliteEntry : StandardAccountSnapshotSqliteEntry<NeuraliumStandardAccountAttributeSqliteEntry>, INeuraliumStandardAccountSnapshotSqliteEntry {
		
		public Amount Balance { get; set; } = new Amount();
		}
}