using Microsoft.Xna.Framework;
using Platformer008.Sprites;
using System;
using System.Collections.Generic;

namespace Platformer008.Entities;

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

public class CollidesWithSpriteEventArgs
{
    public readonly Actor CollidingSprite;
    public readonly int MovementAmount;
    public readonly float NewPosition;

    public CollidesWithSpriteEventArgs(Actor collidingSprite, int movementAmount, float newPosition)
    {
        CollidingSprite = collidingSprite;
        MovementAmount = movementAmount;
        NewPosition = newPosition;
    }
}


public abstract class Actor
{
    protected Vector2 _position;
    protected Rectangle _size;
    protected GameSprite _gameSprite;
    protected GameAnimation _currentAnimation;
    protected string _id;
    Vector2 _origin = Vector2.Zero;
    float _rotation = 0;
    private Tilemap _tilemap;


    public Rectangle BoundingBox => GetBoundingBox();
    public GameAnimation CurrentAnimation => _currentAnimation;
    public Matrix Matrix => GetTranformationMatrix();

    public Vector2 Position => _position;

    public Actor(Vector2 position,
        Rectangle size,
        GameSprite gameSprite,
        Tilemap tilemap,
        string id)
    {
        _position = position;
        _size = size;
        _gameSprite = gameSprite;
        _tilemap = tilemap;
        _id = id;

        _gameSprite.GameAnimationStarted += OnGameAnimationStarted;
        _gameSprite.GameAnimationComplete += OnGameAnimationComplete;
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

    public virtual void OnGameAnimationStarted(object sender, GameAnimationStartedEventArgs args)
    {
        _currentAnimation = args.Animation;
    }

    public virtual void OnGameAnimationComplete(object sender, GameAnimationCompleteEventArgs args)
    {
        var test = "";
        // _currentAnimation = null;
    }

    public virtual void MoveX(float movementAmount, Action<CollidesWithTileEventArgs> onCollidesWithTile)
    {
        int xAmount = (int)Math.Round(movementAmount);

        var newXPosition = _position.X + xAmount;
        var newXPositionBoundingBox = new Rectangle((int)newXPosition, (int)_position.Y, _size.Width, _size.Height);

        var colliders = _tilemap.GetNearestColliders(newXPositionBoundingBox);

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

        var colliders = _tilemap.GetNearestColliders(newYPositionBoundingBox);

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

    private bool CollidesWithTile(List<TileCollider> colliders, Vector2 newPosition, out Rectangle collidingTile)
    {
        if (newPosition.X != _position.X)
        {
            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;

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
                if (collider == null)
                    continue;

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
