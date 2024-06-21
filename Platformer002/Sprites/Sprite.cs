
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer002.Sprites;

public class SpriteContent
{
    public Texture2D Texture;
    public string AnimationConfig;
}

public abstract class Sprite
{
    protected Vector2 _position;
    protected Rectangle _size;
    protected SpriteContent _spriteContent;
    protected AnimationManager _animationManager;

    public Sprite(Vector2 position, 
        Rectangle size, 
        SpriteContent spriteContent,
        AnimationManager animationManager)
    {
        _position = position;
        _size = size;
        _spriteContent = spriteContent;
        _animationManager = animationManager;       

        _animationManager.AddAnimations(_spriteContent.AnimationConfig, _spriteContent.Texture);
        AnimationManager.AnimationComplete += OnAnimationComplete;
    }

    protected Rectangle GetBoundingBox(Vector2 position)
    {
        return new Rectangle((int)position.X, (int)position.Y, _size.Width, _size.Height);
    }

    public abstract void OnAnimationComplete(object sender, AnimationCompleteEventArgs args);

    public abstract void Update();

    public abstract void UpdatePosition();

    public abstract void UpdateAnimation();

    public abstract void DrawToRenderTarget();

    public abstract void Draw();


}
