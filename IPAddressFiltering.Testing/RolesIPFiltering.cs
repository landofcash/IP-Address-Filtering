using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IPAddressFiltering.Testing
{
    [TestClass]
    public class RolesIPFiltering
    {
        [TestMethod]
        public void TestRolesIP()
        {
            //no roles
            IPAddressRoleFilterAttribute attribute = new IPAddressRoleFilterAttribute("admin, user", IPAddressFilteringAction.Allow);
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "192.168.10.100"));
            //add roles
            var rolesWithIpList = new Dictionary<string, List<string>>()
            {
                { "admin", new List<string>() {"192.168.10.100"}}
            };
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "192.168.10.100"));
            //same address in 2 roles
            rolesWithIpList = new Dictionary<string, List<string>>()
            {
                { "admin", new List<string>() {"192.168.10.100"}},
                { "user", new List<string>() {"192.168.10.100"}}
            };
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "192.168.10.100"));
            //wrong role
            rolesWithIpList = new Dictionary<string, List<string>>()
            {
                { "master", new List<string>() {"192.168.10.100"}},
            };
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "192.168.10.100"));
            //Many IPs
            rolesWithIpList = new Dictionary<string, List<string>>()
            {
                { "user", new List<string>() { "192.168.10.99", "192.168.10.100","192.168.10.101" }},
            };
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "192.168.10.100"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "192.168.10.101"));
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "192.168.10.102"));
        }
    }
}