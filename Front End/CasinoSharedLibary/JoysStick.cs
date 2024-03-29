﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CasinoSharedLibary
{
    public class JoysStick
    {
        private SpritesStorage storage;
        private SpriteBatch painter;
        
        public DrawingButton UpArrow;
        public DrawingButton DownArrow;
        public DrawingButton RightArrow;
        public DrawingButton LeftArrow;


        public JoysStick(SpritesStorage i_givenStorage, SpriteBatch i_painter)
        {
            storage = i_givenStorage;
            painter = i_painter;
        }

        public void Load()
        {
            UpArrow = new DrawingButton(storage.GreenUI[1], storage.Fonts[1]);
            DownArrow = new DrawingButton(storage.GreenUI[2], storage.Fonts[1]);
            RightArrow = new DrawingButton(storage.GreenUI[3], storage.Fonts[1]);
            LeftArrow = new DrawingButton(storage.GreenUI[4], storage.Fonts[1]);
        }

        public Keys Update(GameTime i_gameTime, Vector2 i_mainPlayerPosition)
        {
            UpArrow.Position = new Vector2(-500, -300) + i_mainPlayerPosition;
            UpArrow.Update(i_gameTime, (int)i_mainPlayerPosition.X - 640, (int)i_mainPlayerPosition.Y - 360);
            DownArrow.Position = new Vector2(0, 85) + UpArrow.Position;
            DownArrow.Update(i_gameTime, (int)i_mainPlayerPosition.X - 640, (int)i_mainPlayerPosition.Y - 360);
            RightArrow.Position = new Vector2(40, 50) + UpArrow.Position;
            RightArrow.Update(i_gameTime, (int)i_mainPlayerPosition.X - 640, (int)i_mainPlayerPosition.Y - 360);
            LeftArrow.Position = new Vector2(-50, 50) + UpArrow.Position;
            LeftArrow.Update(i_gameTime, (int)i_mainPlayerPosition.X - 640, (int)i_mainPlayerPosition.Y - 360);

            Keys returnKey = Keys.None;

            if (UpArrow.Clicked)
            {
                returnKey = Keys.Up;
            }
            else if (DownArrow.Clicked)
            {
                returnKey = Keys.Down;
            }
            else if (RightArrow.Clicked)
            {
                returnKey = Keys.Right;
            }
            else if (LeftArrow.Clicked)
            {
                returnKey = Keys.Left;
            }

            return returnKey;
        }

        public void Draw(GameTime i_gameTime)
        {
            UpArrow.Draw(i_gameTime, painter);
            DownArrow.Draw(i_gameTime, painter);
            RightArrow.Draw(i_gameTime, painter);
            LeftArrow.Draw(i_gameTime, painter);
        }
    }
}
