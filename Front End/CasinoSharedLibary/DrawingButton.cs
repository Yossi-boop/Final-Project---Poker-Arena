using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace CasinoSharedLibary
{
    public class DrawingButton
    {
        #region Fields

        private MouseState _currentMouse;

        private SpriteFont _font;

        private bool _isHovering;

        private MouseState _previousMouse;

        private Texture2D _texture;

        #endregion

        #region Properties

        public bool IsVisible { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public string Name { get; set; }

        public Size Size { get; set; } = new Size(-1, -1);

        public Rectangle Rectangle
        {
            get
            {
                if (Size.Width == -1 && Size.Height == -1)
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
                }
                else
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, Size.Width, Size.Height);
                }
            }
        }

        public string Text { get; set; }

        #endregion

        #region Methods

        public DrawingButton(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            PenColour = Color.Black;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                var colour = Color.White;

                MouseState mState = Mouse.GetState();

                if (_isHovering)
                    colour = Color.Gray;

                spriteBatch.Draw(_texture, Rectangle, colour);

                if (!string.IsNullOrEmpty(Text))
                {
                    var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                    spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color i_buttonColor)
        {
            if (IsVisible)
            {
                var colour = i_buttonColor;

                MouseState mState = Mouse.GetState();

                if (_isHovering)
                    colour = Color.Gray;

                spriteBatch.Draw(_texture, Rectangle, colour);

                if (!string.IsNullOrEmpty(Text))
                {
                    var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                    spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
                }
            }
        }

        public void Update(GameTime gameTime, int width, int height)
        {
            if (IsEnabled)
            {
                _previousMouse = _currentMouse;
                _currentMouse = Mouse.GetState();

                MouseState mState = Mouse.GetState();

                var mouseRectangle = new Rectangle(mState.X + width, mState.Y + height, 1, 1);


                _isHovering = false;

                if (mouseRectangle.Intersects(Rectangle))
                {
                    _isHovering = true;

                    if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                    {
                        Click?.Invoke(this, new EventArgs());
                    }

                    if (_currentMouse.LeftButton == ButtonState.Pressed)
                    {
                        Clicked = true;
                    }
                    else
                    {
                        Clicked = false;
                    }
                        
                }
            }
        }

        #endregion
    }
}
