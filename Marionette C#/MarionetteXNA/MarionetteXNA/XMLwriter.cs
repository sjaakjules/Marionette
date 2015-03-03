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
using System.Xml;

namespace MarionetteXNA
{
    class XMLwriter 
    {

        #region Fields
        public String RobotIP = "192.0.1.2";
        public String RobotPort = "6008";
        #endregion

        #region Properties

        #endregion

        #region Constructor
        public XMLwriter()
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
        
        #endregion

        #region Methods
        public void updateRobot(RobotData robot)
        {
            using (XmlWriter updatePos = XmlWriter.Create("Update.xml"))
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
            
            
        }
        public void updateRobot()
        {
            using ( XmlWriter updatePos = XmlWriter.Create("Update.xml"))
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
        }

        #endregion
    }
}
