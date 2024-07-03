using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer004.Sprites;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Platformer004.Managers;

public class GameManager
{
    private PlayableSprite _player1;
    private PlayableSprite _player2;
    private TileMap _tileMap;
    private bool _colliding = false;
    SpriteFont _font = Globals.Content.Load<SpriteFont>("Font");
    private RenderTarget2D _collisionRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);
    CollisionData _collisionData = new()
    {
        ScreenCoordinates = Vector2.Zero,
        PixelCoordinatesA = Vector2.Zero,
        PixelCoordinatesB = Vector2.Zero
    };
    private int _player1Score;
    private int _player2Score;

    public GameManager()
    {
        _tileMap = new TileMap();
        _player1 = GetPlayer1();
        _player2 = GetPlayer2();
    }

    private PlayableSprite GetPlayer1()
    {
        var startPosition = new Vector2(0, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("knight_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\knight_spritesheet_array.json")
        };

        var animationManager = new AnimationManager();

        var inputManager = new InputManager(
            leftKey: Keys.A,
            righttKey: Keys.D,
            attack1Key: Keys.Space,
            attack2Key: Keys.LeftControl,
            runKey: Keys.LeftShift,
            jumpKey: Keys.W);

        var player1 = new PlayableSprite(position: startPosition,
            size: size,
            spriteContent: spriteContent,
            animationManager: animationManager,
            inputManager: inputManager);

        return player1;
    }

    private PlayableSprite GetPlayer2()
    {
        var startPosition = new Vector2(250, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent2 = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("skeleton_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\skeleton_spritesheet_array.json")
        };

        var animationManager2 = new AnimationManager();

        var inputManager = new InputManager(
               leftKey: Keys.J,
               righttKey: Keys.L,
               attack1Key: Keys.Enter,
               attack2Key: Keys.RightControl,
               runKey: Keys.RightShift,
               jumpKey: Keys.I);

        var player2 = new PlayableSprite(position: startPosition,
        size: size,
        spriteContent: spriteContent2,
        animationManager: animationManager2,
        inputManager: inputManager);

        return player2;
    }

    public void Update()
    {
        _player1.Update();
        _player2.Update();

        CheckPlayerCollision();

        UpdatePlayerScores();
    }

    private void CheckPlayerCollision()
    {
        _colliding = false;

        if (_player1.BoundingBox.Intersects(_player2.BoundingBox))
        {
            var texturesCollide = Globals.SpriteTexturesCollide(_player1, _player2, _collisionData);

            if (texturesCollide)
            {
                _colliding = true;            
            }

        }
    }
    private void UpdatePlayerScores()
    {
        if (_colliding)
        {
            if (_player1.Attacking && _collisionData.CurrentAnimationFrameA.Hit)
                _player1Score += 1;

            if (_player2.Attacking && _collisionData.CurrentAnimationFrameB.Hit)
                _player2Score += 1;
        }
    }

    public void Draw()
    {
        _player1.DrawToRenderTarget();
        _player2.DrawToRenderTarget();

        if (_colliding)
            DrawCollisionTextureToRenderTarget();

        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        if (_colliding)
            Globals.SpriteBatch.Draw(_collisionRenderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);

        _tileMap.Draw();
        _player1.Draw();
        _player2.Draw();
        DrawPlayerScores();

        Globals.SpriteBatch.End();
    }

    private void DrawCollisionTextureToRenderTarget()
    {
        Globals.GraphicsDevice.SetRenderTarget(_collisionRenderTarget);
        Globals.GraphicsDevice.Clear(Color.Red);
        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        Globals.SpriteBatch.Draw(new Texture2D(Globals.GraphicsDevice, 1,1), new Vector2(0, 0), Color.White);
        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawPlayerScores()
    {
        Globals.SpriteBatch.DrawString(_font, $"Player 1 Score: {_player1Score}", new Vector2(0, 0), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"Player 2 Score: {_player2Score} ", new Vector2(0, 20), Color.White);
    }

}
