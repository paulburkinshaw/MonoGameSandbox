using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer004.Managers;
using Platformer004.Sprites;
using System.IO;

namespace Platformer004;

public class GameManager
{
    private PlayableSprite _player1;
    private PlayableSprite _player2;
    private TileMap _tileMap;

    // TODO: Delete
    Texture2D _sprite1BoundingBoxTexture;
    Texture2D _sprite2BoundingBoxTexture;
    Rectangle _overlappingBoundingBox;
    Texture2D _overlappingBoundingBoxTexture;
    private RenderTarget2D _boundingBoxRenderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.InternalSize.Width, Globals.InternalSize.Height);
    SpriteFont _font;
    bool _colliding = false;
    Vector2 _collidingPixelsScreenCoordinates = Vector2.Zero;
    Vector2 _collidingPixelsScreenCoordinates2 = Vector2.Zero;
    int hitPoints1;
    int hitPoints2;


    public GameManager()
    {
        _tileMap = new TileMap();
        _player1 = GetPlayer1();
        _player2 = GetPlayer2();

        _font = Globals.Content.Load<SpriteFont>("Font");
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
    }

    public void Draw()
    {
        _player1.DrawToRenderTarget();
        _player2.DrawToRenderTarget();
        DrawBoundingBoxTexturesToRenderTarget();

        Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        
        _tileMap.Draw();
        _player1.Draw();
        _player2.Draw();

        // TODO: Delete
        if (_boundingBoxRenderTarget != null)
            Globals.SpriteBatch.Draw(_boundingBoxRenderTarget, new Rectangle(0, 0, Globals.WindowSize.Width, Globals.WindowSize.Height), Color.White);

        Globals.SpriteBatch.End();
    }

    // TODO: Delete
    private void DrawBoundingBoxTexturesToRenderTarget()
    {
        Globals.GraphicsDevice.SetRenderTarget(_boundingBoxRenderTarget);
        Globals.GraphicsDevice.Clear(Color.Transparent);

        //dddddGlobals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        Globals.SpriteBatch.Begin();
        if (_overlappingBoundingBoxTexture != null)
            Globals.SpriteBatch.Draw(_overlappingBoundingBoxTexture, new Vector2(_overlappingBoundingBox.X, _overlappingBoundingBox.Y), Color.White * 0.5f);
        if (_sprite1BoundingBoxTexture != null)
            Globals.SpriteBatch.Draw(_sprite1BoundingBoxTexture, new Vector2(_player1.BoundingBox.X, _player1.BoundingBox.Y), Color.White * 0.5f);
        if (_sprite2BoundingBoxTexture != null)
            Globals.SpriteBatch.Draw(_sprite2BoundingBoxTexture, new Vector2(_player2.BoundingBox.X, _player2.BoundingBox.Y), Color.White * 0.5f);

        Globals.SpriteBatch.DrawString(_font, $"colliding: {_colliding}", new Vector2(10, 00), Color.White);
        Globals.SpriteBatch.DrawString(_font, $"X1", _collidingPixelsScreenCoordinates, Color.White);
        //Globals.SpriteBatch.DrawString(_font, $"X2", _collidingPixelsScreenCoordinates2, Color.White);

        Globals.SpriteBatch.DrawString(_font, $"_collidingPixelsScreenCoordinates: { _collidingPixelsScreenCoordinates.X}, {_collidingPixelsScreenCoordinates.Y} ", new Vector2(10, 50), Color.White);
        //Globals.SpriteBatch.DrawString(_font, $"_collidingPixelsScreenCoordinates2: {_collidingPixelsScreenCoordinates2.X}, {_collidingPixelsScreenCoordinates2.Y} ", new Vector2(10, 80), Color.White);


        Globals.SpriteBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    private void CheckPlayerCollision()
    {
        // TODO: Delete
        //_sprite1BoundingBoxTexture = GetBoundingBoxTexture(_player1.BoundingBox);
        //_sprite2BoundingBoxTexture = GetBoundingBoxTexture(_player2.BoundingBox);

        _colliding = false;
        _collidingPixelsScreenCoordinates = Vector2.Zero;
        _collidingPixelsScreenCoordinates2 = Vector2.Zero;

        if (_player1.BoundingBox.Intersects(_player2.BoundingBox))
        {
            // TODO: Delete
            //_overlappingBoundingBox = Rectangle.Intersect(_player1.BoundingBox, _player2.BoundingBox);
            //_overlappingBoundingBoxTexture = GetBoundingBoxTexture(_overlappingBoundingBox);

            var tc = TexturesCollide();

            if (tc == true)
            {
                _colliding = true;
            }
                   
        }

    }

    // TODO: Delete
    private Texture2D GetBoundingBoxTexture(Rectangle boundingBox)
    {
        var overlappingBoundingBoxTexture = new Texture2D(Globals.GraphicsDevice, boundingBox.Width, boundingBox.Height);
        Color[] colourData = new Color[boundingBox.Width * boundingBox.Height];
        for (int i = 0; i < colourData.Length; ++i) colourData[i] = Color.Pink;
        overlappingBoundingBoxTexture.SetData(colourData);
        return overlappingBoundingBoxTexture;
    }

    private bool TexturesCollide()
    {
        var currentAnimationFrame1 = _player1.CurrentAnimationFrame;
        var currentAnimationFrame2 = _player2.CurrentAnimationFrame;

        var currentAnimationType1 = _player1.CurrentAnimation.AnimationType;
        var currentAnimationType2 = _player2.CurrentAnimation.AnimationType;

        var sourceRectangle1 = _player1.CurrentAnimation.Frames[currentAnimationFrame1].FrameSourceRectangle;
        var sourceRectangle2 = _player2.CurrentAnimation.Frames[currentAnimationFrame2].FrameSourceRectangle;

        var widthA = sourceRectangle1.Width;
        var heightA = sourceRectangle1.Height;
        var widthB = sourceRectangle2.Width;
        var heightB = sourceRectangle2.Height;

        var matrixA = _player1.Matrix;
        var matrixB = _player2.Matrix;
        var matrixAtoB = _player1.Matrix * Matrix.Invert(_player2.Matrix);
        var matrixBtoA = _player2.Matrix * Matrix.Invert(_player1.Matrix);

        for (int x1 = 0; x1 < widthA; x1++)
        {
            for (int y1 = 0; y1 < heightA; y1++)
            {
                var pixelCoordinateA = new Vector2(x1, y1);
                var pixelCoordinateB = Vector2.Transform(pixelCoordinateA, matrixAtoB);

                int x2 = (int)pixelCoordinateB.X;
                int y2 = (int)pixelCoordinateB.Y;
                if ((x2 >= 0) && (x2 < widthB))
                {
                    if ((y2 >= 0) && (y2 < heightB))
                    {        
                        var colourData1 = _player1.CurrentAnimation.ColourData[new(currentAnimationType1, currentAnimationFrame1)];
                        var colourData2 = _player2.CurrentAnimation.ColourData[new(currentAnimationType2, currentAnimationFrame2)];                     

                        if (colourData1[x1, y1].A > 0)
                        {
                            if (colourData2[x2, y2].A > 0)
                            {
                                _collidingPixelsScreenCoordinates = Vector2.Transform(pixelCoordinateA, matrixA);                             
                                _collidingPixelsScreenCoordinates2 = Vector2.Transform(pixelCoordinateB, matrixB);

                                return true;
                            }
                        }
                    }
                }

            }
        }

        return false;
    }

}
