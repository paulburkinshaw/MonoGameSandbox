using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer004.Sprites;
using System.IO;

namespace Platformer004.Managers;

public class GameManager
{
    private PlayableSprite _player1;
    private PlayableSprite _player2;
    private TileMap _tileMap;
    CollisionData _collisionData = new()
    {
        ScreenCoordinates = Vector2.Zero,
        PixelCoordinatesA = Vector2.Zero,
        PixelCoordinatesB = Vector2.Zero
    };

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
            AnimationConfig = File.ReadAllText(@"Content\knight_spritesheet.json")
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
            AnimationConfig = File.ReadAllText(@"Content\skeleton_spritesheet.json")
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
    }

    private void CheckPlayerCollision()
    {
        if (_player1.BoundingBox.Intersects(_player2.BoundingBox))
        {
            var texturesCollide = Globals.SpriteTexturesCollide(_player1, _player2, _collisionData);

            if (texturesCollide)
            {
                if (_collisionData.CurrentAnimationFrameA.Hit)
                {                                    
                    _player2.OnHit();                 
                }
                if (_collisionData.CurrentAnimationFrameB.Hit)
                {                  
                    _player1.OnHit();                   
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

        Globals.SpriteBatch.End();
    }

}
