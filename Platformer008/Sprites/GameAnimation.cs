using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Platformer008.Sprites
{
    public class GameAnimationStartedEventArgs : EventArgs
    {
        public readonly GameAnimation Animation;

        public GameAnimationStartedEventArgs(GameAnimation animation)
        {
            Animation = animation;
        }
    }
    public class GameAnimationCompleteEventArgs : EventArgs
    {
        public readonly GameAnimationType AnimationType;

        public GameAnimationCompleteEventArgs(GameAnimationType animationType)
        {
            AnimationType = animationType;
        }
    }

    public enum GameAnimationType
    {
        Ready,
        Walk,
        Run,
        Block,
        ReverseBlock,
        Attack1,
        Attack2,
        Jump,
        Hit,
        Fall,
        Standup
    }

    public class GameAnimationFrame
    {
        public Rectangle SourceRectangle { get; set; }
        public int Duration { get; set; }
        public bool IsAttacking { get; set; }

        public GameAnimationFrame(Rectangle sourceRectangle, int duration, bool isAttacking = false)
        {
            SourceRectangle = sourceRectangle;
            Duration = duration;
            IsAttacking = isAttacking;
        }
    }

    public class GameAnimation 
    {
        private readonly Texture2D _texture;
        private RenderTarget2D _animationRenderTarget;
        private bool _isPlaying = false;
        private readonly int _frameCount;
        private int _currentFrameNumber;
        private float _elapsedGameTimeMs;

        public List<GameAnimationFrame> Frames { get; private set; }
        public int CurrentFrameNumber => _currentFrameNumber;
        public bool IsPlaying => _isPlaying;
     
        public GameAnimationType AnimationType { get; private set; }

        public bool AnimationLoops { get; private set; }

        public event EventHandler<GameAnimationStartedEventArgs> GameAnimationStarted = delegate { };
        public event EventHandler<GameAnimationCompleteEventArgs> GameAnimationComplete = delegate { };

        public Dictionary<Tuple<GameAnimationType, int>, Color[,]> ColourData { get; private set; }

        protected virtual void OnAnimationStarted(GameAnimationStartedEventArgs args)
        {
            if (GameAnimationStarted != null)
                GameAnimationStarted(this, args);
        }
        protected virtual void OnAnimationComplete(GameAnimationCompleteEventArgs args)
        {
            if (GameAnimationComplete != null)
                GameAnimationComplete(this, args);
        }

        public GameAnimation(GameAnimationType gameAnimationType, 
            Texture2D texture, 
            IEnumerable<GameAnimationFrame> frames,
            int frameCount,
            bool animationLoops, 
            RenderTarget2D animationRenderTarget)
        {
            AnimationType = gameAnimationType;
            _texture = texture;
            _animationRenderTarget = animationRenderTarget;
            _frameCount = frameCount;

            AnimationLoops = animationLoops;
            Frames = frames.ToList();
           
            var colourData = new Dictionary<Tuple<GameAnimationType, int>, Color[,]>();
           
            for (int i = 0; i < Frames.Count(); i++)
            {
                var frameColourData = Globals.GetColourDataFromTexture(_texture, Frames[i].SourceRectangle);
                colourData.Add(new(AnimationType, i), frameColourData);
            }

            ColourData = colourData;
        }
      
        public void Start()
        {
            _isPlaying = true;
            OnAnimationStarted(new GameAnimationStartedEventArgs(this));
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void Reset()
        {
            _currentFrameNumber = 0;
        }

        public void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsedGameTimeMs += Globals.ElapsedGameTimeMs;

            if (_elapsedGameTimeMs >= Globals.Physics.Frameduration)
            {
                _currentFrameNumber++;

                if (_currentFrameNumber == _frameCount)
                {
                    OnAnimationComplete(new GameAnimationCompleteEventArgs(AnimationType));

                    if (AnimationLoops)
                        _currentFrameNumber = 0;
                    else
                        _currentFrameNumber = _currentFrameNumber - 1;
                }

                _elapsedGameTimeMs = 0;
            }

        }

        public void DrawToRenderTarget(Vector2 position)
        {
            Globals.GraphicsDevice.SetRenderTarget(_animationRenderTarget);
            Globals.GraphicsDevice.Clear(Color.Transparent);
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(_texture, position, Frames[_currentFrameNumber].SourceRectangle, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            Globals.SpriteBatch.End();
            Globals.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw()
        {
            Globals.SpriteBatch.Draw(_animationRenderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);
        }

    }
}
