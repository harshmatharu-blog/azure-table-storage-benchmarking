using Azure.Data.Tables;
using AzureTableStorageBenchmark;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Metrics;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

var connectionString = "";
var builder = WebApplication.CreateBuilder(args);

// Add Services
var tableClient = new TableClient(connectionString, "BenchmarkTable");
builder.Services.AddSingleton(tableClient);
builder.Services.AddSingleton<ProductGenerator>();
builder.Services.AddSingleton<EmployeeGenerator>();
builder.Services.AddApplicationInsightsTelemetry();

// Set default namespace for all custom metrics
MetricIdentifier.DefaultMetricNamespace = Assembly.GetExecutingAssembly().GetName().Name;

var employeeIds = new ConcurrentBag<Guid>();
var productIds = new ConcurrentBag<Guid>();

var app = builder.Build();

// Endpoints
app.UseExceptionHandler(builder =>
{
    builder.Run((ctx) => Results.StatusCode(500).ExecuteAsync(ctx));
});

app.MapPost("/AddProduct", async (TableClient client, ProductGenerator pg, TelemetryClient tc) =>
{
    var product = pg.Generate()[0];
    var payloadSize = Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(product));
    tc.GetMetric($"AddProduct.PayloadSize").TrackValue(payloadSize);

    var s = Stopwatch.StartNew();
    await client.AddEntityAsync(product);
    s.Stop();

    tc.GetMetric($"AddProduct.Duration").TrackValue(s.Elapsed.TotalMilliseconds);

    // Store Product IDs and then we can call /GetProduct by picking an ID from the Bag.
    productIds.Add(product.Id);

    return Results.Ok();
});

// Point Query
app.MapGet("/GetProduct", async (TableClient client, TelemetryClient tc) =>
{
    productIds.TryTake(out var id);
    
    var s = Stopwatch.StartNew();
    var response = await client.GetEntityAsync<Product>(id.ToString(), id.ToString());
    s.Stop();

    tc.GetMetric($"GetProduct.Duration").TrackValue(s.Elapsed.TotalMilliseconds);

    return Results.Ok(response.Value);
});


// Employee Entity size is 1 KB whereas Product Entity size is 0.5 KB.
app.MapPost("/AddEmployee", async (TableClient client, EmployeeGenerator eg, TelemetryClient tc) =>
{
    var employee = eg.Generate()[0];
    var payloadSize = Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(employee));
    tc.GetMetric($"AddEmployee.PayloadSize").TrackValue(payloadSize);

    var s = Stopwatch.StartNew();
    await client.AddEntityAsync(employee);
    s.Stop();

    tc.GetMetric($"AddEmployee.Duration").TrackValue(s.Elapsed.TotalMilliseconds);

    // Store Employee IDs and then we can call /GetEmployee by picking an ID from the Bag.
    employeeIds.Add(employee.Id);

    return Results.Ok();
});

// Point Query
app.MapGet("/GetEmployee", async (TableClient client, TelemetryClient tc) =>
{
    employeeIds.TryTake(out var id);

    var s = Stopwatch.StartNew();
    var response = await client.GetEntityAsync<Employee>(id.ToString(), id.ToString());
    s.Stop();

    tc.GetMetric($"GetEmployee.Duration").TrackValue(s.Elapsed.TotalMilliseconds);

    return Results.Ok(response.Value);
});

// Table Scan
app.MapGet("/GetEmployeeById", async (TableClient client, TelemetryClient tc) =>
{
    employeeIds.TryTake(out var id);
    Employee employee = null!;

    var s = Stopwatch.StartNew();
    var page = client.QueryAsync<Employee>(e => e.Id == id);
    await foreach (var e in page)
    {
        employee = e;
    }
    s.Stop();

    tc.GetMetric($"GetEmployee.Duration").TrackValue(s.Elapsed.TotalMilliseconds);

    return Results.Ok(employee);
});

app.MapPost("/ClearBags", () =>
{
    employeeIds.Clear();
    productIds.Clear();

    return Results.Ok();
});

app.Run();