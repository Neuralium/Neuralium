using System.Threading.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Models;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Messages.Appointments;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Messages.Appointments {
	public interface INeuraliumPuzzleExecutionWorkflow : IPuzzleExecutionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumPuzzleExecutionWorkflow  : PuzzleExecutionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumPuzzleExecutionWorkflow {

		public NeuraliumPuzzleExecutionWorkflow(INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base( centralCoordinator, correlationContext) {
		}
		
	}
}