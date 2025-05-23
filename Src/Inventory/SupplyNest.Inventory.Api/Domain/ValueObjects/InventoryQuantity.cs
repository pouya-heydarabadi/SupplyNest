using System;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyNest.Inventory.Api.Domain.Exceptions;
using SupplyNest.Inventory.Api.Domain.Exceptions.Base;

namespace SupplyNest.Inventory.Api.Domain.ValueObjects
{
    public record InventoryQuantity
    {
        public int Value { get; init; }


        private InventoryQuantity(int value)
        {
            Value = value;
        }
    
        public static InventoryQuantity FromInt(int value) =>
            new(value);

        public static InventoryQuantity operator +(InventoryQuantity a, InventoryQuantity b) =>
            new(a.Value + b.Value);

        public static InventoryQuantity operator -(InventoryQuantity a, InventoryQuantity b) =>
            new(a.Value - b.Value);

        public static implicit operator int(InventoryQuantity quantity) => quantity.Value;
    }
} 