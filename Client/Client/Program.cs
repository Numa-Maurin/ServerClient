using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string id = "default";
            string ip = "127.0.0.1";
            int port = 5000;

            //saisir id du client
            Console.WriteLine("bienvenue mr le client veuillez saisir un id");
            id = Console.ReadLine();
            Console.WriteLine("ok " + id);

            Client client = new Client(ip, port, id);
 
            client.ConnectToServer();


        }
    }
}
