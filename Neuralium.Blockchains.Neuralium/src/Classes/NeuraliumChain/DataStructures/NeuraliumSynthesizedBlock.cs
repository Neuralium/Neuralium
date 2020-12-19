using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Core.General.Types;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {

	public class NeuraliumSynthesizedBlock : SynthesizedBlock {

		public override SynthesizedElectionResult CreateSynthesizedElectionResult() {
			return new NeuraliumSynthesizedElectionResult();
		}

		public class NeuraliumSynthesizedElectionResult : SynthesizedElectionResult {

			public decimal InfrastructureServiceFees { get; set; }

			public Dictionary<AccountId, decimal> DelegateBounties { get; set; } = new Dictionary<AccountId, decimal>();
			public Dictionary<AccountId, (decimal bountyShare, decimal tips)> ElectedGains { get; set; } = new Dictionary<AccountId, (decimal bountyShare, decimal tips)>();
		}
	}
}