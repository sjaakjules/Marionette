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
        private Matrix[] sequentialHomeTransform;
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
            sequentialHomeTransform = getDH(getKr10());
            

            // Construct Jacobean
            // Start Simulation
            //      Assign home position, end effector and angles for simulation
            TipTransform = sequentialHomeTransform[0] * sequentialHomeTransform[1] * sequentialHomeTransform[2] * sequentialHomeTransform[3] * sequentialHomeTransform[4] * sequentialHomeTransform[5] * sequentialHomeTransform[6];

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
