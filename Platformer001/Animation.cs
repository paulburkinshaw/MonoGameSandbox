using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Platformer001
{
    public class AnimationCompleteEventArgs : EventArgs
    {
        public readonly AnimationType AnimationType;
        
        public AnimationCompleteEventArgs(AnimationType animationType)
        {
            AnimationType = animationType;
        }
    }

    public enum AnimationType 
    { 
        Ready, 
        Walk, 
        Run, 
        Attack1,
        Attack2,
        Jump,
        Hit,
        Fall,
        Standup
    }

    public class Frame
    {
        [JsonProperty("frame")]
        public Rectangle FrameSourceRectangle { get; set; }
        public bool Rotated { get; set; }
        public bool Trimmed { get; set; }
        public Rectangle SpriteSourceSize { get; set; }
        public Rectangle SourceSize { get; set; }
        public int Duration { get; set; }
        public AnimationType AnimationType { get; set; }
        public string FrameNumber { get; set; }
    }

    public class Animation
    {
        private readonly Texture2D _texture;
        private readonly AnimationType _animationType;
        private readonly int _frameCount;
        private readonly List<Frame> _frames;
        private int _currentFrame;
        private static readonly float _framesPerSecond = 10;
        private readonly float _frameDuration = 1 / _framesPerSecond * 1000;   
        private float _elapsedGameTimeMs;
        private bool _active = false;

        public bool Active => _active;
        public static event EventHandler<AnimationCompleteEventArgs> AnimationComplete = delegate { };

        protected virtual void OnAnimationComplete(AnimationCompleteEventArgs args)
        {
            if (AnimationComplete != null)
                AnimationComplete(this, args);
        }

        public AnimationType AnimationType => _animationType;

        public IEnumerable<Frame> Frames => _frames;

        public Animation(Texture2D texture, AnimationType animationType, List<Frame> frames, int frameCount)
        {
            _texture = texture;
            _animationType = animationType;
            _frames = frames;
            _frameCount = frameCount;        
        }

        public void Start()
        {
            _active = true;
        }

        public void Stop()
        {
            _active = false;
        }

        public void Reset()
        {
            _currentFrame = 0;
        }

        public void Update()
        {
            if (!_active) 
            {
                return;
            }
           
            _elapsedGameTimeMs += Globals.ElapsedGameTimeMs;

            if (_elapsedGameTimeMs >= _frameDuration)
            {
                _currentFrame++;
                if (_currentFrame == _frameCount)
                {
                    _currentFrame = 0;
                    OnAnimationComplete(new AnimationCompleteEventArgs(AnimationType));
                }
                    
                _elapsedGameTimeMs = 0;
            }
        }

        public void Draw(Vector2 position)
        {
            Globals.SpriteBatch.Draw(_texture, position, _frames[_currentFrame].FrameSourceRectangle, Color.White, 0, Vector2.Zero, 4.0f, SpriteEffects.None, 1);
        }

    }

    
}
