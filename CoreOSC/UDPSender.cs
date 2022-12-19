using System;
using System.Net;
using System.Net.Sockets;

namespace CoreOSC
{
    public class UDPSender
    {
        public int Port
        {
            get { return _port; }
        }

        private int _port;

        public string Address
        {
            get { return _address; }
        }

        private string _address;

        private IPEndPoint RemoteIpEndPoint;
        private Socket sock;

        public UDPSender(string address, int port)
        {
            _port = port;
            _address = address;

            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var addresses = System.Net.Dns.GetHostAddresses(address);
            if (addresses.Length == 0) throw new Exception("Unable to find IP address for " + address);

            // Try all IP Addresses
            for (int i = 0; i < addresses.Length; i++ ) {
                if ( addresses[i].AddressFamily != AddressFamily.InterNetwork )
                    continue;
                RemoteIpEndPoint = new IPEndPoint(addresses[i], port);
                return;
            }
            throw new Exception($"Found IP address for {address}, but it is not suitable for OSC!");
        }

        public void Send(byte[] message)
        {
            sock.SendTo(message, RemoteIpEndPoint);
        }

        public void Send(OscPacket packet)
        {
            byte[] data = packet.GetBytes();
            Send(data);
        }

        public void Close()
        {
            sock.Close();
        }
    }
}
