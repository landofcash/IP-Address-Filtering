using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;

namespace IPAddressFiltering
{
    /// <summary>
    /// Roles linked to IPs attribute 
    /// </summary>
    public class IPAddressRoleFilterAttribute : IPAddressFilterAttribute
    {
        private static readonly IPAddress _emptyIP = IPAddress.Parse("255.255.255.255"); //broadcast address is never used so it is safe to use here
        public List<string> IPRoles { get; set; } = new List<string>();

        /// <summary>
        /// List of roles to get IPs from for this rule
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="filteringType"></param>
        public IPAddressRoleFilterAttribute(string roles, IPAddressFilteringAction filteringType)
        {
            IPAddressRanges = new IPAddressRange[] { };
            FilteringType = filteringType;
            //set roles list
            IPRoles.AddRange(roles.Split(new [] {',',';'},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()));
            
        }
        protected override bool IsAuthorized(HttpActionContext context)
        {
            //validate IP
            string ipAddressString = ((HttpContextWrapper)context.Request.Properties["MS_HttpContext"]).Request.UserHostName;
            return IsIPAddressAllowed(ipAddressString);
        }

        protected override bool IsIPAddressAllowed(string ipAddressString)
        {
            var ipaddressesTemp = new List<IPAddress>();
            ipaddressesTemp.Add(_emptyIP); //need to add something to the list , something that is never used
            foreach (string role in IPRoles)
            {
                ipaddressesTemp.AddRange(RolesContainer.GetRoleIPs(role));
            }
            IPAddresses = ipaddressesTemp;
            return base.IsIPAddressAllowed(ipAddressString);
        }
    }
}