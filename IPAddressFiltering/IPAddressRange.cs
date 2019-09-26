using System.Net;

namespace IPAddressFiltering
{
    public class IPAddressRange
    {
        public IPAddress StartIPAddress { get; }
        public IPAddress EndIPAddress { get; }

        public IPAddressRange(string startIPAddress, string endIPAddress)
            : this(IPAddress.Parse(startIPAddress), IPAddress.Parse(endIPAddress))
        {

        }
        public IPAddressRange(IPAddress startIPAddress, IPAddress endIPAddress)
        {
            StartIPAddress = startIPAddress;
            EndIPAddress = endIPAddress;
        }

        private IPAddressRange()
        {
        }

        /// <summary>
        /// Parses the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        internal static IPAddressRange Parse(string arg)
        {
            IPAddressRange networkIpRange = new IPAddressRange();
            var splitString = arg.Split('-', '/');
            if (arg.Contains("-"))//straight forward range
            {
                networkIpRange = new IPAddressRange(splitString[0], splitString[1]);
            }
            else if (arg.Contains("/"))//CIDR  needs calculation
            {
                int networkPrefix = 24;
                if (splitString[1].Contains("."))//subnet mask detected
                {
                    networkPrefix = IPAddressExtensions.getPrefixByNetworkMask(IPAddress.Parse(splitString[1]));
                }
                else
                {
                    networkPrefix = int.Parse(splitString[1]);
                }
                networkIpRange = GetNetworkIPRange(IPAddress.Parse(splitString[0]), networkPrefix);                
            }
            return networkIpRange;
        }

        /// <summary>
        /// Gets the network ip range.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="networkPrefix">The network prefix.</param>
        /// <returns></returns>
        public static IPAddressRange GetNetworkIPRange(IPAddress ip, int networkPrefix)
        {
            //String from, to;
            /**
			 * Start
			 * +1 because first = network
			 */
            IPAddress startIP = ip.GetNetworkAddress(networkPrefix).Add(3,1);// + 1);
            /**
			 * End
			 * -1 because last = broadcast
			 */
            IPAddress endIP = ip.GetBroadcastAddress(networkPrefix).Add(3, -1);// - 1);
            return new IPAddressRange(startIP,endIP);
        }

        /// <summary>
        /// Checks if the IP belongs to range
        /// </summary>
        /// <param name="address">IP to check</param>
        /// <returns></returns>
        public bool IsInRange(IPAddress address)
        {
            return address.IsInRange(StartIPAddress, EndIPAddress);
        }
        
        /// <summary>
        /// checks if IP in range
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsInRange(string ip, string range)
        {
            IPAddressRange ipAddressRange = Parse(range);
            IPAddress ipToCheck = IPAddress.Parse(ip);
            return ipAddressRange.IsInRange(ipToCheck);
        }
    }
}
