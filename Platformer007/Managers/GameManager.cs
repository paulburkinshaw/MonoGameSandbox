using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer007.Sprites;
using System;
using System.IO;

namespace Platformer007.Managers;

public class PlayerHitEventArgs : EventArgs
{
    public readonly string PlayerId;

    public PlayerHitEventArgs(string playerId)
    {
        PlayerId = playerId;
    }
}

public class GameManager
{
    private PlayableSprite _player1;
    private PlayableSprite _player2;
    private Tilemap _tileMap;
    SpriteFont _font = Globals.Content.Load<SpriteFont>("Font");
    private RenderTarget2D _collisionRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);
    CollisionData _collisionData = new()
    {
        ScreenCoordinates = Vector2.Zero,
        PixelCoordinatesA = Vector2.Zero,
        PixelCoordinatesB = Vector2.Zero
    };

    public event EventHandler<PlayerHitEventArgs> PlayerHit = delegate { };
   
    protected virtual void OnPlayerHit(PlayerHitEventArgs args)
    {
        if (PlayerHit != null)
            PlayerHit(this, args);
    }

    public GameManager()
    {
        var screen1TilemapFilepath = @"Content\Tilemaps\Platformer_Screen1_Tilemap.tmj";
     
        // _tileMap = new Tilemap(Globals.FileSystem, Globals.TiledTilemapJsonConverterService, screen1TilemapFilepath);

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
            AnimationConfig = File.ReadAllText(@"Content\knight_spritesheet.json")
        };

        var animationManager = new AnimationManager();

        var inputManager = new InputManager(
            leftKey: Keys.A,
            righttKey: Keys.D,
            attack1Key: Keys.Space,
            attack2Key: Keys.LeftControl,
            runKey: Keys.LeftShift,
            jumpKey: Keys.W,
            blockKey: Keys.LeftAlt);

        var player1 = new PlayableSprite(position: startPosition,
            size: size,
            spriteContent: spriteContent,
            animationManager: animationManager,
            tilemap: _tileMap,
            inputManager: inputManager,
            id: "player1");

        PlayerHit += player1.OnHit;

        return player1;
    }

    private PlayableSprite GetPlayer2()
    {
        var startPosition = new Vector2(250, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent2 = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("skeleton_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\skeleton_spritesheet.json")
        };

        var animationManager2 = new AnimationManager();

        var inputManager = new InputManager(
               leftKey: Keys.J,
               righttKey: Keys.L,
               attack1Key: Keys.Enter,
               attack2Key: Keys.RightControl,
               runKey: Keys.RightShift,
               jumpKey: Keys.I,
                blockKey: Keys.N);

        var player2 = new PlayableSprite(position: startPosition,
        size: size,
        spriteContent: spriteContent2,
        animationManager: animationManager2,
        tilemap: _tileMap,
        inputManager: inputManager,
        id: "player2");

        PlayerHit += player2.OnHit;

        return player2;
    }

    public void Update()
    {
        _player1.Update();
        _player2.Update();

        CheckPlayerCollision();
    }

    private void CheckPlayerCollision()
    {
        if (_player1.BoundingBox.Intersects(_player2.BoundingBox))
        {
            var texturesCollide = Globals.SpriteTexturesCollide(_player1, _player2, _collisionData);

            if (texturesCollide)
            {
                if (_collisionData.CurrentFrameA.Hits || _collisionData.CurrentFrameB.Hits)
                {
                    var playerId = _collisionData.CurrentFrameA.Hits ? "player2" : "player1";
                    OnPlayerHit(new PlayerHitEventArgs(playerId));
                }
                else
                {
                    _player1.OnPlayersCollide(_player2);
                    _player2.OnPlayersCollide(_player1);
                }
            }

        }
    }

    public void Draw()
    {
        _player1.DrawToRenderTarget();
        _player2.DrawToRenderTarget();

        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        _tileMap.Draw();
        _player1.Draw();
        _player2.Draw();
        DrawPlayerScores();

        Globals.SpriteBatch.End();
    }

    private void DrawPlayerScores()
    {
        Globals.SpriteBatch.DrawString(_font, $"Player 1 Health: {_player1.Health}", new Vector2(0, 0), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"Player 2 Health: {_player2.Health} ", new Vector2(0, 20), Color.White);
    }

}
