using System.Collections.Generic;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1 {
	public interface INeuraliumElectionContextImplementation {
		
		
		Dictionary<Enums.MiningTiers, decimal> MiningTierBounties {get;}
		
		bool MaintenanceServiceFeesEnabled { get; set; }
		SimplePercentage MaintenanceServiceFees { get; set; }

		IBountyAllocationMethod BountyAllocationMethod { get; set; }
		ITransactionTipsAllocationMethod TransactionTipsAllocationMethod { get; set; }
	}

	public class NeuraliumElectionContextImplementation : INeuraliumElectionContextImplementation {

		/// <summary>
		///     The bounty we offer to the miners for participating in this election.
		/// </summary>
		public Dictionary<Enums.MiningTiers, decimal> MiningTierBounties {get;} = new Dictionary<Enums.MiningTiers, decimal>();
		
		/// <summary>
		///     are network service fees enabled?
		/// </summary>
		public bool MaintenanceServiceFeesEnabled { get; set; }

		/// <summary>
		///     A polite service fees applied on this block and returned to the moderators to help fund maintenance and expansion
		///     of the network
		/// </summary>
		public SimplePercentage MaintenanceServiceFees { get; set; } = new SimplePercentage();

		/// <summary>
		///     What heuristic method will we use to allocate the bounties
		/// </summary>
		public IBountyAllocationMethod BountyAllocationMethod { get; set; }

		/// <summary>
		///     How we will distribute the transaction fees.
		/// </summary>
		public ITransactionTipsAllocationMethod TransactionTipsAllocationMethod { get; set; }

		public NeuraliumElectionContextImplementation() {
			MiningTierUtils.FillMiningTierSet(this.MiningTierBounties, 0);
		}
		
		public HashNodeList GetStructuresArray() {
			HashNodeList nodeList = new HashNodeList();

			MiningTierUtils.AddStructuresArray(nodeList, this.MiningTierBounties);

			nodeList.Add(this.MaintenanceServiceFeesEnabled);
			nodeList.Add(this.MaintenanceServiceFees);

			nodeList.Add(this.BountyAllocationMethod);
			nodeList.Add(this.TransactionTipsAllocationMethod);

			return nodeList;
		}

		public void Rehydrate(IDataRehydrator rehydrator, IElectionContextRehydrationFactory electionContextRehydrationFactory) {
			
			MiningTierUtils.RehydrateMiningSet<decimal, Amount>(this.MiningTierBounties, 0, rehydrator, (v) => (decimal)v);

			this.MaintenanceServiceFeesEnabled = rehydrator.ReadBool();
			this.MaintenanceServiceFees = rehydrator.ReadRehydratable<SimplePercentage>();

			this.BountyAllocationMethod = BountyAllocationMethodRehydrator.Rehydrate(rehydrator);
			this.TransactionTipsAllocationMethod = TransactionTipsAllocationMethodRehydrator.Rehydrate(rehydrator);
		}
		
		public void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			jsonDeserializer.SetProperty("Mining Tiers Count", this.MiningTierBounties.Count);
			
			foreach(var entry in this.MiningTierBounties) {
				jsonDeserializer.SetProperty($"{entry.Key.ToString()}Bounty", entry.Value);
			}

			jsonDeserializer.SetProperty("MaintenanceServiceFeesEnabled", this.MaintenanceServiceFeesEnabled);
			jsonDeserializer.SetProperty("MaintenanceServiceFees", this.MaintenanceServiceFees?.Value??0);
			
			jsonDeserializer.SetProperty("TransactionTipsAllocationMethod", this.TransactionTipsAllocationMethod);
		}
	}
}