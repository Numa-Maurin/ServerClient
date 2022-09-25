using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server
{
    [Keyless]
    public class Client
    {
        private Client() { }

        public Client(string ipAdress, string id, int port, string tcpClient, List<string> sendMessages)
        {
            this.ipAdress = ipAdress;
            this.id = id;
            this.port = port;
            this.tcpClient = tcpClient;
            this.SendMessages = sendMessages;
        }
        public string ipAdress { get; set; }
        public string id { get; set; }
        public int port { get; set; }
        public string tcpClient { get; set; }
        [NotMapped] public List<String> SendMessages { get; set; }


        public Client(string ipAddress, int port, TcpClient tcpClient)
        {
            this.ipAdress = ipAddress;
            this.port = port;
            this.id = "client";
            this.tcpClient = tcpClient.ToString();
            this.SendMessages = new List<string>(200);
        }

    
    }
}