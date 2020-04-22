using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Neuralium.Api.Common {

	public interface INeuraliumApiMethods {

		Task<bool> ToggleServerMessages(bool enable);
		Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase, bool setKeysToo = false);
		Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase);
		Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode);

		Task<object> QuerySystemInfo();
		Task<List<object>> QuerySupportedChains();
		Task<string> Ping();

		Task<bool> CompleteLongRunningEvent(int correlationId, object data);
		Task<bool> RenewLongRunningEvent(int correlationId);

		Task<bool> IsBlockchainSynced(ushort chainType);
		Task<bool> IsWalletSynced(ushort chainType);

		Task<bool> SyncBlockchain(ushort chainType, bool force);
		Task<bool> Shutdown();
		Task<object> BackupWallet(ushort chainType);
		Task<bool> RestoreWalletFromBackup(ushort chainType, string backupsPath, string passphrase, string salt, int iterations);

		Task<int> QueryTotalConnectedPeersCount();
		Task<bool> QueryMiningPortConnectable();
		Task<object> QueryChainStatus(ushort chainType);
		Task<object> QueryWalletInfo(ushort chainType);
		
		Task<object> QueryBlockChainInfo(ushort chainType);
		
		
		Task<bool> IsWalletLoaded(ushort chainType);
		Task<bool> WalletExists(ushort chainType);
		Task<int> LoadWallet(ushort chainType, string passphrase = null);
		Task<long> QueryBlockHeight(ushort chainType);
		Task<long> QueryLowestAccountBlockSyncHeight(ushort chainType);
		
		Task<string> QueryBlock(ushort chainType, long blockId);
		Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId);
		Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId);
		Task<int> CreateAccount(ushort chainType, string accountName, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases);
		Task<bool> SetActiveAccount(ushort chainType, Guid accountUuid);

		Task<int> CreateNewWallet(ushort chainType, string accountName, bool encryptWallet, bool encryptKey, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases, bool publishAccount);

		Task<bool> SetWalletPassphrase(int correlationId, string passphrase, bool setKeysToo = false);
		Task<bool> SetKeysPassphrase(int correlationId, string passphrase);

		Task<List<object>> QueryWalletTransactionHistory(ushort chainType, Guid accountUuid);
		Task<object> QueryWalletTransationHistoryDetails(ushort chainType, Guid accountUuid, string transactionId);
		Task<List<object>> QueryWalletAccounts(ushort chainType);
		Task<string> QueryDefaultWalletAccountId(ushort chainType);
		Task<Guid> QueryDefaultWalletAccountUuid(ushort chainType);
		Task<object> QueryWalletAccountDetails(ushort chainType, Guid accountUuid);
		Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, Guid accountUuid);

		Task<int> PublishAccount(ushort chainType, Guid? accountUuid);
		Task StartMining(ushort chainType, string delegateAccountId);
		Task StopMining(ushort chainType);
		Task<bool> IsMiningEnabled(ushort chainType);
		Task<bool> IsMiningAllowed(ushort chainType);
		Task<bool> QueryBlockchainSynced(ushort chainType);
		Task<bool> QueryWalletSynced(ushort chainType);
		Task<object> QueryAccountTotalNeuraliums(Guid accountUuid);
		Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel);

		Task<bool> CreateNextXmssKey(ushort chainType, Guid accountUuid, byte ordinal);

		Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal fees, string note);
		Task<object> QueryNeuraliumTimelineHeader(Guid accountUuid);
		Task<List<object>> QueryNeuraliumTimelineSection(Guid accountUuid, DateTime firstday, int skip, int take);
		
		Task<byte[]> SignXmssMessage(ushort chainType, Guid accountUuid, byte[] message);

#if TESTNET || DEVNET
		Task<int> RefillNeuraliums(Guid accountUuid);
#endif

		Task<object> QueryElectionContext(ushort chainType, long blockId);
		Task<List<object>> QueryNeuraliumTransactionPool();
		
		Task<bool> RestoreWalletNarballBackup(string source, string dest);
	}
}