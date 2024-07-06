using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Platformer005;

public class Collider
{
    public Rectangle CollidingTile;
    public Matrix Matrix;
    public Color[,] ColourData;
}

public class TileMap
{
    private readonly RenderTarget2D _renderTarget;
    public static readonly int TILE_SIZE = 32;

    // 6 rows(y), 10 columns(x)
    public static readonly int[,] tiles = new int[6, 10] {
        {0,0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0,0 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private static Collider[,] Colliders { get; } = new Collider[tiles.GetLength(0), tiles.GetLength(1)];

    public TileMap()
    {
        _renderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);

        var tile1Texture = Globals.Content.Load<Texture2D>("Tile1");

        Globals.GraphicsDevice.SetRenderTarget(_renderTarget);
        Globals.GraphicsDevice.Clear(Color.Transparent);
        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                if (tiles[y, x] == 0) continue;

                var positionX = x * TILE_SIZE;
                var positionY = y * TILE_SIZE;

                Colliders[y, x] = new Collider
                {
                    CollidingTile = new Rectangle(positionX, positionY, TILE_SIZE, TILE_SIZE),
                    Matrix = GetTranformationMatrix(positionX, positionY),
                    ColourData = Globals.GetColourDataFromTexture(tile1Texture, new Rectangle(0, 0, 32, 32))
                };
             
                Globals.SpriteBatch.Draw(tile1Texture, new Vector2(positionX, positionY), new Rectangle(0, 0, 32, 32), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }
        }

        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);

    }

    public static List<Collider> GetNearestColliders(Rectangle boundingBox)
    {
        int leftTile = (int)Math.Floor((float)boundingBox.Left / TILE_SIZE);
        int rightTile = (int)Math.Ceiling((float)boundingBox.Right / TILE_SIZE) - 1;
        int topTile = (int)Math.Floor((float)boundingBox.Top / TILE_SIZE);
        int bottomTile = (int)Math.Ceiling((float)boundingBox.Bottom / TILE_SIZE) - 1;

        var leftTile2 = MathHelper.Clamp(leftTile, 0, tiles.GetLength(1));
        var rightTile2 = MathHelper.Clamp(rightTile, 0, tiles.GetLength(1));
        var topTile2 = MathHelper.Clamp(topTile, 0, tiles.GetLength(0));
        var bottomTile2 = MathHelper.Clamp(bottomTile, 0, tiles.GetLength(0));

        List<Collider> colliders = [];

        for (int y = topTile2; y <= bottomTile2; y++)
        {
            for (int x = leftTile2; x <= rightTile2; x++)
            {
                if (tiles[y, x] != 0)
                {
                    colliders.Add(Colliders[y, x]);
                }
            }
        }

        return colliders;
    }

    private Matrix GetTranformationMatrix(int positionX, int positionY)
    {
        return Matrix.CreateTranslation(positionX, positionY, 0);
    }

    public void Draw()
    {
        Globals.SpriteBatch.Draw(_renderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);
    }
}
