namespace Lab2
{
    // 1. Класс Customer
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // 2. Класс Product
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // 3. Интерфейс IProductRepository
    public interface IProductRepository
    {
        Product GetProductById(int id);
    }

    // 4. Интерфейс ICustomerRepository
    public interface ICustomerRepository
    {
        Customer GetCustomerById(int id);
    }

    // 5. Класс OrderService
    public class OrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderService(IProductRepository productRepository, ICustomerRepository customerRepository)
        {
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        public bool PlaceOrder(int customerId, int productId)
        {
            var customer = _customerRepository.GetCustomerById(customerId);
            var product = _productRepository.GetProductById(productId);

            if (customer == null || product == null)
            {
                return false;
            }

            return true;
        }
    }
}