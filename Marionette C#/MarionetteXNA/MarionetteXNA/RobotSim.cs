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
    class RobotSim : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields
        SpriteBatch spriteBatch;
        Game1 game;

        private Model robot;
        private Matrix[] Transforms;
        private ModelBone[] Bones;
        private float[] Angles;
        private Vector3 EndEffector;

        #endregion


        #region Constructor
        public RobotSim(Game1 game) : base(game)
        {
            this.game = game;
        }
        #endregion


        #region Properties

        #endregion

        #region Methods
        #region Initialise
        public override void Initialize()
        {
            base.Initialize();
        }
        #endregion


        #region LoadContent
        protected override void LoadContent()
        {

            robot =  game.Content.Load<Model>("robot");
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
            robot.CopyAbsoluteBoneTransformsTo(transforms);

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
        #endregion
    }
}
