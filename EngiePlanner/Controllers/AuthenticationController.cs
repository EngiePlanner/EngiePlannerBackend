using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Enums;
using BusinessObjectLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EngiePlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private string currentUsername;
        private readonly IUserService userService;

        public AuthenticationController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
            currentUsername = this.httpContextAccessor.HttpContext.User.Identity.Name[5..];
        }

        [DisableCors]
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login()
        {
            if (currentUsername == "OP-OHBS1P5\\Cristi")
            {
                this.currentUsername = "ROC2CLJ";
            }

            object token;
            UserDto loggedUser;
            var user = await userService.GetUserByUsernameAsync(currentUsername);
            if (user != null)
            {
                token = await GenerateJwtToken(user);
                return Ok(new { token });
            }

            var group = LdapWrapper.GetGroupByUserName(currentUsername);
            var groupLeaderUsername = LdapWrapper.GetGroupLeaderUserName(group.Split(" ")[0]);
            var departments = LdapWrapper.GetDepartmentByUsername(currentUsername);
            var departmentHeadUsername = LdapWrapper.GetGroupLeaderUserName(departments.First());
            var groupLeader = await userService.GetUserByUsernameAsync(groupLeaderUsername);
            var departmentHead = await userService.GetUserByUsernameAsync(departmentHeadUsername);
            var groups = group.Split(" ").ToList();
            RoleType role;

            if (currentUsername == departmentHeadUsername)
            {
                role = RoleType.Leader;
                var groupsWithoutDepartment = LdapWrapper.GetGroupByUserName(departmentHeadUsername)
                    .Split(" ")
                    .Where(x => !departments.Contains(x))
                    .ToList();

                loggedUser = new UserDto
                {
                    Username = currentUsername,
                    Name = LdapWrapper.GetFirstNameByUsername(currentUsername) + " " + LdapWrapper.GetLastNameByUsername(currentUsername),
                    DisplayName = LdapWrapper.GetDisplayNameByUserName(currentUsername),
                    Email = LdapWrapper.GetEmailAddressByUserName(currentUsername),
                    RoleType = role,
                    Groups = groupsWithoutDepartment,
                    Departments = departments,
                    LeaderUsername = Constants.NEUsername,
                };


                await userService.CreateUserAsync(loggedUser);
                loggedUser.LeaderUsername = null;

                token = await GenerateJwtToken(loggedUser);
                return Ok(new { token });
            }

            if (currentUsername == groupLeaderUsername)
            {
                role = RoleType.Leader;

                if (departmentHead == null)
                {
                    var departmentHeadDepartments = LdapWrapper.GetDepartmentByUsername(departmentHeadUsername);
                    var groupsWithoutDepartment = LdapWrapper.GetGroupByUserName(departmentHeadUsername)
                        .Split(" ")
                        .Where(x => !departmentHeadDepartments.Contains(x))
                        .ToList();

                    departmentHead = new UserDto
                    {
                        Username = departmentHeadUsername,
                        Name = LdapWrapper.GetFirstNameByUsername(departmentHeadUsername) + " " + LdapWrapper.GetLastNameByUsername(departmentHeadUsername),
                        DisplayName = LdapWrapper.GetDisplayNameByUserName(departmentHeadUsername),
                        Email = LdapWrapper.GetEmailAddressByUserName(departmentHeadUsername),
                        RoleType = RoleType.Leader,
                        Groups = groupsWithoutDepartment,
                        Departments = departmentHeadDepartments,
                        LeaderUsername = Constants.NEUsername,
                    };
                    await userService.CreateUserAsync(departmentHead);
                }

                loggedUser = new UserDto
                {
                    Username = currentUsername,
                    Name = LdapWrapper.GetFirstNameByUsername(currentUsername) + " " + LdapWrapper.GetLastNameByUsername(currentUsername),
                    DisplayName = LdapWrapper.GetDisplayNameByUserName(currentUsername),
                    Email = LdapWrapper.GetEmailAddressByUserName(currentUsername),
                    RoleType = role,
                    Groups = groups,
                    Departments = departments,
                    LeaderUsername = departmentHead.Username,
                };

                await userService.CreateUserAsync(loggedUser);
                loggedUser.LeaderUsername = null;

                token = await GenerateJwtToken(loggedUser);
                return Ok(new { token });
            }

            if (departmentHead == null)
            {
                var departmentHeadDepartments = LdapWrapper.GetDepartmentByUsername(departmentHeadUsername);
                var groupsWithoutDepartment = LdapWrapper.GetGroupByUserName(departmentHeadUsername)
                    .Split(" ")
                    .Where(x => !departmentHeadDepartments.Contains(x))
                    .ToList();

                departmentHead = new UserDto
                {
                    Username = departmentHeadUsername,
                    Name = LdapWrapper.GetFirstNameByUsername(departmentHeadUsername) + " " + LdapWrapper.GetLastNameByUsername(departmentHeadUsername),
                    DisplayName = LdapWrapper.GetDisplayNameByUserName(departmentHeadUsername),
                    Email = LdapWrapper.GetEmailAddressByUserName(departmentHeadUsername),
                    RoleType = RoleType.Leader,
                    Groups = groupsWithoutDepartment,
                    Departments = departmentHeadDepartments,
                    LeaderUsername = Constants.NEUsername,
                    LeaderName = Constants.NEName
                };
                await userService.CreateUserAsync(departmentHead);
            }

            if (groupLeader == null)
            {
                groupLeader = new UserDto
                {
                    Username = groupLeaderUsername,
                    Name = LdapWrapper.GetFirstNameByUsername(groupLeaderUsername) + " " + LdapWrapper.GetLastNameByUsername(groupLeaderUsername),
                    DisplayName = LdapWrapper.GetDisplayNameByUserName(groupLeaderUsername),
                    Email = LdapWrapper.GetEmailAddressByUserName(groupLeaderUsername),
                    RoleType = RoleType.Leader,
                    Groups = LdapWrapper.GetGroupByUserName(groupLeaderUsername).Split(" ").ToList(),
                    Departments = LdapWrapper.GetDepartmentByUsername(groupLeaderUsername),
                    LeaderUsername = departmentHead.Username,
                    LeaderName = departmentHead.DisplayName
                };
                await userService.CreateUserAsync(groupLeader);
            }

            loggedUser = new UserDto
            {
                Username = currentUsername,
                Name = LdapWrapper.GetFirstNameByUsername(currentUsername) + " " + LdapWrapper.GetLastNameByUsername(currentUsername),
                DisplayName = LdapWrapper.GetDisplayNameByUserName(currentUsername),
                Email = LdapWrapper.GetEmailAddressByUserName(currentUsername),
                RoleType = RoleType.Associate,
                Groups = groups,
                Departments = departments,
                LeaderUsername = groupLeader.Username,
                LeaderName = groupLeader.Name,
            };

            await userService.CreateUserAsync(loggedUser);

            token = await GenerateJwtToken(loggedUser);
            return Ok(new { token });
        }

        private async Task<object> GenerateJwtToken(UserDto user)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim("Username", user.Username),
                new Claim("Name", user.Name),
                new Claim("DisplayName", user.DisplayName),
                new Claim("Email", user.Email),
                new Claim("Role", user.RoleType.ToString()),
                new Claim("LeaderUsername", user.LeaderUsername),
                new Claim("LeaderName", user.LeaderName),
            };

            claims.AddRange(user.Groups.Select(group => new Claim("Groups", group)));
            claims.AddRange(user.Departments.Select(department => new Claim("Departments", department)));

            ClaimsIdentity identity = new ClaimsIdentity(claims);

            var keyByteArray = Encoding.ASCII.GetBytes(configuration["JwtKey"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JwtExpireDays"]));
            var issuer = configuration["JwtIssuer"];
            var audience = configuration["JwtAudience"];

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials,
                Subject = identity,
                Expires = expires,
                NotBefore = DateTime.Now
            });

            return handler.WriteToken(securityToken);
        }
    }
}
