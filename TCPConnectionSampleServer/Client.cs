using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnectionSampleServer
{
    internal class Client
    {
        public TcpClient client { get; private set; }
        public NetworkStream stream { get; private set; } = null;
        public StreamReader reader { get; private set; } = null;
        public StreamWriter writer { get; private set; } = null;
        public Client(TcpClient tcp)
        {
            client = tcp;
            stream = tcp.GetStream();
            reader = new StreamReader(stream, true);
            writer = new StreamWriter(stream);
            Console.WriteLine(ToString() + " - Received connection");
        }
        public bool Connected()
        {
            if (!client.Connected)
            {
                Console.WriteLine(ToString() + " - Disconnected");
            }
            return client.Connected;
        }
        public (string message, bool connected) CheckMessage()
        {
            string data = null;
            bool connected = Connected();
            if (connected)
            {
                if (stream.DataAvailable)
                {
                    data = reader.ReadLine();
                }
                connected = Connected();
            }
            return (data, connected);
        }
        public override string ToString()
        {
            return client.Client.RemoteEndPoint.ToString();
        }
    }
}
