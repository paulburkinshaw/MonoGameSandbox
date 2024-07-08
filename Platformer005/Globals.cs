using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer005.Sprites;
using System;

namespace Platformer005;

public class Physics
{
    public float Gravity => 1000;
    public static float FramesPerSecond = 10;
    public float Frameduration = 1 / FramesPerSecond * 1000;
} 

public class CollisionData
{
    public Vector2 PixelCoordinatesA { get; set; }
    public Vector2 PixelCoordinatesB { get; set; }
    public Vector2 ScreenCoordinates {  get; set; }
    public Frame CurrentFrameA { get; set; }
    public Frame CurrentFrameB { get; set; }

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

    public static bool SpriteTexturesCollide(PlayableSprite spriteA, PlayableSprite spriteB, CollisionData collisionData = null)
    {     
        var currentFrameNumberA = spriteA.CurrentAnimation.CurrentFrameNumber;
        var currentFrameNumberB = spriteB.CurrentAnimation.CurrentFrameNumber;

        var widthA = spriteA.CurrentAnimation.Frames[currentFrameNumberA].FrameSourceRectangle.Width;
        var heightA = spriteA.CurrentAnimation.Frames[currentFrameNumberA].FrameSourceRectangle.Height;
        var widthB = spriteB.CurrentAnimation.Frames[currentFrameNumberB].FrameSourceRectangle.Width;
        var heightB = spriteB.CurrentAnimation.Frames[currentFrameNumberB].FrameSourceRectangle.Height;

        var matrixAtoB = spriteA.Matrix * Matrix.Invert(spriteB.Matrix);
        var matrixBtoA = spriteB.Matrix * Matrix.Invert(spriteA.Matrix);

        for (int x1 = 0; x1 < widthA; x1++)
        {
            for (int y1 = 0; y1 < heightA; y1++)
            {
                var pixelCoordinateA = new Vector2(x1, y1);
                var pixelCoordinateB = Vector2.Transform(pixelCoordinateA, matrixAtoB);

                int x2 = (int)pixelCoordinateB.X;
                int y2 = (int)pixelCoordinateB.Y;
                if (x2 >= 0 && x2 < widthB)
                {
                    if (y2 >= 0 && y2 < heightB)
                    {
                        var colourData1 = spriteA.CurrentAnimation.ColourData[new(spriteA.CurrentAnimation.AnimationType, currentFrameNumberA)];
                        var colourData2 = spriteB.CurrentAnimation.ColourData[new(spriteB.CurrentAnimation.AnimationType, currentFrameNumberB)];

                        if (colourData1[x1, y1].A > 0)
                        {
                            if (colourData2[x2, y2].A > 0)
                            {
                                if (collisionData != null)
                                {
                                    collisionData.PixelCoordinatesA = pixelCoordinateA;
                                    collisionData.PixelCoordinatesB = pixelCoordinateB;       
                                    collisionData.CurrentFrameA = spriteA.CurrentAnimation.Frames[currentFrameNumberA];
                                    collisionData.CurrentFrameB = spriteB.CurrentAnimation.Frames[currentFrameNumberB];
                                    collisionData.ScreenCoordinates = Vector2.Transform(pixelCoordinateA, spriteA.Matrix);
                                }
                                                 
                                return true;
                            }
                        }
                    }
                }

            }
        }

        if (collisionData != null)
        {
            collisionData.PixelCoordinatesA = Vector2.Zero;
            collisionData.PixelCoordinatesB = Vector2.Zero;
            collisionData.ScreenCoordinates = Vector2.Zero;
        }
        return false;
    }

    public static void Update(GameTime gameTime)
    {
        ElapsedGameTimeSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        ElapsedGameTimeMs = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    
}
