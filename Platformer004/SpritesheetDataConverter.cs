using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Platformer004
{
    public class SpritesheetData
    {
        public List<Frame> Frames { get; set; }
        public List<Layer> Layers { get; set; }
    }

    public class Layer
    {
        public string Name { get; set; }

        [JsonProperty("cels")]
        public List<Cell> Cells { get; set; }
    }

    public class Cell
    {
        [JsonProperty("frame")]
        public string FrameNumber { get; set; }
        public string Data { get; set; }
    }

    public class SpritesheetDataConverter : JsonConverter<SpritesheetData>
    {
        public override SpritesheetData ReadJson(JsonReader reader, Type objectType, SpritesheetData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var spritesheetData = new SpritesheetData();     

            var frames = JsonConvert.DeserializeObject<List<Frame>>(
            jsonObject["frames"].ToString(),
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new AnimationFrameConverter() }
            });

            var layers = JsonConvert.DeserializeObject<List<Layer>>(jsonObject["meta"]["layers"].ToString());
               
            foreach ( var frame in frames)
            {          
                var frameHit = (layers.Where(layer => GetLayerName(layer.Name ) == frame.AnimationType.ToString() 
                                       && layer.Cells?.Where(cell => cell.FrameNumber == frame.FrameNumber && cell.Data == "hit").Count() > 0)
                               ).Any();

                if (frameHit)
                    frame.Hit = true;
            }

            spritesheetData.Frames = frames;
            spritesheetData.Layers = layers;

            return spritesheetData;
        }

        private string GetLayerName(string fileName)
        {
            var filenameParts = (fileName.Split(','));

            return filenameParts[0];
        }

        public override void WriteJson(JsonWriter writer, SpritesheetData value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not implemented for SpritesheetDataConverter");
        }
    }

    public class AnimationFrameConverter : JsonConverter<Frame>
    {
        static string runonce;

        public override Frame ReadJson(JsonReader reader, Type objectType, Frame existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var spriteFrame = new Frame
            {
                FrameSourceRectangle = new Rectangle
                {
                    X = (int)jsonObject["frame"]["x"],
                    Y = (int)jsonObject["frame"]["y"],
                    Width = (int)jsonObject["frame"]["w"],
                    Height = (int)jsonObject["frame"]["h"]
                },
                SpriteSourceSize = new Rectangle
                {
                    X = (int)jsonObject["spriteSourceSize"]["x"],
                    Y = (int)jsonObject["spriteSourceSize"]["y"],
                    Width = (int)jsonObject["spriteSourceSize"]["w"],
                    Height = (int)jsonObject["spriteSourceSize"]["h"]
                },
                SourceSize = new Rectangle
                {
                    Width = (int)jsonObject["sourceSize"]["w"],
                    Height = (int)jsonObject["sourceSize"]["h"]
                },
                Duration = (int)jsonObject["duration"],
                Trimmed = (bool)jsonObject["trimmed"],
                Rotated = (bool)jsonObject["rotated"]
            };

            var filenameParts = ((string)jsonObject["filename"]).Split(',');

            if (filenameParts.Length == 3)
            {
                spriteFrame.AnimationType = ConvertAnimationNameToEnum(filenameParts[0]);
                spriteFrame.RunOnce = filenameParts[1] == nameof(runonce) ? true : false;
                spriteFrame.FrameNumber = filenameParts[2];
            }

            return spriteFrame;
        }

        private AnimationType ConvertAnimationNameToEnum(string AnimationName)
        {
            Enum.TryParse(AnimationName, out AnimationType animationType);
            return animationType;
        }

        public override void WriteJson(JsonWriter writer, Frame value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not implemented for SpriteFrameConverter");
        }
    }
}
