using System.Threading.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet.Extra;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Tools.Locking;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet.Extra {

	public interface INeuraliumAccountFileInfo : IAccountFileInfo {
		INeuraliumWalletTimelineFileInfo WalletTimelineFileInfo { get; set; }
	}

	public class NeuraliumAccountFileInfo : AccountFileInfo, INeuraliumAccountFileInfo {

		public NeuraliumAccountFileInfo(AccountPassphraseDetails accountSecurityDetails) : base(accountSecurityDetails) {
		}

		public INeuraliumWalletTimelineFileInfo WalletTimelineFileInfo { get; set; }

		public override async Task Load(LockContext lockContext) {
			await base.Load(lockContext).ConfigureAwait(false);

			await WalletTimelineFileInfo.Load(lockContext).ConfigureAwait(false);
		}

		public override async Task Save(LockContext lockContext) {
			await base.Save(lockContext).ConfigureAwait(false);

			await this.WalletTimelineFileInfo.Save(lockContext).ConfigureAwait(false);
		}

		public override async Task ChangeEncryption(LockContext lockContext) {
			await base.ChangeEncryption(lockContext).ConfigureAwait(false);

			await this.WalletTimelineFileInfo.ChangeEncryption(lockContext).ConfigureAwait(false);
		}

		public override async Task Reset(LockContext lockContext) {
			await base.Reset(lockContext).ConfigureAwait(false);

			await this.WalletTimelineFileInfo.Reset(lockContext).ConfigureAwait(false);
		}

		public override async Task ReloadFileBytes(LockContext lockContext) {
			await base.ReloadFileBytes(lockContext).ConfigureAwait(false);

			await this.WalletTimelineFileInfo.ReloadFileBytes(lockContext).ConfigureAwait(false);
		}
	}
}