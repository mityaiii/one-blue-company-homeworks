using System.Collections.Concurrent;
using ProductService.DataAccess.Exceptions;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Entities.Repositories;
using ProductService.Domain.Models;

namespace ProductService.DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<long, Product> _products;
    private long _productCounter;
    private readonly object _lock;
    
    public ProductRepository()
    {
        _products = new ConcurrentDictionary<long, Product>();
        _productCounter = 1;
        _lock = new object();
    }

    public async Task<long> AddAsync(Product product)
    {
        product.Id = _productCounter;
        bool wasAdded = _products.TryAdd(_productCounter, product);

        if (!wasAdded)
            throw new NoValueWasReceived($"Product {nameof(product)} wasn't added");
        
        lock (_lock)
        {
            _productCounter++;   
        }
        
        return await Task.FromResult(product.Id);
    }

    public Task RemoveAsync(long productId)
    {
        if (!_products.ContainsKey(productId))
            throw new ValueNotFoundException($"Product {productId} not found");

        if (!_products.TryRemove(productId, out _))
            throw new NoValueWasReceived($"Product {productId} wasn't removed");
        
        return Task.CompletedTask;
    }


    public async Task<Product> UpdatePriceAsync(long productId, double newPrice)
    {
        var product = await GetAsync(productId);
        
        lock (_lock)
        {
            product.Price = newPrice;
        }

        return product;
    }
    
    public async Task<Product> GetAsync(long productId)
    {
        bool wasGot = _products.TryGetValue(productId, out var product);

        if (!wasGot)
            throw new NoValueWasReceived($"Product {nameof(product)} wasn't got");
        
        if (product is null)
        {
            throw new ValueNotFoundException($"product with id {nameof(productId)} not found");
        }

        return await Task.FromResult(product);
    }
    
    public async Task<IReadOnlyCollection<Product>> GetAllWithFilterAsync(ProductFilter productFilter)
    {
        var pageAmount = productFilter.PageNumber ?? 1;
        var pageSize = productFilter.PageSize ?? int.MaxValue;
    
        var result = _products
            .Values
            .Where(p => FilterByName(p, productFilter.Name))
            .Where(p => FilterByProductType(p, productFilter.ProductType))
            .Where(p => FilterByDateOfCreation(p, productFilter.DateOfCreation))
            .Where(p => FilterByWarehouseId(p, productFilter.WarehouseId));

        result = MakePagination(result, pageAmount, pageSize);
        
        return await Task.FromResult(result.ToList());
    }

    private bool FilterByName(Product product, string? name)
    {
        if (name is null)
        {
            return true;
        }
        
        return product.Name.Equals(name);
    }

    private bool FilterByProductType(Product product, ProductType? productType)
    {
        if (productType is null)
        {
            return true;
        }

        return product.ProductType == productType;
    }

    private bool FilterByDateOfCreation(Product product, DateTime? dateOfCreation)
    {
        if (dateOfCreation is null)
        {
            return true;
        }

        return product.DateOfCreation.Equals(dateOfCreation);
    }

    private bool FilterByWarehouseId(Product product, long? warehouseId)
    {
        if (warehouseId is null)
        {
            return true;
        }

        return product.WarehouseId == warehouseId;
    }
    
    private IEnumerable<Product> MakePagination(IEnumerable<Product> products, int pageAmount, int pageSize)
    {
        return products
            .Skip((pageAmount - 1) * pageSize)
            .Take(pageSize);
    }
}
