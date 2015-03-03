using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace MarionetteXNA
{
    class Server
    {
        #region Fields
        // Tcp fields
        private bool tcpIsRunning;
        private int tcpPort;
        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private List<TcpClient> tcpClients = new List<TcpClient>();

        // Udp fields 
        private bool udpIsRunning;
        private int udpPort;
        private EndPoint udpEP = new IPEndPoint(IPAddress.Any, 0);
        private UdpClient udpServer;
        private Socket udpSocket;
        private Thread udpServerThread;


        #endregion
        #region Properties
        public int Port { get; set; }
        #endregion

        #region Constructors
        public Server(int port)
        {
            this.tcpPort = port;
        }

        #endregion

        #region Methods
        public void StartTCP()
        {
            tcpIsRunning = true;
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            tcpListener.Start();
            tcpListenerThread = new Thread(new ThreadStart(TCPListener));
            tcpListenerThread.Start();
        }

        public void StartUDP()
        {
            udpIsRunning = true;
            udpServerThread = new Thread(new ThreadStart(UDPListener));
            udpServerThread.Start();
        }

        public void StopUDP()
        {

        }

        public void StopTCP()
        {
            tcpListenerThread.Abort();
        }

        private void UDPListener()
        {
            IPHostEntry localHostEntry;
            try
            {
                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    localHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                }
                catch (Exception)
                {
                    Console.WriteLine("Local Host not found");
                    return;
                }

                IPEndPoint localEP = new IPEndPoint(localHostEntry.AddressList[0], udpPort);
                udpSocket.Bind(localEP);
                while (udpIsRunning)
                {
                    Byte[] received = new Byte[256];
                    IPEndPoint tmpIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], udpPort);
                    EndPoint remoteEP = (tmpIpEndPoint);
                    int bytesReceived = udpSocket.ReceiveFrom(received, ref remoteEP);
                    String dataReceived = System.Text.Encoding.ASCII.GetString(received);
                    Console.WriteLine("SampleClient is connected through UDP.");
                    Console.WriteLine(dataReceived);
                    String returningString = "The Server got your message through UDP:" + dataReceived;
                    Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(returningString.ToCharArray());
                    udpSocket.SendTo(returningByte, remoteEP);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
            }
        }
        

        private void TCPListener()
        {
            while (tcpIsRunning)
            {
                try
                {
                    TcpClient newClient = tcpListener.AcceptTcpClient();
                    tcpClients.Add(newClient);
                    (new Thread(new ParameterizedThreadStart(ReadExternalCommand))).Start(newClient);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in listener thread: ");
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        private void ReadExternalCommand(object obj)
        {
            // recieves client from thread
            TcpClient client = (TcpClient)obj;
            List<byte> backData = new List<byte>();
            // get stream ID
            byte[] packetIDData = new byte[2];
            client.GetStream().Read(packetIDData, 0, 2);
            char packetID = BitConverter.ToChar(packetIDData, 0);
            switch (packetID)
            {
                case 'p':
                    // clear backData from last packet
                    // read data length information
                    byte[] data = new byte[4];
                    client.GetStream().Read(data, 0, 4);
                    int length = BitConverter.ToInt32(data, 0);
                    // read new data
                    data = new byte[length];
                    client.GetStream().Read(data, 0, length);
                    // tell user and send backData
                    Console.WriteLine("Recieved data: '{0}'", Encoding.ASCII.GetString(data));
                    break;
            }
            

        }
        #region Initialise

        #endregion

        #region Update

        #endregion

        #endregion
    }
}
