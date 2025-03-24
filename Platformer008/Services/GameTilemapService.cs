using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Tiled.NET.Models;

namespace Platformer008.Services
{
    public interface IGameTilemapService
    {
        GameTilemap MapTiledTilemapToGameTilemap(TiledTilemap tiledTilemap);
    }

    public class GameTilemapService : IGameTilemapService
    {
        public GameTilemap MapTiledTilemapToGameTilemap(TiledTilemap tiledTilemap)
        {
            // Map TiledTilemap to GameTilemap steps
            //  - Map TiledTiles to GameTiles
            //  - Build tilesetTexturesDictionary from tiledTilemap.Tilesets

            var tileLayer = tiledTilemap.Layers.Where(x => x.TiledLayerType == TiledLayerType.TileLayer).FirstOrDefault() as TileLayer;
            var tiledTiles = tileLayer.Tiles;

            GameTile[,] gameTiles = new GameTile[tileLayer.TileCountY, tileLayer.TileCountX];

            for (int y = 0; y < tiledTilemap.TileCountY; y++)
            {
                for (int x = 0; x < tiledTilemap.TileCountX; x++)
                {
                    var tiledTile = tiledTiles[y, x];

                    if (tiledTile == null)
                        continue;
                    
                    gameTiles[y, x] = new GameTile
                    {
                        TileSetId = tiledTile.TileSetId,
                        LocalTileId = tiledTile.LocalTileId,
                        TileSourceRectangle = new Rectangle(tiledTile.TileSourceRectangle.X,
                                                            tiledTile.TileSourceRectangle.Y,
                                                            tiledTile.TileSourceRectangle.Width,
                                                            tiledTile.TileSourceRectangle.Height),
                        PositionX = x * tiledTile.TileSourceRectangle.Width,
                        PositionY = y * tiledTile.TileSourceRectangle.Height
                    };

                }
            }

            var tilesetTexturesDictionary = new Dictionary<int, Texture2D>();

            foreach (var tileset in tiledTilemap.Tilesets)
            {
                var _tilesetTexture = Globals.Content.Load<Texture2D>($"Tilesets/{tileset.Name}");
                tilesetTexturesDictionary.Add(tileset.FirstGID, _tilesetTexture);
            }

            var gameTilemap = new GameTilemap(tileCountX: tiledTilemap.TileCountX,
                                              tileCountY: tiledTilemap.TileCountY,
                                              tileWidth: tiledTilemap.TileWidth,
                                              tileHeight: tiledTilemap.TileHeight,
                                              gameTiles: gameTiles,
                                              tilesetTexturesDictionary: tilesetTexturesDictionary);

            return gameTilemap;
        }



    }


}
