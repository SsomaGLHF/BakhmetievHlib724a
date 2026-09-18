using Castle.Core.Resource;
using Lab2;
using Moq;
using Xunit;

namespace Lab2.Tests
{
    public class OrderServiceTests
    {
        // 1. Тест с использованием STUB[cite: 1]
        [Fact]
        public void PlaceOrder_WithStub_ShouldReturnTrue()
        {
            // Arrange
            var productRepo = new ProductRepositoryStub();
            var customerRepo = new CustomerRepositoryStub();
            var service = new OrderService(productRepo, customerRepo);

            // Act
            bool result = service.PlaceOrder(1, 1);

            // Assert
            Assert.True(result);
        }

        // 2. Тест с использованием FAKE[cite: 1]
        [Fact]
        public void PlaceOrder_WithFake_WhenCustomerNotFound_ShouldReturnFalse()
        {
            // Arrange
            var productRepo = new FakeProductRepository();
            var customerRepo = new FakeCustomerRepository();

            // Добавляем только продукт, но клиент отсутствует в Fake-базе
            productRepo.AddProduct(new Product { Id = 1, Name = "Phone", Price = 500m });

            var service = new OrderService(productRepo, customerRepo);

            // Act
            bool result = service.PlaceOrder(customerId: 99, productId: 1);

            // Assert
            Assert.False(result);
        }

        // 3. Тест с использованием MOCK (Moq)[cite: 1]
        [Fact]
        public void PlaceOrder_WithMock_ShouldVerifyRepositoryCalls()
        {
            // Arrange (настройка мок-объектов)[cite: 1]
            var mockProductRepo = new Mock<IProductRepository>();
            var mockCustomerRepo = new Mock<ICustomerRepository>();

            mockCustomerRepo.Setup(repo => repo.GetCustomerById(1))
                .Returns(new Customer { Id = 1, Name = "Ivan", Email = "ivan@example.com" });

            mockProductRepo.Setup(repo => repo.GetProductById(2))
                .Returns(new Product { Id = 2, Name = "Laptop", Price = 1500m });

            var service = new OrderService(mockProductRepo.Object, mockCustomerRepo.Object);

            // Act
            bool result = service.PlaceOrder(customerId: 1, productId: 2);

            // Assert
            Assert.True(result);

            // Проверка взаимодействия: вызывались ли методы репозиториев по 1 разу[cite: 1]
            mockCustomerRepo.Verify(repo => repo.GetCustomerById(1), Times.Once);
            mockProductRepo.Verify(repo => repo.GetProductById(2), Times.Once);
        }
    }
}