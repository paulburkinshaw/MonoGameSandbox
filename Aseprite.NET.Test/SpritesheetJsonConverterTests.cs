using Aseprite.NET.DTOs;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Abstractions.TestingHelpers;

namespace Aseprite.NET.Test
{
    public class SpritesheetJsonConverterTests
    {


        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void ReadJson_ReturnsCorrectSpritesheetDTO()
        {
            // Arrange
            var mockSpritesheetJsonString = "{ \"frames\": [\r\n   {\r\n    \"filename\": \"Ready,loop|0\",\r\n    \"frame\": { \"x\": 0, \"y\": 0, \"w\": 64, \"h\": 64 },\r\n    \"rotated\": false,\r\n    \"trimmed\": false,\r\n    \"spriteSourceSize\": { \"x\": 0, \"y\": 0, \"w\": 64, \"h\": 64 },\r\n    \"sourceSize\": { \"w\": 64, \"h\": 64 },\r\n    \"duration\": 100\r\n   },\r\n   {\r\n    \"filename\": \"Ready,loop|1\",\r\n    \"frame\": { \"x\": 65, \"y\": 0, \"w\": 64, \"h\": 64 },\r\n    \"rotated\": false,\r\n    \"trimmed\": false,\r\n    \"spriteSourceSize\": { \"x\": 0, \"y\": 0, \"w\": 64, \"h\": 64 },\r\n    \"sourceSize\": { \"w\": 64, \"h\": 64 },\r\n    \"duration\": 100\r\n   },\r\n   {\r\n    \"filename\": \"Attack|0\",\r\n    \"frame\": { \"x\": 0, \"y\": 65, \"w\": 64, \"h\": 64 },\r\n    \"rotated\": false,\r\n    \"trimmed\": false,\r\n    \"spriteSourceSize\": { \"x\": 0, \"y\": 0, \"w\": 64, \"h\": 64 },\r\n    \"sourceSize\": { \"w\": 64, \"h\": 64 },\r\n    \"duration\": 100\r\n   },\r\n   {\r\n    \"filename\": \"Attack|1\",\r\n    \"frame\": { \"x\": 65, \"y\": 65, \"w\": 64, \"h\": 64 },\r\n    \"rotated\": false,\r\n    \"trimmed\": false,\r\n    \"spriteSourceSize\": { \"x\": 0, \"y\": 0, \"w\": 64, \"h\": 64 },\r\n    \"sourceSize\": { \"w\": 64, \"h\": 64 },\r\n    \"duration\": 100\r\n   }\r\n ],\r\n \"meta\": {\r\n  \"app\": \"https://www.aseprite.org/\",\r\n  \"version\": \"1.3.7-x64\",\r\n  \"image\": \"spritesheet1.png\",\r\n  \"format\": \"RGBA8888\",\r\n  \"size\": { \"w\": 389, \"h\": 129 },\r\n  \"scale\": \"1\",\r\n  \"frameTags\": [\r\n  ],\r\n  \"layers\": [\r\n   { \"name\": \"Ready,loop\", \"opacity\": 255, \"blendMode\": \"normal\" },\r\n   { \"name\": \"Attack\", \"opacity\": 255, \"blendMode\": \"normal\", \"cels\": [{ \"frame\": 1, \"data\": \"isattacking\" }] }\r\n  ],\r\n  \"slices\": [\r\n  ]\r\n }\r\n}\r\n";
            var mockJsonObject = JObject.Parse(mockSpritesheetJsonString);

            var expected = new SpritesheetDTO
            {
                ImageName = "spritesheet1.png",
                ImageSize = new ImageSizeDTO
                {
                    Width = 389,
                    Height = 129
                },
                ImageFormat = "RGBA8888",
                ImageScale = 1,
                FrameDTOs = new[] {
                    new FrameDTO
                    {
                        Filename = "Ready,loop|0",
                        SourceRectangle = new SourceRectangleDTO
                        {
                            X = 0,
                            Y = 0,
                            Width = 64,
                            Height = 64
                        },
                        Rotated = false,
                        Trimmed = false,
                        Duration = 100
                    },
                    new FrameDTO
                    {
                        Filename = "Ready,loop|1",
                        SourceRectangle = new SourceRectangleDTO
                        {
                            X = 65,
                            Y = 0,
                            Width = 64,
                            Height = 64
                        },
                        Rotated = false,
                        Trimmed = false,
                        Duration = 100
                    },
                    new FrameDTO
                    {
                        Filename = "Attack|0",
                        SourceRectangle = new SourceRectangleDTO
                        {
                            X = 0,
                            Y = 65,
                            Width = 64,
                            Height = 64
                        },
                        Rotated = false,
                        Trimmed = false,
                        Duration = 100
                    },
                    new FrameDTO
                    {
                        Filename = "Attack|1",
                        SourceRectangle = new SourceRectangleDTO
                        {
                            X = 65,
                            Y = 65,
                            Width = 64,
                            Height = 64
                        },
                        Rotated = false,
                        Trimmed = false,
                        Duration = 100
                    }

                },
                LayerDTOs = new LayerDTO[]
                {
                    new LayerDTO
                    {
                        Name = "Ready,loop",
                        Opacity = 255,
                        BlendMode = "normal"
                    },
                    new LayerDTO
                    {
                        Name = "Attack",
                        Opacity = 255,
                        BlendMode = "normal",
                        CelDTOs = new CelDTO[]
                        {
                            new CelDTO
                            {
                                Frame = 1,
                                Data = "isattacking"
                            }
                        }
                    }
                }
            };

            // ACT
            var result = JsonConvert.DeserializeObject<SpritesheetDTO>(mockJsonObject.ToString(),
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new AsepriteSpritesheetJsonConverter() }
            });

            // ASSERT
            AreEqualByJson(expected, result);
        }



        #region Helper Methods

        private static void AreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        #endregion
    }
}