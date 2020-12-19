using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.Gates;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.Gates {

	public interface INeuraliumGatesContext : IGatesContext {
	}

	public interface INeuraliumGatesContext<STANDARD_ACCOUNT_GATES, JOINT_ACCOUNT_GATES> : INeuraliumGatesContext, IGatesContext<STANDARD_ACCOUNT_GATES, JOINT_ACCOUNT_GATES>
		where STANDARD_ACCOUNT_GATES : class, INeuraliumStandardAccountGates 
		where JOINT_ACCOUNT_GATES : class, INeuraliumJointAccountGates {
	}
}