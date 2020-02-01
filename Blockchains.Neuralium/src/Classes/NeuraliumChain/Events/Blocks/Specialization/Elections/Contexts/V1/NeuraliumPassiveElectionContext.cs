using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts.V1;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1 {
	public interface INeuraliumPassiveElectionContext : IPassiveElectionContext, INeuraliumElectionContext {
	}

	public class NeuraliumPassiveElectionContext : PassiveElectionContext, INeuraliumPassiveElectionContext {
		private readonly NeuraliumElectionContextImplementation neuraliumElectionContextImplementation;

		public NeuraliumPassiveElectionContext() {
			this.neuraliumElectionContextImplementation = new NeuraliumElectionContextImplementation();
		}

		public Amount FirstTierBounty {
			get => this.neuraliumElectionContextImplementation.FirstTierBounty;
			set => this.neuraliumElectionContextImplementation.FirstTierBounty = value;
		}
		
		public Amount SecondTierBounty {
			get => this.neuraliumElectionContextImplementation.SecondTierBounty;
			set => this.neuraliumElectionContextImplementation.SecondTierBounty = value;
		}
		
		public Amount ThirdTierBounty {
			get => this.neuraliumElectionContextImplementation.ThirdTierBounty;
			set => this.neuraliumElectionContextImplementation.ThirdTierBounty = value;
		}

		public override void Rehydrate(IDataRehydrator rehydrator, IElectionContextRehydrationFactory electionContextRehydrationFactory) {
			base.Rehydrate(rehydrator, electionContextRehydrationFactory);

			this.neuraliumElectionContextImplementation.Rehydrate(rehydrator, electionContextRehydrationFactory);
		}

		public bool MaintenanceServiceFeesEnabled {
			get => this.neuraliumElectionContextImplementation.MaintenanceServiceFeesEnabled;
			set => this.neuraliumElectionContextImplementation.MaintenanceServiceFeesEnabled = value;
		}

		public SimplePercentage MaintenanceServiceFees {
			get => this.neuraliumElectionContextImplementation.MaintenanceServiceFees;
			set => this.neuraliumElectionContextImplementation.MaintenanceServiceFees = value;
		}

		public IBountyAllocationMethod BountyAllocationMethod {
			get => this.neuraliumElectionContextImplementation.BountyAllocationMethod;
			set => this.neuraliumElectionContextImplementation.BountyAllocationMethod = value;
		}

		public ITransactionTipsAllocationMethod TransactionTipsAllocationMethod {
			get => this.neuraliumElectionContextImplementation.TransactionTipsAllocationMethod;
			set => this.neuraliumElectionContextImplementation.TransactionTipsAllocationMethod = value;
		}
		
		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			this.neuraliumElectionContextImplementation.JsonDehydrate(jsonDeserializer);
		}
	}
}