using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Platformer002;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private GameManager _gameManager;
 
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

        _gameManager = new();
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
        _gameManager.Draw();
        
        base.Draw(gameTime);
    }


}
