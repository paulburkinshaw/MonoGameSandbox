using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Platformer008;

public class GameTile
{
    public int TileSetId { get; set; }
    public int LocalTileId { get; set; }
    public Rectangle TileSourceRectangle { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}

public class TileCollider
{
    public Rectangle CollidingTile;
}

public class GameTilemap
{
    private RenderTarget2D _renderTarget;

    private GameTile[,] _gameTiles;

    private TileCollider[,] _tileColliders;

    private int _tileCountX;

    private int _tileCountY;

    private int _tileWidth;

    private int _tileHeight;

    private Dictionary<int, Texture2D> _tilesetTexturesDictionary;
    public GameTilemap(int tileCountX,
           int tileCountY,
           int tileWidth,
           int tileHeight,
           GameTile[,] gameTiles,
           Dictionary<int, Texture2D> tilesetTexturesDictionary) 
    {
        _tileCountX = tileCountX;
        _tileCountY = tileCountY;
        _tileWidth = tileWidth;
        _tileHeight = tileHeight;
        _gameTiles = gameTiles;
        _tilesetTexturesDictionary = tilesetTexturesDictionary;

        _tileColliders = GetTileCollidersFromTiles();
        DrawTilesToRenderTarget();
    }

    private TileCollider[,] GetTileCollidersFromTiles()
    {
        var tileColliders = new TileCollider[_gameTiles.GetLength(0), _gameTiles.GetLength(1)];

        for (int y = 0; y < _tileCountY; y++)
        {
            for (int x = 0; x < _tileCountX; x++)
            {
                var tile = _gameTiles[y, x];

                if (tile == null) 
                    continue;

                tileColliders[y, x] = new TileCollider
                {
                    CollidingTile = new Rectangle(tile.PositionX, tile.PositionY, _tileWidth, _tileHeight)
                };

            }
        }

        return tileColliders;
    }

    private void DrawTilesToRenderTarget()
    {
        _renderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);

        Globals.GraphicsDevice.SetRenderTarget(_renderTarget);
        Globals.GraphicsDevice.Clear(Color.Transparent);
        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        for (int y = 0; y < _tileCountY; y++)
        {
            for (int x = 0; x < _tileCountX; x++)
            {
                var tile = _gameTiles[y, x];

                if (tile == null) continue;

                Globals.SpriteBatch.Draw(_tilesetTexturesDictionary[tile.TileSetId],
                    new Vector2(tile.PositionX, tile.PositionY),
                    new Rectangle(tile.TileSourceRectangle.X, tile.TileSourceRectangle.Y, tile.TileSourceRectangle.Width, tile.TileSourceRectangle.Height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    public List<TileCollider> GetNearestTileColliders(Rectangle boundingBox)
    {
        int yLength = _gameTiles.GetLength(0) - 1;
        int xLength = _gameTiles.GetLength(1);

        int leftTile = (int)Math.Floor((float)boundingBox.Left / _tileWidth);
        int rightTile = (int)Math.Ceiling((float)boundingBox.Right / _tileWidth);
        int topTile = (int)Math.Floor((float)boundingBox.Top / _tileHeight);
        int bottomTile = (int)Math.Ceiling((float)boundingBox.Bottom / _tileHeight);

        leftTile = MathHelper.Clamp(leftTile, 0, xLength);
        rightTile = MathHelper.Clamp(rightTile, 0, xLength);
        topTile = MathHelper.Clamp(topTile, 0, yLength);
        bottomTile = MathHelper.Clamp(bottomTile, 0, yLength);

        List<TileCollider> nearestTileColliders = [];

        for (int y = topTile; y <= bottomTile; y++)
        {
            for (int x = leftTile; x <= rightTile; x++)
            {
                if (_gameTiles[y, x] != null)
                {
                    nearestTileColliders.Add(_tileColliders[y, x]);
                }
            }
        }

        return nearestTileColliders;
    }

    public void Draw()
    {
        Globals.SpriteBatch.Draw(_renderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);
    }
}
