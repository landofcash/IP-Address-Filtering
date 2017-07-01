using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace IPAddressFiltering
{
    /// <summary>
    /// Roles linked to IPs attribute 
    /// </summary>
    public class IPAddressRoleFilterAttribute : IPAddressFilterAttribute
    {
        //prob can use IPAddress constants here
        private static readonly IPAddress _emptyIP = IPAddress.Parse("255.255.255.255"); //broadcast address is never used so it is safe to use here
        private static readonly IPAddress _anyIP = IPAddress.Parse("0.0.0.0"); //any ip

        public List<string> IPRoles { get; set; } = new List<string>();

        /// <summary>
        /// List of roles to get IPs from for this rule
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="filteringType"></param>
        public IPAddressRoleFilterAttribute(string roles, IPAddressFilteringAction filteringType = IPAddressFilteringAction.Allow)
        {
            IPAddressRanges = new IPAddressRange[] { };
            FilteringType = filteringType;
            //set roles list
            IPRoles.AddRange(roles.Split(new [] {',',';'},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()));
            
        }

        protected override bool AuthorizeCore(HttpContextBase context)
        {
            //validate IP
            string ipAddressString = context.Request.UserHostName;
            return IsIPAddressAllowed(ipAddressString);
        }

        protected override bool IsIPAddressAllowed(string ipAddressString)
        {
            var ipaddressesTemp = new List<IPAddress>();
            ipaddressesTemp.Add(_emptyIP); //HACK need to add something to the list , something that is never used
            foreach (string role in IPRoles)
            {
                if (role== RolesContainer.ANY_ROLE)
                {
                    return true;
                }
                ipaddressesTemp.AddRange(RolesContainer.GetRoleIPs(role));
            }
            if (ipaddressesTemp.Contains(_anyIP))
            {
                return true;
            }
            IPAddresses = ipaddressesTemp;
            return base.IsIPAddressAllowed(ipAddressString);
        }
    }
}