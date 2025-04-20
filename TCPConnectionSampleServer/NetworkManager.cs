using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnectionSampleServer
{
    internal class NetworkManager
    {
        List<Client> clients = new List<Client>();
        TcpListener listener;
        public NetworkManager(int port = 1025)
        {
            listener = new TcpListener(IPAddress.Any, port);
            StartServer();
        }
        void StartServer()
        {
            try
            {
                Console.WriteLine("Starting server...");
                listener.Start();
                Console.WriteLine("Local IP: " + listener.LocalEndpoint);
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0,0,10);
                    try
                    {
                        Console.WriteLine("Remote IP: " + client.GetStringAsync("http://wtfismyip.com/text").Result);
                    }
                    catch (Exception ex)
                    {
                        Program.HandleException(ex);
                    }
                }
                NewConnection();
            }
            catch (Exception ex)
            {
                Program.HandleException(ex);
            }
        }
        void NewConnection()
        {
            Console.WriteLine("Waiting for new connection");
            listener.BeginAcceptTcpClient(AcceptConnection, listener);
        }
        void AcceptConnection(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            lock (clients)
            {
                clients.Add(new Client(listener.EndAcceptTcpClient(ar)));
            }
            NewConnection();
        }
        public void CheckMessage()
        {
            lock (clients)
            {
                for (int i = (clients.Count) - (1); i >= 0; i--)
                {
                    (string message, bool connected) result = clients[i].CheckMessage();
                    if (result.connected)
                    {
                        if(result.message != null)
                        {
                            Console.WriteLine(clients[i].ToString() + " - New message received: " + result.message);
                        }
                    }
                    else
                    {
                        Disconnect(i);
                    }
                }
            }
        }
        public void SendMessage(string message)
        {
            lock (clients)
            {
                for (int i = (clients.Count) - (1); i >= 0; i--)
                {
                    if (clients[i].Connected())
                    {
                        try
                        {
                            clients[i].writer.WriteLine(message);
                            clients[i].writer.Flush();
                        }
                        catch (Exception ex)
                        {
                            Disconnect(i);
                        }
                    }
                    else
                    {
                        Disconnect(i);
                    }
                }
            }
        }
        public void Disconnect(int i)
        {
            if(i < 0 || i >= clients.Count)
            {
                Console.WriteLine("Client " + i + " not in list. Use 'list' to list all clients");
                return;
            }
            lock (clients)
            {
                Console.WriteLine(clients[i].ToString() + " - Disconnecting");
                clients[i].client.Close();
                clients[i].client.Dispose();
                clients.Remove(clients[i]);
            }
        }
        public void Disconnect()
        {
            lock (clients)
            {
                foreach (Client client in clients)
                {
                    Console.WriteLine(client.ToString() + " - Disconnecting");
                    client.client.Close();
                    client.client.Dispose();
                }
                clients.Clear();
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Connected clients (" + clients.Count + "):" + Environment.NewLine);
            for (int i = 0; i < clients.Count; i++)
            {
                sb.Append(i + " - " + clients[i].ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

    }

}
