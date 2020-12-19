using System;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Core.General.Types.Simple;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.P2p.Messages.Base;
using Neuralia.Blockchains.Tools.General.ExclusiveOptions;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging.Test {
	public class TestTrigger : WorkflowTriggerMessage<IBlockchainEventsRehydrationFactory> {
		public readonly ByteExclusiveOption supportedChains = new ByteExclusiveOption();
		public DateTime localTime;

		public override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

			dehydrator.Write(this.localTime);
			dehydrator.Write(this.supportedChains.Value);
		}

		public override void Rehydrate(IDataRehydrator rehydrator, IBlockchainEventsRehydrationFactory rehydrationFactory) {
			base.Rehydrate(rehydrator, rehydrationFactory);

			this.localTime = rehydrator.ReadDateTime();
			this.supportedChains.Value = rehydrator.ReadByte();
		}

		protected override ComponentVersion<SimpleUShort> SetIdentity() {
			return (TestFactory.MESS1, 1, 0);
		}

		protected override short SetWorkflowType() {
			return NeuraliumWorkflowIDs.TEST;
		}
	}
}