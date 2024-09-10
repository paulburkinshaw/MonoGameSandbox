﻿namespace Tiled.NET.Models
{
    public enum TiledLayerType
    {
        TileLayer,
        ObjectLayer,
        ImageLayer,
        GroupLayer
    }

    public enum TiledPropertyType
    {
        String,
        Bool,
        Color,
        File,
        Float,
        Int,
        Object,
        Class
    }

    public class TiledLayer
    {
        public uint[] TileGIDs { get; set; }
        public TiledTile[,] Tiles { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Total horizontal tiles
        /// </summary>
        public int TileCountX { get; set; }

        /// <summary>
        /// Total vertical tiles
        /// </summary>
        public int TileCountY { get; set; }

        public TiledLayerType TiledLayerType { get; set; }
    }

    public class TiledTile
    {
        public int TileSetId { get; set; }
        public int LocalTileId { get; set; }
        public TileSourceRectangle TileSourceRectangle { get; set; }
        public TileFlipFlags TileFlipFlags { get; set; }
    }

    public class TileFlipFlags
    {
        public bool FlippedHorizontally { get; set; }
        public bool FlippedVertically { get; set; }
        public bool FlippedDiagonally { get; set; }
        public bool RotatedHex120 { get; set; }
    }

    public class TileSourceRectangle
    {
        /// <summary>
        /// The x position in pixels of the tile in the source image
        /// </summary>
        public int X;

        /// <summary>
        /// The y position in pixels of the tile in the source image
        /// </summary>
        public int Y;

        /// <summary>
        /// The width in pixels of the tile in the source image
        /// </summary>
        public int Width;

        /// <summary>
        /// The height in pixels of the tile in the source image
        /// </summary>
        public int Height;
    }

    /// <summary>
    /// Represents a property object in both tilesets, maps, layers and objects. Values are all in string but you can use the 'type' property for conversions
    /// </summary>
    public class TiledProperty
    {
        public string Name;

        public TiledPropertyType Type;

        public object Value;
    }

}
