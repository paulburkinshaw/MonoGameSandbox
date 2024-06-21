using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Platformer002.Managers;

public class KnightInputManager : InputManager
{

    public override void Update()
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
