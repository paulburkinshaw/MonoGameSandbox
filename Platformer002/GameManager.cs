using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer002.Managers;
using Platformer002.Sprites;
using System.IO;

namespace Platformer002;

public class GameManager
{
    private Knight _player1;
    private Skeleton _player2;
    private Map _map;

    public GameManager()
    {
        _map = new();
        _player1 = GetPlayer1();
        _player2 = GetPlayer2();
    }

    private Knight GetPlayer1()
    {
        var startPosition = new Vector2(0, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("knight_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\knight_spritesheet_array.json")
        };

        var animationManager = new AnimationManager();
        var knightInputManager = new KnightInputManager();

        var player1 = new Knight(position: startPosition,
            size: size,
            spriteContent: spriteContent,
            animationManager: animationManager,
            inputManager: knightInputManager);

        return player1;
    }

    private Skeleton GetPlayer2()
    {
        var startPosition = new Vector2(250, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("skeleton_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\skeleton_spritesheet_array.json")
        };

    var animationManager = new AnimationManager();
    var skeletonInputManager = new SkeletonInputManager();

    var player2 = new Skeleton(position: startPosition,
        size: size,
        spriteContent: spriteContent,
        animationManager: animationManager,
        inputManager: skeletonInputManager);

        return player2;
    }

public void Update()
{
    _player1.Update();
    _player2.Update();
}

public void Draw()
{
    _player1.DrawToRenderTarget();
    _player2.DrawToRenderTarget();

    Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
    _map.Draw();
    _player1.Draw();
    _player2.Draw();
    Globals.SpriteBatch.End();
}
}
