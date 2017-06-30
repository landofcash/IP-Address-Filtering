using System;
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

    [TestClass]
    public class SingleIPAddressRangeFiltering
    {
        [TestMethod]
        public void TestSingleIPRestrictMatch()
        {
            Assert.AreEqual<bool>(false, CheckIPAddress("94.201.252.25", IPAddressFilteringAction.Restrict));
        }

        [TestMethod]
        public void TestSingleIPRestrictNoMatch()
        {
            Assert.AreEqual<bool>(true, CheckIPAddress("94.201.252.100", IPAddressFilteringAction.Restrict));
        }

        [TestMethod]
        public void TestSingleIPAllowMatch()
        {
            Assert.AreEqual<bool>(true, CheckIPAddress("94.201.252.25", IPAddressFilteringAction.Allow));
        }

        [TestMethod]
        public void TestSingleIPAllowNoMatch()
        {
            Assert.AreEqual<bool>(false, CheckIPAddress("94.201.252.100", IPAddressFilteringAction.Allow));
        }

        private bool CheckIPAddress(string requestIP, IPAddressFilteringAction action)
        {

            IPAddressFilterAttribute attribute = new IPAddressFilterAttribute(new IPAddressRange("94.201.252.5", "94.201.252.90"), action);
            return Common.IsIPAddressAllowed(attribute, requestIP);

        }
    }
}
