using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Content;

using System.Globalization;
using System.Text.RegularExpressions;

namespace CasinoSharedLibary
{
    public class RegisterPage
    {
        private Game1 gameManager;

        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private ContentManager contentManager;
        private SpritesStorage storage;

        private GuiSystem _guiSystem;

        private Button signInButton;
        private Button cancelButton;
        private TextBox userNameTextBox;
        private TextBox emailTextBox;
        private TextBox passWordTextBox;
        private Label errorMessage;

        private const string emailAlreadyExist = "The email address is already being used.";
        private const string enterRequestedFields = "Please fill the requested fields.";
        private const string generalErrorMessage = "Something went wrong.";
        private const string wrongEmailFormat = "Enter valid email.";


        public RegisterPage(Game1 i_gameManager, GraphicsDevice i_grapics, ContentManager i_contentManager)
        {
            gameManager = i_gameManager;
            _graphics = i_grapics;
            contentManager = i_contentManager;
        }

        public void Load(SpriteBatch i_spriteBatch, SpritesStorage i_storage)
        {
            _spriteBatch = i_spriteBatch;
            storage = i_storage;

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

            errorMessage = new Label(generalErrorMessage);
            errorMessage.IsVisible = false;
            errorMessage.TextColor = Color.Red;
            errorMessage.Width = 300;

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

            StackPanel errorMessagePanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Items =
                {
                    errorMessage
                }
            };

            StackPanel middlePanel = new StackPanel()
            {
                Margin = 3,
                Spacing = 5,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Centre,
                VerticalAlignment = VerticalAlignment.Centre,
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
                    cancelStackPanel,
                    errorMessagePanel
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
            errorMessage.IsVisible = false;
            if (userNameTextBox.Text.Length == 0 || passWordTextBox.Text.Length == 0 ||
                emailTextBox.Text.Length == 0)
            {
                errorMessage.Content = enterRequestedFields;
                errorMessage.IsVisible = true;
            }
            else if(!isValidEmail(emailTextBox.Text))
            {
                errorMessage.Content = wrongEmailFormat;
                errorMessage.IsVisible = true;
            }
            else
            {
                string registerResponse = gameManager.server.SignUp(userNameTextBox.Text,
                    emailTextBox.Text, passWordTextBox.Text);
                if (registerResponse.Contains("Successed"))
                {
                    gameManager.mainPlayerEmail = emailTextBox.Text;
                    gameManager.casinoRoom = new CasinoRoom(gameManager, contentManager, storage);
                    gameManager.casinoRoom.Load(_spriteBatch);
                    userNameTextBox.Text = string.Empty;
                    passWordTextBox.Text = string.Empty;
                    gameManager.ScreenType = eScreenType.CasinoRoom;
                }
                else if (registerResponse.Contains("Email already exist"))
                {
                    errorMessage.Content = emailAlreadyExist;
                    errorMessage.IsVisible = true;
                }
                else
                {
                    errorMessage.Content = generalErrorMessage;
                    errorMessage.IsVisible = true;
                }
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

        private bool isValidEmail(string i_email)
        {
            if (string.IsNullOrWhiteSpace(i_email))
                return false;

            try
            {
                i_email = Regex.Replace(i_email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();

                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(i_email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}