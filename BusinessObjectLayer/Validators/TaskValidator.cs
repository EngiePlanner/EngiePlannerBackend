using BusinessObjectLayer.Dtos;
using System;
using System.Text;

namespace BusinessObjectLayer.Validators
{
    public class TaskValidator : IValidator<TaskDto>
    {
        public void Validate(TaskDto task)
        {
            StringBuilder errors = new StringBuilder();

            if (task.Name == "" || task.Name.Length == 0)
            {
                errors.Append("Invalid name!\n");
            }
            if (DateTime.Compare(task.AvailabilityDate.Date, DateTime.Now.Date) < 0)
            {
                errors.Append("Invalid availability date!\n");
            }
            if (DateTime.Compare(task.PlannedDate.Date, task.AvailabilityDate.Date) < 0)
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
            if (task.ResponsibleUsername == "")
            {
                errors.Append("Invalid associate!\n");
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors.ToString());
            }
        }

        public void ValidateCustom(TaskDto task)
        {
            StringBuilder errors = new StringBuilder();

            if (DateTime.Compare((DateTime)task.StartDate, task.AvailabilityDate) < 0)
            {
                errors.Append("Invalid start date!\n");
            }
            if (DateTime.Compare((DateTime)task.StartDate, (DateTime)task.EndDate) > 0)
            {
                errors.Append("Start date is after end date!\n");
            }
            if (DateTime.Compare((DateTime)task.EndDate, task.PlannedDate) > 0)
            {
                errors.Append("Invalid end date!\n");
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors.ToString());
            }
        }
    }
}
