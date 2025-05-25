using System;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyNest.Inventory.Api.Domain.Exceptions;
using SupplyNest.Inventory.Api.Domain.Exceptions.Base;

namespace SupplyNest.Inventory.Api.Domain.ValueObjects
{
    public record InventoryQuantity
    {
        public long Value { get; init; }


        private InventoryQuantity(long value)
        {
            Value = value;
        }
    
        public static InventoryQuantity FromInt(long value) =>
            new(value);

        public static InventoryQuantity operator +(InventoryQuantity a, InventoryQuantity b) =>
            new(a.Value + b.Value);

        public static InventoryQuantity operator -(InventoryQuantity a, InventoryQuantity b) =>
            new(a.Value - b.Value);

        public static implicit operator long(InventoryQuantity quantity) => quantity.Value;
    }
} 