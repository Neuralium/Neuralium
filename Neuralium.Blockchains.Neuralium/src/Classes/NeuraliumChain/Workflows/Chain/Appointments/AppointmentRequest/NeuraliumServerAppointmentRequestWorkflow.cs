using System.Linq;
using System.Threading.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.P2p.Connections;
using Neuralia.Blockchains.Core.P2p.Messages.MessageSets;
using Neuralia.Blockchains.Core.P2p.Workflows.Base;
using Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages;
using Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages.V1;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Core.Types;
using Neuralia.Blockchains.Core.Workflows.Base;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Serilog;

namespace Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest {

	public class NeuraliumServerAppointmentRequestWorkflow : ServerAppointmentRequestWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumAppointmentRequestTrigger, NeuraliumAppointmentRequestServerReply> {

		public NeuraliumServerAppointmentRequestWorkflow(BlockchainTriggerMessageSet<NeuraliumAppointmentRequestTrigger> triggerMessage, PeerConnection peerConnectionn, INeuraliumCentralCoordinator centralCoordinator) : base(triggerMessage, peerConnectionn, centralCoordinator) {
		}
	}
}