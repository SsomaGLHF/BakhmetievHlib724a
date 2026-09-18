namespace Lab2.Tests.TestDoubles
{
    public class ProductRepositoryStub : IProductRepository
    {
        public Product GetProductById(int id)
        {
            return new Product { Id = id, Name = "Stub Product", Price = 100m };
        }
    }

    public class CustomerRepositoryStub : ICustomerRepository
    {
        public Customer GetCustomerById(int id)
        {
            return new Customer { Id = id, Name = "Stub Customer", Email = "stub@example.com" };
        }
    }
}