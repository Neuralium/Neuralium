using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Core.General.Types;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using System.Collections.Generic;
using System.Linq;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures {
	public class NeuraliumBlockHeaderAPI : BlockHeaderAPI{

		public Dictionary<byte, NeuraliumBlockHeaderFinalResultsAPI> GetNeuraliumFinalElectionResults()
        {
			return this.FinalElectionResults.ToDictionary(r => r.Key, r => (NeuraliumBlockHeaderFinalResultsAPI)r.Value);
        }

		public Dictionary<byte, NeuraliumBlockHeaderIntermediaryResultAPI> GetNeuraliumIntermediaryResults()
		{
			return this.IntermediaryElectionResults.ToDictionary(r => r.Key, r => (NeuraliumBlockHeaderIntermediaryResultAPI)r.Value);
		}

		public override BlockHeaderIntermediaryResultAPI CreateBlockHeaderIntermediaryResultAPI() {
			return new NeuraliumBlockHeaderIntermediaryResultAPI();
		}

		public override BlockHeaderFinalResultsAPI CreateBlockHeaderFinalResultsAPI() {
			return new NeuraliumBlockHeaderFinalResultsAPI();
		}
	}
	
	public class NeuraliumBlockHeaderIntermediaryResultAPI: BlockHeaderIntermediaryResultAPI {

	}
	
	public class NeuraliumBlockHeaderFinalResultsAPI: BlockHeaderFinalResultsAPI {

		public List<NeuraliumBlockHeaderElectedResultAPI> GetNeuraliumBlockHeaderElectedResults()
		{
			return this.ElectedResults.Select(e => (NeuraliumBlockHeaderElectedResultAPI)e).ToList();
		}

		public List<NeuraliumBlockHeaderDelegateResultAPI> GetNeuraliumBlockHeaderDelegateResults()
		{
			return this.DelegateResults.Select(e => (NeuraliumBlockHeaderDelegateResultAPI)e).ToList();
		}

		public override BlockHeaderElectedResultAPI CreateBlockHeaderElectedResultAPI() {
			return new NeuraliumBlockHeaderElectedResultAPI();
		}

		public override BlockHeaderDelegateResultAPI CreateBlockHeaderDelegateResultAPI() {
			throw new System.NotImplementedException();
		}
	}
	
	public class NeuraliumBlockHeaderElectedResultAPI: BlockHeaderElectedResultAPI {

		public decimal Bounty { get; set; }
		public decimal Tip { get; set; }
		
		public override void FillFromElectionResult(AccountId accountId, IElectedResults electionResults, IBlock block) {
			base.FillFromElectionResult(accountId, electionResults, block);

			if(electionResults is INeuraliumElectedResults neuraliumElectedResults) {
				this.Bounty = neuraliumElectedResults.BountyShare;

				var transactions = block.GetAllConfirmedTransactions();
				foreach(var transaction in neuraliumElectedResults.Transactions) {
					if(transactions.ContainsKey(transaction) && transactions[transaction] is ITipTransaction tipTransaction) {
						this.Tip += tipTransaction.Tip;
					}
				}
			}
		}
	}
	
	public class NeuraliumBlockHeaderDelegateResultAPI: BlockHeaderDelegateResultAPI {

		public decimal Bounty { get; set; }
		
		public override void FillFromDelegateResult(AccountId accountId, IDelegateResults delegateResults) {
			base.FillFromDelegateResult(accountId, delegateResults);

			if(delegateResults is INeuraliumDelegateResults neuraliumDelegateResults) {
				this.Bounty = neuraliumDelegateResults.BountyShare;
			}
		}
	}
	
	public class NeuraliumDecomposedBlockAPI : DecomposedBlockAPI{

		public NeuraliumBlockHeaderAPI GetNeuraliumBlockHeader()
        {
			return (NeuraliumBlockHeaderAPI)this.BlockHeader;
        }

		public NeuraliumDecomposedBlockAPI() {
			this.BlockHeader = new NeuraliumBlockHeaderAPI();
		}
	}
}