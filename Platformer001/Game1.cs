using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Platformer001
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameManager _gameManager;

        Color _background = Color.White;

        public static Rectangle Screen = new Rectangle(0, 0, 1024, 768);
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = Screen.Width;
            _graphics.PreferredBackBufferHeight = Screen.Height;
            _graphics.ApplyChanges();
            _graphics.SynchronizeWithVerticalRetrace = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33); //33ms = 30fps

            Globals.Content = Content;

            _gameManager = new();
            _gameManager.Init();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.Update(gameTime);
            _gameManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_background);

            _spriteBatch.Begin();
            _gameManager.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
