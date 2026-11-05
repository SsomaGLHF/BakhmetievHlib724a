public interface ICustomerRepository
{
    Customer GetCustomerById(int id);
}

public interface IProductRepository
{
    Product GetProductById(int id);
}