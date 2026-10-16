namespace Lab8
{
    class ShoppingCart<T> where T : IProduct
    {
        private readonly List<T> items = new();

        public int Count => items.Count;

        // Додавання товару
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            items.Add(item);
        }

        // Видалення за об'єктом
        public bool Remove(T item) => items.Remove(item);

        // Видалення за назвою
        public bool RemoveByName(string name)
        {
            T? item = items.FirstOrDefault(i =>
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return item != null && items.Remove(item);
        }

        // Загальна вартість замовлення
        public decimal GetTotalCost() => items.Sum(i => i.Price * i.Quantity);

        // Виведення вмісту кошика
        public void Print()
        {
            if (items.Count == 0)
            {
                Console.WriteLine("Кошик порожній.");
                return;
            }
            foreach (T item in items)
                Console.WriteLine(" • " + item);
            Console.WriteLine($"Разом: {GetTotalCost():F2} грн");
        }
    }
}