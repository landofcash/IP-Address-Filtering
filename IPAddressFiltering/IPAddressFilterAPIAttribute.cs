using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace IPAddressFiltering
{
    /// <summary>
    /// WEB API
    /// </summary>
    public class IPAddressFilterAPIAttribute : AuthorizeAttribute
    {
        protected readonly IPAddressFilterCore _core = new IPAddressFilterCore();
        #region CONSTRUCTORS
        public IPAddressFilterAPIAttribute()
        {
        }


        public IPAddressFilterAPIAttribute(string ipAddress, IPAddressFilteringAction filteringType) : this(new[] { IPAddress.Parse(ipAddress) }, filteringType)
        {
        }

        public IPAddressFilterAPIAttribute(IPAddress ipAddress, IPAddressFilteringAction filteringType) : this(new[] { ipAddress }, filteringType)
        {
        }

        public IPAddressFilterAPIAttribute(IEnumerable<string> ipAddresses, IPAddressFilteringAction filteringType) : this(ipAddresses.Select(a => IPAddress.Parse(a)), filteringType)
        {
        }

        public IPAddressFilterAPIAttribute(IEnumerable<IPAddress> ipAddresses, IPAddressFilteringAction filteringType)
        {
            _core.IPAddresses = ipAddresses;
            _core.FilteringType = filteringType;
        }

        public IPAddressFilterAPIAttribute(string ipAddressRangeStart, string ipAddressRangeEnd, IPAddressFilteringAction filteringType)
            : this(new[] { new IPAddressRange(ipAddressRangeStart, ipAddressRangeEnd) }, filteringType)
        {
        }

        public IPAddressFilterAPIAttribute(IPAddressRange ipAddressRange, IPAddressFilteringAction filteringType)
            : this(new[] { ipAddressRange }, filteringType)
        {
        }

        public IPAddressFilterAPIAttribute(IEnumerable<IPAddressRange> ipAddressRanges, IPAddressFilteringAction filteringType)
        {
            _core.IPAddressRanges = ipAddressRanges;
            _core.FilteringType = filteringType;
        }
        #endregion

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext context)
        {
            string ipAddressString = ((HttpContextWrapper)context.Request.Properties["MS_HttpContext"]).Request.UserHostName;

            var challengeMessage = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
            challengeMessage.ReasonPhrase = $"IP {ipAddressString} is restricted.";
            throw new HttpResponseException(challengeMessage);
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext context)
        {
            string ipAddressString = ((HttpContextWrapper)context.Request.Properties["MS_HttpContext"]).Request.UserHostName;
            return IsIPAddressAllowed(ipAddressString);
        }

        protected virtual bool IsIPAddressAllowed(string ipAddressString)
        {
            return _core.IsIPAddressAllowed(ipAddressString);
        }
    }
}