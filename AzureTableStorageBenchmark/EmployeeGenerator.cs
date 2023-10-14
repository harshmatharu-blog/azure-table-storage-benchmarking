using Bogus;
using Bogus.Extensions.UnitedStates;

namespace AzureTableStorageBenchmark;

public class EmployeeGenerator
{
    public List<Employee> Generate(int count = 1)
    {
        var faker = new Faker<Employee>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.FirstName, f => f.Person.FirstName)
            .RuleFor(p => p.LastName, f => f.Person.LastName)
            .RuleFor(p => p.Address, f => f.Person.Address.Street)
            .RuleFor(p => p.City, f => f.Person.Address.City)
            .RuleFor(p => p.State, f => f.Person.Address.State)
            .RuleFor(p => p.Company, f => f.Person.Company.Name)
            .RuleFor(p => p.CatchPhrase, f => f.Person.Company.CatchPhrase)
            .RuleFor(p => p.Avatar, f => f.Person.Avatar)
            .RuleFor(p => p.Username, f => f.Person.UserName)
            .RuleFor(p => p.Website, f => f.Person.Website)
            .RuleFor(p => p.JobTitle, f => f.Lorem.Word())
            .RuleFor(p => p.Salary, f => f.Random.Int(1000))
            .RuleFor(p => p.JoiningDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(p => p.NextPromotionDate, f => f.Date.Future().ToUniversalTime())
            .RuleFor(p => p.LastPromotionDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(p => p.DateOfBirth, f => f.Person.DateOfBirth.ToUniversalTime())
            .RuleFor(p => p.Gender, f => f.Person.Gender.ToString())
            .RuleFor(p => p.Phone, f => f.Person.Phone)
            .RuleFor(p => p.Email, f => f.Person.Email)
            .RuleFor(p => p.Ssn, f => f.Person.Ssn())
            .RuleFor(p => p.Feedback, f => f.Lorem.Lines(4));

        var employees = new List<Employee>();
        for (var i = 0; i < count; i++)
        {
            var employee = faker.Generate();
            employee.PartitionKey = employee.Id.ToString();
            employee.RowKey = employee.PartitionKey;
            employees.Add(employee);
        }

        return employees;
    }
}
