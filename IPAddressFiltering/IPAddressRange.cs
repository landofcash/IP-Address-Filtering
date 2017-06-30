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
    }
}
