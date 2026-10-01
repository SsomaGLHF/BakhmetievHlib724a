public class OrderService

{

    private readonly IProductRepository _productRepository;

    private readonly ICustomerRepository _customerRepository;



    public OrderService(IProductRepository productRepository,

                         ICustomerRepository customerRepository)

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