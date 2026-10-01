using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab7_Variant2
{
    class LoginRegistry
    {
        private readonly Dictionary<string, DateTime> _logins =
            new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public int Count => _logins.Count;

        // Перевірка, чи вже зареєстровано логін
        public bool IsRegistered(string login) => _logins.ContainsKey(login);

        // Реєстрація: повертає false, якщо логін уже зайнятий
        public bool Register(string login)
        {
            if (_logins.ContainsKey(login))
                return false;

            _logins.Add(login, DateTime.Now);
            return true;
        }

        // Видалення логіну
        public bool Remove(string login) => _logins.Remove(login);

        // Отримання дати реєстрації
        public bool TryGetDate(string login, out DateTime date) =>
            _logins.TryGetValue(login, out date);

        // Усі логіни у вигляді відсортованого списку List<string>
        public List<string> GetAllSorted()
        {
            List<string> list = _logins.Keys.ToList();
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list;
        }
    }

    class Program
    {
        // Допустимий логін: 3–20 символів, літери, цифри, '_' або '.'
        static bool IsValidLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login)) return false;
            if (login.Length < 3 || login.Length > 20) return false;
            return login.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
        }

        static string ReadLogin(string prompt)
        {
            Console.Write(prompt);
            return (Console.ReadLine() ?? "").Trim();
        }

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            var registry = new LoginRegistry();

            // Початкові дані для демонстрації
            registry.Register("anna");
            registry.Register("oleg");
            registry.Register("maksym");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("===== Реєстр унікальних логінів =====");
                Console.WriteLine("1. Зареєструвати новий логін");
                Console.WriteLine("2. Перевірити, чи зайнятий логін");
                Console.WriteLine("3. Видалити логін");
                Console.WriteLine("4. Показати всі логіни");
                Console.WriteLine("5. Показати дату реєстрації логіну");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        {
                            string login = ReadLogin("Введіть логін: ");
                            if (!IsValidLogin(login))
                                Console.WriteLine("Некоректний логін (3–20 символів: літери, цифри, '_' або '.').");
                            else if (registry.Register(login))
                                Console.WriteLine($"Логін \"{login}\" успішно зареєстровано.");
                            else
                                Console.WriteLine($"Логін \"{login}\" уже зайнятий!");
                            break;
                        }
                    case "2":
                        {
                            string login = ReadLogin("Введіть логін для перевірки: ");
                            Console.WriteLine(registry.IsRegistered(login)
                                ? $"Логін \"{login}\" уже зареєстровано."
                                : $"Логін \"{login}\" вільний.");
                            break;
                        }
                    case "3":
                        {
                            string login = ReadLogin("Введіть логін для видалення: ");
                            Console.WriteLine(registry.Remove(login)
                                ? $"Логін \"{login}\" видалено."
                                : $"Логін \"{login}\" не знайдено.");
                            break;
                        }
                    case "4":
                        {
                            Console.WriteLine($"Усього логінів: {registry.Count}");
                            int i = 1;
                            foreach (string login in registry.GetAllSorted())
                                Console.WriteLine($"{i++}. {login}");
                            break;
                        }
                    case "5":
                        {
                            string login = ReadLogin("Введіть логін: ");
                            if (registry.TryGetDate(login, out DateTime date))
                                Console.WriteLine($"Логін \"{login}\" зареєстровано: {date:dd.MM.yyyy HH:mm:ss}");
                            else
                                Console.WriteLine($"Логін \"{login}\" не знайдено.");
                            break;
                        }
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Невірний вибір, спробуйте ще раз.");
                        break;
                }
            }
        }
    }
}