using BusinessObjectLayer.Entities;
using System;
using System.Text;

namespace BusinessObjectLayer.Validators
{
    public class AvailabilityValidator : IValidator<AvailabilityEntity>
    {
        public void Validate(AvailabilityEntity availability)
        {
            StringBuilder errors = new StringBuilder();

            if (availability.UserUsername.Length == 0)
            {
                errors.Append("Invalid username!\n");
            }
            if (availability.AvailableHours < 0 || availability.AvailableHours > 40)
            {
                errors.Append("Invalid number of hours!\n");
            }
            if (availability.FromDate.DayOfWeek != DayOfWeek.Monday)
            {
                errors.Append("Invalid from date!\n");
            }
            if (availability.ToDate != availability.FromDate.AddDays(4))
            {
                errors.Append("Invalid to date!\n");
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors.ToString());
            }
        }
    }
}
