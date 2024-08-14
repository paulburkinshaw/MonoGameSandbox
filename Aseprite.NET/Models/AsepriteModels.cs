using System.Collections.Generic;

namespace Aseprite.NET.Models
{
    /// <summary>
    /// Represents an Aseprite spritesheet
    /// </summary>
    public class AsepriteSpritesheet
    {
        /// <summary>
        /// The spritesheet image filename minus the file extension
        /// </summary>
        public string SpritesheetImageName { get; private set; }

        /// <summary>
        /// The spritesheet image filename including the file extension
        /// </summary>
        public string SpritesheetImageFileName { get; private set; }

        public IEnumerable<AsepriteAnimation> Animations { get; private set; }

        public AsepriteSpritesheet(string spritesheetImageFileName,
            string spritesheetImageName,
            IEnumerable<AsepriteAnimation> animations)
        {
            SpritesheetImageFileName = spritesheetImageFileName;
            SpritesheetImageName = spritesheetImageName;
            Animations = animations;
        }
    }

    public class AsepriteAnimation
    {
        /// <summary>
        /// The name of the sprite animation e.g. Walk, Run
        /// </summary>
        public string Name { get; set; }

        public bool Loop { get; set; }

        public IEnumerable<AsepriteFrame> Frames { get; set; }
    }

    public class AsepriteFrame
    {     
        public SourceRectangle SourceRectangle { get; set; }

        public int FrameNumber { get; set; }

        public bool Rotated { get; set; }

        public bool Trimmed { get; set; }

        public int Duration { get; set; }

        public string FrameData { get; set; }
    }

    public class SourceRectangle
    {
        /// <summary>
        /// The x position in pixels of the sprite in the source image
        /// </summary>
        public int X;

        /// <summary>
        /// The y position in pixels of the sprite in the source image
        /// </summary>
        public int Y;

        /// <summary>
        /// The width in pixels of the sprite in the source image
        /// </summary>
        public int Width;

        /// <summary>
        /// The height in pixels of the sprite in the source image
        /// </summary>
        public int Height;
    }

}
