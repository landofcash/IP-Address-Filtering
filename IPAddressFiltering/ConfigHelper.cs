using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressFiltering
{
    public static class ConfigHelper
    {
        /// <summary>
        /// parses roles with ips config txt file
        /// </summary>
        /// <param name="rawConfig"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ParseRolesWithIpFromTxt(string rawConfig)
        {
            var rolesWithIpList = new Dictionary<string, List<string>>();
            //split rows
            string[] rawConfigRows = rawConfig.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //process rows
            foreach (string row in rawConfigRows)
            {
                //split role:IP list expected  
                //"roleName: 8.8.8.8, 4.4.4.4, 192.168.0.1"
                if (row.Contains(":"))
                {
                    string role = row.Split(':')[0].Trim(); //get role name
                    //IPs list
                    List<string> ips = row.Split(':')[1].Trim().Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                    if (!rolesWithIpList.ContainsKey(role))
                    {
                        rolesWithIpList.Add(role, ips);
                    }
                    else
                    {
                        rolesWithIpList[role].AddRange(ips);
                    }
                }
            }
            return rolesWithIpList;
        }
        /// <summary>
        /// dumps roles and IPs into a config txt file
        /// </summary>
        /// <param name="rolesWithIps"></param>
        /// <returns></returns>
        public static string DumpRolesWithIpToTxt(Dictionary<string, List<string>> rolesWithIps)
        {
            string result = "";
            foreach (var key in rolesWithIps.Keys)
            {
                result += key + ": " + string.Join(", ", rolesWithIps[key]) + "\r\n";
            }
            return result;
        }
    }
}
