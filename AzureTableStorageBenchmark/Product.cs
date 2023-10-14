using Azure;
using Azure.Data.Tables;

namespace AzureTableStorageBenchmark
{
    public class Product : ITableEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public float DiscountPercentage { get; set; }

        public float Rating { get; set; }

        public int Stock { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}
