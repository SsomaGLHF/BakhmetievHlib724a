using System.Collections.Generic;

using System.Linq;



public class ArithmeticUtils

{

    public int Add(int a, int b)

    {

        return a + b;

    }



    public int Gcd(int a, int b)

    {

        while (b != 0)

        {

            int temp = b;

            b = a % b;

            a = temp;

        }

        return a;

    }



    public int SumArray(int[] numbers)

    {

        int sum = 0;

        foreach (var n in numbers)

            sum += n;

        return sum;

    }



    public List<int> FilterOddNumbers(List<int> numbers)

    {

        return numbers.Where(n => n % 2 != 0).ToList();

    }

}