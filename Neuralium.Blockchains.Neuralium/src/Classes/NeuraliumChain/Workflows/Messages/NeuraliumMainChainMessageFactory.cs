using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Chain.ChainSync.Messages;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging.Test;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.BlockchainMessageReceivedGossip.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.BlockReceivedGossip.Messages.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Gossip.TransactionReceivedGossip.Messages.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.Moderation.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain.ChainSync.Messages;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.P2p.Messages.MessageSets;
using Neuralia.Blockchains.Core.P2p.Messages.RoutingHeaders;
using Neuralia.Blockchains.Core.P2p.Workflows.AppointmentRequest.Messages;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages {
	public interface INeuraliumMainChainMessageFactory : IMainChainMessageFactory {
	}

	public class NeuraliumMainChainMessageFactory : MainChainMessageFactory, INeuraliumMainChainMessageFactory {

		public NeuraliumMainChainMessageFactory(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override IChainSyncMessageFactory GetChainSyncMessageFactory() {
			return new NeuraliumChainSyncMessageFactory(this, (BlockchainServiceSet) this.serviceSet);
		}

		public override IAppointmentRequestMessageFactory GetAppointmentRequestMessageFactory() {
			return new NeuraliumAppointmentRequestMessageFactory(this, (BlockchainServiceSet) this.serviceSet);
		}

		public override ITargettedMessageSet<IBlockchainEventsRehydrationFactory> Rehydrate(SafeArrayHandle data, TargettedHeader header, IBlockchainEventsRehydrationFactory rehydrationFactory) {
			using IDataRehydrator dr = DataSerializationFactory.CreateRehydrator(data);

			using SafeArrayHandle messageBytes = NetworkMessageSet.ExtractMessageBytes(dr);
			NetworkMessageSet.ResetAfterHeader(dr);
			using IDataRehydrator messageRehydrator = DataSerializationFactory.CreateRehydrator(messageBytes);

			short workflowType = messageRehydrator.ReadShort();

			if(workflowType == 0) {
				throw new ApplicationException("Invalid workflow type");
			}

			switch(workflowType) {
				case NeuraliumWorkflowIDs.TEST:

					return new TestFactory((BlockchainServiceSet) this.serviceSet).Rehydrate(data, header, rehydrationFactory);

				// default:
				//     throw new ApplicationException("Workflow message factory not found");
			}

			return base.Rehydrate(data, header, rehydrationFactory);
		}

		public override IGossipMessageSet RehydrateGossipMessage(SafeArrayHandle data, GossipHeader header, IBlockchainEventsRehydrationFactory rehydrationFactory) {

			if(header.chainId != NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium) {
				throw new ApplicationException("Chain must be a neuralium chain");
			}

			return base.RehydrateGossipMessage(data, header, rehydrationFactory);
		}

		public override IBlockchainGossipMessageSet CreateTransactionCreatedGossipMessageSet<GOSSIP_MESSAGE_SET, GOSSIP_MESSAGE_TYPE, EVENT_ENVELOPE_TYPE>(ITransactionEnvelope transaction) {
			return this.CreateGossipMessageSet<GOSSIP_MESSAGE_SET, GOSSIP_MESSAGE_TYPE, IDehydratedTransaction, EVENT_ENVELOPE_TYPE, EVENT_ENVELOPE_TYPE>(transaction.Contents);
		}

		public override IBlockchainGossipMessageSet CreateTransactionCreatedGossipMessageSet(ITransactionEnvelope envelope) {
			if(!(envelope.Contents.RehydratedEvent is INeuraliumTransaction castedTransaction)) {
				throw new ApplicationException("Invalid transaction type");
			}

			return this.CreateGossipMessageSet<NeuraliumGossipMessageSet<NeuraliumTransactionCreatedGossipMessage, INeuraliumTransactionEnvelope>, NeuraliumTransactionCreatedGossipMessage, INeuraliumTransactionEnvelope>((INeuraliumTransactionEnvelope) envelope);
		}

		public override IBlockchainGossipMessageSet CreateTransactionCreatedGossipMessageSet(GossipHeader header) {

			return this.CreateGossipMessageSet<NeuraliumGossipMessageSet<NeuraliumTransactionCreatedGossipMessage, INeuraliumTransactionEnvelope>, NeuraliumTransactionCreatedGossipMessage, IDehydratedTransaction, INeuraliumTransactionEnvelope>(header);
		}

		public override IBlockchainGossipMessageSet CreateBlockchainMessageCreatedGossipMessageSet<GOSSIP_MESSAGE_SET, GOSSIP_MESSAGE_TYPE, EVENT_ENVELOPE_TYPE>(IMessageEnvelope envelope) {
			return this.CreateGossipMessageSet<GOSSIP_MESSAGE_SET, GOSSIP_MESSAGE_TYPE, EVENT_ENVELOPE_TYPE>((EVENT_ENVELOPE_TYPE) envelope);
		}

		public override IBlockchainGossipMessageSet CreateBlockchainMessageCreatedGossipMessageSet(IMessageEnvelope envelope) {

			this.ValidateBlockchainMessageCreatedGossipMessageSetTypes(envelope);

			return this.CreateGossipMessageSet<NeuraliumGossipMessageSet<NeuraliumBlockchainMessageCreatedGossipMessage, INeuraliumMessageEnvelope>, NeuraliumBlockchainMessageCreatedGossipMessage, INeuraliumMessageEnvelope>((INeuraliumMessageEnvelope) envelope);
		}

		protected virtual void ValidateBlockchainMessageCreatedGossipMessageSetTypes(IMessageEnvelope envelope) {
			if(!(envelope.Contents.RehydratedEvent is INeuraliumBlockchainMessage castedMessage)) {

				var message = envelope.Contents.RehydratedEvent;
				// we do have some exceptions which are not specialized by chain. lets try them here. if none of those, then its invalid
				if(!(message is ActiveElectionCandidacyMessage || 
				     message is PassiveElectionCandidacyMessage || 
				     message is ElectionsRegistrationMessage ||
				     message is IInitiationAppointmentRequestMessage ||
				     message is IAppointmentRequestMessage ||
				     message is IAppointmentVerificationResultsMessage ||
				     message is IAppointmentVerificationConfirmationMessage)) {
					throw new ApplicationException("Invalid message type");
				}
			}
		}
		
		public override IBlockchainGossipMessageSet CreateBlockchainMessageCreatedGossipMessageSet(GossipHeader header) {

			return this.CreateGossipMessageSet<NeuraliumGossipMessageSet<NeuraliumBlockchainMessageCreatedGossipMessage, INeuraliumMessageEnvelope>, NeuraliumBlockchainMessageCreatedGossipMessage, IDehydratedBlockchainMessage, INeuraliumMessageEnvelope>(header);
		}

		public override IBlockchainGossipMessageSet CreateBlockCreatedGossipMessageSet(GossipHeader header) {
			return this.CreateGossipMessageSet<NeuraliumGossipMessageSet<NeuraliumBlockCreatedGossipMessage, INeuraliumBlockEnvelope>, NeuraliumBlockCreatedGossipMessage, IDehydratedBlock, INeuraliumBlockEnvelope>(header);
		}
	}
}