using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IPAddressFiltering.Testing
{
    [TestClass]
    public class ConfigHelper
    {
        [TestMethod]
        public void ParseRolesWithIpTest()
        {
            string rawConfig = @"servers: 8.8.8.8, 4.4.4.4
employees: 192.168.0.1
clients: 192.168.0.5
";
            var rolesWithIps = IPAddressFiltering.ConfigHelper.ParseRolesWithIpFromTxt(rawConfig);
            Assert.AreEqual<int>(3, rolesWithIps.Count);
            Assert.AreEqual<int>(2, rolesWithIps["servers"].Count);
            Assert.AreEqual<int>(1, rolesWithIps["employees"].Count);
            Assert.AreEqual<int>(1, rolesWithIps["clients"].Count);
            StringAssert.Contains("8.8.8.8", rolesWithIps["servers"][0]);
            StringAssert.Contains("4.4.4.4", rolesWithIps["servers"][1]);
            StringAssert.Contains("192.168.0.1", rolesWithIps["employees"][0]);
            StringAssert.Contains("192.168.0.5", rolesWithIps["clients"][0]);
        }
        [TestMethod]
        public void DumpRolesWithIpTest()
        {
            Dictionary<string, List<string>> rolesWithIp = new Dictionary<string, List<string>>()
            {
                { "servers", new List<string> { "8.8.8.8", "4.4.4.4" }},
                { "employees", new List<string> {"192.168.0.1" }},
                { "clients", new List<string> {"192.168.0.5" }}
            };
            string roles = IPAddressFiltering.ConfigHelper.DumpRolesWithIpToTxt(rolesWithIp);
            Console.WriteLine(roles);
            StringAssert.Contains(roles,"4.4.4.4" );
            StringAssert.Contains(roles, "8.8.8.8");
            StringAssert.Contains(roles, "192.168.0.1");
            StringAssert.Contains(roles, "192.168.0.5");
            StringAssert.Contains(roles, "192.168.0.5");
            StringAssert.Contains(roles, "192.168.0.5");
            StringAssert.Contains(roles, "192.168.0.5");
        }
    }
}