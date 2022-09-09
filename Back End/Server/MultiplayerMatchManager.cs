using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classes;

namespace Server
{
    public static class MultiplayerMatchManager
    {
        /*public static void StartNewRound(Room i_Room, Card i_Card, Category i_Category)
        {
            i_Room.CardsHistory.Add(i_Card);


            MultiplayerMatch currentMatch = i_Room.CurrentMatch;
            User presenter = currentMatch.PlayersList[currentMatch.PresenterTurns[currentMatch.CurrentRound]];
            currentMatch.CurrentRound++;


            Round newRound = new Round(currentMatch.CurrentRound,presenter,i_Room.Players.Count);
            currentMatch.RoundsList.Add(newRound);
        }

        public static void InsertUserGuess(Room i_Room, Guess i_Guess)
        {
            int userIndex = GetUserIndexInRoom(i_Room, i_Guess);
            MultiplayerMatch currentMatch = i_Room.CurrentMatch;
            Round round  = currentMatch.RoundsList[currentMatch.CurrentRound];
            round.GuessList[userIndex] = i_Guess;
            CalculateGuessScore(currentMatch, round, userIndex, i_Guess);
        }

        private static void CalculateGuessScore(MultiplayerMatch i_Match, Round i_Round,int i_UserIndex, Guess i_Guess)
        {
            int score = (int)(100 * i_Guess.TimeInSeconds / 60 + Math.Pow(3, i_Guess.Card.DificultLevel));
            i_Round.ScoreList[i_UserIndex] = score;
            i_Match.ScoreList[i_UserIndex] += score;
        }

        public static int GetUserIndexInRoom(Room i_Room, Guess i_Guess)
        {
            List<User> users = i_Room.Players;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Equals(i_Guess.UserWhoGuessed))
                {
                    return i;
                }
            }

            return -1;
        }
    */
    }
}
