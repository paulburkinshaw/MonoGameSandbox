using Aseprite.NET;
using Aseprite.NET.Converters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer008.Entities;
using Platformer008.Services;
using Platformer008.Sprites;
using System.IO.Abstractions;
using Tiled.NET;
using Tiled.NET.Converters;

namespace Platformer008;

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
    public GameAnimationFrame CurrentFrameA { get; set; }
    public GameAnimationFrame CurrentFrameB { get; set; }

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
    public static FileSystem FileSystem = new FileSystem();
    public static ITiledTilemapJsonConverterService TiledTilemapJsonConverterService = new TiledTilemapJsonConverterService();
    public static IAsepriteSpritesheetJsonConverterService AsepriteSpritesheetJsonConverterService = new AsepriteSpritesheetJsonConverterService();
    public static IAsepriteSpritesheetService AsepriteSpritesheetService = new AsepriteSpritesheetService(FileSystem, AsepriteSpritesheetJsonConverterService);
    public static ITiledTilemapService TiledTilemapService = new TiledTilemapService(FileSystem, TiledTilemapJsonConverterService);
    public static IGameSpriteService GameSpriteService = new GameSpriteService();
    public static IGameTilemapService GameTilemapService = new GameTilemapService();

    public static Color[,] GetColourDataFromTexture(Texture2D texture, Rectangle sourceRectangle)
    {
        Color[] colourData1D = new Color[sourceRectangle.Width * sourceRectangle.Height];
        texture.GetData(0, rect: sourceRectangle, colourData1D, 0, colourData1D.Length);

        Color[,] colourData2D = new Color[sourceRectangle.Width, sourceRectangle.Height];
        for (int x = 0; x < sourceRectangle.Width; x++)
        {
            for (int y = 0; y < sourceRectangle.Height; y++)
            {
                // Get colour data from 1d array:
                // x + (y * sourceRectangle.Width) calculates the column offset needed to move down a row to get the array index for the pixel colour value of the next row in the texture
                // by skipping a fixed number of elements in the array
                // so if we start at 0,0 in the texture 0 + (0 * 64) = 0 so we get the array index 0 which has the pixel colour of 0,0 stored at index 0 in the array 
                // if we move to at 0,1 in the texture 0 + (1 * 64) = 64 so we get the the pixel colour of the texture at 0,1 stored at index 64 in the array - one row down
                // if we move to at 0,2 in the texture 0 + (2 * 64) = 64 so we get the the pixel colour of the texture at 0,2 stored at index 128 in the array 
                // Load colour data into 2d array:
                // we then load the colour data into a 2d array using the x and y values making it easier to retrieve the colour data later
                var arrayIndexOffset = x + (y * sourceRectangle.Width);
                colourData2D[x, y] = colourData1D[arrayIndexOffset];
            }
        }
        return colourData2D;
    }

    public static bool SpriteTexturesCollide(Player spriteA, Player spriteB, CollisionData collisionData = null)
    {     
        var currentFrameNumberA = spriteA.CurrentAnimation.CurrentFrameNumber;
        var currentFrameNumberB = spriteB.CurrentAnimation.CurrentFrameNumber;

        var widthA = spriteA.CurrentAnimation.Frames[currentFrameNumberA].SourceRectangle.Width;
        var heightA = spriteA.CurrentAnimation.Frames[currentFrameNumberA].SourceRectangle.Height;
        var widthB = spriteB.CurrentAnimation.Frames[currentFrameNumberB].SourceRectangle.Width;
        var heightB = spriteB.CurrentAnimation.Frames[currentFrameNumberB].SourceRectangle.Height;

        var matrixAtoB = spriteA.Matrix * Matrix.Invert(spriteB.Matrix);
        var matrixBtoA = spriteB.Matrix * Matrix.Invert(spriteA.Matrix); // not currently used

        for (int x1 = 0; x1 < widthA; x1++)
        {
            for (int y1 = 0; y1 < heightA; y1++)
            {
                var pixelCoordinateA = new Vector2(x1, y1);

                // For each pixel of spriteA 1 we first want to find the corresponding screen coordinates, this is done by transforming spriteA's pixel coordinates with the matrix of spriteA
                // var screenCoordinateA = Vector2.Transform(pixelCoordinateA, matrixA);

                // for the screen coordinates we then want to find the corresponding pixel position in spriteB's texture, this is done by transforming the screen coordinates with the inverse of the spriteB matrix
                // var pixelCoordinateB = Vector2.Transform(screenCoordinateA, Matrix.Invert(spriteB.Matrix));

                // uncomment the above two lines and comment out the below line to use the long hand method

                // A shorthand way to do this is to transform spriteAs pixel coordinates with the matrix that is the combination of both the spriteAs matrix and the inverse of spriteB's matrix
                // transforming a pixel coordinate from spriteAs texture by this matrix will immediately give us the pixel coordinate in spriteB's texture
                var pixelCoordinateB = Vector2.Transform(pixelCoordinateA, matrixAtoB);

                int x2 = (int)pixelCoordinateB.X;
                int y2 = (int)pixelCoordinateB.Y;
                if (x2 >= 0 && x2 < widthB)
                {
                    if (y2 >= 0 && y2 < heightB)
                    {
                        var colourDataA = spriteA.CurrentAnimation.ColourData[new(spriteA.CurrentAnimation.AnimationType, currentFrameNumberA)];
                        var colourDataB = spriteB.CurrentAnimation.ColourData[new(spriteB.CurrentAnimation.AnimationType, currentFrameNumberB)];

                        if (colourDataA[x1, y1].A > 0)
                        {
                            if (colourDataB[x2, y2].A > 0)
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
