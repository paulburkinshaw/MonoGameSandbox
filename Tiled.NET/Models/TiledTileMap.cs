using System.Collections.Generic;

namespace Tiled.NET.Models
{
    public class TiledTilemap
    {
        public IEnumerable<TiledLayer> Layers { get; private set; }
        public IEnumerable<TiledTileset> Tilesets { get; private set; }
        public IEnumerable<TiledProperty> Properties { get; private set; }

        /// <summary>
        /// The amount of horizontal tiles
        /// </summary>
        public int TileCountX { get; private set; }

        /// <summary>
        /// The amount of vertical tiles
        /// </summary>
        public int TileCountY { get; private set; }

        /// <summary>
        /// The tile width in pixels
        /// </summary>
        public int TileWidth { get; private set; }

        /// <summary>
        /// The tile height in pixels
        /// </summary>
        public int TileHeight { get; private set; }

        public TiledTilemap(int tileCountX,
            int tileCountY,
            int tileWidth,
            int tileHeight,
            IEnumerable<TiledTileset> tilesets,
            IEnumerable<TiledLayer> layers,
            IEnumerable<TiledProperty> properties)
        {
            TileCountX = tileCountX;
            TileCountY = tileCountY;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tilesets = tilesets;
            Layers = layers;
            Properties = properties;
        }

    }
}