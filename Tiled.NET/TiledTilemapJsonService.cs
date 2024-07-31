using Newtonsoft.Json;
using System.Collections.Generic;
using Tiled.NET.DTOs;

namespace Tiled.NET
{
    public interface ITiledTilemapJsonService
    {
        TilemapDTO GetTilemapDTOFromJsonFile(string tilemapJsonString);
    }

    public class TiledTilemapJsonService : ITiledTilemapJsonService
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
}
