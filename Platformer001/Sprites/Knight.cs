using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Platformer001.Sprites;

public class Knight
{
    private Vector2 _position = new(300, 200);
    private float _speed = 200f;
    private bool _jumping = false;
    private bool _attacking1 = false;
    private bool _attacking2 = false;
    private readonly AnimationManager _animationManager = new();

    public Knight()
    {
        var knightTexture = Globals.Content.Load<Texture2D>("knight_spritesheet");
        var knightAnimationsJsonFile = File.ReadAllText(@"Content\knight_spritesheet_array.json");

        _animationManager.AddAnimations(knightAnimationsJsonFile, knightTexture);

        InputManager.JumpKeyPressed += OnJumpKeyPressed;
        InputManager.Attack1KeyPressed += OnAttack1KeyPressed;
        InputManager.Attack2KeyPressed += OnAttack2KeyPressed;
        AnimationManager.AnimationComplete += OnAnimationComplete;
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

    void OnAnimationComplete(object sender, AnimationCompleteEventArgs args)
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

    public void Update()
    {
        UpdateSpeed();
        UpdatePosition();
        UpdateAnimation();
    }

    private void UpdateSpeed()
    {
        if (InputManager.Running)
        {
            _speed = 300f;
        }
        else
        {
            _speed = 200f;
        }
    }

    private void UpdatePosition()
    {
        if (InputManager.Moving)
        {
            _position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.ElapsedGameTimeSeconds;
        }
    }

    private void UpdateAnimation()
    {
        if (InputManager.Moving && _jumping || _jumping)
        {
            _animationManager.Update(AnimationType.Jump);
        }
        else if (InputManager.Moving && InputManager.Running)
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
        else if (InputManager.Moving)
        {
            _animationManager.Update(AnimationType.Walk);
        }
        else
        {
            _animationManager.Update(AnimationType.Ready);
        }
    }

    public void Draw()
    {
        _animationManager.Draw(_position);
    }
}
