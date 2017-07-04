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
            IPAddressRoleFilterAttribute attribute = new IPAddressRoleFilterAttribute("admin, user, local");
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

            //local role
            rolesWithIpList = new Dictionary<string, List<string>>(){};
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "127.0.0.1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "::1"));
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "192.168.0.1"));
        }

        [TestMethod]
        public void TestRolesAnyIP()
        {
            IPAddressRoleFilterAttribute attribute = new IPAddressRoleFilterAttribute("internet");
            var rolesWithIpList = new Dictionary<string, List<string>>()
            {
                { "internet", new List<string>() { "0.0.0.0" }},
            };
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "127.0.0.1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "::1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "192.168.0.1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "8.8.8.8"));
        }

        [TestMethod]
        public void TestRolesAnyRoleIP()
        {
            IPAddressRoleFilterAttribute attribute = new IPAddressRoleFilterAttribute("any");
            var rolesWithIpList = new Dictionary<string, List<string>>(){};
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "127.0.0.1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "::1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "192.168.0.1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "8.8.8.8"));
        }

        [TestMethod]
        public void TestRolesGlobalRole()
        {
            IPAddressRoleFilterAttribute attribute = new IPAddressRoleFilterAttribute("nothing");
            var rolesWithIpList = new Dictionary<string, List<string>>()
            {
                { RolesContainer.GLOBAL_ROLE,new List<string>() { "8.8.8.8" } }
                
            };
            RolesContainer.UpdateRolesList(rolesWithIpList);
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "127.0.0.1"));
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "::1"));
            Assert.AreEqual<bool>(false, Common.IsIPAddressAllowed(attribute, "192.168.0.1"));
            Assert.AreEqual<bool>(true, Common.IsIPAddressAllowed(attribute, "8.8.8.8"));
        }
    }
}