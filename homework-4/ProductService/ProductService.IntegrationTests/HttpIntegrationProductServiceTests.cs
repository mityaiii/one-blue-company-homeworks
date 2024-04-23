using System.Net;
using System.Text;
using AutoBogus;
using Bogus;
using Moq;
using ProductService.Api;
using ProductService.DataAccess.Exceptions;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Models;
using ProductService.IntegrationTests.Helpers;
using ProductService.IntegrationTests.RequestJsons;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProductService.IntegrationTests;

public class HttpIntegrationProductServiceTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
    
    public HttpIntegrationProductServiceTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task Get_CorrectProductId_ShouldReturnProduct()
    {
        long productId = 1;
    
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, p => p.IndexFaker)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.DateOfCreation, p => p.Date.Recent().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate();
    
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ReturnsAsync(expectedProduct);
        
        var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync($"/v1/product/get?id={productId}");
        var responseString = await response.Content.ReadAsStringAsync();
        var actualProduct = 
            _webApplicationFactory.MyMapper.Map<Product>(ProductReply.Parser.ParseJson(responseString));

        Assert.Equal(expectedProduct, actualProduct);
    }
    
    [Fact]
    public async Task Get_NegativeProductId_ShouldReturnInternalServerStatus()
    {
        long productId = -1;
    
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .Generate();
    
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ReturnsAsync(expectedProduct);
        
        var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync($"/v1/product/get?id={productId}");
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Get_NotExistProductId_ShouldReturnNotFoundStatus()
    {
        long productId = 1;
    
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ThrowsAsync(new ValueNotFoundException("value didn't find"));
        
        var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync($"/v1/product/get?id={productId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

        var client = _webApplicationFactory.CreateClient();

        var requestModel = new AutoFaker<TestAddProductRequest>()
            .RuleFor(f => f.name, p => p.Commerce.ProductName())
            .RuleFor(f => f.price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.weight, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.productType, p => p.Random.Int(0, 3))
            .RuleFor(f => f.dateOfCreation, p => p.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))
            .RuleFor(f => f.warehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/v1/product/add", requestContent);
        var result = JsonSerializer.Deserialize<TestAddProductResponse>(await response.Content.ReadAsStringAsync());
        
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.id);
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

        var client = _webApplicationFactory.CreateClient();

        var requestModel = new AutoFaker<TestAddProductRequest>()
            .RuleFor(f => f.name, p => p.Commerce.ProductName())
            .RuleFor(f => f.price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.weight, p => p.Random.Double(-1000, 0))
            .RuleFor(f => f.productType, p => p.Random.Int(0, 3))
            .RuleFor(f => f.dateOfCreation, p => p.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))
            .RuleFor(f => f.warehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/v1/product/add", requestContent);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
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

        var client = _webApplicationFactory.CreateClient();

        var requestModel = new AutoFaker<TestAddProductRequest>()
            .RuleFor(f => f.name, "")
            .RuleFor(f => f.price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.weight, p => p.Random.Double(-1000, 0))
            .RuleFor(f => f.productType, p => p.Random.Int(0, 3))
            .RuleFor(f => f.dateOfCreation, p => p.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))
            .RuleFor(f => f.warehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/v1/product/add", requestContent);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_ProductButRepositoryIsDoingAnotherProcess_ShouldReturnInternalServerStatus()
    {
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.AddAsync(It.IsAny<Product>())) 
            .ThrowsAsync(new NoValueWasReceived());

        var client = _webApplicationFactory.CreateClient();

        var requestModel = new AutoFaker<TestAddProductRequest>()
            .RuleFor(f => f.name, p => p.Commerce.ProductName())
            .RuleFor(f => f.price, p => p.Random.Double(1, 1000))
            .RuleFor(f => f.weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.productType, p => p.Random.Int(0, 3))
            .RuleFor(f => f.dateOfCreation, p => p.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))
            .RuleFor(f => f.warehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/v1/product/add", requestContent);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Remove_ProductWithCorrectId_ShouldReturnStatusCodeSucceed()
    {
        var productId = 1;
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.RemoveAsync(It.IsAny<long>())) 
            .Returns(Task.CompletedTask);

        var client = _webApplicationFactory.CreateClient();
        var response = await client.DeleteAsync($"/v1/product/remove?id={productId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Remove_ProductWithNegativeId_ShouldReturnStatusCodeInternalServer()
    {
        var productId = -1;
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.RemoveAsync(It.IsAny<long>())) 
            .Returns(Task.CompletedTask);

        var client = _webApplicationFactory.CreateClient();
        var response = await client.DeleteAsync($"/v1/product/remove?id={productId}");
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Remove_NotExistProduct_ShouldReturnStatusCodeNotFound()
    {
        var productId = 1;
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.RemoveAsync(It.IsAny<long>())) 
            .ThrowsAsync(new ValueNotFoundException("value didn't find"));

        var client = _webApplicationFactory.CreateClient();
        var response = await client.DeleteAsync($"/v1/product/remove?id={productId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePrice_ProductWithCorrectParameters_ShouldReturnSucceedStatus()
    {
        var productId = 1;
        double productPrice = 154.3;
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, productPrice)
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.UpdatePriceAsync(It.IsAny<long>(), It.IsAny<double>())) 
            .ReturnsAsync(expectedProduct);

        var requestModel = new TestUpdatePriceRequest()
        {
            id = productId,
            price = 199
        };
        
        var client = _webApplicationFactory.CreateClient();
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync($"/v1/product/update-price", requestContent);        
        var actualProduct = 
            _webApplicationFactory.MyMapper.Map<Product>(ProductReply.Parser.ParseJson(await response.Content.ReadAsStringAsync()));
        
        Assert.Equal(expectedProduct, actualProduct);
    }
    
    [Fact]
    public async Task UpdatePrice_ProductWithNegativePrice_ShouldReturnInternalServerStatus()
    {
        var productId = 1;
        double productPrice = 103.3;
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, productPrice)
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.UpdatePriceAsync(It.IsAny<long>(), It.IsAny<double>())) 
            .ReturnsAsync(expectedProduct);

        var requestModel = new TestUpdatePriceRequest()
        {
            id = productId,
            price = -0.3
        };
        
        var client = _webApplicationFactory.CreateClient();
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync($"/v1/product/update-price", requestContent);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdatePrice_ProductWithNegativeId_ShouldReturnInternalServerStatus()
    {
        var productId = -1;
        double productPrice = 103.3;
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.Name, p => p.Commerce.ProductName())
            .RuleFor(f => f.Price, productPrice)
            .RuleFor(f => f.Weight, p => p.Random.Double(1, 100))
            .RuleFor(f => f.DateOfCreation, f => f.Date.Past().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Long(1, 100))
            .Generate();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.UpdatePriceAsync(It.IsAny<long>(), It.IsAny<double>())) 
            .ReturnsAsync(expectedProduct);

        var requestModel = new TestUpdatePriceRequest()
        {
            id = productId,
            price = 0.3
        };
        
        var client = _webApplicationFactory.CreateClient();
        var jsonRequest = JsonSerializer.Serialize(requestModel);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync($"/v1/product/update-price", requestContent);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
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

        ProductFilter productFilter = new ProductFilter();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(f => f.GetAllWithFilterAsync(It.IsAny<ProductFilter>())) 
            .ReturnsAsync(expectedProducts);
        
        var client = _webApplicationFactory.CreateClient();
        
        var response = await client.GetAsync("/v1/product/list" + productFilter.ToQueryString());
        var jsonResponse = await response.Content.ReadAsStringAsync();

        var actualProducts = 
            _webApplicationFactory.MyMapper.Map<List<Product>>(ListProductResponse.Parser.ParseJson(jsonResponse).Products);
        Assert.Equal(expectedProducts, actualProducts);
    }
    
    [Fact]
    public async Task Filter_ProductWithFilterWithNegativeWarehouseId_ShouldReturnInternalServerStatus()
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
        
        var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync("/v1/product/list?warehouseId=-3");
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}