using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Tiled.NET.Converters;
using Tiled.NET.DTOs;
using Tiled.NET.Models;

namespace Tiled.NET
{
    public interface ITiledTilemapService
    {
        TiledTilemap GetTiledTilemap(string tilemapFilepath);
    }

    public class TiledTilemapService : ITiledTilemapService
    {
        const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000; // 1000 0000 0000 0000 0000 0000 0000 0000
        const uint FLIPPED_VERTICALLY_FLAG = 0x40000000; // 0100 0000 0000 0000 0000 0000 0000 0000
        const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000; // 0010 0000 0000 0000 0000 0000 0000 0000
        const uint ROTATED_HEXAGONAL_120_FLAG = 0x10000000; // 0001 0000 0000 0000 0000 0000 0000 0000

        private IFileSystem _fileSystem;
        private ITiledTilemapJsonConverterService _tiledTilemapJsonConverterService;

        public TiledTilemapService(IFileSystem fileSystem,
           ITiledTilemapJsonConverterService tiledTilemapJsonConverterService)
        {
            _fileSystem = fileSystem;
            _tiledTilemapJsonConverterService = tiledTilemapJsonConverterService;
        }

        public TiledTilemap GetTiledTilemap(string tilemapFilepath)
        {
            // Construct TiledTileMap steps
            // - Get TilemapDTO from .tmj or .tmx file
            // - Convert dto to model
            //      - Convert TilesetDTO to TiledTileset 
            //      - Convert LayerDTO to TiledLayer  
            //          - Convert TileGIDs to TiledTiles
            //

            if (!_fileSystem.File.Exists(tilemapFilepath))
                throw new FileNotFoundException($"{tilemapFilepath} not found");

            TilemapDTO tilemapDTO;

            var extension = _fileSystem.Path.GetExtension(tilemapFilepath);
            switch (extension)
            {
                case ".tmx":
                    // TODO: Support .tmx files
                    throw new NotImplementedException(extension);
                case ".tmj":
                    var tilemapJsonString = _fileSystem.File.ReadAllText(tilemapFilepath);
                    tilemapDTO = _tiledTilemapJsonConverterService.GetTilemapDTOFromJsonFile(tilemapJsonString);
                    break;
                default:
                    throw new NotImplementedException(extension);
            }

            // Map TilesetDTO to TiledTileset 
            var tilesets = MapTilesetDTOsToTiledTilesets(tilemapDTO.TilesetDTOs);

            // Map LayerDTO to TiledLayer 
            var layers = MapLayerDTOsToTiledLayers(tilemapDTO.LayerDTOs, tilesets.ToList());

            var properties = MapPropertyDTOsToTiledProperties(tilemapDTO.PropertyDTOs);

            return new TiledTilemap(tileCountX: tilemapDTO.Width,
                                                tileCountY: tilemapDTO.Height,
                                                tileWidth: tilemapDTO.TileWidth,
                                                tileHeight: tilemapDTO.TileHeight,
                                                tilesets: tilesets,
                                                layers: layers,
                                                properties: properties);

        }

        private IEnumerable<TiledTileset> MapTilesetDTOsToTiledTilesets(IEnumerable<TilesetDTO> tilesetDTOs)
        {
            var tiledTilesets = new List<TiledTileset>();
            foreach (var tilesetDto in tilesetDTOs)
            {
                tiledTilesets.Add(TiledTileset.FromDTO(tilesetDto));
            }

            return tiledTilesets;
        }

        private IEnumerable<TiledLayer> MapLayerDTOsToTiledLayers(IEnumerable<LayerDTO> layerDTOs, List<TiledTileset> tilesets)
        {
            var tiledLayers = new List<TiledLayer>();

            foreach (var layerDTO in layerDTOs)
            {
                var tiledLayer = new TiledLayer
                {
                    TileGIDs = layerDTO.TileGIDs,
                    Name = layerDTO.Name,
                    TileCountX = layerDTO.Width,
                    TileCountY = layerDTO.Height
                };

                tiledLayer.Tiles = GetTilesForLayer(tiledLayer, tilesets);

                tiledLayers.Add(tiledLayer);
            }

            return tiledLayers;
        }

        private IEnumerable<TiledProperty> MapPropertyDTOsToTiledProperties(IEnumerable<PropertyDTO> propertyDTOs)
        {
            var tiledProperties = new List<TiledProperty>();

            foreach (var propertyDTO in propertyDTOs)
            {
                var tiledProperty = new TiledProperty
                {
                   Name = propertyDTO.Name,
                   Type = GetTiledPropertyType(propertyDTO.Type),
                   Value = propertyDTO.Value
                };

                tiledProperties.Add(tiledProperty);
            }

            return tiledProperties;
        }

