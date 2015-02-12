using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Model robot;
        private Matrix link1Matrix;
        private Matrix viewMatrix, projectionMatrix;
        private Vector3 cameraPosition, endeffector;
        private float aspectRatio;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            cameraPosition = new Vector3(400.0f, 200.0f, 400.0f);
            endeffector = new Vector3(54.0f, 90.0f, 0.0f);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            viewMatrix = Matrix.CreateLookAt(cameraPosition, endeffector, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(20.0f), aspectRatio, 100.0f, 10000.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            robot = Content.Load<Model>("robot");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            link1Matrix = getDH(0, 0, 0, 0);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            Matrix[] transforms = new Matrix[robot.Bones.Count];
            robot.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in robot.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * link1Matrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                    
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        private Matrix getDH(float alpha, float a, float beta, float b)
        {
            Matrix rotationX = Matrix.CreateRotationX(MathHelper.ToRadians(alpha));
            rotationX.Translation = new Vector3(a, 0, 0);
            Matrix rotationZ = Matrix.CreateRotationZ(MathHelper.ToRadians(beta));
            rotationZ.Translation = new Vector3(0, 0, b);
            return rotationX * rotationZ;
        }
    }
}
