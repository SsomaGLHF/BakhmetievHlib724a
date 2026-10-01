using System.Collections.Generic;
using Xunit;



public class ArithmeticUtilsTests

{

    private readonly ArithmeticUtils _utils;



    public ArithmeticUtilsTests()

    {

        _utils = new ArithmeticUtils();

    }



    [Fact]

    public void Add_ShouldReturnCorrectSum()

    {

        int a = 5, b = 3;

        int result = _utils.Add(a, b);

        Assert.Equal(8, result);

    }



    [Theory]

    [InlineData(48, 18, 6)]

    [InlineData(15, 5, 5)]

    [InlineData(7, 13, 1)]

    public void Gcd_ShouldReturnCorrectGcd(int a, int b, int expectedGcd)

    {

        int result = _utils.Gcd(a, b);

        Assert.Equal(expectedGcd, result);

    }



    [Fact]

    public void SumArray_ShouldReturnCorrectSum()

    {

        int[] numbers = { 1, 2, 3, 4, 5 };

        int result = _utils.SumArray(numbers);

        Assert.Equal(15, result);

    }



    [Fact]

    public void FilterOddNumbers_ShouldReturnOnlyOddNumbers()

    {

        var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7 };

        var result = _utils.FilterOddNumbers(numbers);

        var expected = new List<int> { 1, 3, 5, 7 };

        Assert.Equal(expected, result);

    }

}