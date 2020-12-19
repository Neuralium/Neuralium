using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Neuralia.Blockchains.Core;

namespace Neuralium.Api.Common {

	public interface INeuraliumApiMethods {

		public Task<bool> ToggleServerMessages(bool enable);
		public Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase, bool setKeysToo = false);
		public Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase);
		public Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode);

		public Task<bool> TestP2pPort();
		
		public Task<object> QuerySystemInfo();
		public Task<List<object>> QuerySupportedChains();
		public Task<string> Ping();
		public Task<object> GetPortMappingStatus();
		public Task<bool> ConfigurePortMappingMode(bool useUPnP, bool usePmP, int natDeviceIndex);

		public Task<byte> GetPublicIPMode();

		public Task SetUILocale(string locale);
		public Task<byte> GetMiningRegistrationIpMode(ushort chainType);

		public Task<bool> CompleteLongRunningEvent(int correlationId, object data);
		public Task<bool> RenewLongRunningEvent(int correlationId);

		public Task<bool> IsBlockchainSynced(ushort chainType);
		public Task<bool> IsWalletSynced(ushort chainType);

		public Task<int> GetCurrentOperatingMode(ushort chainType);
		public Task<bool> SyncBlockchain(ushort chainType, bool force);
		public Task<bool> Shutdown();
		public Task<object> BackupWallet(ushort chainType);
		public Task<bool> RestoreWalletFromBackup(ushort chainType, string backupsPath, string passphrase, string salt, string nonce, int iterations);

		public Task<int> QueryTotalConnectedPeersCount();
		
		public Task<List<object>> QueryPeerConnectionDetails();
		
		public Task<bool> QueryMiningPortConnectable();
		public Task<object> QueryChainStatus(ushort chainType);
		public Task<object> QueryWalletInfo(ushort chainType);

		public Task<object> QueryBlockChainInfo(ushort chainType);

		public Task<bool> IsWalletLoaded(ushort chainType);
		public Task<bool> WalletExists(ushort chainType);
		public Task<int> LoadWallet(ushort chainType, string passphrase = null);
		public Task<long> QueryBlockHeight(ushort chainType);
		public Task<long> QueryLowestAccountBlockSyncHeight(ushort chainType);
		public Task<string> QueryBlock(ushort chainType, long blockId);
		Task<object> QueryDecomposedBlock(ushort chainType, long blockId);
		Task<string> QueryDecomposedBlockJson(ushort chainType, long blockId);
		public Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId);
		public Task<byte[]> QueryBlockBytes(ushort chainType, long blockId);
		public Task<string> GetBlockSizeAndHash(ushort chainType, long blockId);
		public Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId);
		public Task<int> CreateStandardAccount(ushort chainType, string accountName, int accountType, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases);
		public Task<bool> SetActiveAccount(ushort chainType, string accountCode);
		public Task<object> QueryAppointmentConfirmationResult(ushort chainType, string accountCode);
		public Task<bool> ClearAppointment(ushort chainType, string accountCode);
		public Task<object> CanPublishAccount(ushort chainType, string accountCode);
		public Task SetSMSConfirmationCode(ushort chainType, string accountCode, long confirmationCode);
		public Task SetSMSConfirmationCodeString(ushort chainType, string accountCode, string confirmationCode);

		public Task GenerateXmssKeyIndexNodeCache(ushort chainType, string accountCode, byte ordinal, long index);
		public Task<int> CreateNewWallet(ushort chainType, string accountName, int accountType, bool encryptWallet, bool encryptKey, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases, bool publishAccount);

		public Task<bool> SetWalletPassphrase(int correlationId, string passphrase, bool setKeysToo = false);
		public Task<bool> SetKeysPassphrase(int correlationId, string passphrase);

		public Task<List<object>> QueryWalletTransactionHistory(ushort chainType, string accountCode);
		public Task<object> QueryWalletTransactionHistoryDetails(ushort chainType, string accountCode, string transactionId);
		public Task<List<object>> QueryWalletAccounts(ushort chainType);
		public Task<string> QueryDefaultWalletAccountId(ushort chainType);
		public Task<string> QueryDefaultWalletAccountCode(ushort chainType);
		public Task<object> QueryWalletAccountDetails(ushort chainType, string accountCode);
		public Task<object> QueryWalletAccountAppointmentDetails(ushort chainType, string accountCode);
		public Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, string accountCode);

		public Task<string> Test(string data);
		public Task<int> RequestAppointment(ushort chainType, string accountCode, int preferredRegion);
		public Task<int> PublishAccount(ushort chainType, string accountCode);
		public Task StartMining(ushort chainType, string delegateAccountId, int tier = 0);
		public Task StopMining(ushort chainType);
		public Task<bool> IsMiningEnabled(ushort chainType);
		public Task<bool> IsMiningAllowed(ushort chainType);
		public Task<bool> QueryBlockchainSynced(ushort chainType);
		public Task<bool> QueryWalletSynced(ushort chainType);
		public Task<object> QueryAccountTotalNeuraliums(string accountCode);
		public Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel);
		public Task<object> QueryMiningStatistics(ushort chainType);
		public Task<bool> ClearCachedCredentials(ushort chainType);

		public Task<long> QueryCurrentDifficulty(ushort chainType);

		public Task<bool> CreateNextXmssKey(ushort chainType, string accountCode, byte ordinal);

		public Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal fees, string note);
		public Task<object> QueryNeuraliumTimelineHeader(string accountCode);
		public Task<object> QueryNeuraliumTimelineSection(string accountCode, DateTime day);

		public Task<byte[]> SignXmssMessage(ushort chainType, string accountCode, byte[] message);

		public Task SetPuzzleAnswers(ushort chainType, List<int> answers);

#if TESTNET || DEVNET
		public Task<int> RefillNeuraliums(string accountCode);
#endif
#if COLORADO_EXCLUSION
		public Task<bool> BypassAppointmentVerification(string accountCode);
#endif

		public Task<object> QueryElectionContext(ushort chainType, long blockId);
		public Task<List<object>> QueryNeuraliumTransactionPool();

		public Task<bool> RestoreWalletNarballBackup(string source, string dest);
	}
}