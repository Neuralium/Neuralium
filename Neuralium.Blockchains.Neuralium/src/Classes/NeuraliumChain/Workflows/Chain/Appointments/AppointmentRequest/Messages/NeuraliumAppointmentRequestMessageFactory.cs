using System;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.P2p.Messages.Base;
using Neuralia.Blockchains.Core.P2p.Messages.MessageSets;
using Neuralia.Blockchains.Core.P2p.Messages.RoutingHeaders;
using Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages.V1;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Core.Workflows;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Core.General.Types.Simple;
using Neuralia.Blockchains.Tools.Serialization;
using Serilog;

namespace Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages {

	public class NeuraliumAppointmentRequestMessageFactory : AppointmentRequestMessageFactory<NeuraliumAppointmentRequestTrigger, NeuraliumAppointmentRequestServerReply> {

		public NeuraliumAppointmentRequestMessageFactory(IMainChainMessageFactory mainChainMessageFactory, BlockchainServiceSet serviceSet) : base(mainChainMessageFactory, serviceSet) {
		}
	}
	


	
}