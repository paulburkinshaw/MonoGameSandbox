using Aseprite.NET.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer008.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Platformer008.Services
{
    public interface IGameSpriteService
    {
        GameSprite MapAsepriteSpriteToGameSprite(AsepriteSprite asepriteSprite, Texture2D spritesheetTexture);
    }

    public class GameSpriteService : IGameSpriteService
    {
        public GameSprite MapAsepriteSpriteToGameSprite(AsepriteSprite asepriteSprite, Texture2D spritesheetTexture)
        {
            var animations = MapAsepriteAnimationsToGameAnimations(asepriteSprite.Animations, spritesheetTexture);

            var gameSprite = new GameSprite(asepriteSprite.SpritesheetImageName, spritesheetTexture, animations);

            return gameSprite;
        }

        private IDictionary<GameAnimationType, GameAnimation> MapAsepriteAnimationsToGameAnimations(IEnumerable<AsepriteAnimation> asepriteAnimations, Texture2D spritesheetTexture)
        {
            var animations = new Dictionary<GameAnimationType, GameAnimation>();
            
            var animationRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);

            foreach (var asepriteAnimation in asepriteAnimations)
            {
                var gameAnimationType = GetGameAnimationType(asepriteAnimation.Name);

                var frames = asepriteAnimation.Frames.Select(frame => new GameAnimationFrame(new Rectangle(frame.SourceRectangle.X,
                                                                                              frame.SourceRectangle.Y,
                                                                                              frame.SourceRectangle.Width,
                                                                                              frame.SourceRectangle.Height),
                                                                                              frame.Duration,
                                                                                              GetIsAttackingFromFrameData(frame.FrameData)));

                var gameAnimation = new GameAnimation(gameAnimationType, 
                    texture: spritesheetTexture, 
                    frames: frames, 
                    frameCount: frames.Count(),
                    animationLoops: asepriteAnimation.Loop,
                    animationRenderTarget: animationRenderTarget);

                animations.Add(gameAnimationType, gameAnimation);
            }

            return animations;
        }

        private GameAnimationType GetGameAnimationType(string animationName)
        {
            if (Enum.TryParse(animationName, true, out GameAnimationType animationType))
            {
                return animationType;
            }
            else
            {
                // Handle unknown animation type, maybe throw an exception or log a warning
                throw new ArgumentException($"Unknown animation type: {animationName}");
            }
        }

        private bool GetIsAttackingFromFrameData(string frameData)
        {
            bool isattacking;

            if (string.IsNullOrEmpty(frameData))
                return false;

            if (frameData == nameof(isattacking))
                return true;

            return false;
        }
    }


}
