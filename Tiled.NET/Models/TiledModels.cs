using System.Collections.Generic;

namespace Tiled.NET.Models
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
        CustomTypeClass,
        CustomTypeEnumString,
        CustomTypeEnumNumber
    }

    #region layer types

    /// <summary>
    /// Base class for layer types
    /// </summary>
    public class TiledLayer
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public double Opacity { get; set; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public double ParallaxX { get; set; }

        public double ParallaxY { get; set; }

        public bool RepeatX { get; set; }

        public bool RepeatY { get; set; }

        public string TransparentColor { get; set; }

        public string TintColor { get; set; }

        public TiledLayerType TiledLayerType { get; set; }

        public bool Visible { get; set; }

        public IEnumerable<TiledProperty> Properties { get; private set; }    
    }

    public class TileLayer : TiledLayer
    {
        /// <summary>
        /// Total horizontal tiles
        /// </summary>
        public int TileCountX { get; set; }

        /// <summary>
        /// Total vertical tiles
        /// </summary>
        public int TileCountY { get; set; }

        public uint[] TileGIDs { get; set; }

        public TiledTile[,] Tiles { get; set; }
    }

    public class ImageLayer : TiledLayer
    {

    }

    public class ObjectLayer : TiledLayer
    {

    }

    public class GroupLayer : TiledLayer
    {

    }

    #endregion layer types

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

    public class TiledProperty
    {
        public string Name;

        public TiledPropertyType Type;

        public object Value;
    }

}
