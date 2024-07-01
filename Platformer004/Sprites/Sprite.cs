using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer004.Managers;
using System;
using System.Collections.Generic;

namespace Platformer004.Sprites;

public class CollidesWithTileEventArgs
{
    public readonly Rectangle CollidingTile;
    public readonly int MovementAmount;
    public readonly float NewPosition;

    public CollidesWithTileEventArgs(Rectangle collidingTile, int movementAmount, float newPosition)
    {
        CollidingTile = collidingTile;
        MovementAmount = movementAmount;
        NewPosition = newPosition;
    }
}

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
    protected Animation _currentAnimationz;
    protected int _currentAnimationFrame;
    Vector2 _origin = Vector2.Zero;
    float _rotation = 0;

    public Rectangle BoundingBox => GetBoundingBox();
    public Animation CurrentAnimation => _currentAnimationz;
    public int CurrentAnimationFrame => _currentAnimationFrame;
    public Matrix Matrix => GetTranformationMatrix();

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
        animationManager.AnimationStarted += OnAnimationStarted;
        animationManager.AnimationComplete += OnAnimationComplete;
        animationManager.AnimationFrameChanged += OnAnimationFrameChanged;
    }

    private Matrix GetTranformationMatrix()
    {
        return Matrix.CreateTranslation(_origin.X, _origin.Y, 0) *
               Matrix.CreateRotationZ(_rotation) *
               Matrix.CreateScale(1f) *
               Matrix.CreateTranslation(_position.X, _position.Y, 0);
    }

    private Rectangle GetBoundingBox()
    {
        return new Rectangle((int)_position.X, (int)_position.Y, _size.Width, _size.Height);
    }

    public virtual void OnAnimationStarted(object sender, AnimationStartedEventArgs args)
    {
        _currentAnimationz = args.Animation;
    }

    public virtual void OnAnimationComplete(object sender, AnimationCompleteEventArgs args)
    {
        // _currentAnimation = null;
    }
    public virtual void OnAnimationFrameChanged(object sender, AnimationFrameChangedEventArgs args)
    {
        _currentAnimationFrame = args.CurrentFrame;
    }

    public virtual void MoveX(float movementAmount, Action<CollidesWithTileEventArgs> onCollidesWithTile)
    {
        int xAmount = (int)Math.Round(movementAmount);

        var newXPosition = _position.X + xAmount;
        var newXPositionBoundingBox = new Rectangle((int)newXPosition, (int)_position.Y, _size.Width, _size.Height);

        var colliders = TileMap.GetNearestColliders(newXPositionBoundingBox);
     
        if (!CollidesWithTile(colliders: colliders, newPosition: new Vector2(newXPosition, _position.Y), out Rectangle collidingTile))
        {
            _position.X = newXPosition;
        }
        else
        {
            // Hit a tile
            if (onCollidesWithTile != null)
                onCollidesWithTile(new CollidesWithTileEventArgs(collidingTile, xAmount, newXPosition));
        }

    }

    public virtual void MoveY(float movementAmount, Action<CollidesWithTileEventArgs> onCollidesWithTile)
    {
        int yAmount = (int)Math.Round(movementAmount);

        var newYPosition = _position.Y + yAmount;
        var newYPositionBoundingBox = new Rectangle((int)_position.X, (int)newYPosition, _size.Width, _size.Height);

        var colliders = TileMap.GetNearestColliders(newYPositionBoundingBox);

        if (!CollidesWithTile(colliders: colliders, newPosition: new Vector2(_position.X, newYPosition), out Rectangle collidingTile))
        {
            _position.Y = newYPosition;
        }
        else
        {
            // Hit a tile
            if (onCollidesWithTile != null)
                onCollidesWithTile(new CollidesWithTileEventArgs(collidingTile, yAmount, newYPosition));
        }

    }

    private bool CollidesWithTile(List<Collider> colliders, Vector2 newPosition, out Rectangle collidingTile)
    {
        if (newPosition.X != _position.X)
        {
            foreach (var collider in colliders)
            {
                var newXPositionBoundingBox = new Rectangle((int)newPosition.X, (int)_position.Y, _size.Width, _size.Height);
                if (newXPositionBoundingBox.Intersects(collider.CollidingTile))
                {
                    collidingTile = collider.CollidingTile;
                    return true;
                }
            }
        }

        if (newPosition.Y != _position.Y)
        {
            foreach (var collider in colliders)
            {
                var newYPositionBoundingBox = new Rectangle((int)_position.X, (int)newPosition.Y, _size.Width, _size.Height);
                if (newYPositionBoundingBox.Intersects(collider.CollidingTile))
                {
                    collidingTile = collider.CollidingTile;
                    return true;
                }
            }
        }

        collidingTile = new Rectangle(0, 0, 0, 0);
        return false;
    }

    public abstract void Update();

    public abstract void UpdatePosition();

    public abstract void UpdateAnimation();

    public abstract void DrawToRenderTarget();

    public abstract void Draw();


}
