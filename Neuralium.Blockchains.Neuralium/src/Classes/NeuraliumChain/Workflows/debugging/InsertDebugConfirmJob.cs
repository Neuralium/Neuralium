using System;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Base;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Workflows.Tasks.Routing;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging {
	public interface IInsertDebugConfirmWorkflow : INeuraliumChainWorkflow {
	}

	public class InsertDebugConfirmWorkflow : NeuraliumChainWorkflow, IInsertDebugConfirmWorkflow {
		private readonly TransactionId guid;
		private readonly SafeArrayHandle hash = SafeArrayHandle.Create();

		public InsertDebugConfirmWorkflow(TransactionId guid, SafeArrayHandle hash, INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
			this.guid = guid;
			this.hash = hash;
		}

		protected override Task PerformWork(IChainWorkflow workflow, TaskRoutingContext taskRoutingContext, LockContext lockContext) {
			throw new NotImplementedException();
		}
	}
}