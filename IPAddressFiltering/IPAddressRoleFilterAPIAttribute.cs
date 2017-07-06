using System;
using System.Linq;
using System.Web;

namespace IPAddressFiltering
{
    /// <summary>
    /// WEB API Roles linked to IPs attribute 
    /// </summary>
    public class IPAddressRoleFilterAPIAttribute : IPAddressFilterAPIAttribute
    {
        /// <summary>
        /// List of roles to get IPs from for this rule
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="filteringType"></param>
        public IPAddressRoleFilterAPIAttribute(string roles, IPAddressFilteringAction filteringType = IPAddressFilteringAction.Allow)
        {
            _core.IPAddressRanges = new IPAddressRange[] { };
            _core.FilteringType = filteringType;
            //set roles list
            _core.IPRoles.AddRange(roles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext context)
        {
            //validate IP
            string ipAddressString = ((HttpContextWrapper)context.Request.Properties["MS_HttpContext"]).Request.UserHostName;
            return IsIPAddressAllowed(ipAddressString);
        }
        protected override bool IsIPAddressAllowed(string ipAddressString)
        {
            bool isValid = _core.UpdateIPAddressesFromRoles(); //update IP from roles
            if (isValid)
            {
                return true;
            }
            //validate IP
            return _core.IsIPAddressAllowed(ipAddressString);
        }
    }
}