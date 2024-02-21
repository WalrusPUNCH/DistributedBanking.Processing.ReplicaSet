﻿using Contracts.Models;
using DistributedBanking.Processing.ReplicaSet.Data.Repositories;
using DistributedBanking.Processing.ReplicaSet.Domain.Models.Account;
using Mapster;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Shared.Data.Entities;

namespace DistributedBanking.Processing.ReplicaSet.Domain.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountsRepository _accountsRepository;
    private readonly ICustomersRepository _customersRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountsRepository accountsRepository, 
        ICustomersRepository customersRepository,
        ILogger<AccountService> logger)
    {
        _accountsRepository = accountsRepository;
        _customersRepository = customersRepository;
        _logger = logger;
    }
    
    public async Task<OperationResult<AccountOwnedResponseModel>> CreateAsync(string customerId, AccountCreationModel accountCreationModel)
    {
        using var session = await _accountsRepository.StartSession();
        return await session.WithTransactionAsync(async (_, _) =>
        {
            var customerEntity = await _customersRepository.GetAsync(new ObjectId(customerId));
            if (customerEntity == null)
            {
                _logger.LogError("Customer with the ID '{CustomerId}' does not exist", customerId);
                return OperationResult<AccountOwnedResponseModel>.BadRequest(default,
                    "Error occured while trying to create account. Try again later");
            }

            var account = GenerateNewAccount(customerId, accountCreationModel);
            var accountEntity = account.Adapt<AccountEntity>();
            await _accountsRepository.AddAsync(accountEntity);

            customerEntity.Accounts.Add(accountEntity.Id.ToString());
            await _customersRepository.UpdateAsync(customerEntity);

            var accountModel = accountEntity.Adapt<AccountOwnedResponseModel>();

            return OperationResult<AccountOwnedResponseModel>.Success(accountModel);
        });
    }

    private static AccountModel GenerateNewAccount(string customerId, AccountCreationModel accountModel)
    {
        return new AccountModel
        {
            Name = accountModel.Name,
            Type = accountModel.Type,
            Balance = 0,
            ExpirationDate = Generator.GenerateExpirationDate(),
            SecurityCode = Generator.GenerateSecurityCode(),
            Owner = customerId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<AccountOwnedResponseModel> GetAsync(string id)
    {
        var account = await _accountsRepository.GetAsync(new ObjectId(id));

        return account.Adapt<AccountOwnedResponseModel>();
    }

    public async Task<IEnumerable<AccountResponseModel>> GetCustomerAccountsAsync(string customerId)
    {
        var accounts = await _accountsRepository.GetAsync(x => x.Owner == customerId);
        
        return accounts.Adapt<AccountResponseModel[]>();
    }

    public async Task<bool> BelongsTo(string accountId, string customerId)
    {
        var account = await _accountsRepository.GetAsync(
            a => a.Id == new ObjectId(accountId) && a.Owner != null && a.Owner == customerId);

        return account.Any();
    }

    public async Task<IEnumerable<AccountOwnedResponseModel>> GetAsync()
    {
        var accounts = await _accountsRepository.GetAsync();
        
        return accounts.Adapt<AccountOwnedResponseModel[]>();
    }

    public async Task UpdateAsync(AccountEntity model)
    {
        using var session = await _accountsRepository.StartSession();
        await session.WithTransactionAsync(async (_, _) =>
        {
            await _accountsRepository.UpdateAsync(model);
            return model;
        });
    }

    public async Task<OperationResult> DeleteAsync(string id)
    {
        using var session = await _accountsRepository.StartSession();
        return await session.WithTransactionAsync(async (_, _) =>
        {
            try
            {
                var accountEntity = await _accountsRepository.GetAsync(new ObjectId(id));
                if (string.IsNullOrWhiteSpace(accountEntity?.Owner))
                {
                    _logger.LogWarning(
                        "Unable to delete account '{AccountId}' because such account does not exist or already deleted",
                        id);
                    return OperationResult.BadRequest("Account with the specified ID doesn't exist");
                }

                var customerEntity = await _customersRepository.GetAsync(new ObjectId(accountEntity.Owner));
                if (customerEntity == null)
                {
                    _logger.LogError(
                        "Customer with the ID '{CustomerId}' connected to the account '{AccountId}' does not exist",
                        accountEntity.Owner, id);
                    return OperationResult.InternalFail(
                        "Error occured while trying to delete account. Try again later");
                }

                customerEntity.Accounts.Remove(accountEntity.Id.ToString());
                await _customersRepository.UpdateAsync(customerEntity);
                accountEntity.Owner = null;
                await UpdateAsync(accountEntity);

                return OperationResult.Success();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred while trying to delete account");
                return OperationResult.InternalFail("Error occurred while trying to delete account. Try again later");
            }
        });
    }
}