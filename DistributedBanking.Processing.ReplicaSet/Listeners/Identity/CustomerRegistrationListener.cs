using Contracts.Models;
using DistributedBanking.Processing.ReplicaSet.Domain.Models.Identity;
using DistributedBanking.Processing.ReplicaSet.Domain.Services;
using DistributedBanking.Processing.ReplicaSet.Models;
using Mapster;
using Shared.Data.Entities.Constants;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;
using Shared.Messaging.Messages.Identity.Registration;
using Shared.Redis.Services;

namespace DistributedBanking.Processing.ReplicaSet.Listeners.Identity;

public class CustomerRegistrationListener : BaseListener<string, UserRegistrationMessage, OperationResult>
{
    private readonly IIdentityService _identityService;

    public CustomerRegistrationListener(
        IKafkaConsumerService<string, UserRegistrationMessage> userRegistrationConsumer,
        IIdentityService identityService,
        IRedisSubscriber redisSubscriber,
        ILogger<CustomerRegistrationListener> logger) : base(userRegistrationConsumer, redisSubscriber, logger)
    {
        _identityService = identityService;
    }

    protected override bool FilterMessage(MessageWrapper<UserRegistrationMessage> messageWrapper)
    {
        return base.FilterMessage(messageWrapper) && !string.IsNullOrWhiteSpace(messageWrapper.Message.Email);
    }

    protected override async Task<ListenerResponse<OperationResult>> ProcessMessage(
        MessageWrapper<UserRegistrationMessage> messageWrapper)
    {
        var registrationModel = messageWrapper.Message.Adapt<EndUserRegistrationModel>();
        var registrationResult = await _identityService.RegisterUser(registrationModel, RoleNames.Customer);

        return new ListenerResponse<OperationResult>(
            MessageOffset: messageWrapper.Offset, 
            Response: registrationResult,
            ResponseChannelPattern: messageWrapper.Message.ResponseChannelPattern);
    }

    protected override void OnMessageProcessingException(
        Exception exception, 
        TimeSpan delay, 
        MessageWrapper<UserRegistrationMessage> messageWrapper)
    {
        Logger.LogError(exception, "Error while trying to register customer with an email '{Email}'. Retry in {Delay}",
            messageWrapper.Message.Email, delay);
    }
}