using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer008.Managers;
using System;

namespace Platformer008;

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
       
        // 16:9 aspect ratio
        // 80x45 8x8px tiles
        Globals.InternalSize = new Rectangle(0, 0, 640, 360);

        // 16:9 aspect ratio
        // 3x scale
        Globals.WindowSize = new Rectangle(0, 0, 1920, 1080);

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
        _debugGameManager = new DebugGameManager();
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
