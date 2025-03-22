
using System.Net;
using System.Net.Sockets;

namespace CustomPing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the IP address: ");
            string targetIP = Console.ReadLine() ?? "8.8.8.8";

            SendPing(targetIP);
        }

        private static void SendPing(string targetIP)
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);

            socket.ReceiveTimeout = 3000;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIP), 0);
            EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] icmpPacket = CreateIcmpPacket();
            socket.SendTo(icmpPacket, endPoint);

            Console.WriteLine($"Ping sent to {targetIP}");

            try
            {
                byte[] buffer = new byte[1024];
                int received = socket.ReceiveFrom(buffer, ref senderEndPoint);
                Console.WriteLine($"Received {received} bytes from {senderEndPoint}");
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private static byte[] CreateIcmpPacket()
        {
            byte[] packet = new byte[8];
            packet[0] = 8; // ICMP type
            packet[1] = 0; // ICMP code
            packet[2] = 0; // ICMP checksum
            packet[3] = 0; // ICMP checksum
            packet[4] = 0; // ICMP identifier
            packet[5] = 0; // ICMP identifier
            packet[6] = 0; // ICMP sequence number
            packet[7] = 0; // ICMP sequence number

            ushort checksum = CalculateChecksum(packet);
            packet[2] = (byte)(checksum >> 8);
            packet[3] = (byte)(checksum & 0xFF);

            return packet;
        }

        private static ushort CalculateChecksum(byte[] packet)
        {
            int sum = 0;
            for (int i = 0; i < packet.Length; i += 2)
            {
                ushort word = (ushort)((packet[i] << 8) + (i + 1 < packet.Length ? packet[i + 1] : 0));
                sum += word;
            }
            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }
            return (ushort)~sum;
        }
    }
}
