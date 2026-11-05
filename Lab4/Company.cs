using System.Globalization;

namespace Lab5.Models;

/// <summary>
/// Модель предметної області «Компанія».
/// </summary>
public class Company : IEquatable<Company>
{
    public const int MinFoundationYear = 1800;
    public const int RegistrationNumberLength = 8;

    private string _name = string.Empty;
    private string _industry = string.Empty;
    private int _foundationYear;
    private int _employeesCount;
    private decimal _revenue;

    /// <summary>Реєстраційний номер (код ЄДРПОУ, 8 цифр). Незмінний після створення.</summary>
    public string RegistrationNumber { get; }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва компанії не може бути порожньою.", nameof(value));
            _name = value.Trim();
        }
    }

    public string Industry
    {
        get => _industry;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Галузь не може бути порожньою.", nameof(value));
            _industry = value.Trim();
        }
    }

    public int FoundationYear
    {
        get => _foundationYear;
        set
        {
            if (value < MinFoundationYear || value > DateTime.Now.Year)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Рік заснування має бути в межах {MinFoundationYear}–{DateTime.Now.Year}.");
            _foundationYear = value;
        }
    }

    public int EmployeesCount
    {
        get => _employeesCount;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Кількість працівників не може бути від'ємною.");
            _employeesCount = value;
        }
    }

    public decimal Revenue
    {
        get => _revenue;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Дохід не може бути від'ємним.");
            _revenue = value;
        }
    }

    // ---------- Конструктори ----------

    /// <summary>Компанія за замовчуванням.</summary>
    public Company() : this("Unknown", "00000000", "Unknown", DateTime.Now.Year)
    {
    }

    public Company(string name, string registrationNumber, string industry, int foundationYear)
        : this(name, registrationNumber, industry, foundationYear, 0, 0m)
    {
    }

    public Company(string name, string registrationNumber, string industry, int foundationYear,
                   int employeesCount, decimal revenue)
    {
        ValidateRegistrationNumber(registrationNumber);
        RegistrationNumber = registrationNumber;

        Name = name;
        Industry = industry;
        FoundationYear = foundationYear;
        EmployeesCount = employeesCount;
        Revenue = revenue;
    }

    // ---------- Деконструктор ----------

    public void Deconstruct(out string name, out string registrationNumber, out int employeesCount)
    {
        name = Name;
        registrationNumber = RegistrationNumber;
        employeesCount = EmployeesCount;
    }

    // ---------- Методи ----------

    /// <summary>Наймає вказану кількість працівників.</summary>
    public void Hire(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість має бути додатною.");
        EmployeesCount += count;
    }

    /// <summary>Звільняє вказану кількість працівників.</summary>
    public void Fire(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість має бути додатною.");
        if (count > EmployeesCount)
            throw new InvalidOperationException("Не можна звільнити більше працівників, ніж є в компанії.");
        EmployeesCount -= count;
    }

    /// <summary>Додає суму до доходу компанії.</summary>
    public void AddRevenue(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума має бути додатною.");
        Revenue += amount;
    }

    /// <summary>Вік компанії в роках. Поточний рік можна передати для тестування.</summary>
    public int GetAge(int? currentYear = null)
    {
        int year = currentYear ?? DateTime.Now.Year;
        if (year < FoundationYear)
            throw new ArgumentOutOfRangeException(nameof(currentYear), "Поточний рік не може бути раніше за рік заснування.");
        return year - FoundationYear;
    }

    /// <summary>Дохід на одного працівника (0, якщо працівників немає).</summary>
    public decimal GetRevenuePerEmployee()
        => EmployeesCount == 0 ? 0m : Revenue / EmployeesCount;

    /// <summary>Стартап: вік не більше 5 років і не більше 50 працівників.</summary>
    public bool IsStartup(int? currentYear = null)
        => GetAge(currentYear) <= 5 && EmployeesCount <= 50;

    // ---------- Перевизначення ----------

    public override string ToString()
        => $"{Name} (код {RegistrationNumber}), галузь: {Industry}, засн. {FoundationYear}, " +
           $"працівників: {EmployeesCount}, дохід: {Revenue.ToString("N2", CultureInfo.InvariantCulture)}";

    /// <summary>Дві компанії рівні, якщо збігаються реєстраційні номери.</summary>
    public bool Equals(Company? other)
        => other is not null && RegistrationNumber == other.RegistrationNumber;

    public override bool Equals(object? obj) => Equals(obj as Company);

    public override int GetHashCode() => RegistrationNumber.GetHashCode();

    // ---------- Допоміжні ----------

    private static void ValidateRegistrationNumber(string? number)
    {
        if (number is null || number.Length != RegistrationNumberLength || !number.All(char.IsAsciiDigit))
            throw new ArgumentException(
                $"Реєстраційний номер має складатися рівно з {RegistrationNumberLength} цифр.",
                nameof(number));
    }
}