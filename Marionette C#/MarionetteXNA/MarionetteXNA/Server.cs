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
        // xml fields
        public Position measuredPosition;
        public Position measuredSentPosition;
        public Angles measuredAngles;
        public Angles measuredSentAngles;
        public Angles measuredCurrent;
        public long IPoc;

        public String RobotIP = "192.0.1.2";
        public String RobotPort = "6008";
        public byte[] bCommand;

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


        public void updateRobot(RobotData robot)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter updatePos = XmlWriter.Create(stream))
                {
                    updatePos.WriteStartDocument();
                    updatePos.WriteStartElement("Sen");
                    updatePos.WriteAttributeString("Type", "ImFree");
                    updatePos.WriteStartElement("RKorr");
                    updatePos.WriteAttributeString("X", robot.KukaPosition.X.ToString());
                    updatePos.WriteAttributeString("Y", robot.KukaPosition.Y.ToString());
                    updatePos.WriteAttributeString("Z", robot.KukaPosition.Z.ToString());
                    updatePos.WriteAttributeString("A", robot.KukaPosition.A.ToString());
                    updatePos.WriteAttributeString("B", robot.KukaPosition.B.ToString());
                    updatePos.WriteAttributeString("C", robot.KukaPosition.C.ToString());
                    updatePos.WriteEndElement();
                    updatePos.WriteEndElement();
                    updatePos.WriteEndDocument();
                }
                bCommand = new byte[stream.ToArray().Length];
                bCommand = stream.ToArray();

            }
        }

        public void updateRobot()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter updatePos = XmlWriter.Create("Update.xml"))
                {
                    updatePos.WriteStartDocument();
                    updatePos.WriteStartElement("Sen");
                    updatePos.WriteAttributeString("Type", "ImFree");
                    updatePos.WriteStartElement("RKorr");
                    updatePos.WriteAttributeString("C", "a");
                    updatePos.WriteAttributeString("B", "a");
                    updatePos.WriteAttributeString("A", "a");
                    updatePos.WriteAttributeString("Z", "a");
                    updatePos.WriteAttributeString("Y", Mouse.GetState().Y.ToString());
                    updatePos.WriteAttributeString("X", Mouse.GetState().X.ToString());
                    updatePos.WriteEndElement();
                    updatePos.WriteStartElement("IPOC");
                    updatePos.WriteValue(1563516353);
                    updatePos.WriteEndElement();
                    updatePos.WriteEndElement();
                    updatePos.WriteEndDocument();
                }
                bCommand = new byte[stream.ToArray().Length];
                bCommand = stream.ToArray();
            }

        }

        
        public void XMLwriter()
        {
            using (XmlWriter RSIEthernetSettings = XmlWriter.Create("RSIEthernet.xml"))
            {
                RSIEthernetSettings.WriteStartDocument();
                RSIEthernetSettings.WriteStartElement("ROOT");

                RSIEthernetSettings.WriteStartElement("CONFIG");
                RSIEthernetSettings.WriteStartElement("IP_NUMBER");
                RSIEthernetSettings.WriteString(RobotIP);
                RSIEthernetSettings.WriteEndElement();
                RSIEthernetSettings.WriteStartElement("PORT");
                RSIEthernetSettings.WriteString(RobotPort);
                RSIEthernetSettings.WriteEndElement();
                RSIEthernetSettings.WriteStartElement("PROTOCOL");
                RSIEthernetSettings.WriteString("TCP");
                RSIEthernetSettings.WriteEndElement();
                RSIEthernetSettings.WriteStartElement("SENTYPE");
                RSIEthernetSettings.WriteString("ImFree");
                RSIEthernetSettings.WriteEndElement();
                RSIEthernetSettings.WriteStartElement("PROTCOLLENGTH");
                RSIEthernetSettings.WriteString("Off");
                RSIEthernetSettings.WriteEndElement();
                RSIEthernetSettings.WriteStartElement("ONLYSEND");
                RSIEthernetSettings.WriteString("FALSE");
                RSIEthernetSettings.WriteEndElement();
                RSIEthernetSettings.WriteEndElement();
                // Information to be sent from the robot
                RSIEthernetSettings.WriteStartElement("SEND");
                RSIEthernetSettings.WriteStartElement("ELEMENTS");
                // Cartesian position
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "INTERNAL");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "DEF_RIst");
                RSIEthernetSettings.WriteEndElement();
                // Cartesian setoint position
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "INTERNAL");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "DEF_RSol");
                RSIEthernetSettings.WriteEndElement();
                // Angle position
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "INTERNAL");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "DEF_AIPos");
                RSIEthernetSettings.WriteEndElement();
                // Angle setpoint position
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "INTERNAL");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "DEF_ASPos");
                RSIEthernetSettings.WriteEndElement();
                // Motor current
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "INTERNAL");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "DEF_MACur");
                RSIEthernetSettings.WriteEndElement();

                RSIEthernetSettings.WriteEndElement(); // End elements 
                RSIEthernetSettings.WriteEndElement(); // End Send

                RSIEthernetSettings.WriteStartElement("RECEIVE");
                RSIEthernetSettings.WriteStartElement("ELEMENTS");
                // Cartesian position X
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("HOLDON", "1");
                RSIEthernetSettings.WriteAttributeString("UNIT", "1");
                RSIEthernetSettings.WriteAttributeString("INDX", "1");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "RKorr.X");
                RSIEthernetSettings.WriteEndElement();
                // Cartesian position Y
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("HOLDON", "1");
                RSIEthernetSettings.WriteAttributeString("UNIT", "1");
                RSIEthernetSettings.WriteAttributeString("INDX", "2");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "RKorr.Y");
                RSIEthernetSettings.WriteEndElement();
                // Cartesian position Z
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("HOLDON", "1");
                RSIEthernetSettings.WriteAttributeString("UNIT", "1");
                RSIEthernetSettings.WriteAttributeString("INDX", "3");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "RKorr.Z");
                RSIEthernetSettings.WriteEndElement();
                // Cartesian position Rotation X
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("HOLDON", "1");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "4");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "RKorr.A");
                RSIEthernetSettings.WriteEndElement();
                // Cartesian position Rotation Y
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("HOLDON", "1");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "5");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "RKorr.B");
                RSIEthernetSettings.WriteEndElement();
                // Cartesian position Rotation Z
                RSIEthernetSettings.WriteStartElement("ELEMENT");
                RSIEthernetSettings.WriteAttributeString("HOLDON", "1");
                RSIEthernetSettings.WriteAttributeString("UNIT", "0");
                RSIEthernetSettings.WriteAttributeString("INDX", "6");
                RSIEthernetSettings.WriteAttributeString("TYPE", "DOUBLE");
                RSIEthernetSettings.WriteAttributeString("TAG", "RKorr.C");
                RSIEthernetSettings.WriteEndElement();

                RSIEthernetSettings.WriteEndElement();
                
                RSIEthernetSettings.WriteEndElement(); // End Root
                RSIEthernetSettings.WriteEndDocument();

            }
        }


        public void ConvertByteToXml(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                readStream(stream);
            }
        }
        public void readStream(MemoryStream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "RIst":
                                string RIstValue;
                                RIstValue = reader["X"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.X = float.Parse(RIstValue);
                                }
                                RIstValue = reader["Y"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.Y = float.Parse(RIstValue);
                                }
                                RIstValue = reader["Z"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.Z = float.Parse(RIstValue);
                                }
                                RIstValue = reader["A"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.A = float.Parse(RIstValue);
                                }
                                RIstValue = reader["B"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.B = float.Parse(RIstValue);
                                }
                                RIstValue = reader["C"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.Y = float.Parse(RIstValue);
                                }
                                break;
                            case "RSol":
                                string RSolValue;
                                RSolValue = reader["X"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.X = float.Parse(RSolValue);
                                }
                                RSolValue = reader["Y"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.Y = float.Parse(RSolValue);
                                }
                                RSolValue = reader["Z"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.Z = float.Parse(RSolValue);
                                }
                                RSolValue = reader["A"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.A = float.Parse(RSolValue);
                                }
                                RSolValue = reader["B"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.B = float.Parse(RSolValue);
                                }
                                RSolValue = reader["C"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.Y = float.Parse(RSolValue);
                                }
                                break;
                            case "AIPos":
                                string AIPosValue;
                                AIPosValue = reader["A1"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A1 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A2"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A2 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A3"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A3 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A4"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A4 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A5"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A5 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A6"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A6 = float.Parse(AIPosValue);
                                }
                                break;
                            case "ASPos":
                                string ASPosValue;
                                ASPosValue = reader["A1"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A1 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A2"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A2 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A3"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A3 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A4"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A4 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A5"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A5 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A6"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A6 = float.Parse(ASPosValue);
                                }
                                break;
                            case "MACur":
                                string MACurValue;
                                MACurValue = reader["A1"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A1 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A2"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A2 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A3"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A3 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A4"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A4 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A5"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A5 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A6"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A6 = float.Parse(MACurValue);
                                }
                                break;
                            case "IPOC":
                                IPoc = long.Parse(reader.Value.Trim());
                                break;
                        }
                    }
                }
            }
        }


        #region Initialise

        #endregion

        #region Update

        #endregion

        #endregion


        #region Structures
        public struct RobotValue
        {
            public bool hasUpdated;
            public float Value;
            public string Text;

        }

        public struct ComplexPosition
        {
            private RobotValue XVal;
            private RobotValue YVal;
            private RobotValue ZVal;
            private RobotValue AVal;
            private RobotValue BVal;
            private RobotValue CVal;
            public float X
            {
                get { return XVal.Value; }
                set
                {
                    if (!XVal.hasUpdated)
                    {
                        XVal.Value = value;
                        XVal.hasUpdated = true;
                    }
                }
            }
            public float Y
            {
                get { return YVal.Value; }
                set
                {
                    if (!YVal.hasUpdated)
                    {
                        YVal.Value = value;
                        YVal.hasUpdated = true;
                    }
                }
            }
            public float Z
            {
                get { return ZVal.Value; }
                set
                {
                    if (!ZVal.hasUpdated)
                    {
                        ZVal.Value = value;
                        ZVal.hasUpdated = true;
                    }
                }
            }
            public float A
            {
                get { return AVal.Value; }
                set
                {
                    if (!AVal.hasUpdated)
                    {
                        AVal.Value = value;
                        AVal.hasUpdated = true;
                    }
                }
            }
            public float B
            {
                get { return BVal.Value; }
                set
                {
                    if (!BVal.hasUpdated)
                    {
                        BVal.Value = value;
                        BVal.hasUpdated = true;
                    }
                }
            }
            public float C
            {
                get { return CVal.Value; }
                set
                {
                    if (!CVal.hasUpdated)
                    {
                        CVal.Value = value;
                        CVal.hasUpdated = true;
                    }
                }
            }
            public void resetValues()
            {
                XVal.hasUpdated = false;
                YVal.hasUpdated = false;
                ZVal.hasUpdated = false;
                AVal.hasUpdated = false;
                BVal.hasUpdated = false;
                CVal.hasUpdated = false;
            }
        }


        public struct Position
        {
            public float X;
            public float Y;
            public float Z;
            public float A;
            public float B;
            public float C;
        }

        public struct Angles
        {
            public float A1;
            public float A2;
            public float A3;
            public float A4;
            public float A5;
            public float A6;
        }
        #endregion
    }
}
