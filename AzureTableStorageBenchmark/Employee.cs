using Azure;
using Azure.Data.Tables;

namespace AzureTableStorageBenchmark;

public class Employee : ITableEntity
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Company { get; set; }

    public string CatchPhrase { get; set; }

    public string Avatar { get; set; }

    public string Username { get; set; }

    public string Website { get; set; }

    public string JobTitle { get; set; }

    public int Salary { get; set; }

    public DateTime JoiningDate { get; set; }

    public DateTime NextPromotionDate { get; set; }

    public DateTime LastPromotionDate { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Ssn { get; set; }

    public string Feedback { get; set; }

    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
}
