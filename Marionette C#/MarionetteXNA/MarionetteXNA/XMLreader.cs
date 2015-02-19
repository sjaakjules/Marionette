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
    class XMLreader : Server
    {
        #region Fields
        public Position measuredPosition;
        public Position measuredSentPosition;
        public Angles measuredAngles;
        public Angles measuredSentAngles;
        public Angles measuredCurrent;
        public long IPoc;
        #endregion

        #region Properties

        #endregion

        #region Constructor
        public XMLreader()
        {
        }
        #endregion

        #region Methods
        #region Initialise

        #endregion

        #region Update
        public void readFile() 
        {
            using (XmlReader reader = XmlReader.Create("robot.xml"))
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
