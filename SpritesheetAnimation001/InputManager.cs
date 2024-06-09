using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpritesheetAnimation001;

public static class InputManager
{
    private static KeyboardState _keyboardStateOld = Keyboard.GetState();
    private static Vector2 _direction;
    private static bool _running = false;
    

    public static event EventHandler JumpKeyPressed = delegate { };
    public static event EventHandler Attack1KeyPressed = delegate { };
    public static event EventHandler Attack2KeyPressed = delegate { };

    public static void OnJumpKeyPressed()
    {
        if (JumpKeyPressed != null)
            JumpKeyPressed(null, EventArgs.Empty);
    }
    public static void OnAttack1KeyPressed()
    {
        if (Attack1KeyPressed != null)
            Attack1KeyPressed(null, EventArgs.Empty);
    }
    public static void OnAttack2KeyPressed()
    {
        if (Attack2KeyPressed != null)
            Attack2KeyPressed(null, EventArgs.Empty);
    }


    public static Vector2 Direction => _direction;
    public static bool Moving => _direction.X != 0;
    public static bool Running => _running;

    public static void Update()
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

            if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D)) && keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
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

        // _running = false;
        _keyboardStateOld = keyboardState;

    }
}
