using Newtonsoft.Json;
using NUnit.Framework;
using System.ComponentModel;
using Tiled.NET.Models;

namespace Tiled.NET.Test
{
    public class TiledTilemapTests
    {
       
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void GetTilesForLayer_NoEncodedFlipFlags_ReturnsCorrectTiles()
        {
            var tiledTilemap = new TiledTilemap();

            var layer = new TiledLayer
            {
                Name = "Tiles",
                TileCountX = 10,
                TileCountY = 5,
                TiledLayerType = TiledLayerType.TileLayer,
                TileGIDs = new uint[50] {
                    1,2,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0
                }
            };

            var tilesets = new List<TiledTileset>
            {
                new TiledTileset
                {
                    FirstGID = 1,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,
                    Name = "tileset1"
                },
                 new TiledTileset
                {
                    FirstGID = 248,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,
                    Name = "tileset2"
                }
            };

            TiledTile[,] expected = new TiledTile[layer.TileCountY, layer.TileCountX];

            expected[0, 0] = new TiledTile
            {
                TileSetId = 1,
                LocalTileId = 0,
                TileSourceRectangle = new TileSourceRectangle
                {
                    X = 0,
                    Y = 0,
                    Width = 16,
                    Height = 16
                },
                TileFlipFlags = new TileFlipFlags
                {
                    FlippedDiagonally = false,
                    FlippedHorizontally = false,
                    FlippedVertically = false,
                    RotatedHex120 = false,
                }
            };
            expected[0, 1] = new TiledTile
            {
                TileSetId = 1,
                LocalTileId = 1,
                TileSourceRectangle = new TileSourceRectangle
                {
                    X = 16,
                    Y = 0,
                    Width = 16,
                    Height = 16
                },
                TileFlipFlags = new TileFlipFlags
                {
                    FlippedDiagonally = false,
                    FlippedHorizontally = false,
                    FlippedVertically = false,
                    RotatedHex120 = false,
                }
            };

            var result = tiledTilemap.GetTilesForLayer(layer, tilesets);

            for (int y = 0; y < layer.TileCountY; y++)
            {
                for (int x = 0; x < layer.TileCountX; x++)
                {
                    AreEqualByJson(expected[y, x], result[y, x]);
                }
            }

        }

        [Test]
        public void GetTilesForLayer_EncodedFlipFlags_ReturnsCorrectTiles()
        {
            var tiledTilemap = new TiledTilemap();

            // first 4 bits encoded with: 1010, the tile is flipped horizontally and antidiagonally - 90 degrees clockwise, or right rotation
            // LocalTileId = 79
            uint encodedTile1 = 2684354887;

            // first 4 bits encoded with: 0110, the tile is flipped vertically and antidiagonally - 90 degrees anticlockwise, or left rotation
            // LocalTileId = 79
            uint encodedTile2 = 1610613063;

            // first 4 bits encoded with: 1100, the tile is flipped horizontally and vertically - 180 degrees clockwise/anticlockwise, or top rotation
            // LocalTileId = 79
            uint encodedTile3 = 3221225799;

            // LocalTileId = 3
            uint unencodedTile = 251;

            var layer = new TiledLayer
            {
                Name = "Tiles",
                TileCountX = 10,
                TileCountY = 5,
                TiledLayerType = TiledLayerType.TileLayer,
                TileGIDs = new uint[50] {
                    encodedTile1,encodedTile2,encodedTile3,unencodedTile,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,0,0
                }
            };

            var tilesets = new List<TiledTileset>
            {
                 new TiledTileset
                {
                    FirstGID = 1,
                    ImageWidth = 300,
                    ImageHeight = 300,
                    TileWidth = 32,
                    TileHeight = 32,
                    Name = "tileset1"
                },
                new TiledTileset
                {
                    FirstGID = 248,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,
                    Name = "tileset2"
                }
            };

            TiledTile[,] expected = new TiledTile[layer.TileCountY, layer.TileCountX];
            expected[0, 0] = new TiledTile
            {
                TileSetId = 248,
                LocalTileId = 79,
                TileSourceRectangle = new TileSourceRectangle
                {
                    X = 64,
                    Y = 96,
                    Width = 16,
                    Height = 16
                },
                TileFlipFlags = new TileFlipFlags
                {
                    FlippedHorizontally = true,
                    FlippedVertically = false,
                    FlippedDiagonally = true,
                    RotatedHex120 = false,
                }
            };
            expected[0, 1] = new TiledTile
            {
                TileSetId = 248,
                LocalTileId = 79,
                TileSourceRectangle = new TileSourceRectangle
                {
                    X = 64,
                    Y = 96,
                    Width = 16,
                    Height = 16
                },
                TileFlipFlags = new TileFlipFlags
                {
                    FlippedHorizontally = false,
                    FlippedVertically = true,
                    FlippedDiagonally = true,
                    RotatedHex120 = false,
                }
            };
            expected[0, 2] = new TiledTile
            {
                TileSetId = 248,
                LocalTileId = 79,
                TileSourceRectangle = new TileSourceRectangle
                {
                    X = 64,
                    Y = 96,
                    Width = 16,
                    Height = 16
                },
                TileFlipFlags = new TileFlipFlags
                {
                    FlippedHorizontally = true,
                    FlippedVertically = true,
                    FlippedDiagonally = false,
                    RotatedHex120 = false,
                }
            };
            expected[0, 3] = new TiledTile
            {
                TileSetId = 248,
                LocalTileId = 3,
                TileSourceRectangle = new TileSourceRectangle
                {
                    X = 48,
                    Y = 0,
                    Width = 16,
                    Height = 16
                },
                TileFlipFlags = new TileFlipFlags
                {
                    FlippedHorizontally = false,
                    FlippedVertically = false,
                    FlippedDiagonally = false,
                    RotatedHex120 = false,
                }
            };

            var result = tiledTilemap.GetTilesForLayer(layer, tilesets);

            for (int y = 0; y < layer.TileCountY; y++)
            {
                for (int x = 0; x < layer.TileCountX; x++)
                {
                    AreEqualByJson(expected[y, x], result[y, x]);
                }
            }
        }


        #region Helper Methods

        private static void AreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        #endregion
    }
}