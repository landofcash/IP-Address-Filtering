using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;

namespace IPAddressFiltering
{
    public class IPAddressFilterCore
    {
        //prob can use IPAddress constants here
        private static readonly IPAddress _emptyIP = IPAddress.Parse("255.255.255.255"); //broadcast address is never used so it is safe to use here
        private static readonly IPAddress _anyIP = IPAddress.Parse("0.0.0.0"); //any ip

        public IPAddressFilteringAction FilteringType { get; set; }
        public IEnumerable<IPAddress> IPAddresses { get; set; }
        public IEnumerable<IPAddressRange> IPAddressRanges { get; set; }
        public List<string> IPRoles { get; set; } = new List<string>();

        public virtual bool IsIPAddressAllowed(string ipAddressString)
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);
            if (FilteringType == IPAddressFilteringAction.Allow)
            {
                if (IPAddresses != null && IPAddresses.Any() && IsIPAddressInList(ipAddress))
                {
                    return true;
                }
                if (IPAddressRanges != null && IPAddressRanges.Any() && IPAddressRanges.Any(r => ipAddress.IsInRange(r.StartIPAddress, r.EndIPAddress)))
                {
                    return true;
                }
                return false;
            }
            if (FilteringType == IPAddressFilteringAction.Restrict)
            {
                if (IPAddresses != null && IPAddresses.Any() && IsIPAddressInList(ipAddress))
                {
                    return false;
                }
                if (IPAddressRanges != null && IPAddressRanges.Any() && IPAddressRanges.Any(r => ipAddress.IsInRange(r.StartIPAddress, r.EndIPAddress)))
                {
                    return false;
                }
                return true;
            }
            throw new ArgumentException("FilteringType is unknown.");
        }
        /// <summary>
        /// Updates IPAddress list, returns true when allowed for any IP
        /// contains any role or any IP
        /// </summary>
        /// <returns></returns>
        public bool UpdateIPAddressesFromRoles()
        {
            //single IP addresses
            var ipaddressesTemp = new List<IPAddress>();
            var ipaddressesRangeTemp = new List<IPAddressRange>();
            ipaddressesTemp.Add(_emptyIP); //HACK need to add something to the list , something that is never used
            ipaddressesTemp.AddRange(RolesContainer.GetRoleIPs(RolesContainer.GLOBAL_ROLE)); // add global role IPs
            foreach (string role in IPRoles) //add IPs for each role for this attribute
            {
                if (role == RolesContainer.ANY_ROLE)
                {
                    return true;
                }
                ipaddressesTemp.AddRange(RolesContainer.GetRoleIPs(role));
                ipaddressesRangeTemp.AddRange(RolesContainer.GetRoleIPRanges(role));
            }
            if (ipaddressesTemp.Contains(_anyIP))
            {
                return true;
            }
            IPAddresses = ipaddressesTemp;
            IPAddressRanges = ipaddressesRangeTemp;

            return false;
            //IP address ranges
        }

        private bool IsIPAddressInList(IPAddress ipAddress)
        {
            return IPAddresses.Any(x => x.Equals(ipAddress));
        }
    }
}