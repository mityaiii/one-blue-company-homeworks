using AutoBogus;
using Bogus;
using FluentAssertions;
using ProductService.DataAccess.Exceptions;
using ProductService.DataAccess.Repositories;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Entities.Repositories;
using ProductService.Domain.Models;

namespace ProductService.RepositoryTests;

public class RepositoryTests
{
    private readonly IProductRepository _productRepository;

    public RepositoryTests()
    {
        _productRepository = new ProductRepository();
    }

    [Fact]
    public void Get_ProductNotExistInRepository_ShouldThrowExceptionProductNotFound()
    {
        long productId = 1;
        
        Assert.ThrowsAsync<ValueNotFoundException>(() => _productRepository.GetAsync(productId));
    }
    
    [Fact]
    public async Task AddAsync_ProductExistInRepository_ShouldReturnProductFromRepository()
    {
        var product = new AutoFaker<Product>().Generate();
        var filter = new ProductFilter();

        await _productRepository.AddAsync(product);

        var products = 
            await _productRepository.GetAllWithFilterAsync(filter);
        products.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task GetById_ProductExistInRepository_ShouldReturnProductFromRepository()
    {
        var expectedProduct = new AutoFaker<Product>().Generate();
        
        var productId = await _productRepository.AddAsync(expectedProduct);
        var actualProduct = await _productRepository.GetAsync(productId);

        actualProduct.Should().Be(expectedProduct);
    }
    
    [Fact]
    public async Task UpdateCost_ProductExistInRepository_ShouldReturnProductWithNewCost()
    {
        var product = new AutoFaker<Product>().Generate();
        var newPrice = 1.0;
        
        var productId = await _productRepository.AddAsync(product);
        var actualProduct = await _productRepository.UpdatePriceAsync(productId, newPrice);

        var expectedProduct = product;
        expectedProduct.Price = newPrice;
        
        actualProduct.Should().Be(expectedProduct);
    }

    [Fact]
    public async Task UpdateCost_ProductNotExistInRepository_ShouldReturnRepositoryException()
    {
        const int productId = 1;
        const double price = 1.0;

        var act = () => _productRepository.UpdatePriceAsync(productId, price);

        await act.Should().ThrowAsync<NoValueWasReceived>();
    }
    
    [Fact]
    public async void Remove_ProductFromRepository_ShouldThrowExceptionNotFoundValue()
    {
        var product = new Faker<Product>()
            .RuleFor(f => f.Id, f => f.IndexFaker)
            .Generate();
        
        await Assert.ThrowsAsync<ValueNotFoundException>(() => _productRepository.RemoveAsync(product.Id));
    }
}