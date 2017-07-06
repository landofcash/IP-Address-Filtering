using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace IPAddressFiltering
{
    /// <summary>
    /// MVC
    /// </summary>
    public class IPAddressFilterAttribute : AuthorizeAttribute
    {
        protected readonly IPAddressFilterCore _core = new IPAddressFilterCore();

        #region CONSTRUCTORS
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
            _core.IPAddresses = ipAddresses;
            _core.FilteringType = filteringType;
        }

        public IPAddressFilterAttribute(string ipAddressRangeStart, string ipAddressRangeEnd, IPAddressFilteringAction filteringType)
            : this(new[] { new IPAddressRange(ipAddressRangeStart, ipAddressRangeEnd) }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IPAddressRange ipAddressRange, IPAddressFilteringAction filteringType)
            : this(new[] { ipAddressRange }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<IPAddressRange> ipAddressRanges, IPAddressFilteringAction filteringType)
        {
            _core.IPAddressRanges = ipAddressRanges;
            _core.FilteringType = filteringType;
        }
        #endregion

        protected override bool AuthorizeCore(HttpContextBase context)
        {
            string ipAddressString = context.Request.UserHostName;
            return IsIPAddressAllowed(ipAddressString);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var result = new HttpIPAddressRejectedResult($"IP {filterContext.HttpContext.Request.UserHostName} is restricted.");
            filterContext.Result = result;
        }

        protected virtual bool IsIPAddressAllowed(string ipAddressString)
        {
            return _core.IsIPAddressAllowed(ipAddressString);
        }
    }
}