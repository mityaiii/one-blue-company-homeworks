using AutoBogus;
using Bogus;
using Moq;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Entities.Repositories;
using ProductService.Domain.Models;
using ProductService.Domain.Services;

namespace ProductService.UniteTest;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly IProductServiceApplication _productService;

    public ProductServiceTests()
    {
        _productService = new ProductServiceApplication(_productRepositoryMock.Object);
    }
    
    [Fact]
    public async void Get_ProductFromRepository_ShouldReturnProduct()
    {
        long productId = 1;
        var expectedProduct = new AutoFaker<Product>()
            .Generate();

        _productRepositoryMock
            .Setup(f => f.GetAsync(productId))
            .ReturnsAsync(expectedProduct);

        var actualProduct = await _productService.GetAsync(productId);
        
        Assert.Equal(expectedProduct, actualProduct);
        _productRepositoryMock.Verify(f => f.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async void Add_ProductInRepository_ShouldReturnProductId()
    {
        var expectedProduct = new Faker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();
        
        _productRepositoryMock
            .Setup(f => f.AddAsync(expectedProduct))
            .ReturnsAsync(expectedProduct.Id);

        var actualProductId = await _productService.AddAsync(expectedProduct);
        
        Assert.Equal(actualProductId, expectedProduct.Id);
        _productRepositoryMock.Verify(f => f.AddAsync(expectedProduct), Times.Once);
    }

    [Fact]
    public async void Update_ProductInRepository_ShouldReturnProductWithUpdatedValue()
    {
        double newPrice = 150;
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();

        _productRepositoryMock
            .Setup(f => f.UpdatePriceAsync(expectedProduct.Id, newPrice))
            .ReturnsAsync(expectedProduct);

        var actualProduct = await _productService.UpdatePriceAsync(expectedProduct.Id, newPrice);
        
        Assert.Equal(expectedProduct, actualProduct);
        _productRepositoryMock.Verify(f => f.UpdatePriceAsync(expectedProduct.Id, newPrice), Times.Once);
    }

    [Fact]
    public async void Filter_ProductInRepository_ShouldReturnFilteredProductsList()
    {
        IReadOnlyCollection<Product> expectedProductList = new AutoFaker<Product>()
            .Generate(5);
        
        var productFilter = new AutoFaker<ProductFilter>()
            .Generate();
        
        _productRepositoryMock
            .Setup(f => f.GetAllWithFilterAsync(productFilter))
            .ReturnsAsync(expectedProductList);

        var actualProductList = await _productService.List(productFilter);
        
        Assert.Equal(expectedProductList, actualProductList);
        _productRepositoryMock.Verify(f => f.GetAllWithFilterAsync(productFilter), Times.Once);
    }

    [Fact]
    public async void Remove_ProductFromRepository_ShouldRemoveProduct()
    {
        var product = new Faker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();

        _productRepositoryMock
            .Setup(f => f.RemoveAsync(product.Id));

        await _productService.RemoveAsync(product.Id);

        _productRepositoryMock.Verify(f => f.RemoveAsync(product.Id), Times.Once);
    }
}