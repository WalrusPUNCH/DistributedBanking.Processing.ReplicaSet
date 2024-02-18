using Confluent.Kafka;

namespace DistributedBanking.Processing.ReplicaSet.Models;

public record ListenerResponse<TResponse>(
    TopicPartitionOffset MessageOffset,
    TResponse Response,
    string ResponseChannelPattern)
{
    public string ResponseChannel => $"{ResponseChannelPattern}:{MessageOffset.Partition}:{MessageOffset.Offset}";
}