        /// <summary>
        /// Converts each tileGID within a TiledLayer to a TiledTile     
        /// </summary>
        /// <param name="layer">The layer containing the TileGIDs</param>
        /// <param name="tilesets">tileset collection</param>
        /// <returns>TiledTile[,]</returns>
        private TiledTile[,] GetTilesForLayer(TiledLayer layer, List<TiledTileset> tilesets)
        {
            // Convert TileGIDs to TiledTiles steps
            // - For each tile index in the TileGIDs array:
            //      - Get unsignedTileGID - the Global Tile ID with encoded flip flags 
            //      - Extract the flip flags from the unsignedTileGID
            //      - Get the Global Tile ID from the unsignedTileGID
            //          - Clear flip flags 
            //      - Map the Global Tile ID to a Local Tile ID:
            //          - Get the tileset that the global tide ID belongs to
            //          - Subtract the tileset's firstgid from the Global Tile ID to get the local ID of the tile within the tileset
            //      - Get the source rectangle of the tile needed to draw the tile from the tilset image
            //          - Calculate the tile's X and Y coordinates within the tilset image 
            //          - Get the width and height of the tile from the tileset.TileWidth and tileset.TileHeight

            TiledTile[,] tiles = new TiledTile[layer.TileCountY, layer.TileCountX];

            for (int y = 0; y < layer.TileCountY; y++)
            {
                for (int x = 0; x < layer.TileCountX; x++)
                {
                    int offset = y * layer.TileCountX;
                    int tileIndex = x + offset;

                    uint unsignedTileGID = layer.TileGIDs[tileIndex];

                    int tileGID = GetTileGID(unsignedTileGID);
                    var tileFlipFlags = GetTileFlipFlags(unsignedTileGID);

                    if (tileGID > 0)
                    {
                        var tileset = GetTilesetForTileGID(tileGID, tilesets);

                        int localTileId = tileGID - tileset.FirstGID; // Tiled adds 1 to tile id's to allow 0 to be use as the ID for empty space

                        int tileCoordinateValue = localTileId * tileset.TileWidth;

                        var xPosition = tileCoordinateValue % tileset.ImageWidth;
                        var yPosition = tileCoordinateValue / tileset.ImageWidth * tileset.TileHeight;

                        tiles[y, x] = new TiledTile
                        {
                            TileSetId = tileset.FirstGID,
                            LocalTileId = localTileId,
                            TileSourceRectangle = new TileSourceRectangle
                            {
                                X = xPosition,
                                Y = yPosition,
                                Width = tileset.TileWidth,
                                Height = tileset.TileHeight
                            },
                            TileFlipFlags = tileFlipFlags
                        };
                    }

                }
            }

            return tiles;
        }

        private TileFlipFlags GetTileFlipFlags(uint unsignedTileGID)
        {
            /*
            example flags from a flipped tile:
            1010 = the tile is flipped horizontally and antidiagonally - 90 degrees clockwise, or right rotation
            0110 = the tile is flipped vertically and antidiagonally - 90 degrees anticlockwise, or left rotation
            1100 = the tile is flipped horizontally and vertically - 180 degrees clockwise/anticlockwise, or top rotation
            flipping a tile antidiagonally = swapping its bottom left and top right corners
            */

            // Read out the flags
            return new TileFlipFlags
            {
                FlippedHorizontally = (unsignedTileGID & FLIPPED_HORIZONTALLY_FLAG) != 0,
                FlippedVertically = (unsignedTileGID & FLIPPED_VERTICALLY_FLAG) != 0,
                FlippedDiagonally = (unsignedTileGID & FLIPPED_DIAGONALLY_FLAG) != 0,
                RotatedHex120 = (unsignedTileGID & ROTATED_HEXAGONAL_120_FLAG) != 0
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsignedTileGID"></param>
        /// <returns></returns>
        private int GetTileGID(uint unsignedTileGID)
        {
            // Clear all four flags
            unsignedTileGID &= ~(FLIPPED_HORIZONTALLY_FLAG |
                              FLIPPED_VERTICALLY_FLAG |
                              FLIPPED_DIAGONALLY_FLAG |
                              ROTATED_HEXAGONAL_120_FLAG);

            return Convert.ToInt32(unsignedTileGID);
        }

        /// <summary>
        /// https://doc.mapeditor.org/en/stable/reference/global-tile-ids
        /// </summary>
        /// <param name="tilesets"></param>
        /// <param name="tileGID"></param>
        /// <returns></returns>
        private TiledTileset GetTilesetForTileGID(int tileGID, List<TiledTileset> tilesets)
        {
            var firstGIDs = tilesets.Where(x => x.FirstGID <= tileGID).Select(x => x.FirstGID);
            var tileset = tilesets.Where(x => x.FirstGID == firstGIDs.Max()).FirstOrDefault();

            return tileset;
        }

        private TiledPropertyType GetTiledPropertyType(string type)
        {
            if (Enum.TryParse(type, true, out TiledPropertyType tiledPropertyType))
            {
                return tiledPropertyType;
            }
            else
            {
                throw new ArgumentException($"Unknown TiledPropertyType: {tiledPropertyType}");
            }
        }
    }
}
