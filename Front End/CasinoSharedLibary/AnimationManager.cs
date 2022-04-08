using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CasinoSharedLibary
{
    class AnimationManager
    {
        private ContentManager contentManager;
        private SpriteBatch painter;

        private SpritesStorage storage;

        private int currentIndex = 0;

        private PlayerSkin playerSkin;

        public Texture2D currentPicture;

        #region Moving objects
        private bool isMoving;
        private Texture2D playerInRestRight;
        private Texture2D playerInRestLeft;
        private int numberOfWalkingPlayerPictures;
        private Texture2D[] WalkingPlayerRight;
        private Texture2D[] WalkingPlayerLeft;
        #endregion

        #region Not Moving Objects
        private Texture2D[] movingObject;
        private int numberOfObjectPictures;
        private int animationSpeed = 0;
        #endregion

        private readonly int bubbleWidth = 200;
        private readonly int bubbleHeight = 200;

        public AnimationManager(SpriteBatch i_Painter, SpritesStorage i_storage)
        {
            painter = i_Painter;
            storage = i_storage;
            movingObject = storage.Coins.ToArray();
            numberOfObjectPictures = movingObject.Length;
        }

        public AnimationManager(ContentManager i_Content, SpriteBatch i_Painter,
            PlayerSkin i_PlayerSkin, SpritesStorage i_storage)
        {
            contentManager = i_Content;
            painter = i_Painter;
            playerSkin = i_PlayerSkin;
            storage = i_storage;

            playerInRestRight = i_storage.playersSkinInRest[(int)playerSkin][0];
            playerInRestLeft = i_storage.playersSkinInRest[(int)playerSkin][1];
            numberOfWalkingPlayerPictures = i_storage.playerSkinInMovement[(int)playerSkin][0].Count;
            WalkingPlayerRight = new Texture2D[numberOfWalkingPlayerPictures];
            WalkingPlayerLeft = new Texture2D[numberOfWalkingPlayerPictures];
            WalkingPlayerRight = i_storage.playerSkinInMovement[(int)playerSkin][0].ToArray();
            WalkingPlayerLeft = i_storage.playerSkinInMovement[(int)playerSkin][1].ToArray();
        }

        public void UpdateAnimation()
        {
            animationSpeed++;
            if(animationSpeed == 3)
            {
                currentIndex++;
                currentIndex %= numberOfObjectPictures;
                currentPicture = movingObject[currentIndex];
            }
            animationSpeed %= 3;
        }

        public void UpdateAnimation(Direction i_FaceDirection, bool i_isMoving)
        {
            isMoving = i_isMoving;
            if (i_isMoving)
            {
                if (i_FaceDirection == Direction.Right)
                {
                    currentIndex++;
                    currentIndex %= numberOfWalkingPlayerPictures;
                    currentPicture = WalkingPlayerRight[currentIndex];
                }
                else
                {
                    currentIndex++;
                    currentIndex %= numberOfWalkingPlayerPictures;
                    currentPicture = WalkingPlayerLeft[currentIndex];
                }
            }
            else
            {
                if (i_FaceDirection == Direction.Right)
                {
                    currentPicture = playerInRestRight;
                }
                else
                {
                    currentPicture = playerInRestLeft;
                }
            }
        }

        public void DrawAnimation(Vector2 i_drawingPosition, int i_animationWidth, int i_animationHeight)
        {
            if (currentPicture != null)
            {
                painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * i_animationWidth), (int)(storage.heigth * i_animationHeight)), Color.White);
            }
        }

        public void DrawAnimation(Vector2 i_drawingPosition, Direction i_playerDirection, int i_animationWidth, int i_animationHeight)
        {
            if (currentPicture != null)
            {
                if(playerSkin == PlayerSkin.Ninja)
                {
                    if (isMoving)
                    {
                        painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                    }
                    else
                    {
                        painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth * 0.66), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                    }
                }
                else if(playerSkin == PlayerSkin.Jack)
                {
                    painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                }
                else if (playerSkin == PlayerSkin.Knight)
                {
                    painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                }
                else if (playerSkin == PlayerSkin.Zombie)
                {
                    painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                }
            }
        }

        //The comments in this method are for a trial version of cutting the message.
        public void DrawAnimation(Vector2 i_drawingPosition, string i_lastMessage, Direction i_onlinePlayerDirection/*, int i_animationWidth, int i_animationHeight*/)
        {
            if (currentPicture != null)
            {
                if (playerSkin == PlayerSkin.Ninja)
                {
                    if (isMoving)
                    {
                        painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                    }
                    else
                    {
                        painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth * 0.66), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                    }
                }
                else if (playerSkin == PlayerSkin.Jack)
                {
                    painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                }
                else if (playerSkin == PlayerSkin.Knight)
                {
                    painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                }
                else if (playerSkin == PlayerSkin.Zombie)
                {
                    painter.Draw(currentPicture, new Rectangle((int)i_drawingPosition.X, (int)i_drawingPosition.Y, (int)(storage.width * Game1.listOfSprites[(int)playerSkin].playerWidth), (int)(storage.heigth * Game1.listOfSprites[(int)playerSkin].playerHeight)), Color.White);
                }

                if (i_lastMessage != null)
                {
                    Vector2 bubblePosition = new Vector2(-210, -240) + i_drawingPosition; // acording to defualt bubble size(300,300)
                    //List<string> messageSentences;
                    //int font, numberOfSentence = 0;
                    //if(i_lastMessage.Length < 20)
                    //{
                    //    font = 2;
                    //    messageSentences = createBubbleString(i_lastMessage, font);
                    //}
                    //else if(i_lastMessage.Length < 50)
                    //{
                    //    font = 3;
                    //    messageSentences = createBubbleString(i_lastMessage, font);
                    //}
                    //else
                    //{
                    //    font = 4;
                    //    messageSentences = createBubbleString(i_lastMessage, font);
                    //}

                    //painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, bubbleWidth, bubbleHeight), Color.White);
                    //foreach (string sentence in messageSentences)
                    //{
                    //    painter.DrawString(storage.Fonts[font], sentence, new Vector2(120, 115 + numberOfSentence * storage.Fonts[font].MeasureString(sentence).Y) + bubblePosition, Color.Black);
                    //    numberOfSentence++;
                    //}

                    if (i_lastMessage.Length < 17)
                    {
                        painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, bubbleWidth, bubbleHeight), Color.White);
                        painter.DrawString(storage.Fonts[2], createBubbleString(i_lastMessage, 1), new Vector2(120, 115) + bubblePosition, Color.Black);
                    }
                    else if (i_lastMessage.Length < 40)
                    {
                        painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, bubbleWidth, bubbleHeight), Color.White);
                        painter.DrawString(storage.Fonts[3], createBubbleString(i_lastMessage, 2), new Vector2(120, 115) + bubblePosition, Color.Black);
                    }
                    else
                    {
                        painter.Draw(storage.SpeachBubble, new Rectangle((int)bubblePosition.X + 80, (int)bubblePosition.Y + 80, bubbleWidth, bubbleHeight), Color.White);
                        painter.DrawString(storage.Fonts[4], createBubbleString(i_lastMessage, 3), new Vector2(110, 105) + bubblePosition, Color.Black);
                    }
                }
            }
        }

        //The comments in this method are for a trial version of cutting the message.
        private string createBubbleString(string i_message, int i_fontSize)
        {
            //List<string> sentences = new List<string>();
            StringBuilder bubbleString = new StringBuilder();

            string[] words = i_message.Split(' ');

            //Vector2 lineCounter = new Vector2();
            //foreach (string word in words)
            //{
            //    //if ((storage.Fonts[1 + i_fontSize].MeasureString(word) + lineCounter).X < bubbleWidth)
            //    //{
            //        foreach (char letter in word)
            //        {
            //            bubbleString.Append(letter);
            //            lineCounter += storage.Fonts[i_fontSize].MeasureString(letter.ToString());
            //            if(lineCounter.X > bubbleWidth - 100)
            //            {
            //                sentences.Add(bubbleString.ToString());
            //                bubbleString.Clear();
            //            lineCounter = new Vector2();
            //            }
            //        }
            //    bubbleString.Append(' ');
            //    lineCounter += storage.Fonts[i_fontSize].MeasureString(' '.ToString());
            //    //}
            //}

            //if(!string.IsNullOrWhiteSpace(bubbleString.ToString()))
            //{
            //    sentences.Add(bubbleString.ToString());
            //}
            int lineCounter = 0;
            foreach (string word in words)
            {
                //bubbleString.Append(word);
                //lineCounter += word.Length;

                if (lineCounter > 7 && i_fontSize == 1)
                {
                    bubbleString.Append("\n");
                    lineCounter = 0;
                    bubbleString.Append(word);
                    lineCounter += word.Length;
                    lineCounter++;
                    bubbleString.Append(' ');
                    if (bubbleString.Length > 15)
                    {
                        bubbleString.Append("...");
                        break;
                    }
                }
                else if (lineCounter > 10 && i_fontSize == 2)
                {
                    bubbleString.Append("\n");
                    lineCounter = 0;
                    bubbleString.Append(word);
                    lineCounter += word.Length;
                    lineCounter++;
                    bubbleString.Append(' ');
                    if (bubbleString.Length > 22)
                    {
                        bubbleString.Append("...");
                        break;
                    }
                }
                else if (lineCounter > 25 && i_fontSize == 3)
                {
                    bubbleString.Append("\n");
                    lineCounter = 0;
                    bubbleString.Append(word);
                    lineCounter += word.Length;
                    lineCounter++;
                    bubbleString.Append(' ');
                    if (bubbleString.Length > 50)
                    {
                        bubbleString.Append("...");
                        break;
                    }
                }
                else
                {
                    bubbleString.Append(word);
                    lineCounter += word.Length;
                    lineCounter++;
                    bubbleString.Append(' ');
                }
            }

            return bubbleString.ToString();
            //return sentences;
        }

        public void UpdateSkinType(PlayerSkin i_PlayerSkin)
        {
            playerSkin = i_PlayerSkin;
            playerInRestRight = storage.playersSkinInRest[(int)i_PlayerSkin][0];
            playerInRestLeft = storage.playersSkinInRest[(int)i_PlayerSkin][1];
            numberOfWalkingPlayerPictures = storage.playerSkinInMovement[(int)i_PlayerSkin][0].Count;
            WalkingPlayerRight = new Texture2D[numberOfWalkingPlayerPictures];
            WalkingPlayerLeft = new Texture2D[numberOfWalkingPlayerPictures];
            WalkingPlayerRight = storage.playerSkinInMovement[(int)i_PlayerSkin][0].ToArray();
            WalkingPlayerLeft = storage.playerSkinInMovement[(int)i_PlayerSkin][1].ToArray();
        }
    }
}