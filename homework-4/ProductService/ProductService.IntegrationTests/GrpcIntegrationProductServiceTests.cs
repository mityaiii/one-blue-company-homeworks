using AutoBogus;
using Bogus;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Moq;
using ProductService.Api;
using ProductService.DataAccess.Exceptions;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Models;

namespace ProductService.IntegrationTests;

public class GrpcIntegrationProductServiceTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
    
    public GrpcIntegrationProductServiceTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }
    
    [Fact]
    public async Task Get_CorrectProductId_ShouldReturnProduct()
    {
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        long productId = 1;
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .Generate();
    
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ReturnsAsync(expectedProduct);

        GetProductRequest getProductRequest = new AutoFaker<GetProductRequest>()
            .RuleFor(f => f.Id, productId);
        var actualProductReply = await grpcClient.GetAsync(getProductRequest);
        
        actualProductReply.Should().NotBeNull()
            .And.BeEquivalentTo(_webApplicationFactory.MyMapper.Map<ProductReply>(expectedProduct));
    }
    
    [Fact]
    public async Task Get_NegativeProductId_ShouldReturnInternalServerStatus()
    {
        long productId = -1;
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ReturnsAsync(expectedProduct);
    
        GetProductRequest getProductRequest = new AutoFaker<GetProductRequest>()
            .RuleFor(f => f.Id, productId);
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.GetAsync(getProductRequest));
    }

    [Fact]
    public async Task Get_NotExistProductId_ShouldReturnNotFoundStatus()
    {
        long productId = 1;
    
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ThrowsAsync(new ValueNotFoundException("value didn't find"));
        
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ThrowsAsync(new ValueNotFoundException("product not found"));
    
        GetProductRequest getProductRequest = new AutoFaker<GetProductRequest>()
            .RuleFor(f => f.Id, productId);
        
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.GetAsync(getProductRequest));
    }
    
    [Fact]
    public async Task Post_ProductWithCorrectParameters_ShouldReturnProductId()
    {
        var product = new Faker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.AddAsync(It.IsAny<Product>())) 
            .ReturnsAsync(product.Id);

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        AddProductRequest addProductRequest = new AutoFaker<AddProductRequest>()
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.ProductType, ProductReplyType.General)
            .RuleFor(f => f.DateOfCreation, Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()))
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100));
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        var actualProductResponse = await grpcClient.AddAsync(addProductRequest);
        
        Assert.Equal(product.Id, actualProductResponse.Id);
    }
    
    [Fact]
    public async Task Post_ProductWithNegativeWeight_ShouldReturnInternalServerStatus()
    {
        var product = new Faker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.AddAsync(It.IsAny<Product>())) 
            .ReturnsAsync(product.Id);

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        AddProductRequest addProductRequest = new AutoFaker<AddProductRequest>()
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.Weight, p => p.Random.Double(-1, 0))
            .RuleFor(f => f.ProductType, ProductReplyType.General)
            .RuleFor(f => f.DateOfCreation, Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()))
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100));
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.AddAsync(addProductRequest));
    }
    
    [Fact]
    public async Task Post_ProductWithEmptyName_ShouldReturnInternalServerStatus()
    {
        var product = new Faker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.AddAsync(It.IsAny<Product>())) 
            .ReturnsAsync(product.Id);

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        AddProductRequest addProductRequest = new AutoFaker<AddProductRequest>()
            .RuleFor(f => f.Name, string.Empty)
            .RuleFor(f => f.Price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.ProductType, ProductReplyType.General)
            .RuleFor(f => f.DateOfCreation, Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()))
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100));
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.AddAsync(addProductRequest));
    }
    
    [Fact]
    public async Task Remove_ProductWithCorrectId_ShouldReturnStatusCodeSucceed()
    {
        var productId = 1;
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.RemoveAsync(It.IsAny<long>())) 
            .Returns(Task.CompletedTask);

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var response = await grpcClient.RemoveAsync(new RemoveProductRequest()
        {
            Id = productId
        });
        
        Assert.Equal(response, new Empty());
    }
    
    [Fact]
    public async Task Remove_ProductWithNegativeId_ShouldReturnStatusCodeInternalServer()
    {
        var productId = -1;
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.RemoveAsync(It.IsAny<long>())) 
            .Returns(Task.CompletedTask);

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var removeRequest = new RemoveProductRequest()
        {
            Id = productId
        };
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.RemoveAsync(removeRequest));
    }
    
    [Fact]
    public async Task Remove_NotExistProduct_ShouldReturnStatusCodeNotFound()
    {
        var productId = 1;
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.RemoveAsync(It.IsAny<long>())) 
            .ThrowsAsync(new ValueNotFoundException("value didn't find"));

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var removeRequest = new RemoveProductRequest()
        {
            Id = productId
        };
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.RemoveAsync(removeRequest));
    }

    [Fact]
    public async Task UpdatePrice_ProductWithCorrectParameters_ShouldReturnSucceedStatus()
    {
        var productId = 1;
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, p => p.IndexFaker)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, p => p.Random.Double(1, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.UpdatePriceAsync(It.IsAny<long>(), It.IsAny<double>())) 
            .ReturnsAsync(expectedProduct);

        var requestModel = new UpdateProductRequest()
        {
            Id = productId,
            Price = 199
        };
        
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        var actualProduct = _webApplicationFactory.MyMapper.Map<Product>(await grpcClient.UpdateAsync(requestModel));
        
        Assert.Equal(expectedProduct, actualProduct);
    }
    
    [Fact]
    public async Task UpdatePrice_ProductWithNegativePrice_ShouldReturnInternalServerStatus()
    {
        var productId = 1;
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, p => p.Random.Double(1, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.UpdatePriceAsync(It.IsAny<long>(), It.IsAny<double>())) 
            .ReturnsAsync(expectedProduct);

        var requestModel = new AutoFaker<UpdateProductRequest>()
            .RuleFor(p => p.Id, p => p.IndexFaker)
            .RuleFor(p => p.Price, p => p.Random.Number(-3, 0));
        
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.UpdateAsync(requestModel));
    }
    
    [Fact]
    public async Task Filter_ProductWithCorrectFilter_ShouldReturnFilteredProducts()
    {
        var expectedProducts = new AutoFaker<Product>()
            .RuleFor(f => f.Id, p => p.IndexFaker)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, p => p.Random.Double(1, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate(5);
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAllWithFilterAsync(It.IsAny<ProductFilter>())) 
            .ReturnsAsync(expectedProducts);

        var requestModel = new AutoFaker<FilteredListRequest>()
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 3))
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.PageNumber, p => p.Random.Int(1, 100))
            .RuleFor(f => f.PageSize, p => p.Random.Int(1, 100))
            .RuleFor(f => f.DateOfCreation, new Timestamp()
            {
                Seconds = 100,
                Nanos = 10
            })
            .Generate();
        
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        var actualProducts = 
            _webApplicationFactory.MyMapper.Map<List<Product>>((await grpcClient.ListAsync(requestModel)).Products);
        
        Assert.Equal(expectedProducts, actualProducts);
    }
    
    [Fact]
    public async Task Filter_ProductWithFilterWithEmptyName_ShouldReturnInternalServerStatus()
    {
        var expectedProducts = new AutoFaker<Product>()
            .RuleFor(f => f.Id, p => p.IndexFaker)
            .RuleFor(f => f.Name, string.Empty)
            .RuleFor(f => f.Price, p => p.Random.Double(1, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate(5);
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAllWithFilterAsync(It.IsAny<ProductFilter>())) 
            .ReturnsAsync(expectedProducts);

        var requestModel = new AutoFaker<FilteredListRequest>()
            .RuleFor(p => p.WarehouseId, p => p.Random.Number(-3, 0))
            .Generate();
        
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        await Assert.ThrowsAsync<RpcException>(async () => await grpcClient.ListAsync(requestModel));
    }
}