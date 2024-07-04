using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Platformer004.Sprites;
using System.IO;
using Platformer004.Controls;
using System.Collections.Generic;

namespace Platformer004.Managers;

public class DebugGameManager
{
    private PlayableSprite _player1;
    private PlayableSprite _player2;
    private TileMap _tileMap;
    private bool _colliding = false;
    SpriteFont _font = Globals.Content.Load<SpriteFont>("Font");
    private RenderTarget2D _collisionRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);
    CollisionData _collisionData = new()
    {
        ScreenCoordinates = Vector2.Zero,
        PixelCoordinatesA = Vector2.Zero,
        PixelCoordinatesB = Vector2.Zero
    };
    private int _player1Score;
    private int _player2Score;
    private float _elapsedGameTimeMs;

    #region debug
    Texture2D _sprite1BoundingBoxTexture;
    Texture2D _sprite2BoundingBoxTexture;
    bool showSprite1BoundingBox = false;
    bool showSprite2BoundingBox = false;

    Rectangle _overlappingBoundingBox;
    Texture2D _overlappingBoundingBoxTexture;
    bool showOverlappingBoundingBox = false;

    private RenderTarget2D _boundingBoxRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);

    private List<Component> _components;
    #endregion


    public DebugGameManager()
    {
        #region debug
        var btnToggleBoundingBox = new CheckBox(Globals.Content.Load<Texture2D>("Controls/CheckBoxUnchecked"), Globals.Content.Load<Texture2D>("Controls/CheckBoxChecked"), Globals.Content.Load<SpriteFont>("Controls/ButtonFont"))
        {
            Position = new Vector2(1, 100),
            Text = "Toggle Bounding Boxes",
        };
        btnToggleBoundingBox.Click += ToggleBoundingBox_Click;

        var btnToggleOverlappingBoundingBox = new CheckBox(Globals.Content.Load<Texture2D>("Controls/CheckBoxUnchecked"), Globals.Content.Load<Texture2D>("Controls/CheckBoxChecked"), Globals.Content.Load<SpriteFont>("Controls/ButtonFont"))
        {
            Position = new Vector2(1, 140),
            Text = "Toggle Overlapping Bounding Box",
        };
        btnToggleOverlappingBoundingBox.Click += ToggleOverlappingBoundingBox_Click;

        _components = new List<Component>()
        {
            btnToggleBoundingBox,
            btnToggleOverlappingBoundingBox
        };
        #endregion

        _tileMap = new TileMap();
        _player1 = GetPlayer1();
        _player2 = GetPlayer2();
    }

    private PlayableSprite GetPlayer1()
    {
        var startPosition = new Vector2(0, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("knight_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\knight_spritesheet_array.json")
        };

        var animationManager = new AnimationManager();

        var inputManager = new InputManager(
            leftKey: Keys.A,
            righttKey: Keys.D,
            attack1Key: Keys.Space,
            attack2Key: Keys.LeftControl,
            runKey: Keys.LeftShift,
            jumpKey: Keys.W);

        var player1 = new PlayableSprite(position: startPosition,
            size: size,
            spriteContent: spriteContent,
            animationManager: animationManager,
            inputManager: inputManager);

        return player1;
    }

    private PlayableSprite GetPlayer2()
    {
        var startPosition = new Vector2(250, 0);
        var size = new Rectangle(0, 0, 64, 64);

        var spriteContent2 = new SpriteContent
        {
            Texture = Globals.Content.Load<Texture2D>("skeleton_spritesheet"),
            AnimationConfig = File.ReadAllText(@"Content\skeleton_spritesheet_array.json")
        };

        var animationManager2 = new AnimationManager();

        var inputManager = new InputManager(
               leftKey: Keys.J,
               righttKey: Keys.L,
               attack1Key: Keys.Enter,
               attack2Key: Keys.RightControl,
               runKey: Keys.RightShift,
               jumpKey: Keys.I);

        var player2 = new PlayableSprite(position: startPosition,
        size: size,
        spriteContent: spriteContent2,
        animationManager: animationManager2,
        inputManager: inputManager);

        return player2;
    }

    public void Update()
    {
        _player1.Update();
        _player2.Update();

        CheckPlayerCollision();

        UpdatePlayerScores();

        #region debug
        if (showSprite1BoundingBox)
            _sprite1BoundingBoxTexture = GetBoundingBoxTexture(_player1.BoundingBox, Color.Red);

        if (showSprite2BoundingBox)
            _sprite2BoundingBoxTexture = GetBoundingBoxTexture(_player2.BoundingBox, Color.Blue);

        _overlappingBoundingBox = Rectangle.Intersect(_player1.BoundingBox, _player2.BoundingBox);
        if (_overlappingBoundingBox.Width > 0 && _overlappingBoundingBox.Height > 0)
            _overlappingBoundingBoxTexture = GetBoundingBoxTexture(_overlappingBoundingBox, Color.Purple);

        foreach (var component in _components)
            component.Update();
        #endregion
    }

    private void CheckPlayerCollision()
    {

        _colliding = false;

        if (_player1.BoundingBox.Intersects(_player2.BoundingBox))
        {
            var texturesCollide = Globals.SpriteTexturesCollide(_player1, _player2, _collisionData);

            if (texturesCollide)
            {
                _colliding = true;
            }
        }

    }

    private void UpdatePlayerScores()
    {
        _elapsedGameTimeMs += Globals.ElapsedGameTimeMs;
       
        if (_colliding)
        {
            if (_elapsedGameTimeMs >= Globals.Physics.Frameduration)
            {
                if (_collisionData.CurrentAnimationFrameA.Hit)
                {
                    _player1Score += 1;
                }
                if (_collisionData.CurrentAnimationFrameB.Hit)
                {
                    _player2Score += 1;
                }
                _elapsedGameTimeMs = 0;
            }
        }
    }

    public void Draw()
    {
        _player1.DrawToRenderTarget();
        _player2.DrawToRenderTarget();

        if (_colliding)
            DrawCollisionTextureToRenderTarget();

        #region debug
        DrawBoundingBoxTexturesToRenderTarget();
        #endregion

        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        if (_colliding)
            Globals.SpriteBatch.Draw(_collisionRenderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);

        _tileMap.Draw();
        _player1.Draw();
        _player2.Draw();
        DrawPlayerScores();

        #region debug
        if (_boundingBoxRenderTarget != null)
            Globals.SpriteBatch.Draw(_boundingBoxRenderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);

        DrawDebugValues();

        foreach (var component in _components)
            component.Draw();
        #endregion

        Globals.SpriteBatch.End();
    }

    private void DrawCollisionTextureToRenderTarget()
    {
        Globals.GraphicsDevice.SetRenderTarget(_collisionRenderTarget);
        Globals.GraphicsDevice.Clear(Color.Red);
        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        Globals.SpriteBatch.Draw(new Texture2D(Globals.GraphicsDevice, 1, 1), new Vector2(0, 0), Color.White);
        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawPlayerScores()
    {
        Globals.SpriteBatch.DrawString(_font, $"Player 1 Score: {_player1Score}", new Vector2(0, 180), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"Player 2 Score: {_player2Score} ", new Vector2(0, 200), Color.White);
    }

    #region debug
    private void ToggleBoundingBox_Click(object sender, System.EventArgs e)
    {
        showSprite1BoundingBox = !showSprite1BoundingBox;
        showSprite2BoundingBox = !showSprite2BoundingBox;
    }
    private void ToggleOverlappingBoundingBox_Click(object sender, System.EventArgs e)
    {
        showOverlappingBoundingBox = !showOverlappingBoundingBox;
    }

    private Texture2D GetBoundingBoxTexture(Rectangle boundingBox, Color color)
    {
        var boundingBoxTexture = new Texture2D(Globals.GraphicsDevice, boundingBox.Width, boundingBox.Height);
        Color[] colourData = new Color[boundingBox.Width * boundingBox.Height];
        for (int i = 0; i < colourData.Length; ++i) colourData[i] = color;
        boundingBoxTexture.SetData(colourData);
        return boundingBoxTexture;
    }

    private void DrawBoundingBoxTexturesToRenderTarget()
    {
        Globals.GraphicsDevice.SetRenderTarget(_boundingBoxRenderTarget);
        Globals.GraphicsDevice.Clear(Color.Transparent);

        Globals.SpriteBatch.Begin();
        if (showOverlappingBoundingBox && _overlappingBoundingBoxTexture != null)
            Globals.SpriteBatch.Draw(_overlappingBoundingBoxTexture, new Vector2(_overlappingBoundingBox.X, _overlappingBoundingBox.Y), Color.White * 0.5f);
        if (showSprite1BoundingBox && _sprite1BoundingBoxTexture != null)
            Globals.SpriteBatch.Draw(_sprite1BoundingBoxTexture, new Vector2(_player1.BoundingBox.X, _player1.BoundingBox.Y), Color.White * 0.5f);
        if (showSprite2BoundingBox && _sprite2BoundingBoxTexture != null)
            Globals.SpriteBatch.Draw(_sprite2BoundingBoxTexture, new Vector2(_player2.BoundingBox.X, _player2.BoundingBox.Y), Color.White * 0.5f);
        if (_collisionData.ScreenCoordinates != Vector2.Zero)
            Globals.SpriteBatch.Draw(GetPixelCollisionTexture(), _collisionData.ScreenCoordinates, Color.White);

        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawDebugValues()
    {
        Globals.SpriteBatch.DrawString(_font, $"Collision: {_colliding}", new Vector2(0, 00), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"Collision Screen Coordinates: {_collisionData.ScreenCoordinates.X}, {_collisionData.ScreenCoordinates.Y} ", new Vector2(0, 20), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"Collision Texture Pixel Coordinates A: {_collisionData.PixelCoordinatesA.X}, {_collisionData.PixelCoordinatesA.Y} ", new Vector2(0, 40), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"Collision Texture Pixel Coordinates B: {_collisionData.PixelCoordinatesB.X}, {_collisionData.PixelCoordinatesB.Y} ", new Vector2(0, 60), Color.White);

    }

    private Texture2D GetPixelCollisionTexture()
    {
        var pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
        Color[] colourData = new Color[1 * 1];
        for (int i = 0; i < colourData.Length; ++i) colourData[i] = Color.White;
        pixelTexture.SetData(colourData);
        return pixelTexture;
    }
    #endregion

}
