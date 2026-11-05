using Moq;

using Xunit;



public class OrderServiceTests

{

    // ---------- FAKE ---------- 

    [Fact]

    public void PlaceOrder_UsingFake_ReturnsTrue_WhenCustomerAndProductExist()

    {

        var fakeCustomerRepo = new FakeCustomerRepository();

        var fakeProductRepo = new FakeProductRepository();



        fakeCustomerRepo.AddCustomer(new Customer { Id = 1, Name = "Іван", Email = "ivan@test.com" });

        fakeProductRepo.AddProduct(new Product { Id = 1, Name = "Ноутбук", Price = 25000 });



        var service = new OrderService(fakeProductRepo, fakeCustomerRepo);



        var result = service.PlaceOrder(1, 1);



        Assert.True(result);

    }



    [Fact]

    public void PlaceOrder_UsingFake_ReturnsFalse_WhenCustomerDoesNotExist()

    {

        var fakeCustomerRepo = new FakeCustomerRepository();

        var fakeProductRepo = new FakeProductRepository();

        fakeProductRepo.AddProduct(new Product { Id = 1, Name = "Ноутбук", Price = 25000 });



        var service = new OrderService(fakeProductRepo, fakeCustomerRepo);



        var result = service.PlaceOrder(99, 1);



        Assert.False(result);

    }



    // ---------- STUB ---------- 

    [Fact]

    public void PlaceOrder_UsingStub_ReturnsTrue_WhenCustomerAndProductExist()

    {

        var customerStub = new CustomerRepositoryStub(new Customer { Id = 1, Name = "Олена" });

        var productStub = new ProductRepositoryStub(new Product { Id = 1, Name = "Телефон", Price = 12000 });



        var service = new OrderService(productStub, customerStub);



        var result = service.PlaceOrder(1, 1);



        Assert.True(result);

    }



    [Fact]

    public void PlaceOrder_UsingStub_ReturnsFalse_WhenProductIsNull()

    {

        var customerStub = new CustomerRepositoryStub(new Customer { Id = 1, Name = "Олена" });

        var productStub = new ProductRepositoryStub(null);



        var service = new OrderService(productStub, customerStub);



        var result = service.PlaceOrder(1, 1);



        Assert.False(result);

    }



    // ---------- MOCK ---------- 

    [Fact]

    public void PlaceOrder_UsingMock_ReturnsTrue_AndVerifiesCalls()

    {

        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var mockProductRepo = new Mock<IProductRepository>();



        mockCustomerRepo.Setup(r => r.GetCustomerById(1))

            .Returns(new Customer { Id = 1, Name = "Петро" });

        mockProductRepo.Setup(r => r.GetProductById(1))

            .Returns(new Product { Id = 1, Name = "Планшет", Price = 8000 });



        var service = new OrderService(mockProductRepo.Object, mockCustomerRepo.Object);



        var result = service.PlaceOrder(1, 1);



        Assert.True(result);

        mockCustomerRepo.Verify(r => r.GetCustomerById(1), Times.Once);

        mockProductRepo.Verify(r => r.GetProductById(1), Times.Once);

    }



    [Fact]

    public void PlaceOrder_UsingMock_ReturnsFalse_WhenCustomerNotFound()

    {

        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var mockProductRepo = new Mock<IProductRepository>();



        mockCustomerRepo.Setup(r => r.GetCustomerById(It.IsAny<int>())).Returns((Customer)null);

        mockProductRepo.Setup(r => r.GetProductById(1))

            .Returns(new Product { Id = 1, Name = "Планшет", Price = 8000 });



        var service = new OrderService(mockProductRepo.Object, mockCustomerRepo.Object);



        var result = service.PlaceOrder(99, 1);



        Assert.False(result);

        mockProductRepo.Verify(r => r.GetProductById(1), Times.Once);

    }

}