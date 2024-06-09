using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer001;

public static class Globals
{
    public static float ElapsedGameTimeSeconds { get; set; }

    public static float ElapsedGameTimeMs { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }

    public static void Update(GameTime gt)
    {
        ElapsedGameTimeSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
        ElapsedGameTimeMs = (float)gt.ElapsedGameTime.TotalMilliseconds;
    }
}
