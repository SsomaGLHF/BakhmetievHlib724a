using System;

namespace Lab6_Animals
{
    // ===== Базовий клас =====
    public class Animal
    {
        // Поля (інкапсульовані)
        private int age;
        private double weight;
        private string name;

        // Властивості для читання/запису з перевіркою значень
        public int Age
        {
            get => age;
            set => age = value >= 0 ? value : throw new ArgumentException("Вік не може бути від'ємним");
        }

        public double Weight
        {
            get => weight;
            set => weight = value > 0 ? value : throw new ArgumentException("Вага має бути додатною");
        }

        public string Name
        {
            get => name;
            set => name = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Ім'я не може бути порожнім")
                : value;
        }

        // Конструктор без параметрів
        public Animal() : this("Невідома тварина", 0, 1)
        {
        }

        // Конструктор з параметрами
        public Animal(string name, int age, double weight)
        {
            Name = name;
            Age = age;
            Weight = weight;
        }

        // Віртуальний метод
        public virtual void MakeSound()
        {
            Console.WriteLine($"{Name} видає якийсь звук...");
        }

        // Віртуальний метод для виведення інформації
        public virtual string GetInfo()
        {
            return $"Тварина: {Name}, вік: {Age} р., вага: {Weight} кг";
        }
    }

    // ===== Ссавці =====
    public class Mammal : Animal
    {
        public string FurColor { get; set; }

        public Mammal() : base()
        {
            FurColor = "невідомий";
        }

        public Mammal(string name, int age, double weight, string furColor)
            : base(name, age, weight)
        {
            FurColor = furColor;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"Ссавець {Name} ричить/гарчить: Р-р-р!");
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $", колір шерсті: {FurColor} (ссавець)";
        }
    }

    // ===== Птахи =====
    public class Bird : Animal
    {
        public double WingSpan { get; set; } // розмах крил, см

        public Bird() : base()
        {
            WingSpan = 0;
        }

        public Bird(string name, int age, double weight, double wingSpan)
            : base(name, age, weight)
        {
            WingSpan = wingSpan;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"Птах {Name} співає: Чік-чирик!");
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $", розмах крил: {WingSpan} см (птах)";
        }
    }

    // ===== Риби =====
    public class Fish : Animal
    {
        public bool IsFreshwater { get; set; } // true — прісноводна

        public Fish() : base()
        {
            IsFreshwater = true;
        }

        public Fish(string name, int age, double weight, bool isFreshwater)
            : base(name, age, weight)
        {
            IsFreshwater = isFreshwater;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"Риба {Name} видає тихе булькання: Буль-буль...");
        }

        public override string GetInfo()
        {
            string water = IsFreshwater ? "прісноводна" : "морська";
            return base.GetInfo() + $", {water} (риба)";
        }
    }

    // ===== Головна програма =====
    internal class Program
    {
        // Поліморфний метод: приймає будь-яку тварину
        static void Describe(Animal animal)
        {
            Console.WriteLine(animal.GetInfo());
            animal.MakeSound();
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Створення об'єктів ===\n");

            Mammal lion = new Mammal("Лев Симба", 5, 190.5, "золотистий");
            Bird eagle = new Bird("Орел Арго", 3, 4.2, 210);
            Fish goldfish = new Fish("Золота рибка", 1, 0.3, true);
            Animal generic = new Animal("Невідомий звір", 2, 10);

            Describe(lion);
            Describe(eagle);
            Describe(goldfish);
            Describe(generic);

            // Поліморфізм: масив базового типу
            Console.WriteLine("=== Поліморфізм (масив Animal[]) ===\n");
            Animal[] zoo =
            {
                new Mammal("Вовк Сірко", 4, 45, "сірий"),
                new Bird("Папуга Кеша", 2, 0.4, 35),
                new Fish("Акула Мегі", 8, 300, false),
                new Animal()
            };

            foreach (Animal a in zoo)
            {
                a.MakeSound(); // викликається перевизначений метод відповідного класу
            }

            // Upcast / downcast, is, as
            Console.WriteLine("\n=== Приведення типів ===\n");

            Animal pet = lion;              // upcast (неявне)
            Console.WriteLine(pet.Name);    // доступні лише члени Animal
            Mammal m = (Mammal)pet;         // downcast (явне)
            Console.WriteLine($"Колір шерсті: {m.FurColor}");

            Animal other = eagle;
            Fish notFish = other as Fish;   // as повертає null, якщо приведення неможливе
            Console.WriteLine(notFish == null
                ? "other не є Fish (as повернув null)"
                : "other є Fish");

            if (other is Bird b)            // is з оголошенням змінної (C# 7)
                Console.WriteLine($"other є птахом, розмах крил: {b.WingSpan} см");

            try
            {
                Fish wrong = (Fish)other;   // некоректний downcast
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine("Помилка приведення: " + ex.Message);
            }

            // Перевірка властивостей з валідацією
            Console.WriteLine("\n=== Перевірка властивостей ===\n");
            try
            {
                lion.Age = -3;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }

            lion.Weight = 200;
            Console.WriteLine($"Нова вага лева: {lion.Weight} кг");

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}