using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace IPAddressFiltering{
    public static class RolesContainer{
        private static readonly Dictionary<string, List<IPAddress>> _rolesWithIPList = new Dictionary<string, List<IPAddress>>();
        static readonly object _lock = new object();

        /// <summary>
        /// Updates roles and IPs list 
        /// </summary>
        /// <param name="items">dictionary of Row name + list of IPs</param>
        public static void UpdateRolesList(Dictionary<string, List<string>> items)
        {
            lock (_lock)
            {
                _rolesWithIPList.Clear();
                foreach (var item in items)
                {
                    _rolesWithIPList.Add(item.Key, new List<IPAddress>(item.Value.Select(IPAddress.Parse)));
                }
            }
        }

        public static List<IPAddress> GetRoleIPs(string role)
        {
            lock (_lock)
            {
                if (_rolesWithIPList.ContainsKey(role))
                {
                    return new List<IPAddress>(_rolesWithIPList[role]);
                }
            }
            return new List<IPAddress>();
        }
    }

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

    public class IPAddressFilterAttribute : AuthorizeAttribute
    {
        public IPAddressFilteringAction FilteringType { get; set; }

        public IEnumerable<IPAddress> IPAddresses { get; set; }

        public IEnumerable<IPAddressRange> IPAddressRanges { get; set; }

        public IPAddressFilterAttribute()
        {
        }


        public IPAddressFilterAttribute(string ipAddress, IPAddressFilteringAction filteringType) : this(new[] { IPAddress.Parse(ipAddress) }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IPAddress ipAddress, IPAddressFilteringAction filteringType) : this(new[] { ipAddress }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<string> ipAddresses, IPAddressFilteringAction filteringType) : this(ipAddresses.Select(a => IPAddress.Parse(a)), filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<IPAddress> ipAddresses, IPAddressFilteringAction filteringType)
        {
            IPAddresses = ipAddresses;
            FilteringType = filteringType;
        }

        public IPAddressFilterAttribute(string ipAddressRangeStart, string ipAddressRangeEnd, IPAddressFilteringAction filteringType)
            : this(new[] { new IPAddressRange(ipAddressRangeStart, ipAddressRangeEnd) }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IPAddressRange ipAddressRange, IPAddressFilteringAction filteringType)
            :this(new[] { ipAddressRange }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<IPAddressRange> ipAddressRanges, IPAddressFilteringAction filteringType)
        {
            IPAddressRanges = ipAddressRanges;
            FilteringType = filteringType;
        }

        protected override bool IsAuthorized(HttpActionContext context)
        {
            string ipAddressString = ((HttpContextWrapper)context.Request.Properties["MS_HttpContext"]).Request.UserHostName;
            return IsIPAddressAllowed(ipAddressString);
        }

        protected virtual bool IsIPAddressAllowed(string ipAddressString)
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);

            if (FilteringType == IPAddressFilteringAction.Allow)
            {
                if (IPAddresses != null && IPAddresses.Any() &&
                    !IsIPAddressInList(ipAddressString.Trim()))
                {
                    return false;
                }
                if (IPAddressRanges != null && IPAddressRanges.Any() &&
                    !IPAddressRanges.Any(r => ipAddress.IsInRange(r.StartIPAddress, r.EndIPAddress)))
                {
                    return false;
                }
            }
            else
            {
                if (IPAddresses != null && IPAddresses.Any() &&
                   IsIPAddressInList(ipAddressString.Trim()))
                {
                    return false;
                }
                if (IPAddressRanges != null && IPAddressRanges.Any() &&
                    IPAddressRanges.Any(r => ipAddress.IsInRange(r.StartIPAddress, r.EndIPAddress)))
                {
                    return false;
                }
            }

            return true;

        }

        private bool IsIPAddressInList(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                IEnumerable<string> addresses = IPAddresses.Select(a => a.ToString());
                return addresses.Any(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase));
            }
            return false;
        }
    }
}