using Lab2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;

namespace Lab2.Tests.Factories;

public class ApiFactory : WebApplicationFactory<Program>
{
    public MongoDbRunner DbRunner { get; }

    public ApiFactory()
    {
        DbRunner = MongoDbRunner.Start();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<InsuranceDatabaseSettings>(options =>
            {
                options.ConnectionString = DbRunner.ConnectionString;
                options.DatabaseName = "TestDatabase";
                options.CollectionName = "InsuranceContractsTest";
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        DbRunner.Dispose();
    }
}