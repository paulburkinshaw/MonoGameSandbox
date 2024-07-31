using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tiled.NET.DTOs;
using System.Linq;

namespace Tiled.NET
{
    public class TiledTilemapJsonConverter : JsonConverter<TilemapDTO>
    {
        public override TilemapDTO ReadJson(JsonReader reader,
            Type objectType,
            TilemapDTO existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var tilemapDTO = JsonConvert.DeserializeObject<TilemapDTO>(jsonObject.ToString());

            var jObjectTilesets = JsonConvert.DeserializeObject<JArray>(jsonObject["tilesets"].ToString());

            if (jObjectTilesets.Any(x => x["source"] != null))
                tilemapDTO.TilesetSourceDTOs = JsonConvert.DeserializeObject<List<TilesetSourceDTO>>(jObjectTilesets.ToString());
            else
                tilemapDTO.TilesetDTOs = JsonConvert.DeserializeObject<List<TilesetDTO>>(jObjectTilesets.ToString());

            return tilemapDTO;
        }

        public override void WriteJson(JsonWriter writer, TilemapDTO value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not implemented for SpritesheetDataConverter");
        }     
    }
}
