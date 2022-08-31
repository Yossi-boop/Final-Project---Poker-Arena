using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Threading.Tasks;

using Classes;

using Comora;
using System.Timers;

namespace CasinoSharedLibary
{

    public class CasinoRoom
    {
        private Game1 gameManager;

        private ContentManager contentManager;
        private SpriteBatch painter;

        private SpritesStorage storage;

        private Timer casinoTimer;
        private Timer pokerTableTimer;

        private DateTime cameraCounter;
        private bool cameraMoved = true;

        private Player mainPlayer;
        public Camera camera;

        public static readonly object updateobjlock = new object();
        public static readonly object drawobjlock = new object();
        public static readonly object instanceobjlock = new object();

        private List<CharacterInstance> playersInTheCasinoInformation;
        private List<PlayerDrawingInformation> playersInTheCasino = new List<PlayerDrawingInformation>();

        public List<FurnitureInstance> furnituresList { get; set; }
        public List<Instance> instancesList { get; set; } = new List<Instance>();
        public Chest winningChest;

        public bool isReEnterToCasino = false;
        public bool IsUpdated { get; set; } = false;

        #region UpperBar
        private Vector2 coinPosition;
        private AnimationManager coinAnimationManager;
        private NewChat casinoRoomNewChat;
        private DrawingButton exitButton;

        //private DrawingButton knightSkin;
        #region Settings Panel
        private DrawingButton settingsButton;
        private Rectangle settingPanelRectangle;
        private DrawingButton volumeOnOffButton;
        private DrawingButton ninjaSkin;
        private DrawingButton jackSkin;
        private DrawingButton zombieSkin;
        private bool isSettingPanelVisible = false;
        #endregion

        #endregion

        #region Enter Table Panel
        private Rectangle enterTablePanelRectangle;
        private DrawingButton confirmEnterTable;
        private DrawingButton exitEnterTable;
        private bool isEnterTablePanelVisible = false;
        private string givenTableId = null;
        #endregion

        #region Explain Enter Table Panel
        private Rectangle explainToPlayerHowToEnterPokerTablePanelRectangle;
        private bool isExplainToPlayerHowToEnterPokerTablePanelVisibe = false;
        private int speedOfChangedSpaceBar = 0;
        private bool isSpaceBarClicked = false;
        #endregion

        #region Winning Amount Of Chest Panel
        private Rectangle winningAmountOfChestPanelRectangle;
        private bool isWinningAmountOfChestPanelVisible = false;
        private DrawingButton confirmWinButton;
        #endregion

        private KeyboardInput keyboard;
        private JoysStick joystick;
        private Keys currentInput;

        private string lastMessage;

        private bool mainPlayerDraw = false;

        private bool isUpdateStatsAfterPokerTable = true;

        private Table currentTable;

        private TaskScheduler scheduler;

        public CasinoRoom(Game1 i_gameManager, ContentManager i_Content, SpritesStorage i_Storage)
        {
            try
            {
                gameManager = i_gameManager;
                contentManager = i_Content;
                storage = i_Storage;

                camera = new Camera(gameManager.GraphicsDevice);

                winningChest = gameManager.server.getChest("1234");

                furnituresList = gameManager.server.GetCasinoFurnitureInstances("1234");

                instancesList.Add(winningChest);
                instancesList.AddRange(furnituresList);

                User user = gameManager.server.GetUserDetails(gameManager.mainPlayerEmail);

                mainPlayer = new Player(contentManager, gameManager.UserScreenWidth, gameManager.UserScreenHeight,
                  (PlayerSkin)user.Figure, storage, user.Name, gameManager.mainPlayerEmail, gameManager.server.GetStats(gameManager.mainPlayerEmail), furnituresList);
                mainPlayer.updateStuckChest = winningChest;
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.Constructor " + e.Message);
                }
                throw e;
            }
            
        }

