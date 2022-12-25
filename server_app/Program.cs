using System.Net;
using System.Net.Sockets;
using System.Text;

namespace server_app
{
    public class ChatServer
    {
        private UdpClient client;
        private HashSet<IPEndPoint> members = new HashSet<IPEndPoint>();

        public ChatServer()
        {
            client = new UdpClient(3344);
        }

        public void Join(IPEndPoint endPoint)
        {
            members.Add(endPoint);
        }
        public void Leave(IPEndPoint endPoint)
        {
            members.Remove(endPoint);
        }
        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var member in members)
            {
                client.SendAsync(data, data.Length, member);
            }
        }

        public void Start()
        {
            IPEndPoint? clientEndPoint = null;

            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for a request...");
                    byte[] request = client.Receive(ref clientEndPoint);
                    string message = Encoding.UTF8.GetString(request);
                    Console.WriteLine($"Got a message: {message} from {clientEndPoint}");

                    switch (message)
                    {
                        case "<join>":
                            Join(clientEndPoint);
                            break;
                        case "<leave>":
                            Leave(clientEndPoint);
                            break;
                        default:
                            SendMessage(message);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer();
            server.Start();
        }
    }
}