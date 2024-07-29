using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Platformer007.Managers;
public class InputManager
{
    protected KeyboardState _keyboardStateOld = Keyboard.GetState();
    protected Vector2 _direction;
    protected bool _running = false;

    Keys _leftKey;
    Keys _rightKey;
    Keys _attack1Key;
    Keys _attack2Key;
    Keys _runKey;
    Keys _jumpKey;
    Keys _blockKey;

    public event EventHandler JumpKeyPressed = delegate { };
    public event EventHandler Attack1KeyPressed = delegate { };
    public event EventHandler Attack2KeyPressed = delegate { };
    public event EventHandler BlockKeyPressed = delegate { };
    public Vector2 Direction => _direction;
    public bool Moving => _direction.X != 0;
    public bool Running => _running;

    public InputManager(
        Keys leftKey,
        Keys righttKey,
        Keys attack1Key,
        Keys attack2Key,
        Keys runKey,
        Keys jumpKey,
        Keys blockKey
        )
    {
        _leftKey = leftKey;
        _rightKey = righttKey;
        _attack1Key = attack1Key;
        _attack2Key = attack2Key;
        _runKey = runKey;
        _jumpKey = jumpKey;
        _blockKey = blockKey;
    }

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

    protected void OnBlockKeyPressed()
    {
        BlockKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    public void Update()
    {
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.GetPressedKeyCount() > 0)
        {
            if (keyboardState.IsKeyDown(_leftKey))
            {
                _direction.X--;
                _running = false;
            }

            if (keyboardState.IsKeyDown(_rightKey))
            {
                _direction.X++;
                _running = false;
            }

            if ((keyboardState.IsKeyDown(_leftKey) || keyboardState.IsKeyDown(_rightKey)) && keyboardState.IsKeyDown(_runKey))
            {
                _running = true;
            }

            if (keyboardState.IsKeyDown(_jumpKey) && _keyboardStateOld.IsKeyUp(_jumpKey))
            {
                OnJumpKeyPressed();
                _direction.Y--;
            }

            if (keyboardState.IsKeyDown(_attack1Key) && _keyboardStateOld.IsKeyUp(_attack1Key))
            {
                OnAttack1KeyPressed();
            }
            if (keyboardState.IsKeyDown(_attack2Key) && _keyboardStateOld.IsKeyUp(_attack2Key))
            {
                OnAttack2KeyPressed();
            }

            if (keyboardState.IsKeyDown(_blockKey) && _keyboardStateOld.IsKeyUp(_blockKey))
            {
                OnBlockKeyPressed();
            }
        }

        _keyboardStateOld = keyboardState;

    }
}

