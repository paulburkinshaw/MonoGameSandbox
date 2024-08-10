using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Tiled.NET;
using Tiled.NET.Models;

namespace Platformer008;

public class TileCollider
{
    public Rectangle CollidingTile;  
}

public class Tilemap : TiledTilemap
{
    private Dictionary<int, Texture2D> _tilesetTexturesDictionary = new();
    private RenderTarget2D _renderTarget;

    private static TiledTile[,] _tiles;

    private static TileCollider[,] _tileColliders;

    public Tilemap(IFileSystem fileSystem, 
        ITiledTilemapJsonService tiledTilemapJsonService, 
        string filePath) : base(fileSystem, tiledTilemapJsonService, filePath)
    {
        InitializeTilemap();
    }

    public Tilemap(string filePath) : base(filePath)
    {
        InitializeTilemap();
    }

    private void InitializeTilemap()
    {
        // get tileset textures
        foreach (var tileset in Tilesets)
        {
            var _tilesetTexture = Globals.Content.Load<Texture2D>($"Tilesets/{tileset.Name}");
            _tilesetTexturesDictionary.Add(tileset.FirstGID, _tilesetTexture);
        }

        _tiles = Layers.Where(x => x.Name == "CollidableTiles").FirstOrDefault().Tiles;

        _tileColliders = new TileCollider[_tiles.GetLength(0), _tiles.GetLength(1)];

        _renderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);

        Globals.GraphicsDevice.SetRenderTarget(_renderTarget);
        Globals.GraphicsDevice.Clear(Color.Transparent);
        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        for (int y = 0; y < TileCountY; y++)
        {
            for (int x = 0; x < TileCountX; x++)
            {
                var tile = _tiles[y, x];

                if (tile == null) continue;

                var positionX = x * TileWidth;
                var positionY = y * TileHeight;

                var tileset = Tilesets.Where(x => x.FirstGID == tile.TileSetId);

                _tileColliders[y, x] = new TileCollider
                {
                    CollidingTile = new Rectangle(positionX, positionY, TileWidth, TileHeight)
                };

                Globals.SpriteBatch.Draw(_tilesetTexturesDictionary[tile.TileSetId], new Vector2(positionX, positionY), new Rectangle(tile.TileSourceRectangle.X, tile.TileSourceRectangle.Y, tile.TileSourceRectangle.Width, tile.TileSourceRectangle.Height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    public List<TileCollider> GetNearestColliders(Rectangle boundingBox)
    {
        int yLength = _tiles.GetLength(0) - 1;
        int xLength = _tiles.GetLength(1);

        int leftTile = (int)Math.Floor((float)boundingBox.Left / TileWidth);
        int rightTile = (int)Math.Ceiling((float)boundingBox.Right / TileWidth);
        int topTile = (int)Math.Floor((float)boundingBox.Top / TileHeight);
        int bottomTile = (int)Math.Ceiling((float)boundingBox.Bottom / TileHeight);

        leftTile = MathHelper.Clamp(leftTile, 0, xLength);
        rightTile = MathHelper.Clamp(rightTile, 0, xLength);
        topTile = MathHelper.Clamp(topTile, 0, yLength);
        bottomTile = MathHelper.Clamp(bottomTile, 0, yLength);

        List<TileCollider> nearestTileColliders = [];

        for (int y = topTile; y <= bottomTile; y++)
        {
            for (int x = leftTile; x <= rightTile; x++)
            {
                if (_tiles[y, x] != null)
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
