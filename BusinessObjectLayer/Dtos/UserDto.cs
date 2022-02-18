using BusinessObjectLayer.Enums;
using System.Collections.Generic;

namespace BusinessObjectLayer.Dtos
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public RoleType RoleType { get; set; }
        public List<string> Departments { get; set; }
        public List<string> Groups { get; set; }
        public string LeaderUsername { get; set; }
        public string LeaderName { get; set; }
    }
}
