using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IPAddressFiltering
{
    /// <summary>
    /// Static class to store roles with IPs list for authorization
    /// </summary>
    public static class RolesContainer
    {
        public static readonly string LOCAL_ROLE = "local";
        public static readonly string ANY_ROLE = "any";
        public static readonly string GLOBAL_ROLE = "global";

        private static readonly IPAddress _localIPv4 = IPAddress.Parse("127.0.0.1");
        private static readonly IPAddress _localIPv6 = IPAddress.Parse("::1");

        private static readonly Dictionary<string, List<IPAddress>> _rolesWithIPList = new Dictionary<string, List<IPAddress>>();
        private static readonly Dictionary<string, List<IPAddressRange>> _rolesWithIPRangeList = new Dictionary<string, List<IPAddressRange>>();
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
                _rolesWithIPRangeList.Clear();
                foreach (var item in items)
                {
                    if (item.Value.Any(v => v.IndexOfAny(new [] { '-', '/' }) >= 0))
                    {
                        _rolesWithIPRangeList.Add(item.Key, new List<IPAddressRange>(item.Value.Where(v => v.IndexOfAny(new [] { '-', '/' }) >= 0).Select(IPAddressRange.Parse)));
                    }
                    else
                    {
                        _rolesWithIPList.Add(item.Key, new List<IPAddress>(item.Value.Where(v => v.IndexOfAny(new [] { '-', '/' }) < 0).Select(IPAddress.Parse)));
                    }
                }
                //adds local role
                if (!_rolesWithIPList.ContainsKey(LOCAL_ROLE))
                {
                    _rolesWithIPList.Add(LOCAL_ROLE, new List<IPAddress>() { _localIPv4, _localIPv6 });
                }
            }
        }
        /// <summary>
        /// returns a copy of IPs for role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
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

        public static List<IPAddressRange> GetRoleIPRanges(string role)
        {
            lock (_lock)
            {
                if (_rolesWithIPRangeList.ContainsKey(role))
                {
                    return new List<IPAddressRange>(_rolesWithIPRangeList[role]);
                }
            }
            return new List<IPAddressRange>();
        }
    }
}