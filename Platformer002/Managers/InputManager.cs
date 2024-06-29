using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Platformer002.Managers;
public class InputManager
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

    public void Update()
    {
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.GetPressedKeyCount() > 0)
        {
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _direction.X--;
                _running = false;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                _direction.X++;
                _running = false;
            }

            if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D)) && keyboardState.IsKeyDown(Keys.LeftShift))
            {
                _running = true;
            }

            if (keyboardState.IsKeyDown(Keys.W) && _keyboardStateOld.IsKeyUp(Keys.W))
            {
                OnJumpKeyPressed();
                _direction.Y--;
            }

            if (keyboardState.IsKeyDown(Keys.Space) && _keyboardStateOld.IsKeyUp(Keys.Space))
            {
                OnAttack1KeyPressed();
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl) && _keyboardStateOld.IsKeyUp(Keys.LeftControl))
            {
                OnAttack2KeyPressed();
            }
        }

        _keyboardStateOld = keyboardState;

    }
}

