using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer002;

public class Physics
{
    public float GRAVITY => 1000;
}

public static class Globals
{
    public static float ElapsedGameTimeSeconds { get; set; }
    public static float ElapsedGameTimeMs { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
    public static GraphicsDevice GraphicsDevice { get; set; }
    public static Rectangle InternalSize { get; set; }
    public static Rectangle WindowSize { get; set; }
    public static Color Background { get; set; }
    public static Physics Physics => new();

    public static void Update(GameTime gt)
    {
        ElapsedGameTimeSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
        ElapsedGameTimeMs = (float)gt.ElapsedGameTime.TotalMilliseconds;
    }
}
