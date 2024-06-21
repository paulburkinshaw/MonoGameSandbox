using Microsoft.Xna.Framework;
using Platformer002.Managers;
using System;

namespace Platformer002.Sprites;
public class Skeleton : Sprite
{
    private const float JUMP = 200f;
    private float _speed => GetSpeed();
    private Vector2 _velocity;
    private bool _jumping = false;
    private bool _attacking1 = false;
    private bool _attacking2 = false;
    private bool _onGround;
    private SkeletonInputManager _inputManager;

    public Skeleton(Vector2 position,
        Rectangle size,
        SpriteContent spriteContent,
        AnimationManager animationManager,
        SkeletonInputManager inputManager) : base(position, size, spriteContent, animationManager)
    {
        _inputManager = inputManager;
        _inputManager.JumpKeyPressed += OnJumpKeyPressed;
        _inputManager.Attack1KeyPressed += OnAttack1KeyPressed;
        _inputManager.Attack2KeyPressed += OnAttack2KeyPressed;
    }

    private float GetSpeed()
    {
        return _inputManager.Running ? 130f : 100f;
    }

    void OnJumpKeyPressed(object sender, EventArgs args)
    {
        _jumping = true;
    }
    void OnAttack1KeyPressed(object sender, EventArgs args)
    {
        _attacking1 = true;
    }
    void OnAttack2KeyPressed(object sender, EventArgs args)
    {
        _attacking2 = true;
    }

    public override void OnAnimationComplete(object sender, AnimationCompleteEventArgs args)
    {
        if (args.AnimationType == AnimationType.Jump)
        {
            _jumping = false;
        }
        if (args.AnimationType == AnimationType.Attack1)
        {
            _attacking1 = false;
        }
        if (args.AnimationType == AnimationType.Attack2)
        {
            _attacking2 = false;
        }
    }

    public override void Update()
    {
        _inputManager.Update();
        UpdateVelocity();
        UpdatePosition();
        UpdateAnimation();
    }

    private void UpdateVelocity()
    {
        if (_inputManager.Moving && _inputManager.Direction == new Vector2(1, 0))
        {
            _velocity.X = _speed;
        }
        else if (_inputManager.Moving && _inputManager.Direction == new Vector2(-1, 0))
        {
            _velocity.X = -_speed;
        }
        else
        {
            _velocity.X = 0;
        }

        _velocity.Y += Globals.Physics.GRAVITY * Globals.ElapsedGameTimeSeconds;

        if (_jumping && _onGround)
        {
            _velocity.Y = -JUMP;
            _jumping = false;
        }
    }

    public override void UpdatePosition()
    {
        _onGround = false;

        var movementAmount = _velocity * Globals.ElapsedGameTimeSeconds;
        var movementAmountRounded = new Vector2((float)Math.Round(movementAmount.X), (float)Math.Round(movementAmount.Y));

        var newPosition = _position + movementAmountRounded;

        var newPositionBoundingBox = GetBoundingBox(newPosition);

        var colliders = Map.GetNearestColliders(newPositionBoundingBox);

        foreach (var collider in colliders)
        {
            var collidingTile = collider.CollidingTile;

            if (newPosition.X != _position.X)
            {
                var newPositionXBoundingBox = GetBoundingBox(new Vector2(newPosition.X, _position.Y));
                if (newPositionXBoundingBox.Intersects(collidingTile))
                {
                    if (newPosition.X > _position.X) newPosition.X = collidingTile.Left - _size.Width;
                    else newPosition.X = collidingTile.Right;
                    continue;
                }
            }

            var newPositionYBoundingBox = GetBoundingBox(new Vector2(_position.X, newPosition.Y));
            if (newPositionYBoundingBox.Intersects(collidingTile))
            {
                if (_velocity.Y > 0)
                {
                    newPosition.Y = collidingTile.Top - _size.Height;
                    _onGround = true;
                    _velocity.Y = 0;
                }
                else
                {
                    newPosition.Y = collidingTile.Bottom;
                    _velocity.Y = 0;
                }
            }
        }

        _position = newPosition;
    }

    public override void UpdateAnimation()
    {
        if (_inputManager.Moving && _jumping || _jumping)
        {
            _animationManager.Update(AnimationType.Jump);
        }
        else if (_inputManager.Moving && _inputManager.Running)
        {
            _animationManager.Update(AnimationType.Run);
        }
        else if (_attacking1)
        {
            _animationManager.Update(AnimationType.Attack1);
        }
        else if (_attacking2)
        {
            _animationManager.Update(AnimationType.Attack2);
        }
        else if (_inputManager.Moving)
        {
            _animationManager.Update(AnimationType.Walk);
        }
        else
        {
            _animationManager.Update(AnimationType.Ready);
        }
    }

    public override void DrawToRenderTarget()
    {
        _animationManager.DrawToRenderTarget(_position);
    }

    public override void Draw()
    {
        _animationManager.Draw();
    }
}
