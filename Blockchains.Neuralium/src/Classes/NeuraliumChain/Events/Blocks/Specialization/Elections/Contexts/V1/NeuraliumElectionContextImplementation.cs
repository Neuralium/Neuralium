using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1 {
	public interface INeuraliumElectionContextImplementation {
		Amount FirstTierBounty { get; set; }
		Amount SecondTierBounty { get; set; }
		Amount ThirdTierBounty { get; set; }

		bool MaintenanceServiceFeesEnabled { get; set; }
		SimplePercentage MaintenanceServiceFees { get; set; }

		IBountyAllocationMethod BountyAllocationMethod { get; set; }
		ITransactionTipsAllocationMethod TransactionTipsAllocationMethod { get; set; }
	}

	public class NeuraliumElectionContextImplementation : INeuraliumElectionContextImplementation {

		/// <summary>
		///     The bounty we offer to the miners for participating in this election. This is the first tier
		/// </summary>
		public Amount FirstTierBounty { get; set; } = new Amount();
		
		/// <summary>
		///     The bounty we offer to the miners for participating in this election. This is the second tier
		/// </summary>
		public Amount SecondTierBounty { get; set; } = new Amount();
		
		/// <summary>
		///     The bounty we offer to the miners for participating in this election. This is the third tier
		/// </summary>
		public Amount ThirdTierBounty { get; set; } = new Amount();

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

		public HashNodeList GetStructuresArray() {
			HashNodeList nodeList = new HashNodeList();

			nodeList.Add(this.FirstTierBounty);
			nodeList.Add(this.SecondTierBounty);
			nodeList.Add(this.ThirdTierBounty);
			
			nodeList.Add(this.MaintenanceServiceFeesEnabled);
			nodeList.Add(this.MaintenanceServiceFees);

			nodeList.Add(this.BountyAllocationMethod);
			nodeList.Add(this.TransactionTipsAllocationMethod);

			return nodeList;
		}

		public void Rehydrate(IDataRehydrator rehydrator, IElectionContextRehydrationFactory electionContextRehydrationFactory) {

			this.FirstTierBounty.Rehydrate(rehydrator);
			this.SecondTierBounty.Rehydrate(rehydrator);
			this.ThirdTierBounty.Rehydrate(rehydrator);
			
			this.MaintenanceServiceFeesEnabled = rehydrator.ReadBool();
			this.MaintenanceServiceFees = rehydrator.ReadRehydratable<SimplePercentage>();

			this.BountyAllocationMethod = BountyAllocationMethodRehydrator.Rehydrate(rehydrator);
			this.TransactionTipsAllocationMethod = TransactionTipsAllocationMethodRehydrator.Rehydrate(rehydrator);
		}
		
		public void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			jsonDeserializer.SetProperty("FirstTierBounty", this.FirstTierBounty);
			jsonDeserializer.SetProperty("SecondTierBounty", this.SecondTierBounty);
			jsonDeserializer.SetProperty("ThirdTierBounty", this.ThirdTierBounty);
			
			jsonDeserializer.SetProperty("MaintenanceServiceFeesEnabled", this.MaintenanceServiceFeesEnabled);
			jsonDeserializer.SetProperty("MaintenanceServiceFees", this.MaintenanceServiceFees?.Value??0);
			
			jsonDeserializer.SetProperty("TransactionTipsAllocationMethod", this.TransactionTipsAllocationMethod);
		}
	}
}