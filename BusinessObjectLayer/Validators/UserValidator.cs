using BusinessObjectLayer.Entities;
using System.Text;

namespace BusinessObjectLayer.Validators
{
    public class UserValidator : IValidator<UserEntity>
    {
        public void Validate(UserEntity user)
        {
            StringBuilder errors = new StringBuilder();

            if (user.Username.Length == 0 || user.Username.Length > 10)
            {
                errors.Append("Invalid username!\n");
            }
            if (user.Name.Length == 0)
            {
                errors.Append("Invalid name!\n");
            }
            if (user.DisplayName.Length == 0)
            {
                errors.Append("Invalid display name!\n");
            }
            if (user.Email.Length == 0)
            {
                errors.Append("Invalid email!\n");
            }
            if (user.LeaderUsername.Length == 0)
            {
                errors.Append("Invalid leader username!\n");
            }
        }

        public void ValidateCustom(UserEntity e)
        {
            throw new System.NotImplementedException();
        }
    }
}
