using BusinessObjectLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Validators
{
    public class TaskValidator : IValidator<TaskEntity>
    {
        public void Validate(TaskEntity task)
        {
            StringBuilder errors = new StringBuilder();

            if (task.Name == "" || task.Name.Length == 0)
            {
                errors.Append("Invalid name!\n");
            }
            if (task.DeliveryId == 0)
            {
                errors.Append("Invalid delivery!\n");
            }
            if (DateTime.Compare(task.StartDate, DateTime.Now) <= 0)
            {
                errors.Append("Invalid start date!\n");
            }
            if (DateTime.Compare(task.PlannedDate, DateTime.Now) <= 0)
            {
                errors.Append("Invalid planned date!\n");
            }
            if (task.Subteam == "" || task.Subteam.Length == 0)
            {
                errors.Append("Invalid subteam!\n");
            }
            if (task.Duration < 1)
            {
                errors.Append("Invalid duration!\n");
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors.ToString());
            }
        }
    }
}
