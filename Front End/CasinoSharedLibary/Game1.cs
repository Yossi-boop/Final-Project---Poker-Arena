using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Comora;

using Server;
using System.Collections.Generic;
using System;

namespace CasinoSharedLibary
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public int UserScreenWidth { get; set; }
        public int UserScreenHeight { get; set; }

        private SpritesStorage storage;

        public Proxy server;

        public string mainPlayerEmail;

        public eScreenType ScreenType { get; set; } = eScreenType.LoginPage;

        private LoginPage loginPage;
        private RegisterPage registerPage;
        public CasinoRoom casinoRoom;
        public PokerTable pokerTable;

        public static List<CharcterSprite> listOfSprites = new List<CharcterSprite>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            listOfSprites.Add(new CharcterSprite(PlayerSkin.Ninja, 75, 100));
            listOfSprites.Add(new CharcterSprite(PlayerSkin.Jack, 75, 100));
            listOfSprites.Add(new CharcterSprite(PlayerSkin.Zombie, 75, 100));
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            UserScreenWidth = GraphicsDevice.PresentationParameters.Bounds.Width;
            UserScreenHeight = GraphicsDevice.PresentationParameters.Bounds.Height;

            storage = new SpritesStorage(Content, UserScreenHeight, UserScreenWidth);

            //server = new Proxy("http://localhost:61968/");
            server = new Proxy();

            loginPage = new LoginPage(this, GraphicsDevice, Content);
            registerPage = new RegisterPage(this, GraphicsDevice, Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            storage.Load();
            loginPage.Load(_spriteBatch, storage);
            registerPage.Load(_spriteBatch, storage);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                // TODO: Add your update logic here

                switch (ScreenType)
                {
                    case eScreenType.LoginPage:
                        updateLoginPage(gameTime);
                        break;
                    case eScreenType.RegisterPage:
                        registerPage.Update(gameTime);
                        break;
                    case eScreenType.CasinoRoom:
                        updateCasinoRoom(gameTime);
                        break;
                    case eScreenType.PokerTable:
                        updatePokerTable(gameTime);
                        break;
                    default:
                        break;
                }


                base.Update(gameTime);
            }
            catch (Exception) { }
        }

        protected override void Draw(GameTime gameTime)
        {
            try
            {
                GraphicsDevice.Clear(Color.White);

                // TODO: Add your drawing code here

                switch (ScreenType)
                {
                    case eScreenType.LoginPage:
                        drawLoginPage(gameTime);
                        break;
                    case eScreenType.RegisterPage:
                        registerPage.Draw(gameTime);
                        break;
                    case eScreenType.CasinoRoom:
                        drawCasinoRoom(gameTime);
                        break;
                    case eScreenType.PokerTable:
                        drawPokerTable(gameTime);
                        break;
                    default:
                        break;
                }

                base.Draw(gameTime);
            }
            catch (Exception) { }
        }

        private void drawCasinoRoom(GameTime i_gameTime)
        {
            if (casinoRoom != null)
            {
                _spriteBatch.Begin(casinoRoom.camera);

                casinoRoom.Draw(i_gameTime);

                _spriteBatch.End();
            }
        }

        private void updateCasinoRoom(GameTime i_GameTime)
        {
            casinoRoom.IsUpdated = true;
            casinoRoom.Update(i_GameTime);
        }

        private void drawPokerTable(GameTime i_GameTime)
        {
            _spriteBatch.Begin();

            pokerTable.Draw(i_GameTime);

            _spriteBatch.End();
        }

        private void updatePokerTable(GameTime i_GameTime)
        {
            pokerTable.Update(i_GameTime);
        }

        private void drawLoginPage(GameTime i_gameTime)
        {
            loginPage.Draw(i_gameTime);
        }

        private void updateLoginPage(GameTime i_gameTime)
        {
            loginPage.Update(i_gameTime);
        }
    }
}
