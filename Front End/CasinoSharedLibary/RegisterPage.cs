using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Markup;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Content;

namespace CasinoSharedLibary
{
    public class RegisterPage
    {
        private Game1 gameManager;

        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private ContentManager contentManager;

        private GuiSystem _guiSystem;

        private Button signInButton;
        private Button cancelButton;
        private TextBox userNameTextBox;
        private TextBox emailTextBox;
        private TextBox passWordTextBox;

        public RegisterPage(Game1 i_gameManager, GraphicsDevice i_grapics, ContentManager i_contentManager)
        {
            gameManager = i_gameManager;
            //_spriteBatch = i_spriteBatch;
            _graphics = i_grapics;
            contentManager = i_contentManager;
        }

        public void Load(SpriteBatch i_spriteBatch, SpritesStorage i_storage)
        {
            _spriteBatch = i_spriteBatch;

            DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(_graphics);
            GuiSpriteBatchRenderer guiRenderer = new GuiSpriteBatchRenderer(_graphics, () => Matrix.Identity);
            var font = contentManager.Load<BitmapFont>("Sensation");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);

            signInButton = new Button();
            signInButton.Size = new Size(300, 50);
            signInButton.Content = "Sign In";
            signInButton.VerticalAlignment = VerticalAlignment.Centre;
            signInButton.HorizontalAlignment = HorizontalAlignment.Centre;
            signInButton.Clicked += SignInButton_Clicked;

            cancelButton = new Button();
            cancelButton.Size = new Size(300, 50);
            cancelButton.Content = "Cancel";
            cancelButton.VerticalAlignment = VerticalAlignment.Centre;
            cancelButton.HorizontalAlignment = HorizontalAlignment.Centre;
            cancelButton.Clicked += CancelButton_Clicked;

            userNameTextBox = new TextBox();
            userNameTextBox.Size = new Size(200, 50);
            userNameTextBox.VerticalAlignment = VerticalAlignment.Centre;
            userNameTextBox.HorizontalAlignment = HorizontalAlignment.Centre;

            emailTextBox = new TextBox();
            emailTextBox.Size = new Size(200, 50);
            emailTextBox.VerticalAlignment = VerticalAlignment.Centre;
            emailTextBox.HorizontalAlignment = HorizontalAlignment.Right;

            passWordTextBox = new TextBox();
            passWordTextBox.Size = new Size(200, 50);
            passWordTextBox.VerticalAlignment = VerticalAlignment.Centre;
            passWordTextBox.HorizontalAlignment = HorizontalAlignment.Centre;

            StackPanel leftPanel = new StackPanel() { Size = new Size(100, 100), AttachedProperties = { { DockPanel.DockProperty, Dock.Left } } };
            StackPanel rightPanel = new StackPanel() { Size = new Size(100, 100), AttachedProperties = { { DockPanel.DockProperty, Dock.Right } } };
            StackPanel toptPanel = new StackPanel() { Size = new Size(100, 100), AttachedProperties = { { DockPanel.DockProperty, Dock.Top } } };
            StackPanel bottomPanel = new StackPanel() { Size = new Size(100, 100), AttachedProperties = { { DockPanel.DockProperty, Dock.Bottom } } };

            StackPanel nameStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Items =
                {
                    new Label("User Name:") {Margin = 5, TextColor = Color.Black, Width = 100, VerticalAlignment = VerticalAlignment.Centre},
                    userNameTextBox
                }
            };

            StackPanel emailStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Items =
                {
                    new Label("Email:"){Margin = 5, TextColor = Color.Black, Width = 100, VerticalAlignment = VerticalAlignment.Centre},
                    emailTextBox
                }
            };

            StackPanel passwordStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Items =
                {
                    new Label("PassWord:"){Margin = 5, TextColor = Color.Black, Width = 100, VerticalAlignment = VerticalAlignment.Centre},
                    passWordTextBox
                }

            };

            StackPanel cancelStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Items =
                {

                    cancelButton
                }

            };

            StackPanel signInStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Items =
                {
                    signInButton

                }

            };


            StackPanel middlePanel = new StackPanel()
            {
                Margin = 3,
                Spacing = 5,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Centre,
                VerticalAlignment = VerticalAlignment.Centre,
                //AttachedProperties = { { DockPanel.DockProperty, Dock.Left } } ,
                Items =
                {
                    new StackPanel
                     {
                        Size = new Size(120,100)
                     },
                    nameStackPanel,
                    emailStackPanel,
                    passwordStackPanel,
                    signInStackPanel,
                    cancelStackPanel
                }
            };

            DockPanel mainPanel = new DockPanel()
            {
                BackgroundRegion = i_storage.LoginPageBackground,
                Items =
                {
                    toptPanel,
                    bottomPanel,
                    leftPanel,
                    rightPanel,
                    middlePanel
                }
            };

            Screen registerScreen = new Screen()
            { Content = mainPanel };

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = registerScreen };

        }

        private void SignInButton_Clicked(object sender, EventArgs e)
        {
            if (gameManager.server.SignUp(userNameTextBox.Text, emailTextBox.Text, passWordTextBox.Text))
            {
                gameManager.mainPlayerEmail = emailTextBox.Text;
                gameManager.ScreenType = eScreenType.CasinoRoom;
            }
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            gameManager.ScreenType = eScreenType.LoginPage;
        }

        public void Update(GameTime i_gameTime)
        {
            _guiSystem.Update(i_gameTime);
        }

        public void Draw(GameTime i_gameTime)
        {
            _graphics.Clear(Color.Aqua);
            _guiSystem.Draw(i_gameTime);
        }
    }
}