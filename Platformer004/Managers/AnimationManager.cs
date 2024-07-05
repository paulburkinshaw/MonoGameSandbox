using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Platformer004.Managers;

public class AnimationManager
{
    private readonly Dictionary<AnimationType, Animation> _animationDictionary = [];
    private AnimationType? _currentKey;
    private Animation _currentAnimation;
    private RenderTarget2D _animationRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);

    public event EventHandler<AnimationStartedEventArgs> AnimationStarted = delegate { };
    public event EventHandler<AnimationCompleteEventArgs> AnimationComplete = delegate { };

    public AnimationManager()
    {

    }
    void OnAnimationStarted(object sender, AnimationStartedEventArgs args)
    {
        if (AnimationStarted != null)
            AnimationStarted(this, args);
    }
    void OnAnimationComplete(object sender, AnimationCompleteEventArgs args)
    {
        if (AnimationComplete != null)
            AnimationComplete(this, args);
    }

    public void AddAnimation(AnimationType key, Animation animation)
    {
        animation.AnimationStarted += OnAnimationStarted;
        animation.AnimationComplete += OnAnimationComplete;

        _animationDictionary.Add(key, animation);
        _currentKey ??= key;
        _currentAnimation ??= animation;
    }

    public void AddAnimations(string animationsJsonFile, Texture2D texture)
    {
        var animations = GetAnimationsFromJsonFile(animationsJsonFile, texture);

        foreach (var animation in animations)
        {
            AddAnimation(animation.AnimationType, animation);
        }
    }

    private IEnumerable<Animation> GetAnimationsFromJsonFile(string jsonFile, Texture2D texture)
    {
        var jsonObject = JObject.Parse(jsonFile);

        var spritesheetData = JsonConvert.DeserializeObject<SpritesheetData>(
        jsonObject.ToString(),
        new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new SpritesheetDataConverter() }
        });

        return spritesheetData.Frames
            .GroupBy(x => x.AnimationType)
            .Select(g => new Animation(texture: texture,
                                       animationType: g.Key,
                                       frames: g.ToList(),
                                       runOnce: g.ToArray()[0].RunOnce,
                                       frameCount: g.Count(),
                                       animationRenderTarget: _animationRenderTarget)
            );
    }

    public void Update(AnimationType key)
    {
        if (_animationDictionary.TryGetValue(key, out Animation animation))
        {
            if (_currentKey != key)
            {
                _currentAnimation.Stop();
                _currentAnimation.Reset();
            }

            if (!animation.Active)
            {
                animation.Start();
                _currentKey = key;
                _currentAnimation = animation;
            }

            animation.Update();

        }
        else
        {
            _animationDictionary[_currentKey.Value].Stop();
            _animationDictionary[_currentKey.Value].Reset();
        }
    }

    public void DrawToRenderTarget(Vector2 position)
    {
        _animationDictionary[_currentKey.Value].DrawToRenderTarget(position);
    }

    public void Draw()
    {
        _animationDictionary[_currentKey.Value].Draw();
    }

}
