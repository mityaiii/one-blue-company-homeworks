using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Presentation.Entities;
using Presentation.Extensions;
using ProductService.Core.Entities;
using ProductService.CoreInjection;
using ProductService.DataAccess.Entities;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = new HostBuilder()
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder
                    .AddJsonFile(Environment.GetEnvironmentVariable("CONFIG_PATH"), optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, collection) =>
            {
                ICsvParser csvParser = new CsvProductParser(Environment.GetEnvironmentVariable("PRODUCT_PATH"));
                ICsvFileWriter csvFileWriter = new CsvFileWriter(Environment.GetEnvironmentVariable("OUTPUT_PATH"));
                
                collection
                    .AddCoreExtensions(context, csvParser, csvFileWriter)
                    .AddPresentationExtensions();   
            });
        
        var host = builder.Build();

        await host.StartAsync();
        await host.StopAsync();
    }
}