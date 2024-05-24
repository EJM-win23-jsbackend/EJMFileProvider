using Azure.Storage.Blobs;
using Data.Contexts;
using FileProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("SqlServer")));
        services.AddScoped<BlobServiceClient>(x => new BlobServiceClient(Environment.GetEnvironmentVariable("AzureStorageAccount")));
        services.AddScoped<FileService>();
    })
    .Build();

host.Run();
