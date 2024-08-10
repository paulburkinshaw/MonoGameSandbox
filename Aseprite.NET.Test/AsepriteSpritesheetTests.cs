using Aseprite.NET.DTOs;
using Aseprite.NET.Models;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Abstractions.TestingHelpers;

namespace Aseprite.NET.Test
{
    public class AsepriteSpritesheetTests
    {


        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void Constructor_ReturnsCorrectAsepriteSpritesheet()
        {
            // Arrange         
            var mockSpritesheetJsonString = "mockSpritesheetJsonString";

            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"\Content\Spritesheets\Spritesheet1.json", new MockFileData(mockSpritesheetJsonString) }
            });

            var mockSpritesheetJsonFilePath = @"\Content\Spritesheets\Spritesheet1.json";

            var asepriteSpritesheetJsonServiceMock = new Mock<IAsepriteSpritesheetJsonService>();

            var spritesheetDTO = new SpritesheetDTO
            {
                ImageName = "Image1.png",
                FrameDTOs = new[]
                {
                    new FrameDTO
                    {
                        Filename = "Ready,loop|0",
                        SourceRectangle = new SourceRectangleDTO
                        {
                            X = 0,
                            Y = 0,
                            Width = 64,
                            Height = 64
                        }
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
                        }
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
                        }
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
                        }
                    }
                },
                LayerDTOs = new[]
                {
                    new LayerDTO
                    {
                        Name = "Ready,loop",
                        BlendMode = "nomal",
                        Opacity = 255,
                        CelDTOs = null
                    },
                    new LayerDTO
                    {
                        Name = "Attack",
                        BlendMode = "nomal",
                        Opacity = 255,
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

            asepriteSpritesheetJsonServiceMock
                .Setup(x => x.GetSpritesheetDTOFromJsonFile(It.Is<string>(spritesheetJsonString => spritesheetJsonString == mockSpritesheetJsonString)))
                .Returns(spritesheetDTO);

            var expected = new
            {
                SpritesheetImageName = "Image1",
                SpritesheetImageFileName = "Image1.png",
                Animations = new AsepriteAnimation[]
                {
                    new AsepriteAnimation
                    {
                        Name = "Ready",
                        Loop = true,
                        Frames = new AsepriteFrame[]
                        {
                            new AsepriteFrame
                            {
                                FrameNumber = 0,
                                SourceRectangle = new SourceRectangle
                                {
                                    X = 0,
                                    Y = 0,
                                    Width = 64,
                                    Height = 64
                                },
                                Rotated = false,
                                Trimmed = false,
                                Duration = 0,
                                FrameData = null
                            },
                            new AsepriteFrame
                            {
                                FrameNumber = 1,
                                SourceRectangle = new SourceRectangle
                                {
                                    X = 65,
                                    Y = 0,
                                    Width = 64,
                                    Height = 64
                                },
                                Rotated = false,
                                Trimmed = false,
                                Duration = 0,
                                FrameData = null
                            }
                        }
                    },
                    new AsepriteAnimation
                    {
                        Name = "Attack",
                        Loop = false,
                        Frames = new AsepriteFrame[]
                        {
                            new AsepriteFrame
                            {
                                FrameNumber = 0,
                                SourceRectangle = new SourceRectangle
                                {
                                    X = 0,
                                    Y = 65,
                                    Width = 64,
                                    Height = 64
                                },
                                Rotated = false,
                                Trimmed = false,
                                Duration = 0,
                                FrameData = null
                            },
                            new AsepriteFrame
                            {
                                FrameNumber = 1,
                                SourceRectangle = new SourceRectangle
                                {
                                    X = 65,
                                    Y = 65,
                                    Width = 64,
                                    Height = 64
                                },
                                Rotated = false,
                                Trimmed = false,
                                Duration = 0,
                                FrameData = "isattacking"
                            }
                        }
                    }
                }
            };

            // ACT
            var result = new AsepriteSprite(mockFileSystem, asepriteSpritesheetJsonServiceMock.Object, mockSpritesheetJsonFilePath);

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