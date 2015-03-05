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
    class RobotData : DrawableGameComponent
    {
        #region Fields
        // End Effector information 
        private float[] positionEE, lastPositionEE, velocityEE, lastVelocityEE, accelerationEE, lastAccelerationEE;
        private Quaternion orientationEE, lastOrientationEE, orientationVelocityEE, lastOrientationVelocityEE, orientationAccelerationEE, lastOrientationAccelerationEE;
        // Lock pertaining to the above end effector information
        public Object EEInfoLock = new Object();

        // Home position information and DH parameters
        private Matrix[] homeLinkTransforms;
        private Matrix TipTransform;
        private float[][] dhParameters;
        private Matrix ToGlobal;
        private XMLreader.Position kukaPosition;
        private Quaternion angles;

        SpriteBatch spriteBatch;
        Game1 game;

        private Model robot;
        private Model Table;
        private Matrix[] Transforms;
        private ModelBone[] Bones;

        Matrix[] TableTransforms;
        ModelBone[] TableBones;
        private float[] Angles;
        private Vector3 EndEffector;
        #endregion


        #region Constructor
        public RobotData(Game1 game) : base(game)
        {
            this.game = game;
            
            // Construct DH perameters 
            homeLinkTransforms = getDH(getKr10());
            

            // Construct Jacobean
            // Start Simulation
            //      Assign home position, end effector and angles for simulation
            TipTransform = homeLinkTransforms[0] * homeLinkTransforms[1] * homeLinkTransforms[2] * homeLinkTransforms[3] * homeLinkTransforms[4] * homeLinkTransforms[5] * homeLinkTransforms[6];

            // 
        }
        #endregion


        #region Properties
        public Vector3 GlobalPosition
        {
            get { return (ToGlobal * TipTransform).Translation; }
        }

        public XMLreader.Position KukaPosition
        {
            get { return kukaPosition; }
        }

        public float[] setPosition
        {
            set 
            { 
                lastPositionEE = positionEE;
                positionEE = value;
            }
        }
        #endregion

        
        #region Initialise
        public override void Initialize()
        {

            // Start TCP communication with robot
            //      If connected get starting position and set home position, end effector and angles

            base.Initialize();
        }
        #endregion


        
        #region LoadContent
        protected override void LoadContent()
        {

            robot =  game.Content.Load<Model>("robot");
            Table = game.Content.Load<Model>("table");
            base.LoadContent();
        }
        #endregion


        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion


        #region Draw
        public override void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[robot.Bones.Count];
            TableTransforms = new Matrix[Table.Bones.Count];
            robot.CopyAbsoluteBoneTransformsTo(transforms);
            Table.CopyAbsoluteBoneTransformsTo(TableTransforms);
            foreach (ModelMesh mesh in Table.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = TableTransforms[mesh.ParentBone.Index];
                    effect.View = game.ViewMat;
                    effect.Projection = game.ProjectionMat;
                }
            }
            foreach (ModelMesh mesh in robot.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index];
                    effect.View = game.ViewMat;
                    effect.Projection = game.ProjectionMat;

                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
        #endregion

        #region ClassMethods
        private void UpdatePositionInformation(float[] newPosition, TimeSpan loopTime)
        {

            lastPositionEE = positionEE;
            positionEE = newPosition;
            lastVelocityEE = velocityEE;
            for (int i = 0; i < newPosition.Length; i++)
			{
                velocityEE[i] = 1000f * (newPosition[i] - lastPositionEE[i]) / loopTime.Milliseconds;
			}
            lastAccelerationEE = accelerationEE;
            for (int i = 0; i < newPosition.Length; i++)
            {
                accelerationEE[i] = 1000f * (velocityEE[i] - lastVelocityEE[i]) / loopTime.Milliseconds;
            }
        }

        private void UpdateOrientationInformation(Quaternion newOrientation, TimeSpan loopTime)
        {
            Vector3 axis = new Vector3();
            float angle = 0;
            lastOrientationEE = orientationEE;
            orientationEE = newOrientation;
            lastOrientationVelocityEE = orientationVelocityEE;
            Functions.getAxisAngle(Quaternion.Subtract(orientationVelocityEE, lastOrientationVelocityEE), ref axis, ref angle);
            orientationVelocityEE = Quaternion.CreateFromAxisAngle(axis, 1000f * angle / loopTime.Milliseconds);

            lastOrientationVelocityEE = orientationVelocityEE;
            Functions.getAxisAngle(Quaternion.Subtract(orientationVelocityEE, lastOrientationVelocityEE), ref axis, ref angle);
            orientationVelocityEE = Quaternion.CreateFromAxisAngle(axis, 1000f * angle / loopTime.Milliseconds);
            
        }

        private float[][] getKr10()
        {
            dhParameters = new float[7][]{  new float[]{-90,0,0,400},
                                            new float[]{90,25,90,0},
                                            new float[]{0,560,0,0},
                                            new float[]{90,35,0,515},
                                            new float[]{-90,0,-90,0},
                                            new float[]{90,0,0,0},
                                            new float[]{0,0,0,80}};
            return dhParameters;
        }

        private Matrix[] getDH(float[][] dhValues)
        {
            Matrix[] dhRows = new Matrix[dhValues.Length];
            for (int i = 0; i < dhValues.Length; i++)
            {
                Matrix rotationX = Matrix.CreateRotationX(MathHelper.ToRadians(dhValues[i][0]));
                rotationX.Translation = new Vector3(dhValues[i][1], 0, 0);
                Matrix rotationZ = Matrix.CreateRotationZ(MathHelper.ToRadians(dhValues[i][2]));
                rotationZ.Translation = new Vector3(0, 0, dhValues[i][3]);
                dhRows[i] = rotationX * rotationZ; ;
            }
            return dhRows;
        }
        
        #endregion

    }
}
