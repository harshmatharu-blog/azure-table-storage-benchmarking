using Bogus;

namespace AzureTableStorageBenchmark;

public class ProductGenerator
{
    public List<Product> Generate(int count = 1)
    {
        var faker = new Faker<Product>()
            .RuleFor(p => p.Brand, f => f.Company.CompanyName())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.Description, f => f.Lorem.Lines(4))
            .RuleFor(p => p.DiscountPercentage, f => f.Random.Number(90))
            .RuleFor(p => p.Price, f => f.Random.Number(1000, 100000))
            .RuleFor(p => p.Rating, f => f.Random.Number(0, 5))
            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
            .RuleFor(p => p.Stock, f => f.Random.Number(100))
            .RuleFor(p => p.Id, f => Guid.NewGuid());

        var products = new List<Product>();
        for (var i = 0; i < count; i++)
        {
            var product = faker.Generate();
            product.PartitionKey = product.Id.ToString();
            product.RowKey = product.PartitionKey;
            products.Add(product);
        }

        return products;
    }
}
