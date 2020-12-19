using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.Gates;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.Gates;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.Gates;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.Gates {
	
	
	public interface INeuraliumGatesSqliteContext : IGatesSqliteContext<NeuraliumStandardAccountGatesSqlite, NeuraliumJointAccountGatesSqlite>, INeuraliumGatesContext<NeuraliumStandardAccountGatesSqlite, NeuraliumJointAccountGatesSqlite> {

	}

	public class NeuraliumGatesSqliteContext : GatesSqliteContext<NeuraliumStandardAccountGatesSqlite, NeuraliumJointAccountGatesSqlite>, INeuraliumGatesSqliteContext {

	}
}