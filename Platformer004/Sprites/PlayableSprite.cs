using Microsoft.Xna.Framework;
using Platformer004.Managers;
using System;

namespace Platformer004.Sprites;
public class PlayableSprite : Sprite
{
    private const float JUMP = 200f;
    private float _speed => GetSpeed();
    private Vector2 _velocity;
    private bool _jumpKeyPressed = false;
    private bool _jumping = false;
    private bool _attacking1 = false;
    private bool _attacking2 = false;
    private bool _falling = false;
    private bool _standing = false;
    private bool _onGround;
    private InputManager _inputManager;

    public bool Attacking => _attacking1 || _attacking2;

    public PlayableSprite(Vector2 position,
        Rectangle size,
        SpriteContent spriteContent,
        AnimationManager animationManager,
        InputManager inputManager) : base(position, size, spriteContent, animationManager)
    {
        _inputManager = inputManager;
        _inputManager.JumpKeyPressed += OnJumpKeyPressed;
        _inputManager.Attack1KeyPressed += OnAttack1KeyPressed;
        _inputManager.Attack2KeyPressed += OnAttack2KeyPressed;
    }

    private float GetSpeed()
    {
        return _inputManager.Running ? 130f : 70f;
    }

    void OnJumpKeyPressed(object sender, EventArgs args)
    {
        _jumpKeyPressed = true;
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
    public void OnHit()
    {
        _falling = true;
    }
    public void OnStand()
    {
        _standing = true;
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

        _velocity.Y += Globals.Physics.Gravity * Globals.ElapsedGameTimeSeconds;

        if (_jumpKeyPressed && _onGround)
        {
            _velocity.Y = -JUMP;
            _jumpKeyPressed = false;
        }
    }

    public override void UpdatePosition()
    {
        _onGround = false;

        var movementAmount = _velocity * Globals.ElapsedGameTimeSeconds;

        MoveX(movementAmount.X, OnCollidesWithTileX);
        MoveY(movementAmount.Y, OnCollidesWithTileY);
        
    }

    private void OnCollidesWithTileX(CollidesWithTileEventArgs args)
    {    
        if (args.NewPosition > _position.X)
        {
            _position.X = args.CollidingTile.Left - _size.Width;
        }
        else
        {
            _position.X = args.CollidingTile.Right;
        }
    }

    private void OnCollidesWithTileY(CollidesWithTileEventArgs args)
    {
        if (_velocity.Y > 0)
        {
            _position.Y = args.CollidingTile.Top - _size.Height;
            _onGround = true;
            _velocity.Y = 0;
        }
        else
        {
            _position.Y = args.CollidingTile.Bottom;
            _velocity.Y = 0;
        }
    }

    public override void UpdateAnimation()
    {
        if (_inputManager.Moving && _jumping || _jumping)
        {
            _animationManager.Update(AnimationType.Jump);
        }
        else if (_inputManager.Moving && _falling || _falling)
        {
            _animationManager.Update(AnimationType.Fall);
        }
        else if (_inputManager.Moving && _standing || _standing)
        {
            _animationManager.Update(AnimationType.Standup);
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

    public override void OnAnimationComplete(object sender, AnimationCompleteEventArgs args)
    {
        if (args.AnimationType == AnimationType.Jump)
        {
            _jumping = false;
        }
        if (args.AnimationType == AnimationType.Fall)
        {
            _falling = false;
            OnStand();
        }
        if (args.AnimationType == AnimationType.Standup)
        {
            _standing = false;
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

    public override void DrawToRenderTarget()
    {
        _animationManager.DrawToRenderTarget(_position);
    }

    public override void Draw()
    {
        _animationManager.Draw();
    }

}
