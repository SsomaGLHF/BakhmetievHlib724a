using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Lab Work #3. Variant 2 ===");
        Console.WriteLine("1 - Multiplication table");
        Console.WriteLine("2 - Sort array");
        Console.Write("Choose a task: ");

        int choice = int.Parse(Console.ReadLine());

        if (choice == 1)
        {
            Task1_MultiplicationTable();
        }
        else if (choice == 2)
        {
            Task2_SortArray();
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    // Task 1: Multiplication table for numbers from 1 to N
    static void Task1_MultiplicationTable()
    {
        Console.WriteLine("\n--- Task 1: Multiplication table ---");
        Console.WriteLine("Enter N:");
        int n = int.Parse(Console.ReadLine());

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= 10; j++)
            {
                Console.Write($"{i} x {j} = {i * j}\t");
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', 40));
        }
    }

    // Task 2: Sort array elements in ascending order
    static void Task2_SortArray()
    {
        Console.WriteLine("\n--- Task 2: Sort array ---");
        Console.WriteLine("Enter the number of array elements:");
        int size = int.Parse(Console.ReadLine());

        int[] array = new int[size];

        Console.WriteLine("Enter the array elements:");
        for (int i = 0; i < size; i++)
        {
            array[i] = int.Parse(Console.ReadLine());
        }

        // Bubble sort
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - 1 - i; j++)
            {
                if (array[j] > array[j + 1])
                {
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }

        Console.WriteLine("Sorted array:");
        foreach (int element in array)
        {
            Console.Write(element + " ");
        }
        Console.WriteLine();
    }
}