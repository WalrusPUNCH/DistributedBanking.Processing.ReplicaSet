﻿namespace DistributedBanking.Processing.ReplicaSet.Domain.Models.Identity;

public class EndUserRegistrationModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Salt { get; set; }
    public required CustomerPassportModel Passport { get; set; }
}