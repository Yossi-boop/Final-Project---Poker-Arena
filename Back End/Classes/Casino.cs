using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;

namespace Classes
{
    public class Casino
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Heigth { get; set; }

        public List<CharacterInstance> Users { get; set; }

        public List<FurnitureInstance> Furnitures { get; set; }

        public Chest TreasureChest { get; set; }

        public Chat CasinoChat { get; set; }

        public List<Table> Tables { get; set; }

        public static readonly object DeleteObjLock = new object();

        public DateTime TimeFromLastChest { get; set; }

        public Casino()
        {
            Tables = new List<Table>();
            Users = new List<CharacterInstance>();
            Furnitures = new List<FurnitureInstance>();
            CasinoChat = new Chat();

        }

        public void CollectMoney(Stats stats)
        {
            if (!TreasureChest.Collected)
            {
                stats.Money += TreasureChest.GoldAmount;
                writeInDataBase(stats);
            }
            TreasureChest.Collected = true;
        }

        private void writeInDataBase(Stats stats)
        {
            var values = new JObject();
            values = JObject.FromObject(stats);
            HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
            //putRequestAsync("https://pokerarenaapi.azurewebsites.net/api/Stats?i_Email=" + stats.UserEmail, content);
            putRequestAsync("http://localhost:61968/api/Stats?i_Email=" + stats.UserEmail, content);

        }

        private async void putRequestAsync(string i_Url, HttpContent i_Content)
        {
            HttpClient client = new HttpClient();
            string result = String.Empty;
            using (HttpResponseMessage response = await client.PutAsync(i_Url, i_Content))
            {
                using (HttpContent content = response.Content)
                {
                    result = await content.ReadAsStringAsync();
                }
            }
        }

        public Casino(string i_Id, int i_X, int i_Y, int i_Width, int i_Heigth): this()
        {
            Id = i_Id;
            X = i_X;
            Y = i_Y;
            Width = i_Width;
            Heigth = i_Heigth;
        }

        public List<FurnitureInstance> GetFurnitureInstances()
        {
            Furnitures.Sort();
            return Furnitures;
        }

        private bool isBetween(int i_X, int i_Y, int i_UserCurrentXPos, int i_UserCurrentYPos, int size)
        {
            return i_X < i_UserCurrentXPos + size || i_X > i_UserCurrentXPos - size || i_Y < i_UserCurrentYPos + size ||
                   i_Y > i_UserCurrentYPos - size;
        }

        public void CheckIfTimeForChest()
        {
            if(DateTime.Now.Subtract(TimeFromLastChest).TotalSeconds >= 30)
            {
                TimeFromLastChest = DateTime.Now;
                createNewChest();
            }
        }

        private void createNewChest()
        {
            int x;
            int y;
            int amount;
            findPlaceForChest(out x,out y);
            amount = SystemTools.GetRandomGoldAmount();
            TreasureChest = new Chest(amount, x, y);
        }

        private void findPlaceForChest(out int x, out int y)
        {
            bool locationOk = false;
            do
            {
                SystemTools.GetRandomLocation(out x, out y, X + 71, Y + 132, X + Width - 83, Y + Heigth - 64);
                locationOk = checkIfChestInRightPlace(x, y);
            }
            while (!locationOk);
        }

        private bool checkIfChestInRightPlace(int x, int y)
        {
            foreach(FurnitureInstance furniture in Furnitures)
            {
                if(checkIfFurnituresBlockLocation(furniture, x, y))
                {
                    return false;
                }
            }

            foreach (CharacterInstance character in Users)
            {
                if (checkIfCharacterBlockLocation(character, x, y))
                {
                    return false;
                }
            }

            return true;
        }

        private bool checkIfCharacterBlockLocation(CharacterInstance character, int x, int y)
        {
            return x >= character.CurrentXPos - 100 && x <= character.CurrentXPos + 150 &&
                y >= character.CurrentYPos - 100 && y <= character.CurrentYPos + 150;
        }

        private bool checkIfFurnituresBlockLocation(FurnitureInstance furniture, int x, int y)
        {
            return x >= furniture.CurrentXPos - 100 && x <= furniture.CurrentXPos + furniture.Width + 20 &&
                y >= furniture.CurrentYPos - 100 && y <= furniture.CurrentYPos + furniture.Length + 20;
        }

        public CharacterInstance UserInCasino(string i_UserEmail)
        {
            foreach (CharacterInstance user in Users)
            {
                if (user != null && user.Email.Equals(i_UserEmail))
                {
                    return user;
                }
            }

            return null;
        }

        public List<Message> GetMessages()
        {
            if (CasinoChat.Archive.Count > 50)
            {
                List<Message> filteredMessages = (List<Message>)CasinoChat.Archive.Skip(Math.Max(0, CasinoChat.Archive.Count() - 50)).Take(50);
                return filteredMessages;
            }

            return CasinoChat.Archive;
        }

        public List<CharacterInstance> GetUsersPositions(string i_Email, int i_radius = 600)
        {
            List<CharacterInstance> relevantUsers = new List<CharacterInstance>();
            CharacterInstance mainCharacter = UserInCasino(i_Email);
            int x = mainCharacter.CurrentXPos;
            int y = mainCharacter.CurrentYPos;

            foreach (CharacterInstance user in Users)
            {
                if (isBetween(x, y, user.CurrentXPos, user.CurrentYPos, i_radius) && !user.Email.Equals(i_Email))
                {
                    relevantUsers.Add(user);
                }
            }

            relevantUsers.Sort();
            
            return relevantUsers;
        }

        public void CheckIfAllPlayerOnline()
        {
            lock (DeleteObjLock)
            {
                List<CharacterInstance> deletionList = new List<CharacterInstance>();
                foreach (CharacterInstance player in Users)
                {
                    if (DateTime.Now.Subtract(player.LastUpdate).TotalSeconds >= 20)
                    {
                        deletionList.Add(player);
                    }
                    else if (DateTime.Now.Subtract(player.LastUpdate).TotalSeconds >= 5)
                    {
                        player.LastXPos = player.CurrentXPos;
                        player.CurrentYPos = player.CurrentYPos;
                    }
                }

                foreach (CharacterInstance player in deletionList)
                {
                    Users.Remove(player);
                }
            }
        }
    }
}
