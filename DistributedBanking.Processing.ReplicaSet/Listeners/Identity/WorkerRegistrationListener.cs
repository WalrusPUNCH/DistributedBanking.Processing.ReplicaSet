using Contracts.Models;
using DistributedBanking.Processing.ReplicaSet.Domain.Models.Identity;
using DistributedBanking.Processing.ReplicaSet.Domain.Services;
using DistributedBanking.Processing.ReplicaSet.Models;
using Mapster;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;
using Shared.Messaging.Messages.Identity.Registration;
using Shared.Redis.Services;

namespace DistributedBanking.Processing.ReplicaSet.Listeners.Identity;

public class WorkerRegistrationListener : BaseListener<string, WorkerRegistrationMessage, OperationResult>
{
    private readonly IIdentityService _identityService;

    public WorkerRegistrationListener(
        IKafkaConsumerService<string, WorkerRegistrationMessage> workerRegistrationConsumer,
        IIdentityService identityService,
        IRedisSubscriber redisSubscriber,
        IRedisProvider redisProvider,
        ILogger<WorkerRegistrationListener> logger) : base(workerRegistrationConsumer, redisSubscriber, redisProvider, logger)
    {
        _identityService = identityService;
    }

    protected override bool FilterMessage(MessageWrapper<WorkerRegistrationMessage> messageWrapper)
    {
        return base.FilterMessage(messageWrapper) && !string.IsNullOrWhiteSpace(messageWrapper.Message.Email);
    }

    protected override async Task<ListenerResponse<OperationResult>> ProcessMessage(
        MessageWrapper<WorkerRegistrationMessage> messageWrapper)
    {
        var registrationModel = messageWrapper.Message.Adapt<WorkerRegistrationModel>();
        var registrationResult = await _identityService.RegisterUser(registrationModel, messageWrapper.Message.Role);

        return new ListenerResponse<OperationResult>(
            MessageOffset: messageWrapper.Offset,
            Response: registrationResult,
            ResponseChannelPattern: messageWrapper.Message.ResponseChannelPattern);
    }

    protected override void OnMessageProcessingException(
        Exception exception, 
        TimeSpan delay,
        MessageWrapper<WorkerRegistrationMessage> messageWrapper)
    {
        Logger.LogError(exception, "Error while trying to register worker with an email '{Email}'. Retry in {Delay}",
            messageWrapper.Message.Email, delay);
    }
}