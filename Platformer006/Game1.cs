using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer006.Managers;
using System;

namespace Platformer006;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private GameManager _gameManager;

#if DEBUG
    private DebugGameManager _debugGameManager;
#endif

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Globals.Background = Color.Black;
        Globals.InternalSize = new Rectangle(0, 0, 320, 180);
        Globals.WindowSize = new Rectangle(0, 0, 1280, 720);
        //Globals.WindowSize = new Rectangle(0, 0, 1600, 900);
        //Globals.WindowSize = new Rectangle(0, 0, 2560, 1440);
        //Globals.WindowSize = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = Globals.WindowSize.Width;
        _graphics.PreferredBackBufferHeight = Globals.WindowSize.Height;

        _graphics.ApplyChanges();
        _graphics.SynchronizeWithVerticalRetrace = true;

        IsFixedTimeStep = true;
        TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33); //33ms = approx 30fps

        Globals.Content = Content;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.GraphicsDevice = GraphicsDevice;

#if DEBUG
        _debugGameManager = new();
#else
        _gameManager = new();
#endif

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Globals.Update(gameTime);

#if DEBUG
        _debugGameManager.Update();
#else
        _gameManager.Update();
#endif

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {


#if DEBUG
        _debugGameManager.Draw();
#else
        _gameManager.Draw();
#endif

        base.Draw(gameTime);
    }


}
