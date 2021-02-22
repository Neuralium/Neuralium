using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Neuralia.Blockchains.Core;

namespace Neuralium.Api.Common {

	public interface INeuraliumApiMethods {

		Task<bool> ToggleServerMessages(bool enable);
		Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase, bool setKeysToo = false);
		Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase);
		Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode);

		Task<int> TestP2pPort(int testPort, bool callback);
		
		Task<object> QuerySystemInfo();
		Task<List<object>> QuerySupportedChains();
		Task<string> Ping();
		Task<object> GetPortMappingStatus();
		Task<bool> ConfigurePortMappingMode(bool useUPnP, bool usePmP, int natDeviceIndex);

		Task<byte> GetPublicIPMode();

		Task SetUILocale(string locale);
		Task<byte> GetMiningRegistrationIpMode(ushort chainType);

		Task<bool> CompleteLongRunningEvent(int correlationId, object data);
		Task<bool> RenewLongRunningEvent(int correlationId);

		Task<bool> IsBlockchainSynced(ushort chainType);
		Task<bool> IsWalletSynced(ushort chainType);

		Task<int> GetCurrentOperatingMode(ushort chainType);
		Task<bool> SyncBlockchain(ushort chainType, bool force);
		Task<bool> Shutdown();
		Task<object> BackupWallet(ushort chainType);
		Task<bool> RestoreWalletFromBackup(ushort chainType, string backupsPath, string passphrase, string salt, string nonce, int iterations);
		Task<bool> AttemptWalletRescue(ushort chainType);
		
		Task<int> QueryTotalConnectedPeersCount();
		
		Task<List<object>> QueryPeerConnectionDetails();
		
		Task<bool> QueryMiningPortConnectable();
		Task<object> QueryChainStatus(ushort chainType);
		Task<object> QueryWalletInfo(ushort chainType);
		Task<byte[]> GenerateSecureHash(byte[] parameter, ushort chainType);

		Task<object> QueryBlockChainInfo(ushort chainType);

		Task<bool> IsWalletLoaded(ushort chainType);
		Task<bool> WalletExists(ushort chainType);
		Task<int> LoadWallet(ushort chainType, string passphrase = null);
		Task<long> QueryBlockHeight(ushort chainType);
		Task<int> QueryDigestHeight(ushort chainType);
		Task<bool> ResetWalletIndex(ushort chainType);
		Task<long> QueryLowestAccountBlockSyncHeight(ushort chainType);
		Task<string> QueryBlock(ushort chainType, long blockId);
		Task<object> QueryDecomposedBlock(ushort chainType, long blockId);
		Task<string> QueryDecomposedBlockJson(ushort chainType, long blockId);
		Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId);
		Task<byte[]> QueryBlockBytes(ushort chainType, long blockId);
		Task<string> GetBlockSizeAndHash(ushort chainType, long blockId);
		Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId);
		Task<int> CreateStandardAccount(ushort chainType, string accountName, int accountType, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases);
		Task<bool> SetActiveAccount(ushort chainType, string accountCode);
		Task<object> QueryAppointmentConfirmationResult(ushort chainType, string accountCode);
		Task<bool> ClearAppointment(ushort chainType, string accountCode);
		Task<object> CanPublishAccount(ushort chainType, string accountCode);
		Task SetSMSConfirmationCode(ushort chainType, string accountCode, long confirmationCode);
		Task SetSMSConfirmationCodeString(ushort chainType, string accountCode, string confirmationCode);

		Task GenerateXmssKeyIndexNodeCache(ushort chainType, string accountCode, byte ordinal, long index);
		Task<int> CreateNewWallet(ushort chainType, string accountName, int accountType, bool encryptWallet, bool encryptKey, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases, bool publishAccount);

		Task<bool> SetWalletPassphrase(int correlationId, string passphrase, bool setKeysToo = false);
		Task<bool> SetKeysPassphrase(int correlationId, string passphrase);

		Task<List<object>> QueryWalletTransactionHistory(ushort chainType, string accountCode);
		Task<object> QueryWalletTransactionHistoryDetails(ushort chainType, string accountCode, string transactionId);
		Task<List<object>> QueryWalletAccounts(ushort chainType);
		Task<string> QueryDefaultWalletAccountId(ushort chainType);
		Task<string> QueryDefaultWalletAccountCode(ushort chainType);
		Task<object> QueryWalletAccountDetails(ushort chainType, string accountCode);
		Task<object> QueryWalletAccountAppointmentDetails(ushort chainType, string accountCode);
		Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, string accountCode);

		Task<string> Test(string data);
		Task<int> RequestAppointment(ushort chainType, string accountCode, int preferredRegion);
		Task<int> PublishAccount(ushort chainType, string accountCode);
		Task StartMining(ushort chainType, string delegateAccountId, int tier = 0);
		Task StopMining(ushort chainType);
		Task<bool> IsMiningEnabled(ushort chainType);
		Task<bool> IsMiningAllowed(ushort chainType);
		Task<bool> QueryBlockchainSynced(ushort chainType);
		Task<bool> QueryWalletSynced(ushort chainType);
		Task<string> GenerateTestPuzzle();
		Task<object> QueryAccountTotalNeuraliums(string accountCode);
		Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel);
		Task<object> QueryMiningStatistics(ushort chainType);
		Task<bool> ClearCachedCredentials(ushort chainType);

		Task<long> QueryCurrentDifficulty(ushort chainType);

		Task<bool> CreateNextXmssKey(ushort chainType, string accountCode, byte ordinal);

		Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal fees, string note);
		Task<object> QueryNeuraliumTimelineHeader(string accountCode);
		Task<object> QueryNeuraliumTimelineSection(string accountCode, DateTime day);

		Task<byte[]> SignXmssMessage(ushort chainType, string accountCode, byte[] message);

		Task SetPuzzleAnswers(ushort chainType, List<int> answers);

#if TESTNET || DEVNET
		Task<int> RefillNeuraliums(string accountCode);
#endif
#if COLORADO_EXCLUSION
		Task<bool> BypassAppointmentVerification(string accountCode);
#endif

		Task<object> QueryElectionContext(ushort chainType, long blockId);
		Task<List<object>> QueryNeuraliumTransactionPool();
		Task<bool> RestoreWalletNarballBackup(string source, string dest);
		Task<object> ReadAppSetting(string name);
		Task<bool> WriteAppSetting(string name, string value);
		Task<object> ReadAppSettingDomain(string name);
	}
}