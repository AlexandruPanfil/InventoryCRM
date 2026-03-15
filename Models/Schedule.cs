using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Models
{
    public class Schedule : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        // Navigation property for related orders
        //public ICollection<Order>? Orders { get; set; }

        // Валидация: EndTime должен быть позже StartTime
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "EndTime must be later than StartTime.",
                    new[] { nameof(EndTime), nameof(StartTime) }
                );
            }
        }
    }
}
