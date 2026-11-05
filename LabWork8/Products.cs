using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab8
{
    interface IProduct
    {
        string Name { get; }
        decimal Price { get; }
        int Quantity { get; }
    }

    class Product : IProduct
    {
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        public Product(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString() =>
            $"{Name} — {Price:F2} грн × {Quantity} = {Price * Quantity:F2} грн";
    }

    class Electronics : IProduct
    {
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public int WarrantyMonths { get; }

        public Electronics(string name, decimal price, int quantity, int warrantyMonths)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"{Name} — {Price:F2} грн × {Quantity} = {Price * Quantity:F2} грн (гарантія {WarrantyMonths} міс.)";
    }
}