using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Dtos.Schedules
{
    public class ScheduleDto : IValidatableObject
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult("EndTime must be later than StartTime",
                    new[] { nameof(EndTime), nameof(StartTime) });
            }
        }
    }
}