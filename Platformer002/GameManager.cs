using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer002.Managers;
using Platformer002.Sprites;
using System.IO;

namespace Platformer002;

public class GameManager
{
    private PlayableSprite _player;
    private TileMap _tileMap;

    public GameManager()
    {
        _tileMap = new();
        _player = GetPlayer();
    }

    private PlayableSprite GetPlayer()
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

        return new PlayableSprite(position: startPosition,
            size: size,
            spriteContent: spriteContent,
            animationManager: animationManager,
            inputManager: knightInputManager);
    }

    public void Update()
    {
        _player.Update();
    }

    public void Draw()
    {
        _player.DrawToRenderTarget();       

        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        _tileMap.Draw();
        _player.Draw();       
        Globals.SpriteBatch.End();
    }
}
