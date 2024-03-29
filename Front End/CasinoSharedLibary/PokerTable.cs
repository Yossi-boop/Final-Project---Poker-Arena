﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Content;
using System.Timers;
using Classes;
using System.Threading.Tasks;

namespace CasinoSharedLibary
{
    public class PokerTable
    {
        private Game1 gameManager;
        private GraphicsDevice _graphics;
        private SpriteBatch painter;
        private SpritesStorage storage;
        private ContentManager contentManager;
        private Timer aTimer;
        private Timer bTimer;

        private GuiSystem _guiSystem;


        private string casinoId;
        private string tableId;
        private string userName;
        private string userEmail;
        private string signature;
        private int index = -1;
        private int minimumRaise;
        private int totalPot;
        private bool updateEndRound = false;
        private bool roundEnd;
        private PokerPlayer myPlayer;
        private PokerPlayer SittingPlayer;
        private User playerInformation;



        // Server data
        private Table table = new Table();
        private Round round;
        private RoundPart currentRoundPart;
        private RoundPart lastRoundPart = RoundPart.PreFlop;
        private PokerPlayer currentPlayer;
        private Betting currentBettingRound;
        private PlayerSkin playerSkin;
        //private Server server;

        private NewChat pokerTableChat;
        private KeyboardInput keyboard;
        private Keys currentInput;

        private float height;
        private float width;
        private int tableNumber;
        private Texture2D TableBackground;

        private List<Vector2> playersLocations = new List<Vector2>();
        private List<Vector2> chipLocations = new List<Vector2>();
        private List<Vector2> chipMovingLocation = new List<Vector2>();
        private List<int> cardDrawingLocations = new List<int>();
        private int numberOfSharedCardDrawed = 0;
        private int numberOfCardServed = 0;
        private DateTime timeBetweenCards;

        private Button callButton;
        private Button foldButton;
        private Button raiseButton;
        private Button raiseUpButton;
        private Button raiseDownButton;
        private Button allInButton;
        private TextBox raiseAmountTextbox;
        private TextBox addAmountTextbox;
        private Button handsRatingButton;
        private ProgressBar playerRemainingTime;

        private bool isHandsRatingVisible = false;

        private int currentPlayerIndex;
        private DateTime currentTurnTime;

        private DrawingButton closeHandsRatingButton;

        private StackPanel bottomButtonsPanel;


        private Button exitButton;

        #region Sit Buttons
        private DrawingButton sit0Button;
        private DrawingButton sit1Button;
        private DrawingButton sit2Button;
        private DrawingButton sit3Button;
        private DrawingButton sit4Button;
        private DrawingButton sit5Button;
        private DrawingButton sit6Button;
        private DrawingButton sit7Button;
        private DrawingButton sit8Button;
        private List<DrawingButton> sitButtons;
        #endregion

        #region Enter Money Panel
        private Rectangle enterMoneyRectangle;
        private DrawingButton enterMoneyRaiseUp;
        private DrawingButton enterMoneyRaiseDown;
        private DrawingButton enterMoneyConfirm;
        private DrawingButton enterMoneyExit;
        private DrawingTextbox enterMoneyTextbox;
        private bool isEnterMoneyPanelVisible = false;
        #endregion

        private string lastMessage = null;

        #region Stats Panel
        private DrawingButton closeStatsPanelButton;
        private bool isStatsPanelVisible = false;
        private int currentStatsPlayer = -1;
        #endregion

        #region Find chest panel
        private bool isFindChestPanelVisible = false;
        private DrawingButton findChestConfirm;
        private Rectangle findChestRectangle;
        #endregion

        private DrawingButton volumeOnOffButton;

