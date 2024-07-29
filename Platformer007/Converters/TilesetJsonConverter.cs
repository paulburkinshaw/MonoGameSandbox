

namespace Platformer007.Converters
{
    //public class TilesetData
    //{
    //    public List<Frame> Frames { get; set; }
    //    public List<Layer> Layers { get; set; }
    //}

    //public class TilesetJsonConverter : JsonConverter<TilesetData>
    //{
    //    public override TilesetData ReadJson(JsonReader reader,
    //        Type objectType,
    //        TilesetData existingValue,
    //        bool hasExistingValue,
    //        JsonSerializer serializer)
    //    {
    //        JObject jsonObject = JObject.Load(reader);

    //        var tilemapData = new TilesetData();

    //        var frames = JsonConvert.DeserializeObject<List<Frame>>(
    //        jsonObject["frames"].ToString(),
    //        new JsonSerializerSettings
    //        {
    //            Converters = new List<JsonConverter> { new AnimationFrameConverter() }
    //        });

    //        return tilemapData;
    //    }


    //    public override void WriteJson(JsonWriter writer, TilesetData value, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException("Writing JSON is not implemented for SpritesheetDataConverter");
    //    }
    //}


}
