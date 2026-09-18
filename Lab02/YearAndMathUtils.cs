using System;

namespace Lab2
{
    public class YearAndMathUtils
    {
        // 1. Функція, яка перевіряє, чи є рік високосним
        public bool IsLeapYear(int year)
        {
            if (year <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Рік має бути більше 0.");
            }

            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }

        // 2. Функція, яка обчислює факторіал числа
        public long Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), "Факторіал від'ємного числа не існує.");
            }

            long result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }
    }
}