namespace TCPConnectionSampleServer
{
    internal class Program
    {
        public const int PORT_MIN = 1025;
        public const int PORT_MAX = 25565;
        public static bool run = true;
        public static string response;
        public static string responseNormalised;
        public static NetworkManager networkManager;
        static void Main(string[] args)
        {
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
            networkManager = new NetworkManager(port);
            Task sending = Task.Run(() => { HandleSending(); });
            while (run)
            {
                networkManager.CheckMessage();
            }
            sending.Wait();
            networkManager.Disconnect();
        }

        public static void HandleException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void HandleSending()
        {
            while (run)
            {
                Console.WriteLine("What do you want to do? ('help' for help)");
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
                    case "help":
                        Console.WriteLine("'help': help all commands");
                        Console.WriteLine("'exit': exit");
                        Console.WriteLine("'list': list all clients");
                        Console.WriteLine("'disconnect N': disconnect client N");
                        Console.WriteLine("'send Message': send Message to all clients");
                        break;
                    case "exit":
                        run = false;
                        break;
                    case "list":
                        Console.WriteLine(networkManager.ToString());
                        break;
                    case "disconnect":
                        response = response.Remove(0, "disconnect ".Length);
                        int i = -1;
                        if (int.TryParse(response, out i))
                        {
                            networkManager.Disconnect(i);
                        }
                        else
                        {
                            Console.WriteLine("That was not a number");
                        }
                        break;
                    case "send":
                        response = response.Remove(0, "send ".Length);
                        networkManager.SendMessage(response);
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }
    }
}
