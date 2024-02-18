namespace DistributedBanking.Processing.ReplicaSet.Domain.Options;

public record DatabaseOptions(
    string ConnectionString,
    string DatabaseName);