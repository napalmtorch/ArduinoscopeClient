using System;
using System.IO.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui;
using ImGuiNET;

namespace ArduinoscopeClient
{
    public class MainForm : Game
    {
        public Point Size { get { return new Point(Client.GraphicsMgr.PreferredBackBufferWidth, Client.GraphicsMgr.PreferredBackBufferHeight); } }
        public Camera  Camera;
        public Vector2 Position;

        private int _scroll;

        public MainForm()
        {
            Client.GraphicsMgr = new GraphicsDeviceManager(this);

            Window.Title             = "Arduinoscope";
            Window.AllowUserResizing = true;

            Content.RootDirectory = "Content";

            IsMouseVisible  = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Client.UIRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();
        }

        protected override void LoadContent()
        {
            Client.SpriteBatch = new SpriteBatch(GraphicsDevice);

            Client.GraphicsMgr.PreferredBackBufferWidth  = 1024;
            Client.GraphicsMgr.PreferredBackBufferHeight = 576;
            Client.GraphicsMgr.SynchronizeWithVerticalRetrace = false;
            Client.GraphicsMgr.ApplyChanges();

            Client.Font = Content.Load<SpriteFont>("FontDebug");

            Client.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Client.Pixel.SetData(new Color[] { Color.White });

            Camera = new Camera();
            Position = new Vector2((IOController.BufferSize / 2), (IOController.BufferSize / 2));

            ControlPanel.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            IOController.End();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!ControlPanel.IsActive)
            {
                KeyboardState kb = Keyboard.GetState();
                if (kb.IsKeyDown(Keys.W)) { Position.Y -= 500.0f * Client.Delta; }
                if (kb.IsKeyDown(Keys.S)) { Position.Y += 500.0f * Client.Delta; }
                if (kb.IsKeyDown(Keys.A)) { Position.X -= 500.0f * Client.Delta; }
                if (kb.IsKeyDown(Keys.D)) { Position.X += 500.0f * Client.Delta; }

                MouseState ms = Mouse.GetState();
                if (ms.ScrollWheelValue < _scroll) { Camera.Zoom -= 0.066f; }
                if (ms.ScrollWheelValue > _scroll) { Camera.Zoom += 0.066f; }
                _scroll = ms.ScrollWheelValue;
            }

            ControlPanel.Update(gameTime);
            Camera.Update(Position);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Client.SpriteBatch.Begin(transformMatrix: Camera.Transform, samplerState: (Camera.Zoom < 1.0f ? SamplerState.AnisotropicWrap : SamplerState.PointClamp));

            float thickness = (Camera.Zoom < 1.0f ? (1.0f / (Camera.Zoom) * 2) : 1);
            Renderer.DrawGrid(IOController.BufferSize, IOController.BufferSize, 64, (int)thickness, Color.Gray);
            Renderer.DrawLines(thickness, ControlPanel.Speed);
            Client.SpriteBatch.End();

            Client.SpriteBatch.Begin();
            Client.SpriteBatch.DrawString(Client.Font, "FPS:" + Client.FPS + "  Frametime:" + (Client.DeltaTimed * 1000.0f) + "ms", new Vector2(8, 8), Color.Orange);
            Client.SpriteBatch.End();

            ControlPanel.Draw(gameTime);

            Client.MeasurePerformance(gameTime);
            base.Draw(gameTime);
        }
    }
}