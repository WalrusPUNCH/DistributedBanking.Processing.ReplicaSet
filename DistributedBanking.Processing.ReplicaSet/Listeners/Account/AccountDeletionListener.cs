using Contracts.Models;
using DistributedBanking.Processing.ReplicaSet.Domain.Services;
using DistributedBanking.Processing.ReplicaSet.Models;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;
using Shared.Messaging.Messages.Account;
using Shared.Redis.Services;

namespace DistributedBanking.Processing.ReplicaSet.Listeners.Account;

public class AccountDeletionListener : BaseListener<string, AccountDeletionMessage, OperationResult>
{
    private readonly IAccountService _accountService;

    public AccountDeletionListener(
        IKafkaConsumerService<string, AccountDeletionMessage> accountDeletionConsumer,
        IRedisSubscriber redisSubscriber,
        IRedisProvider redisProvider,
        IAccountService accountService,
        ILogger<AccountDeletionListener> logger) : base(accountDeletionConsumer, redisSubscriber, redisProvider, logger)
    {
        _accountService = accountService;
    }

    protected override bool FilterMessage(MessageWrapper<AccountDeletionMessage> messageWrapper)
    {
        return base.FilterMessage(messageWrapper) && !string.IsNullOrWhiteSpace(messageWrapper.Message.AccountId);
    }

    protected override async Task<ListenerResponse<OperationResult>> ProcessMessage(
        MessageWrapper<AccountDeletionMessage> messageWrapper)
    {
        var deletionResult = await _accountService.DeleteAsync(messageWrapper.Message.AccountId);
        return new ListenerResponse<OperationResult>(
            MessageOffset: messageWrapper.Offset, 
            Response: deletionResult,
            ResponseChannelPattern: messageWrapper.Message.ResponseChannelPattern);
    }

    protected override void OnMessageProcessingException(
        Exception exception, 
        TimeSpan delay, 
        MessageWrapper<AccountDeletionMessage> messageWrapper)
    {
        Logger.LogError(exception, "Error while trying to delete '{AccountId}' account. Retry in {Delay}",
            messageWrapper.Message.AccountId, delay);
    }
}