        public void Load(SpriteBatch i_Painter)
        {
            try
            {
                painter = i_Painter;

                mainPlayer.Load(painter);
                camera.Position = mainPlayer.position;

                casinoRoomNewChat = new NewChat(storage, 250, 250);
                casinoRoomNewChat.UpdateMessageList(gameManager.server.getCasinoMessages("1234"));
                casinoRoomNewChat.Load(painter);

                #region Input From User
                keyboard = new KeyboardInput();
                joystick = new JoysStick(storage, painter);
                joystick.Load();
                #endregion

                #region Enter Table Buttons
                confirmEnterTable = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                confirmEnterTable.Text = "Confirm";
                confirmEnterTable.Click += ConfirmEnterTable_Click;

                exitEnterTable = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                exitEnterTable.Text = "Exit";
                exitEnterTable.Click += ExitEnterTable_Click;
                #endregion

                //knightSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                //knightSkin.Text = "KNIGHT";
                //knightSkin.Click += KnightSkin_Click;
                #region UpperBar Buttons And Animations
                #region Settings Buttons
                settingsButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                settingsButton.Text = "Settings";
                settingsButton.Click += SettingsButton_Click;

                volumeOnOffButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                volumeOnOffButton.Text = "Sound On";
                volumeOnOffButton.Click += VolumeOnOffButton_Click;

                ninjaSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                ninjaSkin.Text = "NINJA";
                ninjaSkin.Click += NinjaSkin_Click;

                jackSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                jackSkin.Text = "JACK";
                jackSkin.Click += JackSkin_Click;

                zombieSkin = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                zombieSkin.Text = "ZOMBIE";
                zombieSkin.Click += ZombieSkin_Click;
                #endregion

                exitButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                exitButton.Text = "Exit";
                exitButton.Click += ExitButton_Click;

                coinAnimationManager = new AnimationManager(painter, storage);
                #endregion

                #region Winning Chest Button
                confirmWinButton = new DrawingButton(storage.GreenUI[0], storage.Fonts[0]);
                confirmWinButton.Text = "Confirm";
                confirmWinButton.Click += ConfirmWinButton_Click;
                #endregion

                gameManager.server.UpdatePosition("1234", mainPlayer.playerEmail, mainPlayer.PlayerName, (int)mainPlayer.LastPosition.X,
                (int)mainPlayer.LastPosition.Y, (int)mainPlayer.drawingPosition.X,
                (int)mainPlayer.drawingPosition.Y, (int)mainPlayer.direction, (int)mainPlayer.playerSkin);
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.Load " + e.Message);
                }
                //throw e;
            }
            
        }

        public void UpdateMainPlayer(string i_mainPlayerEmail)
        {
            try
            {
                User user = gameManager.server.GetUserDetails(gameManager.mainPlayerEmail);

                mainPlayer = new Player(contentManager, gameManager.UserScreenWidth, gameManager.UserScreenHeight,
                  (PlayerSkin)user.Figure, storage, user.Name, gameManager.mainPlayerEmail, gameManager.server.GetStats(gameManager.mainPlayerEmail), furnituresList);
                mainPlayer.updateStuckChest = winningChest;
                mainPlayer.Load(painter);
                camera.Position = mainPlayer.position;
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.UpdateMainPlayer " + e.Message);
                }
                //throw e;
            }
            
        }

        #region Winning Chest Money Methods
        private void ConfirmWinButton_Click(object sender, EventArgs e)
        {
            turnConfirmWinChestPanelOff();
        }

        private void turnConfirmWinChestPanelOff()
        {
            isWinningAmountOfChestPanelVisible = false;
        }
        #endregion

        #region Enter Table Methods
        private void ExitEnterTable_Click(object sender, EventArgs e)
        {
            isEnterTablePanelVisible = false;
        }

        private void ConfirmEnterTable_Click(object sender, EventArgs e)
        {
            isEnterTablePanelVisible = false;
            if (givenTableId != null)
            {
                OpenPokerTable(givenTableId, mainPlayer.PlayerName, mainPlayer.playerEmail);
            }
            isUpdateStatsAfterPokerTable = false;
        }
        #endregion
        #region UpperBar Methods
        private void ExitButton_Click(object sender, EventArgs e)
        {
            isReEnterToCasino = true;
            MediaPlayer.Stop();
            gameManager.ScreenType = eScreenType.LoginPage;
            casinoTimer.Enabled = false;
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            switchSettingPanelVisible();
        }

        private void switchSettingPanelVisible()
        {
            isSettingPanelVisible = !isSettingPanelVisible;
        }

        #region Setting Methods
        private void VolumeOnOffButton_Click(object sender, EventArgs e)
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

        private void ZombieSkin_Click(object sender, EventArgs e)
        {
            changeSkinAndCloseSettingPanel((sender as DrawingButton).Text);
        }

        private void JackSkin_Click(object sender, EventArgs e)
        {
            changeSkinAndCloseSettingPanel((sender as DrawingButton).Text);
        }

        private void NinjaSkin_Click(object sender, EventArgs e)
        {
            changeSkinAndCloseSettingPanel((sender as DrawingButton).Text);
        }

        private void changeSkinAndCloseSettingPanel(string i_buttonText)
        {
            switch (i_buttonText)
            {
                case "NINJA":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Ninja);
                    break;
                case "JACK":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Jack);
                    break;
                case "KNIGHT":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Knight);
                    break;
                case "ZOMBIE":
                    mainPlayer.UpdatePlayerSkin(PlayerSkin.Zombie);
                    break;
                default:
                    break;
            }
            gameManager.server.ChangeUserDetails(mainPlayer.playerEmail, new User(mainPlayer.PlayerName, mainPlayer.playerEmail, (int)mainPlayer.playerSkin));
            switchSettingPanelVisible();
        }
        #endregion
        #endregion

        private void initializeIntervalsForPokerTimer()
        {
            try
            {
                if (pokerTableTimer == null)
                {
                    pokerTableTimer = new Timer();
                    pokerTableTimer.Interval = 1000;
                    pokerTableTimer.Elapsed += pokerTableTimer_Elapsed;
                    pokerTableTimer.AutoReset = true;
                    pokerTableTimer.Enabled = true;
                }
                else if (pokerTableTimer.Enabled == false)
                {
                    pokerTableTimer.Enabled = true;
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.initializeIntervalsForPokerTimer " + e.Message);
                }
                throw e;
            }
        }

        private void pokerTableTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                gameManager.server.UpdatePosition("1234", mainPlayer.playerEmail, mainPlayer.PlayerName,
                (int)mainPlayer.LastPosition.X, (int)mainPlayer.LastPosition.Y,
                (int)mainPlayer.drawingPosition.X, (int)mainPlayer.drawingPosition.Y,
                (int)mainPlayer.direction, (int)mainPlayer.playerSkin);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.pokerTableTimer_Elapsed " + ex.Message);
                }
                throw ex;
            }
        }

        private void initializeIntervals()
        {
            try
            {
                if (casinoTimer == null)
                {
                    casinoTimer = new Timer();
                    casinoTimer.Interval = 100;
                    casinoTimer.Elapsed += CasinoTimer_Elapsed;
                    casinoTimer.AutoReset = true;
                    casinoTimer.Enabled = true;
                    if(pokerTableTimer != null)
                        pokerTableTimer.Enabled = false;
                }
                else if (casinoTimer.Enabled == false)
                {
                    if(pokerTableTimer != null)
                        pokerTableTimer.Enabled = false;
                    casinoTimer.Enabled = true;
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.initializeIntervals " + e.Message);
                }
                throw e;
            }   
        }

        private void CasinoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                gameManager.server.UpdatePosition("1234", mainPlayer.playerEmail, mainPlayer.PlayerName,
                (int)mainPlayer.LastPosition.X, (int)mainPlayer.LastPosition.Y, 
                (int)mainPlayer.drawingPosition.X, (int)mainPlayer.drawingPosition.Y, 
                (int)mainPlayer.direction, (int)mainPlayer.playerSkin);
                mainPlayer.LastPosition.X = mainPlayer.position.X;
                mainPlayer.LastPosition.Y = mainPlayer.position.Y;
                List<CharacterInstance> testPlayersInTheCasinoInformation;
                testPlayersInTheCasinoInformation = 
                    gameManager.server.GetPosition("1234", mainPlayer.playerEmail);
                if (testPlayersInTheCasinoInformation != null)
                {
                    playersInTheCasinoInformation = testPlayersInTheCasinoInformation;
                }

                List<Message> testChatData = gameManager.server.getCasinoMessages("1234");
                if (testChatData != null)
                {
                    casinoRoomNewChat.UpdateMessageList(testChatData);
                }

                winningChest = gameManager.server.getChest("1234");
                mainPlayer.updateStuckChest = winningChest;

                lock (updateobjlock)
                {
                    if (playersInTheCasino != null && playersInTheCasino.Count > 0)
                    {
                        playersInTheCasino.Clear();
                    }


                    foreach (CharacterInstance player in playersInTheCasinoInformation)
                    {
                        playersInTheCasino.Add(new PlayerDrawingInformation
                            (player, contentManager, painter, storage));
                    }
                }

                lock (instanceobjlock)
                {
                    instancesList.Clear();
                    instancesList.Add(winningChest);
                    instancesList.AddRange(furnituresList);
                    instancesList.AddRange(playersInTheCasino);
                    instancesList.Sort();
                }
                
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.CasinoTimer_Elapsed " + ex.Message);
                }
                throw ex;
            }
            
        }

        public void casinoPeopleUpdate(GameTime i_GameTime)
        {
            try
            {
                if (playersInTheCasino != null) //The list exist
                {
                    lock (updateobjlock)
                    {
                        foreach (PlayerDrawingInformation player in playersInTheCasino)
                        {
                            player.updateOnlinePlayer(i_GameTime, player.direction);
                        }
                    }   
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.casinoPeopleUpdate " + e.Message);
                }
                throw e;
            }
            
        }

        public void Update(GameTime i_gameTime)
        {
            try
            {
                IsUpdated = true;
                updateStatsAfterPokerTable();
                initializeIntervals();
                updateEnterTablePanel(i_gameTime, camera.Position);
                casinoPeopleUpdate(i_gameTime);
                updateWinningAmountOfChest(i_gameTime, camera.Position);
                updateCurrentInput(i_gameTime, camera.Position);

                if (cameraMoved && (currentInput == Keys.Up || currentInput == Keys.Down || currentInput == Keys.Right || currentInput == Keys.Left))
                {
                    cameraCounter = DateTime.Now;
                    cameraMoved = false;
                }

                FurnitureInstance furniture = nearToPokerTable();
                if (furniture != null)
                {
                    if ((furniture.Type == 0 || furniture.Type == 4 || furniture.Type == 18 ||
                        furniture.Type == 2 || furniture.Type == 17 || furniture.Type == 1) &&
                        !casinoRoomNewChat.IsChatVisible)
                    {
                        isExplainToPlayerHowToEnterPokerTablePanelVisibe = true;
                    }
                    else if (furniture.Type == 8)
                    {
                        if (!winningChest.Collected)
                        {
                            isExplainToPlayerHowToEnterPokerTablePanelVisibe = true;
                        }
                        else
                        {
                            isExplainToPlayerHowToEnterPokerTablePanelVisibe = false;
                        }
                    }
                    else
                    {
                        isExplainToPlayerHowToEnterPokerTablePanelVisibe = false;
                    }
                    if (currentInput == Keys.Space && !casinoRoomNewChat.IsChatVisible)
                    {
                        switch (furniture.Type)
                        {
                            case 0:
                                {
                                    isEnterTablePanelVisible = true;
                                    givenTableId = furniture.Id;
                                    break;
                                }
                            case 1:
                                {
                                    MediaPlayer.Play(storage.TreesSound);
                                    break;
                                }
                            case 2:
                                {
                                    MediaPlayer.Play(storage.StatueSound);
                                    break;
                                }
                            case 4:
                                {
                                    MediaPlayer.Play(storage.RouletteWheelSound);
                                    break;
                                }
                            case 8:
                                {
                                    if (!winningChest.Collected)
                                    {
                                        gameManager.server.CollectChest("1234", mainPlayer.playerEmail);
                                        mainPlayer.stats = gameManager.server.GetStats(mainPlayer.playerEmail);
                                        MediaPlayer.Play(storage.ChestSound);
                                        isWinningAmountOfChestPanelVisible = true;
                                    }
                                    break;
                                }
                            case 17:
                                {
                                    isEnterTablePanelVisible = true;
                                    givenTableId = furniture.Id;
                                    break;
                                }
                            case 18:
                                {
                                    MediaPlayer.Play(storage.RouletteWheelSound);
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    isExplainToPlayerHowToEnterPokerTablePanelVisibe = false;
                }

                mainPlayer.updatePlayer(i_gameTime, currentInput, instanceobjlock);
                lastMessage = casinoRoomNewChat.Update(i_gameTime, mainPlayer.position, currentInput, keyboard.isCapsLockOn, keyboard.isShiftOn);

                if (lastMessage != null)
                {
                    mainPlayer.LastActionTime = DateTime.Now;
                    mainPlayer.DrawBubble(lastMessage);
                    string serverAnswer;
                    do
                    {
                        serverAnswer = gameManager.server.SendMessageToCasinoChat("1234", mainPlayer.playerEmail, null, mainPlayer.PlayerName, lastMessage);
                    }
                    while (serverAnswer == null);
                    lastMessage = null;
                }

                updateSettingPanel(i_gameTime, camera.Position);

                if (!cameraMoved && DateTime.Now.Subtract(cameraCounter).TotalMilliseconds >= 1000)
                {
                    float xDifrance = camera.Position.X - mainPlayer.position.X;
                    float yDifrance = camera.Position.Y - mainPlayer.position.Y;
                    Vector2 temp = camera.Position;
                    if (xDifrance > 0)
                    {
                        if (xDifrance - (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 > 0)
                        {
                            temp.X -= (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.X = mainPlayer.position.X;
                        }
                    }
                    else if (xDifrance < 0)
                    {
                        if (xDifrance + (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 < 0)
                        {
                            temp.X += (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.X = mainPlayer.position.X;
                        }

                    }

                    if (yDifrance > 0)
                    {
                        if (yDifrance - (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 > 0)
                        {
                            temp.Y -= (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.Y = mainPlayer.position.Y;
                        }
                    }
                    else if (yDifrance < 0)
                    {
                        if (yDifrance + (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150 < 0)
                        {
                            temp.Y += (float)i_gameTime.ElapsedGameTime.TotalSeconds * 150;
                        }
                        else
                        {
                            temp.Y = mainPlayer.position.Y;
                        }

                    }

                    camera.Position = temp;

                    if (camera.Position == mainPlayer.position)
                    {
                        cameraMoved = true;
                    }
                }

                camera.Update(i_gameTime);
                updateExplainToPlayerHowToEnterPokerTable(i_gameTime, camera.Position);
                updateUpperBar(i_gameTime);
                updateChat(i_gameTime);
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.Update " + e.Message);
                }
                throw e;
            }
            
        }

        private void updateCurrentInput(GameTime i_gameTime, Vector2 i_cameraPosition)
        {
            try
            {
                currentInput = keyboard.Update();
                if (currentInput == Keys.None)
                {
                    currentInput = joystick.Update(i_gameTime, i_cameraPosition);
                }
                else
                {
                    joystick.Update(i_gameTime, i_cameraPosition);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateCurrentInput " + e.Message);
                }
            }
            
        }

        private void updateWinningAmountOfChest(GameTime i_gameTime, Vector2 i_mainPosition)
        {
            try
            {
                if (isWinningAmountOfChestPanelVisible)
                {
                    Vector2 panelLocation = new Vector2(-100, -100) + i_mainPosition;
                    confirmWinButton.Position = new Vector2(80, 90) + panelLocation;
                    confirmWinButton.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    winningAmountOfChestPanelRectangle = new Rectangle((int)panelLocation.X, (int)panelLocation.Y, 350, 150);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateWinningAmountOfChest " + e.Message);
                }
            }
        }

        private void updateStatsAfterPokerTable()
        {
            try
            {
                if (!isUpdateStatsAfterPokerTable)
                {
                    mainPlayer.stats = gameManager.server.GetStats(mainPlayer.playerEmail);
                    isUpdateStatsAfterPokerTable = true;
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateStatsAfterPokerTable " + e.Message);
                }
            }
            
        }

        private void updateExplainToPlayerHowToEnterPokerTable(GameTime i_gameTime, Vector2 i_mainPosition)
        {
            try
            {
                if (isExplainToPlayerHowToEnterPokerTablePanelVisibe && !casinoRoomNewChat.IsChatVisible)
                {
                    Vector2 panelLocation = new Vector2(390, 154) + i_mainPosition;
                    explainToPlayerHowToEnterPokerTablePanelRectangle = new Rectangle((int)panelLocation.X, (int)panelLocation.Y, 330, 200);
                    speedOfChangedSpaceBar++;
                    speedOfChangedSpaceBar %= 20;
                    if (speedOfChangedSpaceBar % 20 == 0)
                    {
                        isSpaceBarClicked = !isSpaceBarClicked;
                    }
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateExplainToPlayerHowToEnterPokerTable " + e.Message);
                }
            }
            
        }

        private void updateEnterTablePanel(GameTime i_gameTime, Vector2 i_mainPosition)
        {
            try
            {
                if (isEnterTablePanelVisible)
                {
                    Vector2 enterTablePanelLocation = new Vector2(-200, -200) + i_mainPosition;

                    confirmEnterTable.Position = new Vector2(50, 135) + enterTablePanelLocation;
                    confirmEnterTable.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    exitEnterTable.Position = new Vector2((int)confirmEnterTable.Position.X + confirmEnterTable.Rectangle.Width + 30, (int)confirmEnterTable.Position.Y);
                    exitEnterTable.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);

                    enterTablePanelRectangle = new Rectangle((int)enterTablePanelLocation.X - 25, (int)enterTablePanelLocation.Y, 165 + (confirmEnterTable.Rectangle.Width * 2), 200);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateEnterTablePanel " + e.Message);
                }
            }
            
        }

        private FurnitureInstance nearToPokerTable()
        {
            try
            {
                foreach (FurnitureInstance furniture in furnituresList)
                {
                    if (isNearToPlayer(furniture))
                    {
                        return furniture;
                    }

                }
                if (isNearToPlayer(winningChest))
                {
                    return winningChest;
                }
                return null;
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.nearToPokerTable " + e.Message);
                }
                throw e;
            }
        }

        private bool isNearToPlayer(FurnitureInstance i_Furniture)
        {
            try
            {
                int leftSide;
                int upSide;
                int rightSide;
                int downSide;
                mainPlayer.calculateFurnitureSize(i_Furniture, out leftSide, out rightSide, out upSide, out downSide);
                float playerRightSide = mainPlayer.position.X + storage.width * 75 - 1;
                float playerLeftSide = mainPlayer.position.X + 1;
                float playerUpSide = mainPlayer.position.Y + 1;
                float PlayerDownSide = mainPlayer.position.Y + storage.width * 100 - 1;

                if ((inRange(playerRightSide, leftSide) || inRange(playerLeftSide, rightSide)) &&
                    ((playerUpSide >= upSide && playerUpSide <= downSide) || (PlayerDownSide >= upSide && PlayerDownSide <= downSide) ||
                    (playerUpSide <= upSide && PlayerDownSide >= upSide) || (playerUpSide <= downSide && PlayerDownSide >= downSide)))
                {
                    return true;
                }

                if ((inRange(playerUpSide, downSide) || inRange(PlayerDownSide, upSide)) &&
                    ((playerLeftSide >= leftSide && playerLeftSide <= rightSide) || (playerRightSide >= leftSide && playerRightSide <= rightSide) ||
                    (playerLeftSide <= leftSide && playerRightSide >= leftSide) || (playerLeftSide <= rightSide && playerRightSide >= rightSide)))
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.isNearToPlayer " + e.Message);
                }
                throw e;
            }
        }

        private bool inRange(float playerRightSide, int leftSide)
        {
            try
            {
                return playerRightSide + 5 >= leftSide && playerRightSide - 5 <= leftSide;
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.inRange " + e.Message);
                }
                throw e;
            }   
        }

        private void updateSettingPanel(GameTime i_gameTime, Vector2 i_mainPosition)
        {
            try
            {
                if (isSettingPanelVisible)
                {
                    Vector2 settingPanelLocation = new Vector2(-200, -200) + i_mainPosition;

                    ninjaSkin.Position = new Vector2(5, 60) + settingPanelLocation;
                    ninjaSkin.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    jackSkin.Position = new Vector2((int)ninjaSkin.Position.X + ninjaSkin.Rectangle.Width + 30, (int)ninjaSkin.Position.Y);
                    jackSkin.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    //knightSkin.Position = new Vector2((int)ninjaSkin.Position.X, (int)ninjaSkin.Position.Y + ninjaSkin.Rectangle.Height + 30);
                    //knightSkin.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    zombieSkin.Position = new Vector2(ninjaSkin.Position.X + (ninjaSkin.Rectangle.Width / 2) + 30, (int)jackSkin.Position.Y + jackSkin.Rectangle.Height + 30);
                    zombieSkin.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    volumeOnOffButton.Position = new Vector2(ninjaSkin.Position.X + (ninjaSkin.Rectangle.Width / 2) + 30, zombieSkin.Position.Y + ninjaSkin.Rectangle.Height + 30);
                    volumeOnOffButton.Update(i_gameTime, (int)i_mainPosition.X - 640, (int)i_mainPosition.Y - 360);
                    if (MediaPlayer.Volume == 1f)
                    {
                        volumeOnOffButton.Text = "Sound On";
                    }
                    else
                    {
                        volumeOnOffButton.Text = "Sound Off";
                    }

                    settingPanelRectangle = new Rectangle((int)settingPanelLocation.X - 25, (int)settingPanelLocation.Y, 100 + (ninjaSkin.Rectangle.Width * 2), 100 + (ninjaSkin.Rectangle.Height * 2) + ninjaSkin.Rectangle.Height * 2);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateSettingPanel " + e.Message);
                }
            }
        }

        private void updateChat(GameTime i_gameTime)
        {
            try
            {
                casinoRoomNewChat.ChatButton.Position = new Vector2(450, 313) + camera.Position;
                casinoRoomNewChat.ChatButton.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
                casinoRoomNewChat.MoveChatUpButton.Position = new Vector2(-380, 50) + camera.Position;
                casinoRoomNewChat.MoveChatUpButton.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
                casinoRoomNewChat.MoveChatDownButton.Position = new Vector2(-380, 250) + camera.Position;
                casinoRoomNewChat.MoveChatDownButton.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
                casinoRoomNewChat.SendMessageButton.Position = new Vector2(-640, 313) + camera.Position;
                casinoRoomNewChat.SendMessageButton.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
                casinoRoomNewChat.MoveChatToLastMessage.Position = new Vector2(-380, 300) + camera.Position;
                casinoRoomNewChat.MoveChatToLastMessage.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateChat " + e.Message);
                }
            }
        }

        private void updateUpperBar(GameTime i_gameTime)
        {
            try
            {
                coinPosition = new Vector2(-35, -(gameManager.UserScreenHeight / 2 - 5)) + camera.Position;
                exitButton.Position = new Vector2(485, -5) + coinPosition;
                exitButton.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
                settingsButton.Position = new Vector2(-exitButton.Rectangle.Width, 0) + exitButton.Position;
                settingsButton.Update(i_gameTime, (int)camera.Position.X - 640, (int)camera.Position.Y - 360);
                coinAnimationManager.UpdateAnimation();
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.updateUpperBar " + e.Message);
                }
            }
        }

        private void drawCasinoWalls()
        {
            try
            {
                for (int i = 0; i < 22; i++)
                {
                    for (int j = 0; j < 72; j++)
                    {
                        if ((816 + j * storage.Furnitures[5].Width <= 2096 || 816 + j * storage.Furnitures[5].Width >= 4656) ||
                            (744 + i * storage.Furnitures[5].Height <= 1464 || 744 + i * storage.Furnitures[5].Height >= 2904))
                        {
                            painter.Draw(storage.Furnitures[5], new Vector2(831 + j * storage.Furnitures[5].Width, 804 + i * storage.Furnitures[5].Height), Color.White);
                        }
                    }
                }

                for (int i = 0; i < 72; i++)
                {
                    painter.Draw(storage.Furnitures[5], new Rectangle(2096 + i * storage.Furnitures[5].Width, 2904, storage.Furnitures[5].Width, storage.Furnitures[5].Height + 20), Color.White);
                }

                for (int i = 0; i < 10; i++)
                {
                    if (i == 9)
                    {
                        painter.Draw(storage.Furnitures[5], new Rectangle(4655, 1596 + i * storage.Furnitures[5].Height, storage.Furnitures[5].Width + 10, storage.Furnitures[5].Height - 10), Color.White);
                    }
                    else
                    {
                        painter.Draw(storage.Furnitures[5], new Rectangle(4655, 1596 + i * storage.Furnitures[5].Height, storage.Furnitures[5].Width + 15, storage.Furnitures[5].Height), Color.White);
                    }
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawCasinoWalls " + e.Message);
                }
            }
        }

        public void Draw(GameTime i_gameTime)
        {
            try
            {
                casinoRoomDraw();
                //casinoFurnitureDraw();
                mainPlayerDraw = false;
                drawCasinoWalls();
                drawCasinoInstances(i_gameTime);
                drawExplainToPlayerHowToEnterPokerTable(i_gameTime);
                drawEnterTablePanel(i_gameTime);
                drawWinningAmountOfChest(i_gameTime);
                drawUpperBar(i_gameTime);
                joystick.Draw(i_gameTime);
                casinoRoomNewChat.Draw(i_gameTime);
                //casinoPeopleDraw();
                //mainPlayer.drawPlayer();
                drawSettingPanel(i_gameTime);
                IsUpdated = false;
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.Draw " + e.Message);
                }
                throw e;
            }
            
        }

        private void drawWinningAmountOfChest(GameTime i_gameTime)
        {
            try
            {
                if (isWinningAmountOfChestPanelVisible)
                {
                    painter.Draw(storage.GreenUI[5], winningAmountOfChestPanelRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0],
                        string.Format(@"Enjoy Your {0} 
    New Coins!"
    , winningChest.GoldAmount), new Vector2(winningAmountOfChestPanelRectangle.X + 80, winningAmountOfChestPanelRectangle.Y + 8), Color.Black);
                    confirmWinButton.Draw(i_gameTime, painter);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawWinningAmountOfChest " + e.Message);
                }
            }
        }

        private void drawCasinoInstances(GameTime i_gameTime)
        {
            try
            {
                lock (instanceobjlock)
                {
                    foreach (Instance casinoInstance in instancesList)
                    {
                        if (!mainPlayerDraw && mainPlayer.position.Y < casinoInstance.CurrentYPos)
                        {
                            mainPlayer.drawPlayer();
                            mainPlayerDraw = true;
                        }
                        if ((casinoInstance as FurnitureInstance) != null)
                        {
                            drawFurniture(casinoInstance as FurnitureInstance);
                        }
                        else
                        {
                            (casinoInstance as PlayerDrawingInformation).drawOnlinePlayer(painter);
                        }
                    }
                    if (!mainPlayerDraw)
                    {
                        mainPlayer.drawPlayer();
                        mainPlayerDraw = true;
                    }
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawCasinoInstances " + e.Message);
                }
            }   
        }

        private void drawExplainToPlayerHowToEnterPokerTable(GameTime i_gameTime)
        {
            try
            {
                if (isExplainToPlayerHowToEnterPokerTablePanelVisibe)
                {
                    //painter.Draw(storage.GreenUI[5], explainToPlayerHowToEnterPokerTablePanelRectangle, Color.White);
                    //painter.DrawString(storage.Fonts[0], "Click The Space Bar", new Vector2(explainToPlayerHowToEnterPokerTablePanelRectangle.X + 20, explainToPlayerHowToEnterPokerTablePanelRectangle.Y + 20), Color.White);
                    if (isSpaceBarClicked)
                    {
                        painter.Draw(storage.GreenUI[0], new Vector2(explainToPlayerHowToEnterPokerTablePanelRectangle.X + 60, explainToPlayerHowToEnterPokerTablePanelRectangle.Y + 110), Color.White);
                    }
                    else
                    {
                        painter.Draw(storage.GreenUI[0], new Vector2(explainToPlayerHowToEnterPokerTablePanelRectangle.X + 60, explainToPlayerHowToEnterPokerTablePanelRectangle.Y + 110), Color.Gray);
                    }
                    painter.DrawString(storage.Fonts[3], @"Click On Space", new Vector2(explainToPlayerHowToEnterPokerTablePanelRectangle.X + 80, explainToPlayerHowToEnterPokerTablePanelRectangle.Y + 120), Color.White);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawExplainToPlayerHowToEnterPokerTable " + e.Message);
                }
            }
        }

        private void drawEnterTablePanel(GameTime i_gameTime)
        {
            try
            {
                if (isEnterTablePanelVisible)
                {
                    if (currentTable == null || !currentTable.Id.Equals(givenTableId))
                    {
                        currentTable = gameManager.server.GetTableById(givenTableId, "1234", mainPlayer.playerEmail);
                    }
                    painter.Draw(storage.GreenUI[5], enterTablePanelRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0], "Are You Sure You Want To Enter?", new Vector2(enterTablePanelRectangle.X + 25, enterTablePanelRectangle.Y + 40), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Small Blind: " + currentTable.GameSetting.SmallBlind.ToString(), new Vector2(enterTablePanelRectangle.X + 30, enterTablePanelRectangle.Y + 90), Color.Black);
                    painter.DrawString(storage.Fonts[0], "Big Blind: " + currentTable.GameSetting.BigBlind.ToString(), new Vector2(enterTablePanelRectangle.X + 280, enterTablePanelRectangle.Y + 90), Color.Black);
                    confirmEnterTable.Draw(i_gameTime, painter);
                    exitEnterTable.Draw(i_gameTime, painter);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawEnterTablePanel " + e.Message);
                }
            }   
        }

        private void drawSettingPanel(GameTime i_gameTime)
        {
            try
            {
                if (isSettingPanelVisible)
                {
                    painter.Draw(storage.GreenUI[5], settingPanelRectangle, Color.White);
                    painter.DrawString(storage.Fonts[0], "Please Choose Your Skin:", new Vector2(55, 20) + new Vector2((int)settingPanelRectangle.X, (int)settingPanelRectangle.Y), Color.Black);
                    ninjaSkin.Draw(i_gameTime, painter);
                    jackSkin.Draw(i_gameTime, painter);
                    //knightSkin.Draw(i_gameTime, painter);
                    zombieSkin.Draw(i_gameTime, painter);
                    volumeOnOffButton.Draw(i_gameTime, painter);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawSettingPanel " + e.Message);
                }
            }
        }

        private void drawUpperBar(GameTime i_gameTime)
        {
            try
            {
                coinAnimationManager.DrawAnimation(coinPosition, storage.Coins[0].Width, storage.Coins[0].Height);
                painter.DrawString(storage.Fonts[0], mainPlayer.stats.Money.ToString(), 
                    new Vector2(50, 10) + coinPosition, Color.Black);
                if (!casinoRoomNewChat.IsChatVisible && casinoRoomNewChat.newMessagesAvialble)
                {
                    casinoRoomNewChat.ChatButton.Draw(i_gameTime, painter, Color.Red);
                }
                else
                {
                    casinoRoomNewChat.ChatButton.Draw(i_gameTime, painter);
                }
                
                exitButton.Draw(i_gameTime, painter);
                settingsButton.Draw(i_gameTime, painter);
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawUpperBar " + e.Message);
                }
            }
        }

        public void casinoRoomDraw()
        {
            try
            {
                painter.Draw(storage.BlueRoomBackground, new Rectangle(1500, 1000, 4000, 3000), Color.White);

            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.casinoRoomDraw " + e.Message);
                }
            }
        }

        public void casinoFurnitureDraw()
        {
            try
            {
                foreach (FurnitureInstance furniture in furnituresList)
                {
                    drawFurniture(furniture);
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.casinoFurnitureDraw " + e.Message);
                }
            }
        }

        private void drawFurniture(FurnitureInstance i_furniture)
        {
            try
            {
                if (i_furniture.Type >= 0 && i_furniture.Type <= 16)
                {
                    if (i_furniture.Type == 9)
                    {
                        painter.Draw(storage.Furnitures[i_furniture.Type], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(430, 242, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 10)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(578, 285, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 11)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(625, 242, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 12)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(475, 292, 100, 95), Color.White);
                    }
                    else if (i_furniture.Type == 13)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(586, 49, 67, 140), Color.White);
                    }
                    else if (i_furniture.Type == 14)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(682, 49, 67, 140), Color.White);
                    }
                    else if (i_furniture.Type == 15)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(432, 50, 140, 79), Color.White);
                    }
                    else if (i_furniture.Type == 16)
                    {
                        painter.Draw(storage.Furnitures[9], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), new Rectangle(432, 165, 140, 79), Color.White);
                    }
                    else
                    {
                        if (i_furniture.Type != 8 || ((i_furniture as Chest) != null && !(i_furniture as Chest).Collected))
                        {
                            painter.Draw(storage.Furnitures[i_furniture.Type], new Rectangle(i_furniture.CurrentXPos, i_furniture.CurrentYPos, i_furniture.Width, i_furniture.Length), Color.White);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.drawFurniture " + e.Message);
                }
            }
        }

        public void OpenPokerTable(string i_tableID, string i_playerName, string i_playerEmail)
        {
            try
            {
                MediaPlayer.Play(storage.CoinsMusic);
                gameManager.pokerTable = new PokerTable(gameManager, gameManager.GraphicsDevice, painter, storage,
                    contentManager, "1234", i_tableID, i_playerEmail, i_playerName);
                casinoTimer.Enabled = false;
                initializeIntervalsForPokerTimer();
                gameManager.ScreenType = eScreenType.PokerTable;
                gameManager.pokerTable.Load();
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                {
                    file.WriteLine("CasinoRoom.OpenPokerTable " + e.Message);
                }
            }
        }
    }
}