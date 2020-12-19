using System;
using System.Threading.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.P2p.Connections;
using Neuralia.Blockchains.Core.P2p.Messages.MessageSets;
using Neuralia.Blockchains.Core.P2p.Workflows.Base;
using Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages;
using Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages.V1;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Core.Workflows.Base;
using Neuralia.Blockchains.Tools.Locking;
using Neuralium.Blockchains.Neuralium.Classes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Block;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages.V1.Digest;
using Serilog;

namespace Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest {
	
	public class NeuraliumClientAppointmentRequestWorkflow : ClientAppointmentRequestWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumAppointmentRequestTrigger, NeuraliumAppointmentRequestServerReply> {

		public NeuraliumClientAppointmentRequestWorkflow(Guid? requesterId, int? requesterIndex, DateTime? appointment, Enums.AppointmentRequestModes mode, INeuraliumCentralCoordinator centralCoordinator) : base(requesterId, requesterIndex, appointment, mode, NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, centralCoordinator) {

		}
	}
}

