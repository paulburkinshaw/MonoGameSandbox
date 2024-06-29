using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using Microsoft.Xna.Framework;

namespace Platformer004
{
    public class AnimationFrameConverter : JsonConverter<Frame>
    {
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

            if (filenameParts.Length == 2)
            {
                spriteFrame.AnimationType = ConvertAnimationNameToEnum(filenameParts[0]);
                spriteFrame.FrameNumber = filenameParts[1];
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
