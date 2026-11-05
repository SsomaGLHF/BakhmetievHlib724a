using Intercom.Core;
using Lab5.Models;
using System.Reflection;
using Xunit;

namespace Lab5.Company.Tests;

public class CompanyTests
{
    private static Models.Company Create(int employees = 10, decimal revenue = 1000m)
        => new("Acme", "12345678", "IT", 2020, employees, revenue);

    // ---------- Конструктори ----------

    [Fact]
    public void DefaultConstructor_SetsDefaultValues()
    {
        var c = new Models.Company();
        Assert.Equal("Unknown", c.Name);
        Assert.Equal("00000000", c.RegistrationNumber);
        Assert.Equal("Unknown", c.Industry);
        Assert.Equal(DateTime.Now.Year, c.FoundationYear);
        Assert.Equal(0, c.EmployeesCount);
        Assert.Equal(0m, c.Revenue);
    }

    [Fact]
    public void ShortConstructor_SetsFieldsAndZeroes()
    {
        var c = new Models.Company("  Acme  ", "12345678", " IT ", 2015);
        Assert.Equal("Acme", c.Name);
        Assert.Equal("IT", c.Industry);
        Assert.Equal(2015, c.FoundationYear);
        Assert.Equal(0, c.EmployeesCount);
        Assert.Equal(0m, c.Revenue);
    }

    [Fact]
    public void FullConstructor_SetsAllFields()
    {
        var c = Create(25, 5000.5m);
        Assert.Equal(25, c.EmployeesCount);
        Assert.Equal(5000.5m, c.Revenue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234567a")]
    public void Constructor_InvalidRegistrationNumber_Throws(string? number)
    {
        Assert.Throws<ArgumentException>(() => new Models.Company("Acme", number!, "IT", 2020));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_Throws(string? name)
    {
        Assert.Throws<ArgumentException>(() => new Models.Company(name!, "12345678", "IT", 2020));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_InvalidIndustry_Throws(string? industry)
    {
        Assert.Throws<ArgumentException>(() => new Models.Company("Acme", "12345678", industry!, 2020));
    }

    [Fact]
    public void Constructor_YearTooEarly_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Models.Company("Acme", "12345678", "IT", Models.Company.MinFoundationYear - 1));
    }

    [Fact]
    public void Constructor_YearInFuture_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Models.Company("Acme", "12345678", "IT", DateTime.Now.Year + 1));
    }

    [Fact]
    public void Constructor_NegativeEmployees_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Models.Company("Acme", "12345678", "IT", 2020, -1, 0m));
    }

    [Fact]
    public void Constructor_NegativeRevenue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Models.Company("Acme", "12345678", "IT", 2020, 0, -1m));
    }

    // ---------- Властивості ----------

    [Fact]
    public void Properties_ValidValues_AreSet()
    {
        var c = Create();
        c.Name = "NewName";
        c.Industry = "Finance";
        c.FoundationYear = 2000;
        c.EmployeesCount = 100;
        c.Revenue = 999m;

        Assert.Equal("NewName", c.Name);
        Assert.Equal("Finance", c.Industry);
        Assert.Equal(2000, c.FoundationYear);
        Assert.Equal(100, c.EmployeesCount);
        Assert.Equal(999m, c.Revenue);
    }

    [Fact]
    public void NameSetter_Empty_Throws()
        => Assert.Throws<ArgumentException>(() => Create().Name = " ");

    [Fact]
    public void IndustrySetter_Empty_Throws()
        => Assert.Throws<ArgumentException>(() => Create().Industry = "");

    [Fact]
    public void FoundationYearSetter_Invalid_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().FoundationYear = 1700);

    [Fact]
    public void EmployeesCountSetter_Negative_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().EmployeesCount = -5);

    [Fact]
    public void RevenueSetter_Negative_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().Revenue = -0.01m);

    // ---------- Методи ----------

    [Fact]
    public void Hire_IncreasesEmployees()
    {
        var c = Create(10);
        c.Hire(5);
        Assert.Equal(15, c.EmployeesCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Hire_NonPositive_Throws(int count)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().Hire(count));

    [Fact]
    public void Fire_DecreasesEmployees()
    {
        var c = Create(10);
        c.Fire(4);
        Assert.Equal(6, c.EmployeesCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Fire_NonPositive_Throws(int count)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().Fire(count));

    [Fact]
    public void Fire_MoreThanExists_Throws()
        => Assert.Throws<InvalidOperationException>(() => Create(3).Fire(4));

    [Fact]
    public void AddRevenue_IncreasesRevenue()
    {
        var c = Create(10, 1000m);
        c.AddRevenue(250.5m);
        Assert.Equal(1250.5m, c.Revenue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void AddRevenue_NonPositive_Throws(int amount)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().AddRevenue(amount));

    [Fact]
    public void GetAge_WithExplicitYear_ReturnsDifference()
        => Assert.Equal(5, Create().GetAge(2025));

    [Fact]
    public void GetAge_WithoutArgument_UsesCurrentYear()
        => Assert.Equal(DateTime.Now.Year - 2020, Create().GetAge());

    [Fact]
    public void GetAge_YearBeforeFoundation_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Create().GetAge(2019));

    [Fact]
    public void GetRevenuePerEmployee_Calculates()
        => Assert.Equal(100m, Create(10, 1000m).GetRevenuePerEmployee());

    [Fact]
    public void GetRevenuePerEmployee_NoEmployees_ReturnsZero()
        => Assert.Equal(0m, Create(0, 1000m).GetRevenuePerEmployee());

    [Fact]
    public void IsStartup_YoungAndSmall_True()
        => Assert.True(Create(10).IsStartup(2024));

    [Fact]
    public void IsStartup_TooOld_False()
        => Assert.False(Create(10).IsStartup(2030));

    [Fact]
    public void IsStartup_TooManyEmployees_False()
        => Assert.False(Create(51).IsStartup(2024));

    // ---------- Деконструктор ----------

    [Fact]
    public void Deconstruct_ReturnsValues()
    {
        var (name, number, employees) = Create(7);
        Assert.Equal("Acme", name);
        Assert.Equal("12345678", number);
        Assert.Equal(7, employees);
    }

    // ---------- ToString / Equals / GetHashCode ----------

    [Fact]
    public void ToString_ContainsAllData()
    {
        var text = Create(10, 1234.5m).ToString();
        Assert.Contains("Acme", text);
        Assert.Contains("12345678", text);
        Assert.Contains("IT", text);
        Assert.Contains("2020", text);
        Assert.Contains("10", text);
        Assert.Contains("1,234.50", text);
    }

    [Fact]
    public void Equals_SameRegistrationNumber_True()
    {
        var a = new Models.Company("A", "12345678", "IT", 2020);
        var b = new Models.Company("B", "12345678", "Finance", 2000);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentRegistrationNumber_False()
    {
        var a = new Models.Company("A", "12345678", "IT", 2020);
        var b = new Models.Company("A", "87654321", "IT", 2020);
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_Null_False()
    {
        Assert.False(Create().Equals(null));
        Assert.False(Create().Equals((object?)null));
    }

    [Fact]
    public void Equals_OtherType_False()
        => Assert.False(Create().Equals("not a company"));
}