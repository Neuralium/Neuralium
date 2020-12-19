using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.Gates;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.Gates;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.Gates {
	public interface INeuraliumJointAccountGatesSqlite : IJointAccountGatesSqlite , INeuraliumJointAccountGates{
		
	}
	public class NeuraliumJointAccountGatesSqlite : JointAccountGatesSqlite, INeuraliumJointAccountGatesSqlite {

	}
}