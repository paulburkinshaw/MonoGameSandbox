using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aseprite.NET.DTOs
{
    public class SpritesheetDTO
    {
        [JsonProperty("image")]
        public string ImageName { get; set; }

        [JsonProperty("size")]
        public ImageSizeDTO ImageSize { get; set; }

        [JsonProperty("format")]
        public string ImageFormat { get; set; }

        [JsonProperty("scale")]
        public int ImageScale { get; set; }
  

        [JsonProperty("frames")]
        public IEnumerable<FrameDTO> FrameDTOs { get; set; }

        [JsonProperty("layers")]
        public IEnumerable<LayerDTO> LayerDTOs { get; set; }
    }

    public class ImageSizeDTO
    {
        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }
    }

    public class FrameDTO
    {
        public string Filename { get; set; }

        [JsonProperty("frame")]
        public SourceRectangleDTO SourceRectangle { get; set; }

        public bool Rotated { get; set; }

        public bool Trimmed { get; set; }

        public int Duration { get; set; }
    }

    public class SourceRectangleDTO
    {
        public int X { get; set; }
        public int Y { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }
    }

    public class LayerDTO
    {
        public string Name { get; set; }

        public int Opacity { get; set; }

        public string BlendMode { get; set; }

        [JsonProperty("cels")]
        public IEnumerable<CelDTO> CelDTOs { get; set; }
    }

    public class CelDTO
    {
        public int Frame { get; set; }
        public string Data { get; set; }
    }

}
