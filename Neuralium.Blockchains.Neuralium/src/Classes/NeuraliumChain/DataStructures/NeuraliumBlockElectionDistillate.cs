using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {

	public class NeuraliumBlockElectionDistillate : BlockElectionDistillate {

		public override IntermediaryElectionContextDistillate CreateIntermediateElectionContext() {
			return new NeuraliumIntermediaryElectionContextDistillate();
		}

		public override PassiveElectionContextDistillate CreatePassiveElectionContext() {
			return new NeuraliumPassiveElectionContextDistillate();
		}

		public override FinalElectionResultDistillate CreateFinalElectionResult() {
			return new NeuraliumFinalElectionResultDistillate();
		}
	}

	public class NeuraliumPassiveElectionContextDistillate : PassiveElectionContextDistillate {
	}

	public class NeuraliumFinalElectionResultDistillate : FinalElectionResultDistillate {
		public decimal BountyShare;
		public decimal TransactionTips;
	}

	public class NeuraliumElectedCandidateResultDistillate : ElectedCandidateResultDistillate {
	}

	public class NeuraliumIntermediaryElectionContextDistillate : IntermediaryElectionContextDistillate {
	}
}