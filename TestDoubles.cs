using Castle.Core.Resource;
using Lab2;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Tests
{
    // ==========================================
    // 1. STUB (Заглушка с фиксированным ответом)[cite: 1]
    // ==========================================
    public class ProductRepositoryStub : IProductRepository
    {
        public Product GetProductById(int id)
        {
            // Возвращает зафиксированный объект независимо от передаваемого id[cite: 1]
            return new Product { Id = id, Name = "Stub Product", Price = 100m };
        }
    }

    public class CustomerRepositoryStub : ICustomerRepository
    {
        public Customer GetCustomerById(int id)
        {
            // Возвращает зафиксированного клиента[cite: 1]
            return new Customer { Id = id, Name = "Stub Customer", Email = "stub@example.com" };
        }
    }

    // ==========================================
    // 2. FAKE (Спрощенное хранилище в памяти)[cite: 1]
    // ==========================================
    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }

    public class FakeCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new();

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        public Customer GetCustomerById(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }
    }
}