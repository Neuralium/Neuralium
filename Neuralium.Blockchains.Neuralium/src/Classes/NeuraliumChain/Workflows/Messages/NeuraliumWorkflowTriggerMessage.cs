using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Core.P2p.Messages.Base;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages {
	/// <summary>
	///     the base class for trigger messages in the Neuralium chain
	/// </summary>
	public abstract class NeuraliumWorkflowTriggerMessage : WorkflowTriggerMessage<IBlockchainEventsRehydrationFactory> {
	}
}