        public PokerTable(Game1 i_gameManager, GraphicsDevice i_graphics, SpriteBatch i_Painter, SpritesStorage i_Storage, ContentManager i_contentManager, string i_CasinoId, string i_TableId, string i_Email, string i_Name)
        {
            try
            {
                userEmail = i_Email;
                userName = i_Name;
                casinoId = i_CasinoId;
                tableId = i_TableId;
                gameManager = i_gameManager;
                _graphics = i_graphics;
                painter = i_Painter;
                storage = i_Storage;
                contentManager = i_contentManager;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        public void Load()
        {
            try
            {
                initializeIntervals();
                table = gameManager.server.GetTableById(tableId, casinoId, userEmail);
                playerInformation = gameManager.server.GetUserDetails(userEmail);

                width = gameManager.UserScreenWidth / 1280;
                height = gameManager.UserScreenHeight / 720;

                pokerTableChat = new NewChat(storage, 250, 250);
                pokerTableChat.Load(painter);

                keyboard = new KeyboardInput();

                playersLocations.Add(new Vector2(850 * width, 90 * height));
                playersLocations.Add(new Vector2(1040 * width, 220 * height));
                playersLocations.Add(new Vector2(1050 * width, 370 * height));
                playersLocations.Add(new Vector2(885 * width, 510 * height));
                playersLocations.Add(new Vector2(585 * width, 510 * height));
                playersLocations.Add(new Vector2(285 * width, 510 * height));
                playersLocations.Add(new Vector2(150 * width, 370 * height));
                playersLocations.Add(new Vector2(140 * width, 220 * height));
                playersLocations.Add(new Vector2(330 * width, 90 * height));

                chipLocations.Add(new Vector2(playersLocations[0].X - width * 100, (int)playersLocations[0].Y));
                chipLocations.Add(new Vector2(playersLocations[1].X - width * 100, (int)playersLocations[1].Y));
                chipLocations.Add(new Vector2((int)(playersLocations[2].X - width * 100), (int)playersLocations[2].Y));
                chipLocations.Add(new Vector2((int)(playersLocations[3].X), (int)(playersLocations[3].Y - 90 * height)));
                chipLocations.Add(new Vector2((int)(playersLocations[4].X), (int)(playersLocations[4].Y - 90 * height)));
                chipLocations.Add(new Vector2((int)(playersLocations[5].X), (int)(playersLocations[5].Y - 90 * height)));
                chipLocations.Add(new Vector2((int)(playersLocations[6].X + width * 100), (int)playersLocations[6].Y));
                chipLocations.Add(new Vector2((int)(playersLocations[7].X + width * 100), (int)playersLocations[7].Y));
                chipLocations.Add(new Vector2((int)(playersLocations[8].X + width * 100), (int)playersLocations[8].Y));
                chipLocations.Add(new Vector2(585 * (int)width, 190 * (int)height));

                restartChipLocations();

                minimumRaise = 0;

                DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(_graphics);
                GuiSpriteBatchRenderer guiRenderer = new GuiSpriteBatchRenderer(_graphics, () => Matrix.Identity);
                var font = contentManager.Load<BitmapFont>("Sensation");
                BitmapFont.UseKernings = false;
                Skin.CreateDefault(font);

                handsRatingButton = new Button();
                handsRatingButton.Size = new Size(150, 50);
                handsRatingButton.Content = "Hands Rating";
                handsRatingButton.HorizontalAlignment = HorizontalAlignment.Centre;
                handsRatingButton.VerticalAlignment = VerticalAlignment.Centre;
                handsRatingButton.Clicked += HandsRatingButton_Clicked;

                playerRemainingTime = new ProgressBar();
                playerRemainingTime.Progress = 1f;
                playerRemainingTime.Size = new Size(150, 50);
                playerRemainingTime.BarColor = Color.Green;
                playerRemainingTime.IsVisible = false;
                playerRemainingTime.HorizontalAlignment = HorizontalAlignment.Centre;
                playerRemainingTime.VerticalAlignment = VerticalAlignment.Centre;

                callButton = new Button();
                callButton.Size = new Size(150, 50);
                callButton.Content = "Call";
                callButton.Name = "1";
                callButton.HorizontalAlignment = HorizontalAlignment.Centre;
                callButton.VerticalAlignment = VerticalAlignment.Centre;
                callButton.Clicked += MakeAction_Clicked;

                foldButton = new Button();
                foldButton.Size = new Size(150, 50);
                foldButton.Content = "Fold";
                foldButton.Name = "3";
                foldButton.HorizontalAlignment = HorizontalAlignment.Centre;
                foldButton.VerticalAlignment = VerticalAlignment.Centre;
                foldButton.Clicked += MakeAction_Clicked;

                raiseButton = new Button();
                raiseButton.Size = new Size(150, 50);
                raiseButton.Content = "Raise";
                raiseButton.Name = "2";
                raiseButton.HorizontalAlignment = HorizontalAlignment.Centre;
                raiseButton.VerticalAlignment = VerticalAlignment.Centre;
                raiseButton.Clicked += RaiseButton_Clicked;

                raiseUpButton = new Button();
                raiseUpButton.Size = new Size(50, 50);
                raiseUpButton.Content = ">";
                raiseUpButton.HorizontalAlignment = HorizontalAlignment.Centre;
                raiseUpButton.VerticalAlignment = VerticalAlignment.Centre;
                raiseUpButton.IsVisible = false;
                raiseUpButton.IsEnabled = false;
                raiseUpButton.Clicked += RaiseUpButton_Clicked;

                raiseDownButton = new Button();
                raiseDownButton.Size = new Size(50, 50);
                raiseDownButton.Content = "<";
                raiseDownButton.HorizontalAlignment = HorizontalAlignment.Centre;
                raiseDownButton.VerticalAlignment = VerticalAlignment.Centre;
                raiseDownButton.IsVisible = false;
                raiseDownButton.IsEnabled = true;
                raiseDownButton.Clicked += RaiseDownButton_Clicked;

                allInButton = new Button();
                allInButton.Size = new Size(50, 50);
                allInButton.Content = "All In";
                allInButton.HorizontalAlignment = HorizontalAlignment.Centre;
                allInButton.VerticalAlignment = VerticalAlignment.Centre;
                allInButton.IsVisible = false;
                allInButton.IsEnabled = false;
                allInButton.Clicked += allInButton_Clicked;

                raiseAmountTextbox = new TextBox(minimumRaise.ToString());
                raiseAmountTextbox.Size = new Size(100, 50);
                raiseAmountTextbox.IsVisible = false;
                raiseAmountTextbox.IsEnabled = true;
                raiseAmountTextbox.HorizontalAlignment = HorizontalAlignment.Centre;
                raiseAmountTextbox.VerticalAlignment = VerticalAlignment.Centre;
                raiseAmountTextbox.TextChanged += raiseAmountTextbox_TextChanged;

                addAmountTextbox = new TextBox("0");
                addAmountTextbox.Size = new Size(100, 50);
                addAmountTextbox.IsVisible = true;
                addAmountTextbox.IsEnabled = true;
                addAmountTextbox.HorizontalAlignment = HorizontalAlignment.Centre;
                addAmountTextbox.VerticalAlignment = VerticalAlignment.Centre;

                exitButton = new Button();
                exitButton.Size = new Size(200, 50);
                exitButton.Content = "Exit Back To Casino";
                exitButton.HorizontalAlignment = HorizontalAlignment.Centre;
                exitButton.VerticalAlignment = VerticalAlignment.Centre;
                exitButton.Clicked += ExitButton_Clicked;

                closeHandsRatingButton = new DrawingButton(storage.GreenUI[7], storage.Fonts[1]);
                closeHandsRatingButton.Position = new Vector2(1070, 80);
                closeHandsRatingButton.Size = new Size(50, 50);
                closeHandsRatingButton.IsEnabled = false;
                closeHandsRatingButton.IsVisible = false;
                closeHandsRatingButton.Click += CloseHandsRatingButton_Click;

                closeStatsPanelButton = new DrawingButton(storage.GreyUI[7], storage.Fonts[1]);
                closeStatsPanelButton.Position = new Vector2(765, 125);
                closeStatsPanelButton.Size = new Size(50, 50);
                closeStatsPanelButton.Click += CloseStatsPanel_Click;

                sit0Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit0Button.Text = "Sit";
                sit0Button.Name = "0";
                sit0Button.Size = new Size(100, 50);
                sit0Button.Position = playersLocations[0];
                sit0Button.Click += SitButton_Clicked;

                sit1Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit1Button.Text = "Sit";
                sit1Button.Name = "1";
                sit1Button.Size = new Size(100, 50);
                sit1Button.Position = playersLocations[1];
                sit1Button.Click += SitButton_Clicked;

                sit2Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit2Button.Text = "Sit";
                sit2Button.Name = "2";
                sit2Button.Size = new Size(100, 50);
                sit2Button.Position = playersLocations[2];
                sit2Button.Click += SitButton_Clicked;

                sit3Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit3Button.Text = "Sit";
                sit3Button.Name = "3";
                sit3Button.Size = new Size(100, 50);
                sit3Button.Position = playersLocations[3];
                sit3Button.Click += SitButton_Clicked;

                sit4Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit4Button.Text = "Sit";
                sit4Button.Name = "4";
                sit4Button.Size = new Size(100, 50);
                sit4Button.Position = playersLocations[4];
                sit4Button.Click += SitButton_Clicked;

                sit5Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit5Button.Text = "Sit";
                sit5Button.Name = "5";
                sit5Button.Size = new Size(100, 50);
                sit5Button.Position = playersLocations[5];
                sit5Button.Click += SitButton_Clicked;

                sit6Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit6Button.Text = "Sit";
                sit6Button.Name = "6";
                sit6Button.Size = new Size(100, 50);
                sit6Button.Position = playersLocations[6];
                sit6Button.Click += SitButton_Clicked;

                sit7Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit7Button.Text = "Sit";
                sit7Button.Name = "7";
                sit7Button.Size = new Size(100, 50);
                sit7Button.Position = playersLocations[7];
                sit7Button.Click += SitButton_Clicked;

                sit8Button = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                sit8Button.Text = "Sit";
                sit8Button.Name = "8";
                sit8Button.Size = new Size(100, 50);
                sit8Button.Position = playersLocations[8];
                sit8Button.Click += SitButton_Clicked;

                sitButtons = new List<DrawingButton> { sit0Button, sit1Button, sit2Button, sit3Button, sit4Button, sit5Button, sit6Button, sit7Button, sit8Button };

                enterMoneyRaiseUp = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                enterMoneyRaiseUp.Text = "+";
                enterMoneyRaiseUp.Click += AddUpButton_Clicked;

                enterMoneyRaiseDown = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                enterMoneyRaiseDown.Text = "-";
                enterMoneyRaiseDown.Click += AddDownButton_Clicked;

                enterMoneyConfirm = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                enterMoneyConfirm.Text = "Confirm";
                enterMoneyConfirm.Click += AddButton_Clicked;

                enterMoneyExit = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                enterMoneyExit.Text = "Exit";
                enterMoneyExit.Click += exitRebuyPanel_Clicked;

                enterMoneyTextbox = new DrawingTextbox(150, 50, storage.GreenUI[6], storage.Fonts[1]);
                enterMoneyTextbox.message = "0";

                enterMoneyRectangle = new Rectangle(450, 130, 400, 400);

                volumeOnOffButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                if (MediaPlayer.Volume == 1f)
                {
                    volumeOnOffButton.Text = "Sound On";
                }
                else
                {
                    volumeOnOffButton.Text = "Sound Off";
                }
                volumeOnOffButton.Click += VolumeOnOffButton_Click;

                findChestConfirm = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                findChestConfirm.Text = "Confrim";
                findChestConfirm.Click += FindChestConfirm_Click;

                StackPanel fixedButtonsPanel = new StackPanel()
                {
                    Margin = 5,
                    Spacing = 5,
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Centre,
                    Items =
                {
                    playerRemainingTime,
                    handsRatingButton
                }
                };

                bottomButtonsPanel = new StackPanel()
                {
                    Spacing = 5,
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Centre,
                    VerticalAlignment = VerticalAlignment.Centre,
                    Height = 100,
                    Items =
                {
                    callButton,
                    foldButton,
                    raiseButton,
                    raiseDownButton,
                    raiseUpButton,
                    allInButton,
                    raiseAmountTextbox
                }
                };

                StackPanel bottomPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    AttachedProperties = { { DockPanel.DockProperty, Dock.Bottom } },
                    Items =
                {
                    fixedButtonsPanel,
                    bottomButtonsPanel
                }
                };

                StackPanel topButtonPanel = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    AttachedProperties = { { DockPanel.DockProperty, Dock.Top } },
                    Items =
                {
                    exitButton
                }
                };

                DockPanel pokerTablePanel = new DockPanel()
                {
                    LastChildFill = false,
                    BackgroundRegion = storage.PokerBackGround,
                    Items =
                {
                    topButtonPanel,
                    bottomPanel
                }
                };

                Screen pokerTableScreen = new Screen()
                {
                    Content = pokerTablePanel
                };

                _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = pokerTableScreen };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void FindChestConfirm_Click(object sender, EventArgs e)
        {
            isFindChestPanelVisible = false;
        }

        private void VolumeOnOffButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MediaPlayer.Volume == 1f)
                {
                    MediaPlayer.Volume = 0f;
                    volumeOnOffButton.Text = "Sound Off";
                }
                else
                {
                    MediaPlayer.Volume = 1f;
                    volumeOnOffButton.Text = "Sound On";
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void allInButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (myPlayer != null)
                {
                    raiseAmountTextbox.Text = (myPlayer.Money + myPlayer.CurrentRoundBet).ToString();
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private void raiseAmountTextbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int raiseAmount;
                if (!(int.TryParse(raiseAmountTextbox.Text, out raiseAmount)))
                {
                    raiseAmountTextbox.Text = minimumRaise.ToString();
                }
                else if (myPlayer != null && raiseAmount > myPlayer.Money + myPlayer.CurrentRoundBet)
                {
                    raiseAmountTextbox.Text = (myPlayer.Money + myPlayer.CurrentRoundBet).ToString();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void CloseStatsPanel_Click(object sender, EventArgs e)
        {
            try {
                closeStatsPanel();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void closeStatsPanel()
        {
            try
            {
                isStatsPanelVisible = false;
            }
            catch(Exception ex)
            {
                
            }
        }

        private void CloseHandsRatingButton_Click(object sender, EventArgs e)
        {
            try {
                isHandsRatingVisible = false;
            }
            catch (Exception ex)
            {
                
            }
        }
        private void exitRebuyPanel_Clicked(object sender, EventArgs e)
        {
            try {
                isEnterMoneyPanelVisible = false;
            }
            catch (Exception ex)
            {
                
            }
        }
        private void HandsRatingButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                handsRatingPictureSwitch();
            }
            catch(Exception ex)
            {
                
            }
        }

        private void handsRatingPictureSwitch()
        {
            try {
                isHandsRatingVisible = !isHandsRatingVisible;
                closeHandsRatingButton.IsVisible = true;
                closeHandsRatingButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                
            }
        }

        private void MakeAction_Clicked(object sender, EventArgs e)
        {
            try {
                Button button = sender as Button;
                gameManager.server.MakeAnAction(tableId, casinoId, userEmail, signature, int.Parse(button.Name), int.Parse(raiseAmountTextbox.Text));
            }
            catch (Exception ex)
            {
                
            }
        }

        private void SitButton_Clicked(object sender, EventArgs e)
        {
            try {
                DrawingButton button = sender as DrawingButton;
                index = int.Parse(button.Name);
                string result = gameManager.server.AddPlayerToTable(tableId, casinoId, userEmail, userName, 1000, int.Parse(button.Name));
                if(result.Contains("User not have enough balance"))
                {
                    isFindChestPanelVisible = true;
                }
                else
                {
                    signature = result;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void initializeIntervals()
        {
            try
            {
                aTimer = new System.Timers.Timer();
                aTimer.Interval = 600;
                aTimer.Elapsed += getData;

                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                
            }
        }

        private void getData(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Table tempTable = gameManager.server.GetTableById(tableId, casinoId, userEmail);
                if (tempTable != null && tempTable.Players != null)
                {
                    table = tempTable;
                    round = table.CurrentRound;
                    if (round != null)
                    {
                        currentRoundPart = round.Part;
                        lastRoundPart = (currentRoundPart == RoundPart.PreFlop) ? currentRoundPart : lastRoundPart;
                        currentBettingRound = round.currentBettingRound;
                        if(currentBettingRound != null)
                            currentPlayer = currentBettingRound.CurrentPlayer;
                        cardDrawingLocations = calculateCardLocation(round);
                    }

                    List<Message> testChatData = gameManager.server.GetTableChatMessages(tableId, casinoId);
                    if (testChatData != null)
                    {
                        pokerTableChat.UpdateMessageList(testChatData);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private List<int> calculateCardLocation(Round round)
        {
            try
            {
                List<int> list = new List<int>();
                int sitNumber = round.Dealer + 1;
                if (round.ActivePlayersIndex != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (round.ActivePlayersIndex[(sitNumber + i) % 9] != null && round.ActivePlayersIndex[(sitNumber + i) % 9].InHand)
                        {
                            list.Add((sitNumber + i) % 9);
                        }
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        if (round.ActivePlayersIndex[(sitNumber + i) % 9] != null && round.ActivePlayersIndex[(sitNumber + i) % 9].InHand)
                        {
                            list.Add(((sitNumber + i) % 9) + 10);
                        }
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void AddDownButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(enterMoneyTextbox.message) - table.GameSetting.MinBalance >= table.GameSetting.MinBalance)
                {
                    enterMoneyTextbox.updateMessageExtern((int.Parse(enterMoneyTextbox.message) - table.GameSetting.MinBalance).ToString());
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void AddUpButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (myPlayer != null && myPlayer.Stat != null)
                {
                    if (int.Parse(enterMoneyTextbox.message) + table.GameSetting.MinBalance < myPlayer.Stat.Money && int.Parse(enterMoneyTextbox.message) + table.GameSetting.MinBalance <= table.GameSetting.MaxBalance)
                    {
                        enterMoneyTextbox.updateMessageExtern((int.Parse(enterMoneyTextbox.message) + table.GameSetting.MinBalance).ToString());
                    }
                    else
                    {
                        enterMoneyTextbox.updateMessageExtern(myPlayer.Stat.Money.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                gameManager.server.AddOnReBuy(tableId, casinoId, userEmail, signature, int.Parse(enterMoneyTextbox.message));
                isEnterMoneyPanelVisible = false;
            }
            catch (Exception ex)
            {
               
            }
        }
        private void RaiseDownButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(raiseAmountTextbox.Text) > minimumRaise)
                {
                    raiseAmountTextbox.Text = (int.Parse(raiseAmountTextbox.Text) - currentBettingRound.RaiseJump).ToString();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void RaiseUpButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(raiseAmountTextbox.Text) + currentBettingRound.RaiseJump < myPlayer.Money + myPlayer.CurrentRoundBet)
                {
                    raiseAmountTextbox.Text = (int.Parse(raiseAmountTextbox.Text) + currentBettingRound.RaiseJump).ToString();
                }
                else
                {
                    raiseAmountTextbox.Text = (myPlayer.Money + myPlayer.CurrentRoundBet).ToString();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void RaiseButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (((string)raiseButton.Content).Equals("Confirm"))
                {
                    Button button = sender as Button;
                    gameManager.server.MakeAnAction(tableId, casinoId, userEmail, signature, int.Parse(button.Name), int.Parse(raiseAmountTextbox.Text));
                    raiseButton.Content = "Raise";
                    raiseUpButton.IsVisible = false;
                    raiseUpButton.IsEnabled = false;
                    raiseDownButton.IsVisible = false;
                    raiseDownButton.IsEnabled = false;
                    allInButton.IsVisible = false;
                    allInButton.IsEnabled = false;
                    raiseAmountTextbox.IsVisible = false;
                }
                else if (((string)raiseButton.Content).Equals("Raise"))
                {
                    raiseButton.Content = "Confirm";
                    raiseUpButton.IsVisible = true;
                    raiseUpButton.IsEnabled = true;
                    raiseDownButton.IsVisible = true;
                    raiseDownButton.IsEnabled = true;
                    allInButton.IsVisible = true;
                    allInButton.IsEnabled = true;
                    raiseAmountTextbox.IsVisible = true;
                    minimumRaise = currentBettingRound.MinimumBet;
                    raiseAmountTextbox.Text = minimumRaise.ToString();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void ExitButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                gameManager.server.SitOut(casinoId, tableId, userEmail, signature, true);
                aTimer.Enabled = false;
                gameManager.ScreenType = eScreenType.CasinoRoom;
            }
            catch (Exception ex)
            {
                
            }
        }

        public void Update(GameTime i_gametime)
        {
            try
            {
                aTimer.Enabled = true;
                currentInput = keyboard.Update();
                lastMessage = pokerTableChat.Update(i_gametime, new Vector2(640, 360), currentInput, keyboard.isCapsLockOn, keyboard.isShiftOn);
                if (lastMessage != null)
                {
                    gameManager.server.SendMessageToTableChat(tableId, casinoId, userEmail, signature, userName, lastMessage);
                }
                updateChat(i_gametime);

                updateDrawHandsRating(i_gametime);

                updateStatsPanel(i_gametime);

                volumeOnOffButton.Update(i_gametime, 0, 0);

                activateBettingButtons();
                foreach (DrawingButton button in sitButtons)
                {
                    button.Update(i_gametime, 0, 0);
                }

                if (table != null)
                {
                    index = checkIfInTable();
                    if (index == -1)
                    {

                        myPlayer = null;

                    }
                    else
                    {
                        if (table.Players[index] != null)
                        {
                            myPlayer = table.Players[index];
                            SittingPlayer = table.Players[index];
                            signature = myPlayer.Signature;
                        }
                    }

                    if (round != null)
                    {
                        movingChipsInEndOfPart();
                        if (currentRoundPart == RoundPart.Result)
                        {
                            bottomButtonsPanel.IsEnabled = false;
                            bottomButtonsPanel.IsVisible = false;
                            playerRemainingTime.IsVisible = false;
                            if (!updateEndRound)
                            {
                                Task.Delay(8000).ContinueWith(task => gameManager.server.ConfirmEndRound(tableId, casinoId, userEmail));
                                updateEndRound = true;
                                if (myPlayer != null)
                                {
                                    CheckIfNeedReBuy();
                                }
                                hideBettingButtons();
                            }
                        }
                        else
                        {
                            updateEndRound = false;
                            roundEnd = false;
                            if (myPlayer != null && myPlayer.InHand &&
                                currentPlayer.Email.Equals(myPlayer.Email))
                            {
                                bottomButtonsPanel.IsEnabled = true;
                                bottomButtonsPanel.IsVisible = true;
                                CheckOrCall();
                            }
                            else
                            {
                                bottomButtonsPanel.IsEnabled = false;
                                bottomButtonsPanel.IsVisible = false;
                            }

                            if (currentPlayerIndex != currentBettingRound.CurrentPlayerIndex || currentRoundPart != lastRoundPart)
                            {
                                currentPlayerIndex = currentBettingRound.CurrentPlayerIndex;
                                currentTurnTime = DateTime.Now;
                            }
                            playerRemainingTime.IsVisible = true;
                            playerRemainingTime.Progress = (float)DateTime.Now.Subtract(currentTurnTime).TotalMilliseconds / 15000;
                            if (playerRemainingTime.Progress > 1f)
                            {
                                playerRemainingTime.Progress = 1f;
                            }
                        }
                    }
                    else
                    {
                        bottomButtonsPanel.IsEnabled = false;
                        bottomButtonsPanel.IsVisible = false;
                    }
                }

                _guiSystem.Update(i_gametime);
                updateEnterMoneyPanel(i_gametime, currentInput);
                updateFindChestPanel(i_gametime);
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        private void updateFindChestPanel(GameTime i_gametime)
        {
            if (isFindChestPanelVisible)
            {
                findChestRectangle = new Rectangle(400, 250, 550, 200);
                findChestConfirm.Position = new Vector2(findChestRectangle.X + 180, findChestRectangle.Y + 135);
                findChestConfirm.Update(i_gametime, 0, 0);
            }
        }

        private int checkIfInTable()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    if (table.PlayersInTable[i] != null && table.PlayersInTable[i].Email.Equals(userEmail))
                    {
                        return i;
                    }
                }

                return -1;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        private void movingChipsInEndOfPart()
        {
            try
            {
                bool partChange = currentRoundPart != lastRoundPart;
                if (!partChange && currentRoundPart == RoundPart.Result && !roundEnd)
                {
                    if (totalBetStopMoving() || round.WinnersIndex.Count > 1)
                    {
                        numberOfCardServed = 0;
                        roundEnd = true;
                        restartChipLocations();
                    }
                    else
                    {
                        moveChipsToTheWinner();
                    }
                }
                else
                {
                    if (partChange && currentRoundPart != RoundPart.PreFlop)
                    {
                        if (chipsStopMoving())
                        {
                            MediaPlayer.Play(storage.CoinsMusic2);
                            if (numberOfCardServed != 0 || (currentRoundPart != RoundPart.River && numberOfCardServed == 0))
                            {
                                lastRoundPart = currentRoundPart;
                            }
                            restartChipLocations();
                        }
                        else
                        {
                            collectMoneyFromPlayer();
                        }
                    }
                    else
                    {
                        restartChipLocations();
                    }
                }
            }
            catch (Exception e)
            {
                
                throw e;
            }

        }

        private bool totalBetStopMoving()
        {
            try
            {
                int numberOfChipsLocations = chipMovingLocation.Count - 1;
                while (round.WinnersIndex.Count + 8 != numberOfChipsLocations)
                {
                    chipMovingLocation.Add(new Vector2(chipLocations[9].X, chipLocations[9].Y));
                    numberOfChipsLocations++;
                }
                for (int i = 0; i < round.WinnersIndex.Count; i++)
                {
                    if (!(chipMovingLocation[9 + i].X <= (chipLocations[round.WinnersIndex[i]].X + 5) && chipMovingLocation[9].X >= (chipLocations[round.WinnersIndex[i]].X - 5)))
                    {
                        return false;
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        private void restartChipLocations()
        {
            try
            {
                chipMovingLocation.Clear();
                chipMovingLocation.Add(new Vector2(playersLocations[0].X - width * 100, (int)playersLocations[0].Y));
                chipMovingLocation.Add(new Vector2(playersLocations[1].X - width * 100, (int)playersLocations[1].Y));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[2].X - width * 100), (int)playersLocations[2].Y));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[3].X), (int)(playersLocations[3].Y - 90 * height)));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[4].X), (int)(playersLocations[4].Y - 90 * height)));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[5].X), (int)(playersLocations[5].Y - 90 * height)));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[6].X + width * 100), (int)playersLocations[6].Y));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[7].X + width * 100), (int)playersLocations[7].Y));
                chipMovingLocation.Add(new Vector2((int)(playersLocations[8].X + width * 100), (int)playersLocations[8].Y));
                chipMovingLocation.Add(new Vector2(585 * (int)width, 190 * (int)height));
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }
        private bool chipsStopMoving()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    if (!(chipMovingLocation[i].X <= (chipLocations[9].X + 5) && chipMovingLocation[i].X >= (chipLocations[9].X - 5)))
                    {
                        if (round.ActivePlayersIndex[i] != null)
                        {
                            return false;
                        }
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        private void collectMoneyFromPlayer()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    float diffrenceX = (chipLocations[9].X - chipLocations[i].X) / (float)50;
                    float diffrenceY = (chipLocations[9].Y - chipLocations[i].Y) / (float)50;
                    float x = chipMovingLocation[i].X + diffrenceX;
                    float y = chipMovingLocation[i].Y + diffrenceY;
                    chipMovingLocation[i] = new Vector2(x, y);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void moveChipsToTheWinner()
        {
            try
            {
                int winner = round.WinnersIndex[0];
                float diffrenceX = (chipLocations[9].X - chipLocations[winner].X) / (float)50;
                float diffrenceY = (chipLocations[9].Y - chipLocations[winner].Y) / (float)50;
                float x = chipMovingLocation[9].X - diffrenceX;
                float y = chipMovingLocation[9].Y - diffrenceY;
                chipMovingLocation[9] = new Vector2(x, y);
            }
            catch (Exception ex)
            {
                
            }
        }

        private void updateStatsPanel(GameTime i_gameTime)
        {
            try
            {
                if (!isStatsPanelVisible)
                {
                    MouseState mState = Mouse.GetState();
                    Rectangle mouseRectange = new Rectangle(mState.X, mState.Y, 1, 1);
                    Rectangle playerRectangel = new Rectangle(0, 0, (int)(75 * width), (int)(75 + storage.Cards[0].Height * height));
                    for (int i = 0; i < playersLocations.Count; i++)
                    {
                        playerRectangel.X = (int)playersLocations[i].X;
                        playerRectangel.Y = (int)(playersLocations[i].Y - 60 * height);
                        if (mouseRectange.Intersects(playerRectangel) && mState.LeftButton == ButtonState.Pressed && table.PlayersInTable[i] != null)
                        {
                            isStatsPanelVisible = true;
                            currentStatsPlayer = i;
                        }
                    }
                }
                else
                {
                    closeStatsPanelButton.Update(i_gameTime, 0, 0);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void updateDrawHandsRating(GameTime i_gameTime)
        {
            try
            {
                if (isHandsRatingVisible)
                {
                    closeHandsRatingButton.Update(i_gameTime, 0, 0);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void updateEnterMoneyPanel(GameTime i_gameTime, Keys i_input)
        {
            try
            {
                if (isEnterMoneyPanelVisible && myPlayer != null)
                {
                    enterMoneyRaiseUp.IsEnabled = true;
                    enterMoneyRaiseUp.IsVisible = true;
                    enterMoneyRaiseDown.IsEnabled = true;
                    enterMoneyRaiseDown.IsVisible = true;
                    enterMoneyConfirm.IsEnabled = true;
                    enterMoneyConfirm.IsVisible = true;
                    enterMoneyExit.IsEnabled = true;
                    enterMoneyExit.IsVisible = true;
                    enterMoneyRaiseUp.Position = new Vector2(enterMoneyRectangle.X + 290, enterMoneyRectangle.Y + 150);
                    enterMoneyRaiseUp.Size = new Size(80, 50);
                    enterMoneyRaiseUp.Update(i_gameTime, 0, 0);
                    enterMoneyRaiseDown.Position = new Vector2(enterMoneyRectangle.X + 30, enterMoneyRectangle.Y + 150);
                    enterMoneyRaiseDown.Size = new Size(80, 50);
                    enterMoneyRaiseDown.Update(i_gameTime, 0, 0);
                    enterMoneyConfirm.Position = new Vector2(enterMoneyRectangle.X + 30, enterMoneyRectangle.Y + 330);
                    enterMoneyConfirm.Size = new Size(150, 50);
                    enterMoneyConfirm.Update(i_gameTime, 0, 0);
                    enterMoneyExit.Position = new Vector2(enterMoneyRectangle.X + 250, enterMoneyRectangle.Y + 330);
                    enterMoneyExit.Size = new Size(120, 50);
                    enterMoneyExit.Update(i_gameTime, 0, 0);
                    enterMoneyTextbox.Update(i_input, new Vector2(enterMoneyRaiseDown.Position.X + 93, enterMoneyRaiseDown.Position.Y));
                    if (enterMoneyTextbox.message.Length > 0)
                    {
                        if (int.Parse(enterMoneyTextbox.message) > myPlayer.Stat.Money)
                        {
                            if(myPlayer.Stat.Money <= table.GameSetting.MaxBalance)
                                enterMoneyTextbox.updateMessageExtern(myPlayer.Stat.Money.ToString());
                            else
                                enterMoneyTextbox.updateMessageExtern(table.GameSetting.MaxBalance.ToString());
                        }
                    }
                }
                else
                {
                    isEnterMoneyPanelVisible = false;
                    enterMoneyRaiseUp.IsEnabled = false;
                    enterMoneyRaiseUp.IsVisible = false;
                    enterMoneyRaiseDown.IsEnabled = false;
                    enterMoneyRaiseDown.IsVisible = false;
                    enterMoneyConfirm.IsEnabled = false;
                    enterMoneyConfirm.IsVisible = false;
                    enterMoneyExit.IsEnabled = false;
                    enterMoneyExit.IsVisible = false;
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void updateChat(GameTime i_gametime)
        {
            try
            {
                pokerTableChat.ChatButton.Position = new Vector2(1280 - storage.GreenUI[0].Width, 720 - storage.GreenUI[0].Height);
                pokerTableChat.ChatButton.Update(i_gametime, 0, 0);
                pokerTableChat.MoveChatUpButton.Position = new Vector2(pokerTableChat.ChatMessagesWidth + 10, 410);
                pokerTableChat.MoveChatUpButton.Update(i_gametime, 0, 0);
                pokerTableChat.MoveChatDownButton.Position = new Vector2(pokerTableChat.ChatMessagesWidth + 10, 605);
                pokerTableChat.MoveChatDownButton.Update(i_gametime, 0, 0);
                pokerTableChat.SendMessageButton.Position = new Vector2(0, 680);
                pokerTableChat.SendMessageButton.Update(i_gametime, 0, 0);
                pokerTableChat.MoveChatToLastMessage.Position = new Vector2(pokerTableChat.ChatMessagesWidth + 10, 655);
                pokerTableChat.MoveChatToLastMessage.Update(i_gametime,0 ,0);
            }
            catch (Exception e)
            {
                
            }
        }

        private void CheckIfNeedReBuy()
        {
            try
            {
                if (myPlayer.Money <= 0)
                {
                    if (myPlayer.Stat.Money >= table.GameSetting.MinBalance)
                        Task.Delay(3000).ContinueWith(task => changeRebuyPanel());
                    else
                        isFindChestPanelVisible = true;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void changeRebuyPanel()
        {
            try
            {
                isEnterMoneyPanelVisible = true;
                enterMoneyTextbox.updateMessageExtern(table.GameSetting.MinBalance.ToString());
            }
            catch (Exception ex)
            {
                
            }
        }

        private void hideBettingButtons()
        {
            try
            {
                callButton.IsVisible = false;
                callButton.IsEnabled = false;
                raiseButton.IsVisible = false;
                raiseButton.IsEnabled = false;
                foldButton.IsVisible = false;
                foldButton.IsEnabled = false;
                raiseAmountTextbox.IsVisible = false;
                raiseAmountTextbox.IsEnabled = false;
            }
            catch (Exception e)
            {
                
            }
        }

        private void activateBettingButtons()
        {
            try
            {
                callButton.IsVisible = true;
                callButton.IsEnabled = true;
                raiseButton.IsVisible = true;
                raiseButton.IsEnabled = true;
                foldButton.IsVisible = true;
                foldButton.IsEnabled = true;
                raiseAmountTextbox.IsVisible = true;
                raiseAmountTextbox.IsEnabled = true;
            }
            catch (Exception e)
            {
                
            }
        }

        private void CheckOrCall()
        {
            try
            {
                if (myPlayer != null)
                {
                    if (myPlayer.CurrentRoundBet < currentBettingRound.BiggestMoneyInPot)
                    {
                        callButton.Content = "Call";
                        callButton.Name = "1";
                    }
                    else
                    {
                        callButton.Content = "Check";
                        callButton.Name = "0";
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public void Draw(GameTime i_gametime)
        {
            try
            {
                painter.Draw(storage.Furnitures[6], new Rectangle(new Point(0, 0), new Point(1280, 620)), Color.White);
                foreach (DrawingButton button in sitButtons)
                {
                    button.Draw(i_gametime, painter);
                }

                if (!pokerTableChat.IsChatVisible && pokerTableChat.newMessagesAvialble)
                    pokerTableChat.ChatButton.Draw(i_gametime, painter, Color.Red);
                else
                    pokerTableChat.ChatButton.Draw(i_gametime, painter);
                pokerTableChat.Draw(i_gametime);


                volumeOnOffButton.Draw(i_gametime, painter);

                PaintPlayersAndSits();
                DrawCards();

                drawTotalBets(i_gametime);

                drawHandsRating(i_gametime);

                drawStatsPanel(i_gametime);

                _guiSystem.Draw(i_gametime);
                drawEnterMoneyPanel(i_gametime);
                drawFindChestPanel(i_gametime);
            }
            catch (Exception e)
            {
                
                throw e;
            }

        }

        private void drawFindChestPanel(GameTime i_gametime)
        {
            if(isFindChestPanelVisible)
            {
                painter.Draw(storage.GreenUI[5], findChestRectangle, Color.White);
                painter.DrawString(storage.Fonts[0], 
                    @"You're out of GOLD!
Go find a chest 
inside the casino room!"
, new Vector2(findChestRectangle.X + 120, findChestRectangle.Y + 15), Color.Black);
                findChestConfirm.Draw(i_gametime, painter);
            }
        }

        private void drawTotalBets(GameTime i_gametime)
        {
            try
            {
                if (round != null)
                {
                    round.CalculateTotalBets();
                    if (currentRoundPart == lastRoundPart && currentRoundPart == RoundPart.Result && !roundEnd)
                    {
                        for (int i = 0; i < round.WinnersIndex.Count; i++)
                        {
                            painter.Draw(storage.PokerChips[4], new Rectangle((int)chipMovingLocation[9].X, (int)chipMovingLocation[9].Y, 25 * (int)width, 25 * (int)height), Color.White);
                            painter.DrawString(storage.Fonts[0], round.TotalBets.ToString(), new Vector2(chipMovingLocation[9].X + 30 * width, chipMovingLocation[9].Y - 10 * height), Color.White);

                        }
                    }
                    else if (!roundEnd)
                    {
                        painter.Draw(storage.PokerChips[4], new Rectangle((int)(585 * width), (int)(190 * height), 25 * (int)width, 25 * (int)height), Color.White);
                        painter.DrawString(storage.Fonts[0], round.TotalBets.ToString(), new Vector2(615 * (int)width, 180 * (int)height), Color.White);
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void drawStatsPanel(GameTime i_gameTime)
        {
            try
            {
                if (isStatsPanelVisible)
                {
                    PokerPlayer currentStatPlayer = table.PlayersInTable[currentStatsPlayer];
                    if (currentStatPlayer.Stat.Card1 == null)
                    {
                        painter.Draw(storage.GreyUI[5], new Rectangle(450, 100, 400, 480), Color.White);
                        painter.DrawString(storage.Fonts[0], "Player didn't win yet.", new Vector2(480, 390), Color.Black);
                        painter.Draw(storage.Cards[52], new Vector2(480, 430), Color.White);
                        painter.Draw(storage.Cards[52], new Vector2(480 + storage.Cards[0].Width, 430), Color.White);
                        painter.Draw(storage.Cards[52], new Vector2(480 + storage.Cards[0].Width * 2, 430), Color.White);
                        painter.Draw(storage.Cards[52], new Vector2(480 + storage.Cards[0].Width * 3, 430), Color.White);
                        painter.Draw(storage.Cards[52], new Vector2(480 + storage.Cards[0].Width * 4, 430), Color.White);
                    }
                    else
                    {
                        painter.Draw(storage.GreyUI[5], new Rectangle(450, 100, 400, 400), Color.White);
                        painter.Draw(storage.Cards[(int)currentStatPlayer.Stat.Card1], new Vector2(480, 390), Color.White);
                        painter.Draw(storage.Cards[(int)currentStatPlayer.Stat.Card2], new Vector2(480 + storage.Cards[0].Width, 390), Color.White);
                        painter.Draw(storage.Cards[(int)currentStatPlayer.Stat.Card3], new Vector2(480 + storage.Cards[0].Width * 2, 390), Color.White);
                        painter.Draw(storage.Cards[(int)currentStatPlayer.Stat.Card4], new Vector2(480 + storage.Cards[0].Width * 3, 390), Color.White);
                        painter.Draw(storage.Cards[(int)currentStatPlayer.Stat.Card5], new Vector2(480 + storage.Cards[0].Width * 4, 390), Color.White);
                    }

                    closeStatsPanelButton.Draw(i_gameTime, painter);
                    painter.DrawString(storage.Fonts[0], currentStatPlayer.Name + " Stats:", new Vector2(550, 110), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Money: " + currentStatPlayer.Stat.Money.ToString(), new Vector2(480, 150), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Hands Play: " + currentStatPlayer.Stat.NumberOfHandsPlay.ToString(), new Vector2(480, 190), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Hands Won: " + currentStatPlayer.Stat.NumberOfHandsWon.ToString(), new Vector2(480, 230), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Victory Percentage: " + (int)(currentStatPlayer.Stat.VictoryPercentage * 100) + "%", new Vector2(480, 270), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Biggest Pot: " + currentStatPlayer.Stat.BiggestPot.ToString(), new Vector2(480, 310), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Best Hand Played: ", new Vector2(480, 350), Color.Black);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void drawHandsRating(GameTime i_gameTime)
        {
            try
            {
                if (isHandsRatingVisible)
                {
                    painter.Draw(storage.HandsRating, new Rectangle(135, 65, 1000, 490), Color.White);
                    closeHandsRatingButton.Draw(i_gameTime, painter);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void drawEnterMoneyPanel(GameTime i_gameTime)
        {
            try
            {
                if (isEnterMoneyPanelVisible && myPlayer != null)
                {
                    painter.Draw(storage.GreenUI[5], enterMoneyRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0], "Buy Into Game", new Vector2(enterMoneyRectangle.X + 100, enterMoneyRectangle.Y + 20), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Min: 200", new Vector2(enterMoneyRectangle.X + 30, enterMoneyRectangle.Y + 60), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Max: 2K", new Vector2(enterMoneyRectangle.X + 250, enterMoneyRectangle.Y + 60), Color.Black);
                    enterMoneyRaiseDown.Draw(i_gameTime, painter);
                    enterMoneyTextbox.Draw(painter);
                    enterMoneyRaiseUp.Draw(i_gameTime, painter);
                    painter.DrawString(storage.Fonts[0], "Your Account Balance:", new Vector2(enterMoneyRectangle.X + 40, enterMoneyRectangle.Y + 200), Color.Black);
                    painter.DrawString(storage.Fonts[0], myPlayer.Stat.Money.ToString(), new Vector2(enterMoneyRectangle.X + 140, enterMoneyRectangle.Y + 240), Color.Black);
                    enterMoneyConfirm.Draw(i_gameTime, painter);
                    enterMoneyExit.Draw(i_gameTime, painter);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void DrawCards()
        {
            try
            {
                if (table != null)
                {
                    if (round != null)
                    {
                        darwDealerButton();
                        if (!isEnterMoneyPanelVisible)
                        {
                            drawSharedCards(currentRoundPart);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void drawSharedCards(RoundPart roundPart)
        {
            try
            {
                Card card1 = round.SharedCards[0];
                Card card2 = round.SharedCards[1];
                Card card3 = round.SharedCards[2];
                Card card4 = round.SharedCards[3];
                Card card5 = round.SharedCards[4];

                switch (roundPart)
                {
                    case RoundPart.PreFlop:
                        {
                            numberOfSharedCardDrawed = 0;
                            if (numberOfCardServed <= cardDrawingLocations.Count)
                            {
                                if (numberOfSharedCardDrawed == 0)
                                {
                                    MediaPlayer.Play(storage.PokerCardSound);
                                }
                                for (int j = 0; j < numberOfCardServed; j++)
                                {
                                    int i = cardDrawingLocations[j] % 10;
                                    bool secoundCard = cardDrawingLocations[j] >= 10;
                                    if (table.Players[i].Email.Equals(userEmail) || table.CurrentRound.Part == RoundPart.Result)
                                    {
                                        painter.Draw(storage.Cards[convertCardsToint(table.CurrentRound.UsersCards[i][0])], new Rectangle((int)playersLocations[i].X, (int)playersLocations[i].Y, 64 * (int)width, 89 * (int)height), Color.White);
                                        if (secoundCard)
                                        {
                                            painter.Draw(storage.Cards[convertCardsToint(table.CurrentRound.UsersCards[i][1])], new Rectangle((int)playersLocations[i].X + (int)(storage.Cards[1].Width / 2), (int)playersLocations[i].Y, 64 * (int)width, 89 * (int)height), Color.White);
                                        }
                                    }
                                    else
                                    {
                                        painter.Draw(storage.Cards[52], new Vector2(playersLocations[i].X, playersLocations[i].Y), Color.White);
                                        if (secoundCard)
                                        {
                                            painter.Draw(storage.Cards[52], new Vector2(playersLocations[i].X + (storage.Cards[1].Width / 2), playersLocations[i].Y), Color.White);
                                        }
                                    }
                                }
                                if (DateTime.Now.Subtract(timeBetweenCards).TotalMilliseconds >= 200)
                                {
                                    MediaPlayer.Play(storage.PokerCardSound);
                                    timeBetweenCards = DateTime.Now;
                                    numberOfCardServed++;
                                }
                            }
                            else
                            {
                                drawPlayerCard();
                            }
                            break;
                        }
                    case RoundPart.Flop:
                        {
                            drawPlayerCard();
                            if (numberOfSharedCardDrawed == 0)
                            {
                                painter.Draw(storage.Cards[convertCardsToint(card1)], new Vector2(width * 480, height * 250), Color.White);
                                MediaPlayer.Play(storage.PokerCardSound);
                                numberOfSharedCardDrawed++;
                                timeBetweenCards = DateTime.Now;
                            }
                            else if (numberOfSharedCardDrawed == 1)
                            {
                                painter.Draw(storage.Cards[convertCardsToint(card1)], new Vector2(width * 480, height * 250), Color.White);
                                if (DateTime.Now.Subtract(timeBetweenCards).TotalMilliseconds >= 200)
                                {
                                    painter.Draw(storage.Cards[convertCardsToint(card2)], new Vector2(480 * width + (storage.Cards[1].Width + 5), height * 250), Color.White);
                                    MediaPlayer.Play(storage.PokerCardSound);
                                    numberOfSharedCardDrawed++;
                                    timeBetweenCards = DateTime.Now;
                                }

                            }
                            else if (numberOfSharedCardDrawed == 2)
                            {
                                painter.Draw(storage.Cards[convertCardsToint(card1)], new Vector2(width * 480, height * 250), Color.White);
                                painter.Draw(storage.Cards[convertCardsToint(card2)], new Vector2(480 * width + (storage.Cards[1].Width + 5), height * 250), Color.White);
                                if (DateTime.Now.Subtract(timeBetweenCards).TotalMilliseconds >= 200)
                                {
                                    painter.Draw(storage.Cards[convertCardsToint(card3)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 2, height * 250), Color.White);
                                    MediaPlayer.Play(storage.PokerCardSound);
                                    numberOfSharedCardDrawed++;
                                    timeBetweenCards = DateTime.Now;
                                }
                            }
                            else
                            {
                                painter.Draw(storage.Cards[convertCardsToint(card1)], new Vector2(width * 480, height * 250), Color.White);
                                painter.Draw(storage.Cards[convertCardsToint(card2)], new Vector2(480 * width + (storage.Cards[1].Width + 5), height * 250), Color.White);
                                painter.Draw(storage.Cards[convertCardsToint(card3)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 2, height * 250), Color.White);
                            }
                            break;
                        }
                    case RoundPart.Turn:
                        {
                            drawPlayerCard();
                            painter.Draw(storage.Cards[convertCardsToint(card1)], new Vector2(width * 480, height * 250), Color.White);
                            painter.Draw(storage.Cards[convertCardsToint(card2)], new Vector2(480 * width + (storage.Cards[1].Width + 5), height * 250), Color.White);
                            painter.Draw(storage.Cards[convertCardsToint(card3)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 2, height * 250), Color.White);

                            if (numberOfSharedCardDrawed == 3)
                            {
                                if (DateTime.Now.Subtract(timeBetweenCards).TotalMilliseconds >= 200)
                                {
                                    painter.Draw(storage.Cards[convertCardsToint(card4)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 3, height * 250), Color.White);
                                    numberOfSharedCardDrawed++;
                                    timeBetweenCards = DateTime.Now;
                                    MediaPlayer.Play(storage.PokerCardSound);
                                }
                            }
                            else
                            {
                                painter.Draw(storage.Cards[convertCardsToint(card4)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 3, height * 250), Color.White);
                            }


                            break;
                        }
                    case RoundPart.River:
                        {
                            drawPlayerCard();
                            painter.Draw(storage.Cards[convertCardsToint(card1)], new Vector2(width * 480, height * 250), Color.White);
                            painter.Draw(storage.Cards[convertCardsToint(card2)], new Vector2(480 * width + (storage.Cards[1].Width + 5), height * 250), Color.White);
                            painter.Draw(storage.Cards[convertCardsToint(card3)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 2, height * 250), Color.White);
                            painter.Draw(storage.Cards[convertCardsToint(card4)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 3, height * 250), Color.White);

                            if (numberOfSharedCardDrawed == 4)
                            {
                                if (DateTime.Now.Subtract(timeBetweenCards).TotalMilliseconds >= 200)
                                {
                                    timeBetweenCards = DateTime.Now;
                                    MediaPlayer.Play(storage.PokerCardSound);
                                    numberOfSharedCardDrawed++;
                                    painter.Draw(storage.Cards[convertCardsToint(card5)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 4, height * 250), Color.White);
                                }
                            }
                            else
                            {
                                painter.Draw(storage.Cards[convertCardsToint(card5)], new Vector2(480 * width + (storage.Cards[1].Width + 5) * 4, height * 250), Color.White);
                            }
                            break;
                        }
                    default:
                        {
                            if (lastRoundPart != RoundPart.PreFlop)
                            {
                                switch (numberOfSharedCardDrawed)
                                {
                                    case 0:
                                        {
                                            drawSharedCards(RoundPart.Flop);
                                            break;
                                        }
                                    case 1:
                                        {
                                            drawSharedCards(RoundPart.Flop);
                                            break;
                                        }
                                    case 2:
                                        {
                                            drawSharedCards(RoundPart.Flop);
                                            break;
                                        }
                                    case 3:
                                        {
                                            drawSharedCards(RoundPart.Turn);
                                            break;
                                        }
                                    default:
                                        {
                                            drawSharedCards(RoundPart.River);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void drawPlayerCard()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    if (table.Players[i] != null && table.Players[i].InHand)
                    {

                        if (table.Players[i].Email.Equals(userEmail) || table.CurrentRound.Part == RoundPart.Result)
                        {
                            painter.Draw(storage.Cards[convertCardsToint(table.CurrentRound.UsersCards[i][0])], new Rectangle((int)playersLocations[i].X, (int)playersLocations[i].Y, 64 * (int)width, 89 * (int)height), Color.White);
                            painter.Draw(storage.Cards[convertCardsToint(table.CurrentRound.UsersCards[i][1])], new Rectangle((int)playersLocations[i].X + (int)(storage.Cards[1].Width / 2), (int)playersLocations[i].Y, 64 * (int)width, 89 * (int)height), Color.White);
                        }
                        else
                        {
                            painter.Draw(storage.Cards[52], new Vector2(playersLocations[i].X, playersLocations[i].Y), Color.White);
                            painter.Draw(storage.Cards[52], new Vector2(playersLocations[i].X + (storage.Cards[1].Width / 2), playersLocations[i].Y), Color.White);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }
        private void darwDealerButton()
        {
            try
            {
                int dealerButton = table.CurrentRound.Dealer;
                switch (dealerButton)
                {
                    case 0:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[0].X - width * 100), (int)playersLocations[0].Y + 60, 25 * (int)width, 25 * (int)height), Color.White);
                            break;
                        }
                    case 1:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[1].X - width * 100), (int)playersLocations[1].Y + 60, 25 * (int)width, 25 * (int)height), Color.White);
                            break;
                        }
                    case 2:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[2].X - width * 100), (int)playersLocations[2].Y + 60, 25 * (int)width, 25 * (int)height), Color.White);
                            break;
                        }
                    case 3:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[3].X + 70), (int)(playersLocations[3].Y - 110 * height), 20 * (int)width, 20 * (int)height), Color.White);
                            break;
                        }
                    case 4:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[4].X + 70), (int)(playersLocations[4].Y - 110 * height), 20 * (int)width, 20 * (int)height), Color.White);
                            break;
                        }
                    case 5:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[5].X + 70), (int)(playersLocations[5].Y - 110 * height), 20 * (int)width, 20 * (int)height), Color.White);
                            break;
                        }
                    case 6:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[6].X + width * 100), (int)playersLocations[6].Y + 60, 25 * (int)width, 25 * (int)height), Color.White);
                            break;
                        }
                    case 7:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[7].X + width * 100), (int)playersLocations[7].Y + 60, 25 * (int)width, 25 * (int)height), Color.White);
                            break;
                        }
                    case 8:
                        {
                            painter.Draw(storage.PokerChips[0], new Rectangle((int)(playersLocations[8].X + width * 100), (int)playersLocations[8].Y + 60, 25 * (int)width, 25 * (int)height), Color.White);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void drawActionBar(int i_PlayerIndex, eAction eAction)
        {
            try
            {
                bool isChipsMoving = currentRoundPart != lastRoundPart;
                if (eAction == eAction.None && !isChipsMoving || roundEnd)
                {
                    return;
                }

                switch (i_PlayerIndex)
                {
                    case 0:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[0].X), (int)chipMovingLocation[0].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[0].X - width * 100), (int)playersLocations[0].Y, 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0],
                                        table.CurrentRound.ActivePlayersIndex[0].CurrentRoundBet.ToString() + "$",
                                        new Vector2((int)(playersLocations[0].X - width * 75),
                                        (int)playersLocations[0].Y - 10 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ?  Color.Gold : Color.White);
                                }
                            }


                            break;
                        }
                    case 1:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[1].X), (int)chipMovingLocation[1].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[1].X - width * 100), (int)playersLocations[1].Y, 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[1].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[1].X - width * 75), 
                                        (int)playersLocations[1].Y - 10 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[2].X), (int)chipMovingLocation[2].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[2].X - width * 100), (int)playersLocations[2].Y, 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[2].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[2].X - width * 75), 
                                        (int)playersLocations[2].Y - 10 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[3].X), (int)chipMovingLocation[3].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[3].X), (int)(playersLocations[3].Y - 90 * height), 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[3].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[3].X + width * 25), 
                                        (int)playersLocations[3].Y - 100 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 4:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[4].X), (int)chipMovingLocation[4].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[4].X), (int)(playersLocations[4].Y - 90 * height), 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[4].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[4].X + width * 25), 
                                        (int)playersLocations[4].Y - 100 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 5:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[5].X), (int)chipMovingLocation[5].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[5].X), (int)(playersLocations[5].Y - 90 * height), 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[5].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[5].X + width * 25), 
                                        (int)playersLocations[5].Y - 100 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 6:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[6].X), (int)chipMovingLocation[6].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[6].X + width * 100), (int)playersLocations[6].Y, 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[6].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[6].X + width * 125), 
                                        (int)playersLocations[6].Y - 10 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 7:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[7].X), (int)chipMovingLocation[7].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[7].X + width * 100), (int)playersLocations[7].Y, 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[7].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[7].X + width * 125), 
                                        (int)playersLocations[7].Y - 10 * height),
                                       i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                    case 8:
                        {
                            if (isChipsMoving)
                            {
                                painter.Draw(storage.PokerChips[4], new Rectangle((int)(chipMovingLocation[8].X), (int)chipMovingLocation[8].Y, 20 * (int)width, 20 * (int)height), Color.White);

                            }
                            else
                            {
                                painter.Draw(storage.PokerSigns[(int)eAction], new Rectangle((int)(playersLocations[8].X + width * 100), (int)playersLocations[8].Y, 20 * (int)width, 20 * (int)height), Color.White);
                                if (eAction != eAction.Fold && eAction != eAction.Check)
                                {
                                    painter.DrawString(storage.Fonts[0], 
                                        table.CurrentRound.ActivePlayersIndex[8].CurrentRoundBet.ToString() + "$", 
                                        new Vector2((int)(playersLocations[8].X + width * 125), 
                                        (int)playersLocations[8].Y - 10 * height),
                                        i_PlayerIndex == table.CurrentRound.currentBettingRound.CurrentPlayerIndex ? Color.Gold : Color.White);
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private int convertCardsToint(Card i_Card)
        {
            try
            {
                int count = i_Card.Value - 2;
                string suit = i_Card.Suit;
                switch (suit)
                {
                    case "Diamonds":
                        count += 13;
                        break;
                    case "Clubs":
                        count += 26;
                        break;
                    case "Spades":
                        count += 39;
                        break;
                }
                return count;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }
        private void PaintPlayersAndSits()
        {
            try
            {
                if (table != null)
                {
                    if (table.Players != null)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (table.PlayersInTable[i] != null)
                            {
                                sitButtons[i].IsVisible = false;
                                sitButtons[i].IsEnabled = false;
                                painter.Draw(storage.PlayerFaces[table.PlayersInTable[i].Figure][0], new Rectangle((int)(playersLocations[i].X * width), (int)(playersLocations[i].Y - 60 * height), (int)(75 * width), (int)(70 * height)), Color.White);
                                if (table.CurrentRound != null && i == table.CurrentRound.currentBettingRound.CurrentPlayerIndex)
                                {
                                    painter.DrawString(storage.Fonts[0], table.PlayersInTable[i].Name, new Vector2(playersLocations[i].X + (int)(75 * width), playersLocations[i].Y - 50 * height), Color.Gold);
                                    painter.DrawString(storage.Fonts[0], table.PlayersInTable[i].Money.ToString() + "$", new Vector2(playersLocations[i].X, playersLocations[i].Y + storage.Cards[0].Height), Color.Gold);
                                }
                                else
                                {
                                    painter.DrawString(storage.Fonts[0], table.PlayersInTable[i].Name, new Vector2(playersLocations[i].X + (int)(75 * width), playersLocations[i].Y - 50 * height), Color.White);
                                    painter.DrawString(storage.Fonts[0], table.PlayersInTable[i].Money.ToString() + "$", new Vector2(playersLocations[i].X, playersLocations[i].Y + storage.Cards[0].Height), Color.White);
                                }
                                if (table.CurrentRound != null && table.CurrentRound.UsersCards[i] != null && table.CurrentRound.ActivePlayersIndex[i].InHand)
                                {
                                    if (table.Players[i] != null)
                                    {
                                        drawActionBar(i, table.PlayersInTable[i].LastAction);
                                        if (table.CurrentRound.Part != RoundPart.Result)
                                        {
                                            painter.Draw(storage.CurrentPlayerMark, new Rectangle((int)playersLocations[table.CurrentRound.currentBettingRound.CurrentPlayerIndex].X - (int)width * 2, (int)playersLocations[table.CurrentRound.currentBettingRound.CurrentPlayerIndex].Y - 2 * (int)height, 100 * (int)width, 93 * (int)height), Color.White);
                                            painter.Draw(storage.CurrentPlayerMark, new Rectangle((int)playersLocations[table.CurrentRound.currentBettingRound.CurrentPlayerIndex].X - (int)width * 4, (int)playersLocations[table.CurrentRound.currentBettingRound.CurrentPlayerIndex].Y - (int)height * 4, 104 * (int)width, 97 * (int)height), Color.White);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (index == -1)
                                {
                                    sitButtons[i].IsVisible = true;
                                    sitButtons[i].IsEnabled = true;
                                }
                                else
                                {
                                    sitButtons[i].IsVisible = false;
                                    sitButtons[i].IsEnabled = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }
    }
}