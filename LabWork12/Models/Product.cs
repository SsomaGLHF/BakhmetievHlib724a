using System.Text.Json.Serialization;

namespace LabWork12.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Зв'язок 1 до багатьох
    public int CategoryId { get; set; }
    [JsonIgnore]
    public Category? Category { get; set; }
}
