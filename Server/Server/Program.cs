using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    class Program
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_TCPclients = new Dictionary<int, TcpClient>();
        

        static void Main(string[] args)
        {
            int count = 1;
            var context = new MyContext();

            TcpListener ServerSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            ServerSocket.Start();
            Console.WriteLine("Ouverture du Server :)");

            while (true)
            {
                //save in list
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_TCPclients.Add(count, client);
                

                Console.WriteLine("un client vient de se connecter");


                Thread t = new Thread(handle_clients);
                //création du thread pour la gestion de ce nouveau client
                t.Start(count);
                count++;
            }
        }

        public static void handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_TCPclients[id];

            while (true)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, byte_count);

                broadcastAsync("id"+data);
                Console.WriteLine(data);
            }

            lock (_lock) list_TCPclients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public static async Task broadcastAsync(string data)
        {
            var context = new MyContext();
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (TcpClient c in list_TCPclients.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }

            //save in bdd
            //context +await
            foreach (TcpClient c in list_TCPclients.Values)
            {
                Client clientencours = await context.People.FirstOrDefaultAsync(x => x.tcpClient == c.ToString());
                context.People.Add(new Client("127.0.0.1", 5000, c));
                await context.SaveChangesAsync();
                clientencours.SendMessages.Add(data);

                NetworkStream stream = c.GetStream();

                stream.Write(buffer, 0, buffer.Length);
            }

        }
    }
}
