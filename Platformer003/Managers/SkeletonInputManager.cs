using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Platformer003.Managers;

public class SkeletonInputManager : InputManager
{

    public override void Update()
    {
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.GetPressedKeyCount() > 0)
        {
            if (keyboardState.IsKeyDown(Keys.J))
            {
                _direction.X--;
                _running = false;
            }

            if (keyboardState.IsKeyDown(Keys.L))
            {
                _direction.X++;
                _running = false;
            }

            if ((keyboardState.IsKeyDown(Keys.J) || keyboardState.IsKeyDown(Keys.L)) && keyboardState.IsKeyDown(Keys.RightShift))
            {
                _running = true;
            }

            if (keyboardState.IsKeyDown(Keys.I) && _keyboardStateOld.IsKeyUp(Keys.I))
            {
                OnJumpKeyPressed();
                _direction.Y--;
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && _keyboardStateOld.IsKeyUp(Keys.Enter))
            {
                OnAttack1KeyPressed();
            }
            if (keyboardState.IsKeyDown(Keys.RightControl) && _keyboardStateOld.IsKeyUp(Keys.RightControl))
            {
                OnAttack2KeyPressed();
            }
        }

        _keyboardStateOld = keyboardState;

    }
}
