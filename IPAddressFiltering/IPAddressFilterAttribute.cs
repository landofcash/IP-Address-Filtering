using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace IPAddressFiltering{
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