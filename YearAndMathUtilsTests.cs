using System;
using Xunit;

namespace Lab2.Tests
{
    public class YearAndMathUtilsTests
    {
        private readonly YearAndMathUtils _utils;

        public YearAndMathUtilsTests()
        {
            _utils = new YearAndMathUtils();
        }

        // Тести для перевірки високосного року
        [Theory]
        [InlineData(2000, true)]  // Ділиться на 400 -> високосний
        [InlineData(2024, true)]  // Ділиться на 4, але не на 100 -> високосний
        [InlineData(1900, false)] // Ділиться на 100, але не на 400 -> не високосний
        [InlineData(2023, false)] // Не ділиться на 4 -> не високосний
        public void IsLeapYear_ShouldReturnExpectedResult(int year, bool expected)
        {
            // Act
            bool result = _utils.IsLeapYear(year);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsLeapYear_NegativeYear_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _utils.IsLeapYear(-2020));
        }

        // Тести для обчислення факторіала
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(5, 120)]
        [InlineData(7, 5040)]
        public void Factorial_ShouldReturnCorrectResult(int n, long expected)
        {
            // Act
            long result = _utils.Factorial(n);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Factorial_NegativeNumber_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _utils.Factorial(-5));
        }
    }
}