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

namespace MarionetteXNA
{
    class RobotData : GameComponent
    {
        #region Fields
        Matrix[] sequentialHomeTransform;
        Matrix TipTransform;
        float[][] dhParameters;
        #endregion


        #region Constructor
        public RobotData(Game1 game) : base(game)
        {
            // Construct DH perameters
            dhParameters = new float[7][]{  new float[]{-90,0,0,400},
                                            new float[]{90,25,90,0},
                                            new float[]{0,560,0,0},
                                            new float[]{90,35,0,515},
                                            new float[]{-90,0,-90,0},
                                            new float[]{90,0,0,0},
                                            new float[]{0,0,0,80}};
            sequentialHomeTransform = new Matrix[dhParameters.Length];
            for (int i = 0; i < dhParameters.Length; i++)
            {
                sequentialHomeTransform[i] = getDH(dhParameters[i]);
            }
            TipTransform = sequentialHomeTransform[0] * sequentialHomeTransform[1] * sequentialHomeTransform[2] * sequentialHomeTransform[3] * sequentialHomeTransform[4] * sequentialHomeTransform[5] * sequentialHomeTransform[6];


            // Construct Jacobean
            // Start Simulation
            //      Assign home position, end effector and angles for simulation
            // 
        }
        #endregion


        #region Properties

        #endregion

        #region Methods
        #region Initialise
        public override void Initialize()
        {

            // Start TCP communication with robot
            //      If connected get starting position and set home position, end effector and angles

            base.Initialize();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        private Matrix getDH(float[] dhValues)
        {
            Matrix rotationX = Matrix.CreateRotationX(MathHelper.ToRadians(dhValues[0]));
            rotationX.Translation = new Vector3(dhValues[1], 0, 0);
            Matrix rotationZ = Matrix.CreateRotationZ(MathHelper.ToRadians(dhValues[2]));
            rotationZ.Translation = new Vector3(0, 0, dhValues[3]);
            return rotationX * rotationZ;
        }
        
        #endregion
    }
}
