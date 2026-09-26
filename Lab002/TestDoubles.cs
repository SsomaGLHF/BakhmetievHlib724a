using System.Collections.Generic;
using System.Linq;

public class FakeCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public void AddCustomer(Customer customer) => _customers.Add(customer);

    public Customer GetCustomerById(int id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }
}

public class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public void AddProduct(Product product) => _products.Add(product);

    public Product GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }
}   // <-- вот эта закрывающая скобка была пропущена

public class CustomerRepositoryStub : ICustomerRepository
{
    private readonly Customer? _customerToReturn;

    public CustomerRepositoryStub(Customer? customerToReturn)
    {
        _customerToReturn = customerToReturn;
    }

    public Customer? GetCustomerById(int id) => _customerToReturn;
}

public class ProductRepositoryStub : IProductRepository
{
    private readonly Product? _productToReturn;

    public ProductRepositoryStub(Product? productToReturn)
    {
        _productToReturn = productToReturn;
    }

    public Product? GetProductById(int id) => _productToReturn;
}