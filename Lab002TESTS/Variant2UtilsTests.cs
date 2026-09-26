using System;

using Xunit;



public class Variant2UtilsTests

{

    private readonly Variant2Utils _utils;



    public Variant2UtilsTests()

    {

        _utils = new Variant2Utils();

    }



    [Theory]

    [InlineData(2000, true)]   // ділиться на 400 

    [InlineData(1900, false)]  // ділиться на 100, але не на 400 

    [InlineData(2024, true)]   // ділиться на 4, не на 100 

    [InlineData(2023, false)]  // не ділиться на 4 

    [InlineData(2400, true)]

    public void IsLeapYear_ShouldReturnCorrectResult(int year, bool expected)

    {

        var result = _utils.IsLeapYear(year);

        Assert.Equal(expected, result);

    }



    [Fact]

    public void IsLeapYear_NegativeYear_ThrowsArgumentException()

    {

        Assert.Throws<ArgumentException>(() => _utils.IsLeapYear(-10));

    }



    [Theory]

    [InlineData(0, 1)]

    [InlineData(1, 1)]

    [InlineData(5, 120)]

    [InlineData(10, 3628800)]

    public void Factorial_ShouldReturnCorrectResult(int n, long expected)

    {

        var result = _utils.Factorial(n);

        Assert.Equal(expected, result);

    }



    [Fact]

    public void Factorial_NegativeNumber_ThrowsArgumentException()

    {

        Assert.Throws<ArgumentException>(() => _utils.Factorial(-3));

    }

}