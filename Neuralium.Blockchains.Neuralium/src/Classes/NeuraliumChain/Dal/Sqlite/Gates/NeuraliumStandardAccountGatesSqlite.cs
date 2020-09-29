using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.Gates;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.Gates;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.Gates {

	public interface INeuraliumStandardAccountGatesSqlite : IStandardAccountGatesSqlite , INeuraliumStandardAccountGates{
		
	}
	public class NeuraliumStandardAccountGatesSqlite : StandardAccountGatesSqlite, INeuraliumStandardAccountGatesSqlite {

	}
}