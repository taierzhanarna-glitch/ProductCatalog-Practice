using ProductCatalogPractice.Models;

namespace ProductCatalogPractice.Services;

public interface IProductCatalog
{
    IReadOnlyList<Product> GetAll();
    Product? GetById(int id);
    Product Add(Product product);
}
