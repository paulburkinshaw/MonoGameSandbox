using Microsoft.Xna.Framework;
using Platformer008.Managers;
using Platformer008.Sprites;
using System;

namespace Platformer008.Entities;
public class Player : Actor
{
    private const float JUMP = 200f;
    private float _speed => GetSpeed();
    private Vector2 _velocity;
    private int _health;
    private bool _jumpKeyPressed = false;
    private bool _jumping = false;
    private bool _attacking1 = false;
    private bool _attacking2 = false;
    private bool _falling = false;
    private bool _standing = false;
    private bool _blocking = false;
    private bool _reverseBlocking = false;
    private bool _onGround;
    private InputManager _inputManager;

    public int Health => _health;
    public bool Attacking => _attacking1 || _attacking2;

    public Player(Vector2 position,
        Rectangle size,
        GameSprite gameSprite,
        InputManager inputManager,
        Tilemap tilemap,
        string id) : base(position, size, gameSprite, tilemap, id)
    {
        _health = 100;

        _inputManager = inputManager;
        _inputManager.JumpKeyPressed += OnJumpKeyPressed;
        _inputManager.Attack1KeyPressed += OnAttack1KeyPressed;
        _inputManager.Attack2KeyPressed += OnAttack2KeyPressed;
        _inputManager.BlockKeyPressed += OnBlockKeyPressed;
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
    void OnBlockKeyPressed(object sender, EventArgs args)
    {
        if (_blocking)
        {
            _reverseBlocking = true;
            _blocking = false;
        }
        else
        {
            _blocking = true;
        }

    }

    public void OnHit(object sender, PlayerHitEventArgs args)
    {
        if (args.PlayerId == _id)
        {
            if (!_blocking)
            {
                _falling = true;
                _health -= 1;
            }
        }
    }

    public void OnPlayersCollide(Actor otherSprite)
    {
        if (otherSprite.Position.X >= _position.X)
        {
            _position = new Vector2(_position.X - 2, _position.Y);
        }
        else
        {
            _position = new Vector2(_position.X + 2, _position.Y);
        }
    }


    void OnStand()
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
            _gameSprite.Update(GameAnimationType.Jump);
        }
        else if (_inputManager.Moving && _falling || _falling)
        {
            _gameSprite.Update(GameAnimationType.Fall);
        }
        else if (_inputManager.Moving && _standing || _standing)
        {
            _gameSprite.Update(GameAnimationType.Standup);
        }
        else if (_inputManager.Moving && _inputManager.Running)
        {
            _gameSprite.Update(GameAnimationType.Run);
        }
        else if (_attacking1)
        {
            _gameSprite.Update(GameAnimationType.Attack1);
        }
        else if (_attacking2)
        {
            _gameSprite.Update(GameAnimationType.Attack2);
        }
        else if (_blocking)
        {
            _gameSprite.Update(GameAnimationType.Block);
        }
        else if (_reverseBlocking)
        {
            _gameSprite.Update(GameAnimationType.ReverseBlock);
        }
        else if (_inputManager.Moving)
        {
            _gameSprite.Update(GameAnimationType.Walk);
        }
        else
        {
            _gameSprite.Update(GameAnimationType.Ready);
        }
    }

    public override void OnGameAnimationComplete(object sender, GameAnimationCompleteEventArgs args)
    {
        if (args.AnimationType == GameAnimationType.Jump)
        {
            _jumping = false;
        }
        if (args.AnimationType == GameAnimationType.Fall)
        {
            _falling = false;
            OnStand();
        }
        if (args.AnimationType == GameAnimationType.Standup)
        {
            _standing = false;
        }
        if (args.AnimationType == GameAnimationType.Attack1)
        {
            _attacking1 = false;
        }
        if (args.AnimationType == GameAnimationType.Attack2)
        {
            _attacking2 = false;
        }
        if (args.AnimationType == GameAnimationType.Block && _inputManager.Moving)
        {
            _blocking = false;
        }
        if (args.AnimationType == GameAnimationType.ReverseBlock)
        {
            _reverseBlocking = false;
        }
    }

    public override void DrawToRenderTarget()
    {
        _gameSprite.DrawToRenderTarget(_position);
    }

    public override void Draw()
    {
        _gameSprite.Draw();
    }

}
