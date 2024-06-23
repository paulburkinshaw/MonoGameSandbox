using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Platformer003.Managers;
public abstract class InputManager
{
    protected KeyboardState _keyboardStateOld = Keyboard.GetState();
    protected Vector2 _direction;
    protected bool _running = false;

    public event EventHandler JumpKeyPressed = delegate { };
    public event EventHandler Attack1KeyPressed = delegate { };
    public event EventHandler Attack2KeyPressed = delegate { };

    protected void OnJumpKeyPressed()
    {
        JumpKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    protected void OnAttack1KeyPressed()
    {
        Attack1KeyPressed?.Invoke(this, EventArgs.Empty);
    }

    protected void OnAttack2KeyPressed()
    {
        Attack2KeyPressed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 Direction => _direction;
    public bool Moving => _direction.X != 0;
    public bool Running => _running;

    public abstract void Update();
}

