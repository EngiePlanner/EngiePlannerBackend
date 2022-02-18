using BusinessObjectLayer.Helpers;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace EngiePlanner
{
    public static class LdapWrapper
    {
        private static DirectoryEntry SearchRoot =
            new DirectoryEntry("GC://DC=bosch,DC=com")
            {
                AuthenticationType = AuthenticationTypes.Secure,
                Username = Constants.NTUser,
                Password = Constants.NTPassword
            };

        public static string GetDisplayNameByUserName(string userName)
            => GetUserProperty(userName, "SAMAccountName", "DisplayName");

        public static string GetUserNameByDisplayName(string displayName)
            => GetUserProperty(displayName, "DisplayName", "SAMAccountName");

        public static string GetEmailAddressByUserName(string userName)
            => GetUserProperty(userName, "SAMAccountName", "mail");

        public static string GetGroupByUserName(string userName)
            => GetUserProperty(userName, "SAMAccountName", "department");
        public static string GetUserNameByEmailAddress(string email)
            => GetUserProperty(email, "mail", "SAMAccountName");
        public static string GetFirstNameByUsername(string userName)
                    => GetUserProperty(userName, "SAMAccountName", "givenName");
        public static string GetLastNameByUsername(string userName)
            => GetUserProperty(userName, "SAMAccountName", "sn");

        public static string GetGroupLeaderUserName(string groupTitle)
        {
            if (groupTitle.Equals("Contracted Teams", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var filter = $"(&(displayName={groupTitle})(objectClass=group)(mailNickname=wom.*))";
            var propertiesToLoad = new string[] { "cn", "member" };
            var results = FindOneByFilterAndPropertiesToLoad(filter, propertiesToLoad);
            ResultPropertyValueCollection membersResult;

            if (results != null && (membersResult = results.Properties["member"]).Count == 1)
            {
                var memberInfo = membersResult[0].ToString().Split(',');
                foreach (var info in memberInfo)
                {
                    var pair = info.Split('=');
                    if (pair[0].Equals("cn", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return pair[1];
                    }
                }
            }

            return null;
        }

        public static List<string> GetDepartmentByUsername(string username)
        {
            var search = new DirectorySearcher(SearchRoot, $"(sAMAccountName={username})", new[] { "memberOf" });

            var result = search.FindOne();
            var memberships = result.Properties["memberof"];

            var groups = new List<string>();
            foreach (var membership in memberships)
            {
                groups.Add(membership.ToString());
            }

            var woms = groups
                .Where(member => member.StartsWith("CN=WOM.", StringComparison.InvariantCultureIgnoreCase) ||
                                   member.StartsWith("CN=HWOM.", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            groups = new List<string>();
            foreach (var wom in woms)
            {
                search = new DirectorySearcher(SearchRoot, $"(distinguishedName={wom})", new[] { "memberOf" });
                result = search.FindOne();

                foreach (var member in result.Properties["memberOf"])
                {
                    groups.Add(member.ToString());
                }
            }

            var departments = groups
                .Where(member => member.StartsWith("CN=WOM.", StringComparison.InvariantCultureIgnoreCase) ||
                                  member.StartsWith("CN=HWOM.", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Split(".")[1].Split(',')[0])
                .Select(x => x.Substring(0, x.LastIndexOf('-')).Replace('_', '/'))
                .Distinct()
                .ToList();


            return departments;
        }


        private static string GetUserProperty(string searchFilter, string searchProperty, string resultProperty)
        {
            var filter = $"({searchProperty}={searchFilter})";
            var propertiesToLoad = new string[] { resultProperty };
            var userProperties = FindOneByFilterAndPropertiesToLoad(filter, propertiesToLoad)?.Properties[resultProperty];

            return (userProperties?.Count ?? 0) > 0
                ? userProperties[0]?.ToString()
                : null;
        }

        private static SearchResult FindOneByFilterAndPropertiesToLoad(string filter, string[] propertiesToLoad)
        {
            var search = new DirectorySearcher(SearchRoot, filter, propertiesToLoad);

            try
            {
                return search.FindOne();
            }
            catch
            {
                return null;
            }
        }
    }
}
