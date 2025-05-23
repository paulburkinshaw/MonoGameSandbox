﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using Aseprite.NET.DTOs;
using System.Collections.Generic;

namespace Aseprite.NET.Converters
{
    public interface IAsepriteSpritesheetJsonConverterService
    {
        SpritesheetDTO MapSpritesheetJsonFileToSpritesheetDTO(string spritesheetJsonString);
    }

    public class AsepriteSpritesheetJsonConverterService : IAsepriteSpritesheetJsonConverterService
    {
        public SpritesheetDTO MapSpritesheetJsonFileToSpritesheetDTO(string spritesheetJsonString)
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

    public class AsepriteSpritesheetJsonConverter : JsonConverter<SpritesheetDTO>
    {
        public override SpritesheetDTO ReadJson(JsonReader reader,
            Type objectType,
            SpritesheetDTO existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var spritesheetDTO = JsonConvert.DeserializeObject<SpritesheetDTO>(jsonObject["meta"].ToString());
            spritesheetDTO.FrameDTOs = JsonConvert.DeserializeObject<FrameDTO[]>(jsonObject["frames"].ToString());

            return spritesheetDTO;
        }

        public override void WriteJson(JsonWriter writer, SpritesheetDTO value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not implemented for SpritesheetDataConverter");
        }
    }


}
