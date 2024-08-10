using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Abstractions;
using Aseprite.NET.DTOs;
using System.Globalization;

namespace Aseprite.NET.Models
{
    /// <summary>
    /// Represents an Aseprite spritesheet including layers
    /// </summary>
    public class AsepriteSprite
    {
        private IFileSystem _fileSystem;
        private IAsepriteSpritesheetJsonService _asepriteSpritesheetJsonService;

        /// <summary>
        /// The spritesheet image filename minus the file extension
        /// </summary>
        public string SpritesheetImageName { get; private set; }

        /// <summary>
        /// The spritesheet image filename including the file extension
        /// </summary>
        public string SpritesheetImageFileName { get; private set; }

        public IEnumerable<AsepriteAnimation> Animations { get; private set; }

        public AsepriteSprite(IFileSystem fileSystem,
            IAsepriteSpritesheetJsonService asepriteSpritesheetJsonService,
            string spritesheetJsonFilePath)
        {
            _fileSystem = fileSystem;
            _asepriteSpritesheetJsonService = asepriteSpritesheetJsonService;
            InitializeSpritesheet(spritesheetJsonFilePath);
        }

        private void InitializeSpritesheet(string spritesheetJsonFilePath)
        {
            // Construct spritesheet steps
            // - Get SpritesheetDTO from .json file
            // - Convert dto to model
            // - Get animations
            //      - Group frames by animation name
            //      - 

            if (!_fileSystem.File.Exists(spritesheetJsonFilePath))
                throw new FileNotFoundException($"{spritesheetJsonFilePath} not found");

            var spritesheetDTO = _asepriteSpritesheetJsonService.GetSpritesheetDTOFromJsonFile(_fileSystem.File.ReadAllText(spritesheetJsonFilePath));

            SpritesheetImageFileName = spritesheetDTO.ImageName;
            SpritesheetImageName = _fileSystem.Path.GetFileNameWithoutExtension(spritesheetDTO.ImageName);

            var animations = spritesheetDTO.FrameDTOs
            .GroupBy(x => GetAnimationNameFromFilename(x.Filename))
            .Select(g => new AsepriteAnimation
            {
                Name = g.Key,
                Loop = GetLoopFromFilename(g.ToArray()[0].Filename),
                Frames = g.ToArray().Select(frame => new AsepriteFrame
                {
                    FrameNumber = GetFrameNumberFromFilename(frame.Filename),
                    Rotated = frame.Rotated,
                    Trimmed = frame.Trimmed,
                    Duration = frame.Duration,
                    SourceRectangle = new SourceRectangle
                    {
                        X = frame.SourceRectangle.X,
                        Y = frame.SourceRectangle.Y,
                        Width = frame.SourceRectangle.Width,
                        Height = frame.SourceRectangle.Height
                    },
                    FrameData = GetFrameDataFromLayers(spritesheetDTO.LayerDTOs, g.Key, GetFrameNumberFromFilename(frame.Filename))
                })
            });
            Animations = animations;

        }

        private string GetAnimationNameFromFilename(string filename)
        {
            var filenameParts = filename.Split('|');

            var part1Parts = filenameParts[0].Split(',');
            if (part1Parts.Length == 2)
            {
                return part1Parts[0];
            }
            else
            {
                return filenameParts[0];
            }
        }

        private bool GetLoopFromFilename(string filename)
        {
            bool loop = false;

            var filenameParts = filename.Split('|');

            var part1Parts = filenameParts[0].Split(',');
            if (part1Parts.Length == 2)
            {
                loop = part1Parts[1] == nameof(loop) ? true : false;
            }

            return loop;
        }

        private int GetFrameNumberFromFilename(string filename)
        {
            var filenameParts = filename.Split('|');

            if (filenameParts.Length == 2)
            {
                Int32.TryParse(filenameParts[1], out int frameNumber);
                return frameNumber;
            }
            else
                return 0;
        }

        private string GetFrameDataFromLayers(IEnumerable<LayerDTO> layerDtos, string animationName, int frameNumber)
        {
            var layersWithCelData = layerDtos.Where(layerDTO => GetLayerName(layerDTO.Name) == animationName
                                          && layerDTO.CelDTOs?.Where(celDTO => celDTO.Frame == frameNumber).Count() > 0);

            if (!layersWithCelData.Any())
                return null;
          
            return layersWithCelData.FirstOrDefault().CelDTOs.FirstOrDefault().Data;                       
        }

        private string GetLayerName(string fileName)
        {
            var filenameParts = fileName.Split(',');

            return filenameParts[0];
        }

    }

}