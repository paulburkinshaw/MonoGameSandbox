

namespace Platformer007.Tiled;


//public class TiledTileMapService
//{
//    const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000; // 1000 0000 0000 0000 0000 0000 0000 0000
//    const uint FLIPPED_VERTICALLY_FLAG = 0x40000000; // 0100 0000 0000 0000 0000 0000 0000 0000
//    const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000; // 0010 0000 0000 0000 0000 0000 0000 0000
//    const uint ROTATED_HEXAGONAL_120_FLAG = 0x10000000; // 0001 0000 0000 0000 0000 0000 0000 0000

//    public TiledTileMapService()
//    {

//    }

//    //TODO: this method could be extracted into a TilemapDataConverter that could be its own dll
//    public static Tilemap GetTilemapFromJsonFile(string tilemapJsonFile)
//    {
//        var tilemapDTO = JsonConvert.DeserializeObject<TilemapDTO>(
//        tilemapJsonFile,
//        new JsonSerializerSettings
//        {
//            Converters = new List<JsonConverter> { new TiledTilemapJsonConverter() }
//        });

//        var backgroundTilemapDataLayer = tilemapDTO.LayerDTOs.Where(x => x.Name == "BackgroundTiles").FirstOrDefault();
//        var collidableTilemapDataLayer = tilemapDTO.LayerDTOs.Where(x => x.Name == "CollidableTiles").FirstOrDefault();

//        var backgroundTiles = GetTiles(backgroundTilemapDataLayer, tilemapDTO.TilesetDTOs);
//        var collidableTiles = GetTiles(collidableTilemapDataLayer, tilemapDTO.TilesetDTOs);

//        return new Tilemap
//        {
//            Layers = new List<TiledLayer>
//            {
//                new TiledLayer { Tiles = backgroundTiles, TiledLayerType = TiledLayerType.TileLayer },
//                new TiledLayer { Tiles = collidableTiles, TiledLayerType = TiledLayerType.TileLayer }
//            }
//        };
//    }

//    /// <summary>
//    /// Gets the tileset the tiles belong to
//    /// Calculates the each tile's x,y coordinates within the tilset image 
//    /// 
//    /// </summary>
//    /// <param name="layer"></param>
//    /// <param name="tileset"></param>
//    /// <returns></returns>
//    private static TiledTile[,] GetTiles(LayerDTO layerDto, List<TilesetDTO> tilesets)
//    {
//        TiledTile[,] tiles = new TiledTile[layerDto.Height, layerDto.Width];

//        for (int y = 0; y < layerDto.Height; y++)
//        {
//            for (int x = 0; x < layerDto.Width; x++)
//            {
//                int offset = y * layerDto.Width;
//                int tileIndex = x + offset;

//                uint unsignedTileGID = layerDto.TileGIDs[tileIndex];

//                var tileFlipFlags = GetTileFlipFlags(unsignedTileGID);

//                int tileGID = GetTileGID(unsignedTileGID);

//                if (tileGID > 0)
//                {
//                    TiledTileset tileset;

//                    tileset = GetTilesetForTileGID(tilesets, tileGID);

//                    int localTileId = tileGID - tileset.FirstGID; // Tiled adds 1 to tile id's to allow 0 to be use as the ID for empty space

//                    int tileCoordinateValue = localTileId * tileset.TileWidth;

//                    var xPosition = tileCoordinateValue % tileset.ImageWidth;
//                    var yPosition = tileCoordinateValue / tileset.ImageWidth * tileset.TileHeight;
                   
//                    tiles[y, x] = new TiledTile
//                    {
//                        TileSetId = tileset.FirstGID,
//                        // Tileset = tileset,
//                        TileGID = localTileId,
//                        TileSourceRectangle = new TileSourceRectangle
//                        {
//                            X = xPosition,
//                            Y = yPosition,
//                            Width = tileset.TileWidth,
//                            Height = tileset.TileHeight
//                        },
//                        TileFlipFlags = tileFlipFlags
//                    };
//                }
//                else
//                {
//                    tiles[y, x] = new TiledTile
//                    {
//                        TileGID = 0,
//                    };
//                }

//            }
//        }

//        return tiles;
//    }

//    private static TileFlipFlags GetTileFlipFlags(uint unsignedTileGID)
//    {
//        /*
//        example flags from a flipped tile:
//        1010 = the tile is flipped horizontally and antidiagonally - 90 degrees clockwise, or right rotation
//        0110 = the tile is flipped vertically and antidiagonally - 90 degrees anticlockwise, or left rotation
//        1100 = the tile is flipped horizontally and vertically - 180 degrees clockwise/anticlockwise, or top rotation
//        flipping a tile antidiagonally = swapping its bottom left and top right corners
//        */

//        // Read out the flags
//        return new TileFlipFlags
//        {
//            FlippedHorizontally = (unsignedTileGID & FLIPPED_HORIZONTALLY_FLAG) != 0,
//            FlippedVertically = (unsignedTileGID & FLIPPED_VERTICALLY_FLAG) != 0,
//            FlippedDiagonally = (unsignedTileGID & FLIPPED_DIAGONALLY_FLAG) != 0,
//            RotatedHex120 = (unsignedTileGID & ROTATED_HEXAGONAL_120_FLAG) != 0
//        };
//    }

//    private static int GetTileGID(uint unsignedTileGID)
//    {
//        // Clear all four flags
//        unsignedTileGID &= ~(FLIPPED_HORIZONTALLY_FLAG |
//                          FLIPPED_VERTICALLY_FLAG |
//                          FLIPPED_DIAGONALLY_FLAG |
//                          ROTATED_HEXAGONAL_120_FLAG);

//        return Convert.ToInt32(unsignedTileGID);
//    }

//    /// <summary>
//    /// https://doc.mapeditor.org/en/stable/reference/global-tile-ids
//    /// </summary>
//    /// <param name="tilesets"></param>
//    /// <param name="tileGID"></param>
//    /// <returns></returns>
//    private static TiledTileset GetTilesetForTileGID(List<TilesetDTO> tilesets, int tileGID)
//    {
//        var firstGIDs = tilesets.Where(x => x.FirstGID <= tileGID).Select(x => x.FirstGID);
//        var tilesetDTO = tilesets.Where(x => x.FirstGID == firstGIDs.Max()).FirstOrDefault();

//        return TiledTileset.FromDTO(tilesetDTO);
//    }



//}
