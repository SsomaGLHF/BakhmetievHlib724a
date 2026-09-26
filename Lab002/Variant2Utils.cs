using System;



public class Variant2Utils

{

    /// <summary> 

    /// Перевіряє, чи є рік високосним. 

    /// Рік є високосним, якщо він ділиться на 4, 

    /// але не ділиться на 100, або якщо ділиться на 400. 

    /// </summary> 

    public bool IsLeapYear(int year)

    {

        if (year <= 0)

            throw new ArgumentException("Рік має бути додатним числом", nameof(year));



        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);

    }



    /// <summary> 

    /// Обчислює факторіал невід'ємного цілого числа n (n!). 

    /// </summary> 

    public long Factorial(int n)

    {

        if (n < 0)

            throw new ArgumentException("Факторіал не визначений для від'ємних чисел", nameof(n));



        long result = 1;

        for (int i = 2; i <= n; i++)

            result *= i;



        return result;

    }

}