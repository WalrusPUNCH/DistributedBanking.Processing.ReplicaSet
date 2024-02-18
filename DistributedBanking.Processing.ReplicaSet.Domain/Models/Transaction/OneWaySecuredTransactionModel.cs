namespace DistributedBanking.Processing.ReplicaSet.Domain.Models.Transaction;

public class OneWaySecuredTransactionModel : OneWayTransactionModel
{
    public required string SecurityCode { get; set; }
}