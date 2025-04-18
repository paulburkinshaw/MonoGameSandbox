using Moq;
using Newtonsoft.Json;
using System.IO.Abstractions.TestingHelpers;
using Tiled.NET.Converters;
using Tiled.NET.DTOs;
using Tiled.NET.Models;

namespace Tiled.NET.Test
{
    public class TiledTilemapServiceTests
    {
        TilemapDTO _tilemapDTO;

        [SetUp]
        public void Setup()
        {
            _tilemapDTO = new TilemapDTO
            {
                Width = 10,
                Height = 5,
                TileHeight = 8,
                TileWidth = 8,
                LayerDTOs = new List<LayerDTO> {
                    new LayerDTO
                    {
                        Name = "Tiles",
                        Width = 10,
                        Height = 5,
                        TileGIDs = new uint[50] {
                            1,2,0,0,0,0,0,0,0,0,
                            0,0,0,0,0,0,0,0,0,0,
                            0,0,0,0,0,0,0,0,0,0,
                            0,0,0,0,0,0,0,0,0,0,
                            0,0,0,0,0,0,0,0,0,0
                        }
                    }
                },
                TilesetDTOs = new List<TilesetDTO>
                {
                    new TilesetDTO
                    {
                        Name = "tileset1",
                        Image = "tileset1.png",
                        FirstGID = 1,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16
                    },
                     new TilesetDTO
                    {
                        Name = "tileset2",
                        Image = "tileset2.png",
                        FirstGID = 248,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16
                    }
                }
            };
        }


