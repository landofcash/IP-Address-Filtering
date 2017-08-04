using System;
using System.Net;

namespace IPAddressFiltering
{
    public class IPAddressRange
    {
        private readonly IPAddress _startIPAddress;
        private readonly IPAddress _endIPAddress;

        public IPAddress StartIPAddress
        {
            get
            {
                return _startIPAddress;
            }
        }

        public IPAddress EndIPAddress
        {
            get
            {
                return _endIPAddress;
            }
        }

        public IPAddressRange(string startIPAddress, string endIPAddress)
            : this(IPAddress.Parse(startIPAddress), IPAddress.Parse(endIPAddress))
        {

        }
        public IPAddressRange(IPAddress startIPAddress, IPAddress endIPAddress)
        {
            this._startIPAddress = startIPAddress;
            this._endIPAddress = endIPAddress;
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

            IPAddressRange rnge = new IPAddressRange();
            var splitString = arg.Split(new char[] {'-','/' });
            if (arg.Contains("-"))//straight forward range
            {
                rnge = new IPAddressRange(splitString[0], splitString[1]);
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
                rnge = getNetworkIPRange(IPAddress.Parse(splitString[0]), networkPrefix);                
            }
            return rnge;
        }

        /// <summary>
        /// Gets the network ip range.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="networkPrefix">The network prefix.</param>
        /// <returns></returns>
        public static IPAddressRange getNetworkIPRange(IPAddress ip, int networkPrefix)
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

        ///// <summary>
        ///// Gets the network address.
        ///// </summary>
        ///// <param name="ip">The ip.</param>
        ///// <param name="networkPrefix">The network prefix.</param>
        ///// <returns></returns>
        //public static IPAddress getNetworkAddress(IPAddress ip, int networkPrefix)
        //{
        //    long mask = (long)Math.Pow(2, networkPrefix) - 1;
        //    mask = mask << (32 - networkPrefix);

        //    return new IPAddress(mask & ip.Address);
        //}



        ///// <summary>
        ///// Gets the broad cast address.
        ///// </summary>
        ///// <param name="ip">The ip.</param>
        ///// <param name="networkPrefix">The network prefix.</param>
        ///// <returns></returns>
        //public static IPAddress getBroadCastAddress(IPAddress ip, int networkPrefix)
        //{
        //    long netMask = (long)Math.Pow(2, networkPrefix) - 1;
        //    netMask = netMask << (32 - networkPrefix);
        //    long hostMask = (long)Math.Pow(2, 32 - networkPrefix) - 1;

        //    return new IPAddress(netMask = (ip.Address & netMask) | hostMask);
        //}

        ///// <summary>
        ///// Gets the network mask by prefix.
        ///// </summary>
        ///// <param name="prefix">The prefix.</param>
        ///// <returns></returns>
        //public static String getNetworkMaskByPrefix(int prefix)
        //{
        //    switch (prefix)
        //    {
        //        case 8: return "255.0.0.0";
        //        case 9: return "255.128.0.0";
        //        case 10: return "255.192.0.0";
        //        case 11: return "255.224.0.0";
        //        case 12: return "255.240.0.0";
        //        case 13: return "255.248.0.0";
        //        case 14: return "255.252.0.0";
        //        case 15: return "255.254.0.0";
        //        case 16: return "255.255.0.0";
        //        case 17: return "255.255.128.0";
        //        case 18: return "255.255.192.0";
        //        case 19: return "255.255.224.0";
        //        case 20: return "255.255.240.0";
        //        case 21: return "255.255.248.0";
        //        case 22: return "255.255.252.0";
        //        case 23: return "255.255.254.0";
        //        case 24: return "255.255.255.0";
        //        case 25: return "255.255.255.128";
        //        case 26: return "255.255.255.192";
        //        case 27: return "255.255.255.224";
        //        case 28: return "255.255.255.240";
        //        case 29: return "255.255.255.248";
        //        case 30: return "255.255.255.252";
        //        default: return "";
        //    }
        //}

        ///// <summary>
        ///// Gets the prefix by network mask.
        ///// </summary>
        ///// <param name="subnetMask">The subnet mask.</param>
        ///// <returns></returns>
        //public static int getPrefixByNetworkMask(IPAddress subnetMask)
        //{
        //    switch (subnetMask.ToString())
        //    {
        //        case "255.0.0.0":        return 8;  
        //        case "255.128.0.0":      return 9;  
        //        case "255.192.0.0":      return 10; 
        //        case "255.224.0.0":      return 11; 
        //        case "255.240.0.0":      return 12; 
        //        case "255.248.0.0":      return 13; 
        //        case "255.252.0.0":      return 14; 
        //        case "255.254.0.0":      return 15; 
        //        case "255.255.0.0":      return 16; 
        //        case "255.255.128.0":    return 17; 
        //        case "255.255.192.0":    return 18; 
        //        case "255.255.224.0":    return 19; 
        //        case "255.255.240.0":    return 20; 
        //        case "255.255.248.0":    return 21; 
        //        case "255.255.252.0":    return 22; 
        //        case "255.255.254.0":    return 23; 
        //        case "255.255.255.0":    return 24; 
        //        case "255.255.255.128":  return 25; 
        //        case "255.255.255.192":  return 26; 
        //        case "255.255.255.224":  return 27; 
        //        case "255.255.255.240":  return 28; 
        //        case "255.255.255.248":  return 29; 
        //        case "255.255.255.252":  return 30; 
        //        default: return 24;
        //    }
        //}
    }
}
