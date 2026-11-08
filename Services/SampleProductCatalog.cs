using ProductCatalogPractice.Models;

namespace ProductCatalogPractice.Services;

public sealed class SampleProductCatalog : IProductCatalog
{
    private readonly object _gate = new();
    private readonly List<Product> _products =
    [
        new() { Id = 1, Name = "Nord Headphones", Price = 64900, Category = "Аудио", IsAvailable = true, Description = "Күнделікті тыңдауға арналған сымсыз құлаққап." },
        new() { Id = 2, Name = "Slate Notebook", Price = 589000, Category = "Компьютер", IsAvailable = true, Description = "Жұмыс пен оқуға арналған жеңіл ноутбук." },
        new() { Id = 3, Name = "Arc Watch", Price = 119000, Category = "Аксессуар", IsAvailable = false, Description = "Қозғалысты және хабарландыруларды бақылайтын сағат." },
        new() { Id = 4, Name = "Wave Speaker", Price = 79900, Category = "Аудио", IsAvailable = true, Description = "Үйге арналған ықшам Bluetooth динамигі." },
        new() { Id = 5, Name = "Mica Phone", Price = 429000, Category = "Смартфон", IsAvailable = true, Description = "Жарқын экраны бар заманауи смартфон." },
        new() { Id = 6, Name = "Line Keyboard", Price = 54900, Category = "Аксессуар", IsAvailable = true, Description = "Ықшам механикалық пернетақта." }
    ];

    private int _nextId = 7;

    public IReadOnlyList<Product> GetAll()
    {
        lock (_gate)
            return _products.Select(Clone).ToArray();
    }

    public Product? GetById(int id)
    {
        lock (_gate)
        {
            var product = _products.FirstOrDefault(product => product.Id == id);
            return product is null ? null : Clone(product);
        }
    }

    public Product Add(Product product)
    {
        lock (_gate)
        {
            product.Id = _nextId++;
            var saved = Clone(product);
            _products.Add(saved);
            return Clone(saved);
        }
    }

    private static Product Clone(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Category = product.Category,
        IsAvailable = product.IsAvailable,
        Description = product.Description,
        ImageUrl = product.ImageUrl
    };
}
