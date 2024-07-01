using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer004.Controls
{
    public class Button : Component
    {
        #region Fields

        private MouseState _currentMouse;

        private SpriteFont _font;

        private bool _isHovering;

        private MouseState _previousMouse;

        private Texture2D _textureOff;
        private Texture2D _textureOn;

        private bool onOffState = false;

        #endregion

        #region Properties

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                if (!onOffState)
                    return new Rectangle((int)Position.X, (int)Position.Y, _textureOff.Width, _textureOff.Height);
                else
                    return new Rectangle((int)Position.X, (int)Position.Y, _textureOn.Width, _textureOn.Height);
            } 
    }

        public string Text { get; set; }

        #endregion

        #region Methods

        public Button(Texture2D textureOff, Texture2D textureOn, SpriteFont font)
        {
            _textureOff = textureOff;
            _textureOn = textureOn;
            _font = font;
            PenColour = Color.White ;
        }

        public override void Draw()
        {
            var colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            if(!onOffState)
                Globals.SpriteBatch.Draw(_textureOff, Rectangle, colour);
            else
                Globals.SpriteBatch.Draw(_textureOn, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                Globals.SpriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update()
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                    onOffState = !onOffState;
                }
            }
        }

        #endregion
    }
}
