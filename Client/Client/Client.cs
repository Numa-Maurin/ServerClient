using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;
namespace Client
{
    public class Client
    {
        public IPAddress ipAdress { get; set; }
        public string id { get; set; }
        public int port { get; set; }
        private TcpClient tcpClient;
        public List<String> SendMessages;

        public Client(string ipAddress, int port, string id)
        {
            this.ipAdress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.id = id;
            this.tcpClient = new TcpClient();
            SendMessages = new List<string>(200);
        }

        public void ConnectToServer()
        {
            Console.WriteLine("on se connecte au serveur ");

            tcpClient.Connect(this.ipAdress, port);
            Console.WriteLine(id + " connecté");
            NetworkStream ns = tcpClient.GetStream();
            Thread thread = new Thread(o => ReceiveData((TcpClient)o));

            thread.Start(tcpClient);

            string s;
            while (!string.IsNullOrEmpty((s = Console.ReadLine())))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(id + " : " + s);
                ns.Write(buffer, 0, buffer.Length);
                SendMessages.Add(s);
            }

            tcpClient.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            ns.Close();
            tcpClient.Close();
            Console.WriteLine("deconnection du server");
            foreach(string mess in SendMessages)
            Console.ReadKey();
        }

        static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }




    } 
}