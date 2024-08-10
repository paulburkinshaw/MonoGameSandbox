using Aseprite.NET.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aseprite.NET
{
    public interface IAsepriteSpritesheetJsonService
    {
        SpritesheetDTO GetSpritesheetDTOFromJsonFile(string tilemapJsonString);
    }

    public class AsepriteSpritesheetJsonService : IAsepriteSpritesheetJsonService
    {
        public SpritesheetDTO GetSpritesheetDTOFromJsonFile(string spritesheetJsonString)
        {
            var spritesheetDTO = JsonConvert.DeserializeObject<SpritesheetDTO>(
            spritesheetJsonString,
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new AsepriteSpritesheetJsonConverter() }
            });

            return spritesheetDTO;
        }
    }
}
