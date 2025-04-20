using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCPConnectionSampleClient
{
    internal class Program
    {
        public const int PORT_MIN = 1025;
        public const int PORT_MAX = 25565;
        public static bool run = true;
        public static string response;
        public static string responseNormalised;
        public static NetworkStream stream = null;
        public static StreamWriter writer = null;
        public static StreamReader reader = null;
        static void Main(string[] args)
        {
            Console.WriteLine("Choose a ip:");
            string host = Console.ReadLine();
            int port = -1;
            while (port < PORT_MIN || port > PORT_MAX)
            {
                Console.WriteLine("Choose a port: (" + PORT_MIN + "-" + PORT_MAX + ")");
                response = Console.ReadLine();
                if (!int.TryParse(response, out port))
                {
                    Console.WriteLine("That was not a number");
                }
            }

            try
            {
                using (TcpClient socket = new TcpClient(host, port))
                {
                    stream = socket.GetStream();
                    writer = new StreamWriter(stream);
                    reader = new StreamReader(stream);

                    Task sending = Task.Run(() => { HandleSending(); });
                    while (run)
                    {
                        if (socket.Connected)
                        {
                            if (stream.DataAvailable)
                            {
                                StreamReader reader = new StreamReader(stream, true);
                                Console.WriteLine("Received data: " + reader.ReadLine());
                            }
                        }
                        else
                        {
                            run = false;
                        }
                    }
                    sending.Wait();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public static void HandleSending()
        {
            while (run)
            {
                Console.WriteLine("What Message do you want to send? ('exit' for exit)");
                response = Console.ReadLine();
                try
                {
                    responseNormalised = response.Trim().ToLower().Split(' ')[0];
                }
                catch
                {
                    responseNormalised = "";
                }
                switch (responseNormalised)
                {
                    case "exit":
                        run = false;
                        break;
                    default:

                        if (stream.Socket.Connected)
                        {
                            writer.WriteLine(response);
                            writer.Flush();
                        }
                        else
                        {
                            run = false;
                        }
                        break;
                }
            }
        }

        public static void HandleException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
