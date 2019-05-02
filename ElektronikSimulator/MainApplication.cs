using System;
using ESim.Electronics.Circuitry;
using ESim.GUI;
using ESim.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ESim
{
    public class MainApplication : Game
    {
        private readonly GraphicsDeviceManager graphicsDeviceManager;

        private GUIHandler gui;
        private SpriteBatch batch;

        public MainApplication()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += OnWindowResized;

            InputHandler.Initialize();

            Circuit c = new Circuit();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            gui = new GUIHandler(GraphicsDevice);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler.Update(gameTime);
            gui.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            gui.Draw(batch, GraphicsDevice);

            base.Draw(gameTime);
        }

        private void OnWindowResized(object s, EventArgs e)
        {

        }
    }
}