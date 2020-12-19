using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Core.General.Types.Simple;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.P2p.Messages.Base;
using Neuralia.Blockchains.Tools.General.ExclusiveOptions;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging.Test {
	public class ServerAnswer : NetworkMessage<IBlockchainEventsRehydrationFactory> {
		public enum Statuses : byte {
			Ok = 0,
			TimeOutOfSync = 1,
			ChainsUnsupported = 2
		}

		public readonly ByteExclusiveOption supportedChains = new ByteExclusiveOption();

		public Statuses Status = Statuses.Ok;

		public override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

			dehydrator.Write(this.supportedChains.Value);
			dehydrator.Write((byte) this.Status);
		}

		public override void Rehydrate(IDataRehydrator rehydrator, IBlockchainEventsRehydrationFactory rehydrationFactory) {
			base.Rehydrate(rehydrator, rehydrationFactory);

			this.supportedChains.Value = rehydrator.ReadByte();
			this.Status = rehydrator.ReadByteEnum<Statuses>();
		}

		protected override ComponentVersion<SimpleUShort> SetIdentity() {
			return (TestFactory.MESS2, 1, 0);
		}

		protected override short SetWorkflowType() {
			return NeuraliumWorkflowIDs.TEST;
		}
	}
}