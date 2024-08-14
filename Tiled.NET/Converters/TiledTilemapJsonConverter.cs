using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tiled.NET.DTOs;

/* Unmerged change from project 'Tiled.NET (net8.0)'
Before:
using System.Linq;
After:
using System.Linq;
using Tiled;
using Tiled.NET;
using Tiled.NET.Converters;
*/
using System.Linq;

namespace Tiled.NET.Converters
{
    public interface ITiledTilemapJsonConverterService
    {
        TilemapDTO GetTilemapDTOFromJsonFile(string tilemapJsonString);
    }

    public class TiledTilemapJsonConverterService : ITiledTilemapJsonConverterService
    {
        public TilemapDTO GetTilemapDTOFromJsonFile(string tilemapJsonString)
        {
            var tilemapDTO = JsonConvert.DeserializeObject<TilemapDTO>(
            tilemapJsonString,
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new TiledTilemapJsonConverter() }
            });

            return tilemapDTO;
        }
    }
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
