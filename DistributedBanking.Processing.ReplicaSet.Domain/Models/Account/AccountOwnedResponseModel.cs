namespace DistributedBanking.Processing.ReplicaSet.Domain.Models.Account;

public class AccountOwnedResponseModel : AccountResponseModel
{
    public required string Owner { get; set; }
}