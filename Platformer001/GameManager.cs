using Platformer001.Sprites;

namespace Platformer001;

public class GameManager
{
    private Knight _knight;

    public void Init()
    {
        _knight = new();
    }

    public void Update()
    {
        InputManager.Update();
        _knight.Update();
    }

    public void Draw()
    {
        _knight.Draw();
    }
}
