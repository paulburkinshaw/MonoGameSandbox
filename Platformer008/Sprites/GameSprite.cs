using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Platformer008.Sprites
{
    public class GameSprite
    {
        private GameAnimationType? _currentAnimationKey;
        private GameAnimation _currentAnimation;
        public string Name { get; private set; }
        public Texture2D Texture { get; private set; }
        public IDictionary<GameAnimationType, GameAnimation> Animations { get; private set; }

        public event EventHandler<GameAnimationStartedEventArgs> GameAnimationStarted = delegate { };
        public event EventHandler<GameAnimationCompleteEventArgs> GameAnimationComplete = delegate { };

        public GameSprite(string spritesheetImageName, Texture2D spritesheetTexture, IDictionary<GameAnimationType, GameAnimation> animations)
        {
            Name = spritesheetImageName;
            Texture = spritesheetTexture;
            Animations = animations;

            foreach (var animation in Animations)
            {
                Animations[animation.Key].GameAnimationStarted += OnAnimationStarted;
                Animations[animation.Key].GameAnimationComplete += OnAnimationComplete;

                _currentAnimationKey ??= animation.Key;
                _currentAnimation ??= Animations[animation.Key];
            }
        }

        void OnAnimationStarted(object sender, GameAnimationStartedEventArgs args)
        {
            if (GameAnimationStarted != null)
                GameAnimationStarted(this, args);
        }
        void OnAnimationComplete(object sender, GameAnimationCompleteEventArgs args)
        {
            if (GameAnimationComplete != null)
                GameAnimationComplete(this, args);
        }

        public void Update(GameAnimationType key)
        {
            if (Animations.TryGetValue(key, out GameAnimation animation))
            {
                if (_currentAnimationKey != key)
                {
                    _currentAnimation.Stop();
                    _currentAnimation.Reset();
                }

                if (!animation.IsPlaying)
                {
                    animation.Start();
                    _currentAnimationKey = key;
                    _currentAnimation = animation;
                }

                animation.Update();
            }
            else
            {
                Animations[_currentAnimationKey.Value].Stop();
                Animations[_currentAnimationKey.Value].Reset();
            }
        }

        public void DrawToRenderTarget(Vector2 position)
        {
            Animations[_currentAnimationKey.Value].DrawToRenderTarget(position);
        }

        public void Draw()
        {
            Animations[_currentAnimationKey.Value].Draw();
        }
    }
}
