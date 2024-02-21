using Contracts.Models;
using DistributedBanking.Processing.ReplicaSet.Domain.Services;
using DistributedBanking.Processing.ReplicaSet.Models;
using Shared.Data.Entities.Identity;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;
using Shared.Messaging.Messages.Identity;
using Shared.Redis.Services;

namespace DistributedBanking.Processing.ReplicaSet.Listeners.Identity;

public class RoleCreationListener : BaseListener<string, RoleCreationMessage, OperationResult>
{
    private readonly IRolesManager _rolesManager;

    public RoleCreationListener(
        IKafkaConsumerService<string, RoleCreationMessage> roleCreationConsumer,
        IRolesManager rolesManager,
        IRedisSubscriber redisSubscriber,
        ILogger<RoleCreationListener> logger) : base(roleCreationConsumer, redisSubscriber, logger)
    {
        _rolesManager = rolesManager;
    }

    protected override bool FilterMessage(MessageWrapper<RoleCreationMessage> messageWrapper)
    {
        return base.FilterMessage(messageWrapper) && !string.IsNullOrWhiteSpace(messageWrapper.Message.Name);
    }

    protected override async Task<ListenerResponse<OperationResult>> ProcessMessage(
        MessageWrapper<RoleCreationMessage> messageWrapper)
    {
        var roleCreationResult = await _rolesManager.CreateAsync(new ApplicationRole(messageWrapper.Message.Name));

        return new ListenerResponse<OperationResult>(
            MessageOffset: messageWrapper.Offset, 
            Response: roleCreationResult,
            ResponseChannelPattern: messageWrapper.Message.ResponseChannelPattern);
    }

    protected override void OnMessageProcessingException(
        Exception exception, 
        TimeSpan delay, 
        MessageWrapper<RoleCreationMessage> messageWrapper)
    {
        Logger.LogError(exception, "Error while trying to create a role with the name '{RoleName}'. Retry in {Delay}",
            messageWrapper.Message.Name, delay);
    }
}