        [Test]
        public void GetTiledTilemap_NoEncodedFlipFlags_ReturnsCorrectTiledTilemap()
        {
            // Arrange
            var mockTilemapJsonString = "mockTilemapJsonString";

            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"\Content\Tilemaps\MockTilemap.tmj", new MockFileData(mockTilemapJsonString) }
            });

            var mockTilemapFilepath = @"\Content\Tilemaps\MockTilemap.tmj";

            var tilemapDTO = _tilemapDTO;
            _tilemapDTO.LayerDTOs = new List<LayerDTO> {
                new LayerDTO
                {
                    Name = "Tiles",
                    Type =  "tilelayer",
                    Width = 10,
                    Height = 5,
                    TileGIDs = new uint[50] {
                        1,2,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0
                    }
                }
            };

            _tilemapDTO.TilesetDTOs = new List<TilesetDTO>
            {
                new TilesetDTO
                {
                    Name = "tileset1",
                    Image = "tileset1.png",
                    FirstGID = 1,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,

                },
                new TilesetDTO
                {
                    Name = "tileset2",
                    Image = "tileset2.png",
                    FirstGID = 248,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,
                }
            };

            _tilemapDTO.PropertyDTOs = new List<PropertyDTO>
            {
                new PropertyDTO
                {
                    Name = "Property1",
                    Type = "bool",
                    Value = true
                },
                new PropertyDTO
                {
                    Name = "Property2",
                    Type = "string",
                    Value = "Property2 Value"
                },
            };

            var tiledTilemapJsonConverterServiceMock = new Mock<ITiledTilemapJsonConverterService>();

            tiledTilemapJsonConverterServiceMock
                .Setup(x => x.GetTilemapDTOFromJsonFile(It.Is<string>(tilemapJsonString => tilemapJsonString == mockTilemapJsonString)))
                .Returns(tilemapDTO);

            var expected = new
            {             
                Layers = new List<TileLayer>
                {
                    new TileLayer {
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
                        },
                        Tiles = new TiledTile[5, 10]
                    }
                },
                Tilesets = new List<TiledTileset>
                {
                    new TiledTileset
                    {
                        Name = "tileset1",
                        ImageName = "tileset1.png",
                        FirstGID = 1,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16,
                        
                    },
                    new TiledTileset
                    {
                        Name = "tileset2",
                         ImageName = "tileset2.png",
                        FirstGID = 248,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16,                     
                    }
                },
                Properties = new List<TiledProperty>
                {
                    new TiledProperty
                    {
                        Name = "Property1",
                        Type = TiledPropertyType.Bool,
                        Value = true                     
                    },
                    new TiledProperty
                    {
                        Name = "Property2",
                        Type = TiledPropertyType.String,
                        Value = "Property2 Value"
                    }
                },
                TileCountX = 10,
                TileCountY = 5,
                TileWidth = 8,
                TileHeight = 8
            };

            expected.Layers[0].Tiles[0, 0] = new TiledTile
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
            expected.Layers[0].Tiles[0, 1] = new TiledTile
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

            // ACT
            var tiledTilemapService = new TiledTilemapService(mockFileSystem, tiledTilemapJsonConverterServiceMock.Object);
            var result = tiledTilemapService.GetTiledTilemap(mockTilemapFilepath);

            // ASSERT
            AreEqualByJson(expected, result);
        }

        [Test]
        public void GetTiledTilemap_EncodedFlipFlags_ReturnsCorrectTiledTilemap()
        {
            // Arrange

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

            var mockTilemapJsonString = "mockTilemapJsonString";

            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"\Content\Tilemaps\MockTilemap.tmj", new MockFileData(mockTilemapJsonString) }
            });

            var mockTilemapFilepath = @"\Content\Tilemaps\MockTilemap.tmj";

            var tilemapDTO = _tilemapDTO;
            _tilemapDTO.LayerDTOs = new List<LayerDTO> {
                new LayerDTO
                {
                    Name = "Tiles",
                    Type =  "tilelayer",
                    Width = 10,
                    Height = 5,
                    TileGIDs = new uint[50] {
                        encodedTile1,encodedTile2,encodedTile3,unencodedTile,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0
                    }
                }
            };

            _tilemapDTO.PropertyDTOs = new List<PropertyDTO>
            {
                new PropertyDTO
                {
                    Name = "Property1",
                    Type = "bool",
                    Value = true
                },
                new PropertyDTO
                {
                    Name = "Property2",
                    Type = "string",
                    Value = "Property2 Value"
                },
            };

            var tiledTilemapJsonConverterServiceMock = new Mock<ITiledTilemapJsonConverterService>();

            tiledTilemapJsonConverterServiceMock
                .Setup(x => x.GetTilemapDTOFromJsonFile(It.Is<string>(tilemapJsonString => tilemapJsonString == mockTilemapJsonString)))
                .Returns(tilemapDTO);

            var expected = new
            {
                Layers = new List<TileLayer>
                {
                    new TileLayer {
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
                        },
                        Tiles = new TiledTile[5, 10]
                    }
                },
                Tilesets = new List<TiledTileset>
                {
                    new TiledTileset
                    {
                        Name = "tileset1",
                        ImageName = "tileset1.png",
                        FirstGID = 1,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16,

                    },
                    new TiledTileset
                    {
                        Name = "tileset2",
                         ImageName = "tileset2.png",
                        FirstGID = 248,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16,
                    }
                },
                Properties = new List<TiledProperty>
                {
                    new TiledProperty
                    {
                        Name = "Property1",
                        Type = TiledPropertyType.Bool,
                        Value = true
                    },
                    new TiledProperty
                    {
                        Name = "Property2",
                        Type = TiledPropertyType.String,
                        Value = "Property2 Value"
                    }
                },
                TileCountX = 10,
                TileCountY = 5,
                TileWidth = 8,
                TileHeight = 8
            };

            expected.Layers[0].Tiles[0, 0] = new TiledTile
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
            expected.Layers[0].Tiles[0, 1] = new TiledTile
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
            expected.Layers[0].Tiles[0, 2] = new TiledTile
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
            expected.Layers[0].Tiles[0, 3] = new TiledTile
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

            // ACT
            var tiledTilemapService = new TiledTilemapService(mockFileSystem, tiledTilemapJsonConverterServiceMock.Object);
            var result = tiledTilemapService.GetTiledTilemap(mockTilemapFilepath);
          
            // ASSERT
            AreEqualByJson(expected, result);
        }

        [Test]
        public void GetTiledTilemap_HasObjectlayer_ReturnsCorrectObjectlayer()
        {
            // Arrange
            var mockTilemapJsonString = "mockTilemapJsonString";

            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"\Content\Tilemaps\MockTilemap.tmj", new MockFileData(mockTilemapJsonString) }
            });

            var mockTilemapFilepath = @"\Content\Tilemaps\MockTilemap.tmj";

            var tilemapDTO = _tilemapDTO;
            _tilemapDTO.LayerDTOs = new List<LayerDTO> {
                new LayerDTO
                {
                    Name = "Tiles",
                    Type =  "tilelayer",
                    Width = 10,
                    Height = 5,
                    TileGIDs = new uint[50] {
                        1,2,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,0,0,0,0,0,0
                    }
                },
                new LayerDTO
                {
                    Name = "Object Layer 1",
                    Type =  "objectgroup",
     
                }
            };

            _tilemapDTO.TilesetDTOs = new List<TilesetDTO>
            {
                new TilesetDTO
                {
                    Name = "tileset1",
                    Image = "tileset1.png",
                    FirstGID = 1,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,

                },
                new TilesetDTO
                {
                    Name = "tileset2",
                    Image = "tileset2.png",
                    FirstGID = 248,
                    ImageWidth = 200,
                    ImageHeight = 200,
                    TileWidth = 16,
                    TileHeight = 16,
                }
            };

            _tilemapDTO.PropertyDTOs = new List<PropertyDTO>
            {
                new PropertyDTO
                {
                    Name = "Property1",
                    Type = "bool",
                    Value = true
                },
                new PropertyDTO
                {
                    Name = "Property2",
                    Type = "string",
                    Value = "Property2 Value"
                },
            };

            var tiledTilemapJsonConverterServiceMock = new Mock<ITiledTilemapJsonConverterService>();

            tiledTilemapJsonConverterServiceMock
                .Setup(x => x.GetTilemapDTOFromJsonFile(It.Is<string>(tilemapJsonString => tilemapJsonString == mockTilemapJsonString)))
                .Returns(tilemapDTO);

            var expected = new
            {
                Layers = new List<TiledLayer>
                {
                    new TileLayer {
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
                        },
                        Tiles = new TiledTile[5, 10]
                    },
                    new ObjectLayer {
                        Name = "Object Layer 1",
                        TiledLayerType = TiledLayerType.ObjectLayer
                    }
                },
                Tilesets = new List<TiledTileset>
                {
                    new TiledTileset
                    {
                        Name = "tileset1",
                        ImageName = "tileset1.png",
                        FirstGID = 1,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16,

                    },
                    new TiledTileset
                    {
                        Name = "tileset2",
                         ImageName = "tileset2.png",
                        FirstGID = 248,
                        ImageWidth = 200,
                        ImageHeight = 200,
                        TileWidth = 16,
                        TileHeight = 16,
                    }
                },
                Properties = new List<TiledProperty>
                {
                    new TiledProperty
                    {
                        Name = "Property1",
                        Type = TiledPropertyType.Bool,
                        Value = true
                    },
                    new TiledProperty
                    {
                        Name = "Property2",
                        Type = TiledPropertyType.String,
                        Value = "Property2 Value"
                    }
                },
                TileCountX = 10,
                TileCountY = 5,
                TileWidth = 8,
                TileHeight = 8
            };

            TileLayer tileLayer = expected.Layers[0] as TileLayer;

            tileLayer.Tiles[0, 0] = new TiledTile
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
            tileLayer.Tiles[0, 1] = new TiledTile
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

            // ACT
            var tiledTilemapService = new TiledTilemapService(mockFileSystem, tiledTilemapJsonConverterServiceMock.Object);
            var result = tiledTilemapService.GetTiledTilemap(mockTilemapFilepath);

            // ASSERT
            AreEqualByJson(expected, result);
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