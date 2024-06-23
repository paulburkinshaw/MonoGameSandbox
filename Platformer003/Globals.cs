using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer003;

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


    public static Color[,] GetColourDataFromTexture(Texture2D texture, Rectangle sourceRectangle)
    {
        Color[] colourData1D = new Color[sourceRectangle.Width * sourceRectangle.Height];
        texture.GetData(0, rect: sourceRectangle, colourData1D, 0, colourData1D.Length);

        Color[,] colourData2D = new Color[sourceRectangle.Width, sourceRectangle.Height];
        for (int x = 0; x < sourceRectangle.Width; x++)
        {
            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                var arrayIndexOffset = x + (y * sourceRectangle.Width);
                colourData2D[x, y] = colourData1D[arrayIndexOffset];
            }
        }
        return colourData2D;
    }

    public static bool TexturesCollide(Color[,] texture1, Matrix matrix1, Color[,] texture2, Matrix matrix2)
    {

        return false;
    }

    public static void Update(GameTime gt)
    {
        ElapsedGameTimeSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
        ElapsedGameTimeMs = (float)gt.ElapsedGameTime.TotalMilliseconds;
    }
}
