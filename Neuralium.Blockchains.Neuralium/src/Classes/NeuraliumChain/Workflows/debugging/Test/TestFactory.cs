using System;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Messages;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.General.Types.Simple;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Logging;
using Neuralia.Blockchains.Core.P2p.Messages.Base;
using Neuralia.Blockchains.Core.P2p.Messages.MessageSets;
using Neuralia.Blockchains.Core.P2p.Messages.RoutingHeaders;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Serialization;
using Serilog;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging.Test {
	public class TestFactory : MessageFactory<IBlockchainEventsRehydrationFactory> {
		public const ushort MESS1 = 1;
		public const ushort MESS2 = 2;

		public TestFactory(BlockchainServiceSet serviceSet) : base(serviceSet) {
		}

		public override ITargettedMessageSet<IBlockchainEventsRehydrationFactory> RehydrateMessage(SafeArrayHandle data, TargettedHeader header, IBlockchainEventsRehydrationFactory rehydrationFactory) {
			IDataRehydrator dr = DataSerializationFactory.CreateRehydrator(data);

			SafeArrayHandle messageBytes = NetworkMessageSet.ExtractMessageBytes(dr);
			NetworkMessageSet.ResetAfterHeader(dr);
			IDataRehydrator messageRehydrator = DataSerializationFactory.CreateRehydrator(messageBytes);

			IBlockchainTargettedMessageSet messageSet = null;

			try {
				if(data?.Length == 0) {
					throw new ApplicationException("null message");
				}

				short workflowType = 0;
				ComponentVersion<SimpleUShort> version = null;

				messageRehydrator.Peek(rehydrator => {
					workflowType = rehydrator.ReadShort();

					if(workflowType != NeuraliumWorkflowIDs.TEST) {
						throw new ApplicationException("Invalid workflow type");
					}

					version = rehydrator.Rehydrate<ComponentVersion<SimpleUShort>>();
				});

				switch(version.Type.Value) {
					case MESS1:

						if(version == (1, 0)) {
							messageSet = this.CreateTestWorkflowTriggerSet(header);
						}

						break;

					case MESS2:

						if(version == (1, 0)) {
							messageSet = this.CreateServerAnswerSet(header);
						}

						break;

					default:

						throw new ApplicationException("invalid message type");
				}

				if(messageSet?.BaseMessage == null) {
					throw new ApplicationException("Invalid message type or version");
				}

				messageSet.BaseHeader = header; // set the header explicitely
				messageSet.RehydrateRest(dr, rehydrationFactory);
			} catch(Exception ex) {
				NLog.Default.Error(ex, "Invalid data sent");
			}

			return messageSet;
		}

	#region Explicit Creation methods

		/// <summary>
		///     this is the client side trigger method, when we build a brand new one
		/// </summary>
		/// <param name="workflowCorrelationId"></param>
		/// <returns></returns>
		public BlockchainTriggerMessageSet<TestTrigger> CreateTestWorkflowTriggerSet(uint workflowCorrelationId) {
			BlockchainTriggerMessageSet<TestTrigger> messageSet = this.MainMessageFactory.CreateTriggerMessageSet<BlockchainTriggerMessageSet<TestTrigger>, TestTrigger>(workflowCorrelationId);

			return messageSet;
		}

		private BlockchainTriggerMessageSet<TestTrigger> CreateTestWorkflowTriggerSet(TargettedHeader triggerHeader) {
			BlockchainTriggerMessageSet<TestTrigger> messageSet = this.MainMessageFactory.CreateTriggerMessageSet<BlockchainTriggerMessageSet<TestTrigger>, TestTrigger>(triggerHeader);

			return messageSet;
		}

		public BlockchainTargettedMessageSet<ServerAnswer> CreateServerAnswerSet(TargettedHeader triggerHeader = null) {
			return this.MainMessageFactory.CreateTargettedMessageSet<BlockchainTargettedMessageSet<ServerAnswer>, ServerAnswer>(triggerHeader);
		}

	#endregion

	}
}