using System;
using System.Net;
using System.Net.Sockets;

namespace IPAddressFiltering
{
    public static class IPAddressExtensions
    {
        public static bool IsInRange(this IPAddress address, IPAddress start, IPAddress end)
        {

            AddressFamily addressFamily = start.AddressFamily;
            byte[] lowerBytes = start.GetAddressBytes();
            byte[] upperBytes = end.GetAddressBytes();

            if (address.AddressFamily != addressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < lowerBytes.Length &&
                (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }

        public static IPAddress GetBroadcastAddress(this IPAddress address, int prefix)
        {
            IPAddress subnetMask = getNetworkMaskByPrefix(prefix);
            return address.GetBroadcastAddress(subnetMask);
        }

        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, int prefix)
        {
            IPAddress subnetMask = getNetworkMaskByPrefix(prefix);
            return address.GetNetworkAddress(subnetMask);
        }
        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            IPAddress network1 = address.GetNetworkAddress(subnetMask);
            IPAddress network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }

        /// <summary>
        /// Gets the network mask by prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public static IPAddress getNetworkMaskByPrefix(int prefix)
        {
            switch (prefix)
            {
                case 8: return IPAddress.Parse("255.0.0.0");
                case 9: return IPAddress.Parse("255.128.0.0");
                case 10: return IPAddress.Parse("255.192.0.0");
                case 11: return IPAddress.Parse("255.224.0.0");
                case 12: return IPAddress.Parse("255.240.0.0");
                case 13: return IPAddress.Parse("255.248.0.0");
                case 14: return IPAddress.Parse("255.252.0.0");
                case 15: return IPAddress.Parse("255.254.0.0");
                case 16: return IPAddress.Parse("255.255.0.0");
                case 17: return IPAddress.Parse("255.255.128.0");
                case 18: return IPAddress.Parse("255.255.192.0");
                case 19: return IPAddress.Parse("255.255.224.0");
                case 20: return IPAddress.Parse("255.255.240.0");
                case 21: return IPAddress.Parse("255.255.248.0");
                case 22: return IPAddress.Parse("255.255.252.0");
                case 23: return IPAddress.Parse("255.255.254.0");
                case 24: return IPAddress.Parse("255.255.255.0");
                case 25: return IPAddress.Parse("255.255.255.128");
                case 26: return IPAddress.Parse("255.255.255.192");
                case 27: return IPAddress.Parse("255.255.255.224");
                case 28: return IPAddress.Parse("255.255.255.240");
                case 29: return IPAddress.Parse("255.255.255.248");
                case 30: return IPAddress.Parse("255.255.255.252");
                default: return null;
            }
        }

        /// <summary>
        /// Gets the prefix by network mask.
        /// </summary>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns></returns>
        public static int getPrefixByNetworkMask(IPAddress subnetMask)
        {
            switch (subnetMask.ToString())
            {
                case "255.0.0.0": return 8;
                case "255.128.0.0": return 9;
                case "255.192.0.0": return 10;
                case "255.224.0.0": return 11;
                case "255.240.0.0": return 12;
                case "255.248.0.0": return 13;
                case "255.252.0.0": return 14;
                case "255.254.0.0": return 15;
                case "255.255.0.0": return 16;
                case "255.255.128.0": return 17;
                case "255.255.192.0": return 18;
                case "255.255.224.0": return 19;
                case "255.255.240.0": return 20;
                case "255.255.248.0": return 21;
                case "255.255.252.0": return 22;
                case "255.255.254.0": return 23;
                case "255.255.255.0": return 24;
                case "255.255.255.128": return 25;
                case "255.255.255.192": return 26;
                case "255.255.255.224": return 27;
                case "255.255.255.240": return 28;
                case "255.255.255.248": return 29;
                case "255.255.255.252": return 30;
                default: return 24;
            }
        }

        /// <summary>
        /// Adds the specified octet.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="octet">The octet.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Add is only implimented for IP4 and IP6</exception>
        public static IPAddress Add(this IPAddress address, int octet, int value)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork && octet >= 0 && octet < 4)
            {
                byte[] b = address.GetAddressBytes();
                int val = b[octet] + value;
                b[octet] = (byte)val;
                return new IPAddress(b);
            } else if (address.AddressFamily == AddressFamily.InterNetworkV6 && octet >= 0 && octet < 8)
            {
                byte[] b = address.GetAddressBytes();
                int val = b[octet] + value;
                b[octet] = (byte)val;
                return new IPAddress(b);
            }
            else
            {
                throw new NotImplementedException("Add is only implimented for IP4 and IP6");
            }
            return address;
        }

    }
}
