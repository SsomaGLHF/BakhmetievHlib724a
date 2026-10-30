using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab9
{
    // Клас Student: Ім'я, Вік, Група, Середній бал
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Group { get; set; }
        public double AverageGrade { get; set; }

        public Student(string name, int age, string group, double averageGrade)
        {
            Name = name;
            Age = age;
            Group = group;
            AverageGrade = averageGrade;
        }

        public override string ToString()
        {
            return $"{Name,-12} | Вік: {Age,2} | Група: {Group,-5} | Середній бал: {AverageGrade,5:F1}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Створення та заповнення списку студентів
            var students = new List<Student>
            {
                new Student("Олег",      19, "КН-11", 85.5),
                new Student("Марія",     20, "КН-12", 92.0),
                new Student("Андрій",    19, "КН-11", 74.3),
                new Student("Наталія",   21, "ІН-21", 88.7),
                new Student("Іван",      20, "ІН-21", 67.9),
                new Student("Софія",     19, "КН-12", 95.2),
                new Student("Дмитро",    22, "ПІ-31", 79.8),
                new Student("Олена",     21, "ПІ-31", 81.4),
                new Student("Максим",    20, "КН-11", 90.1),
                new Student("Тетяна",    22, "ІН-21", 72.5)
            };

            Console.WriteLine("=== Усі студенти ===");
            foreach (var s in students)
                Console.WriteLine(s);

            // 2.1 Фільтрація (Where): студенти із середнім балом вище 80
            Console.WriteLine("\n=== 1. Студенти із середнім балом вище 80 ===");
            var highGrade = students.Where(s => s.AverageGrade > 80);
            foreach (var s in highGrade)
                Console.WriteLine(s);

            // 2.2 Сортування (OrderBy): за віком
            Console.WriteLine("\n=== 2. Студенти, відсортовані за віком ===");
            var sortedByAge = students.OrderBy(s => s.Age);
            foreach (var s in sortedByAge)
                Console.WriteLine(s);

            // 2.3 Групування (GroupBy): кількість студентів у кожній групі
            Console.WriteLine("\n=== 3. Кількість студентів у кожній групі ===");
            var groups = students.GroupBy(s => s.Group);
            foreach (var g in groups)
                Console.WriteLine($"Група {g.Key}: {g.Count()} студ. ({string.Join(", ", g.Select(s => s.Name))})");

            // 2.4 Агрегація (Max / MaxBy): студент із найвищим середнім балом
            Console.WriteLine("\n=== 4. Студент із найвищим середнім балом ===");
            double maxGrade = students.Max(s => s.AverageGrade);
            var best = students.First(s => s.AverageGrade == maxGrade);
            Console.WriteLine(best);

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}