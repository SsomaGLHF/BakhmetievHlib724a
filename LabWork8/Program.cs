namespace Lab8
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Кошик для звичайних товарів
            var cart = new ShoppingCart<Product>();
            cart.Add(new Product("Хліб", 25.50m, 2));
            cart.Add(new Product("Молоко", 42.00m, 3));
            cart.Add(new Product("Сир", 180.00m, 1));

            Console.WriteLine("Кошик з продуктами:");
            cart.Print();

            Console.WriteLine("\nВидаляємо «Молоко»...");
            Console.WriteLine(cart.RemoveByName("Молоко")
                ? "Товар видалено."
                : "Товар не знайдено.");
            cart.Print();

            // Той самий клас з іншим типом
            var techCart = new ShoppingCart<Electronics>();
            techCart.Add(new Electronics("Навушники", 1200m, 1, 12));
            techCart.Add(new Electronics("Миша", 450m, 2, 6));

            Console.WriteLine("\nКошик з електронікою:");
            techCart.Print();

            Console.ReadKey();
        }
    }
}