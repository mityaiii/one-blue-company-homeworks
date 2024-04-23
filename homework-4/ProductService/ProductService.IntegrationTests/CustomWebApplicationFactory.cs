using System.Threading.Channels;
using AutoBogus;
using AutoMapper;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using ProductService.Api.AutoMapper;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Entities.Repositories;
using ProductService.Domain.Models;

namespace ProductService.IntegrationTests;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    public readonly Mock<IProductRepository> ProductRepositoryMock = new();
    public readonly IMapper MyMapper;

    public CustomWebApplicationFactory()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DateTimeMapper>();
            cfg.AddProfile<ProductReplyToProductMapper>();
            cfg.AddProfile<ProductTypeMapper>();
        });
        
        MyMapper = configuration.CreateMapper();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Replace(new ServiceDescriptor(typeof(IProductRepository), ProductRepositoryMock.Object));
        });
    }
}