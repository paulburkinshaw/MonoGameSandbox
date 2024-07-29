using Tiled.NET.DTOs;

namespace Tiled.NET.Models
{

    public class TiledTileset
    {
        // TODO: support full tileset functionality

        public string Name { get; set; }

        public int FirstGID { get; set; }
        public string ImageName { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int TileCount { get; set; }
        public int ColumnCount { get; set; }

        public static TiledTileset FromDTO(TilesetDTO dto)
        {
            return new TiledTileset
            {
                Name = dto.Name,
                FirstGID = dto.FirstGID,
                ImageName = dto.Image,
                ImageHeight = dto.ImageHeight,
                ImageWidth = dto.ImageWidth,
                TileWidth = dto.TileWidth,
                TileHeight = dto.TileHeight,
                ColumnCount = dto.Columns
            };
        }
    }

}