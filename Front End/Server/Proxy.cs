﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Classes;
using Newtonsoft.Json.Linq;
using System.IO;



namespace Server
{
    public class Proxy
    {
        public List<KeyValuePair<string,string>> m_Parameters = new List<KeyValuePair<string, string>>();
        public string BaseURL = "http://localhost:61968/";
        //public string BaseURL;

        public HttpClient client;
        public Timer aTimer;

        //public Proxy(string i_BaseUrl = "https://pokerarenaapi.azurewebsites.net/")
        public Proxy(string i_BaseUrl = "https://webapicontrollers20220101232715.azurewebsites.net/")
        {
            BaseURL = i_BaseUrl;
            connerctToServer();
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 10000;
            aTimer.Elapsed += interval;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void interval(Object source, System.Timers.ElapsedEventArgs e)
        {
            connerctToServer();
        }

        private void connerctToServer()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.ConnectionClose = false;
            var sp = ServicePointManager.FindServicePoint(new Uri(BaseURL));
            sp.SetTcpKeepAlive(true, 90000, 90000);
            Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
        }

        public async Task<string> postReq(string json,string url,string method)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = method;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                string result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = await streamReader.ReadToEndAsync();
                }

                return result;
            }
            catch (WebException e)
            {

                using (WebResponse response = e.Response)
                {
                    var httpResponse = (HttpWebResponse)response;
                    string result;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = await streamReader.ReadToEndAsync();
                        return result;
                    }
                }
            }
        }

        public async Task<string> getReq(string uri, string method = "GET")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        private async Task<string> GetRequestAsync(string i_Url)
        {
            string result = String.Empty;
            //if(client.DefaultRequestHeaders.ConnectionClose == true)
            //{
            //    connerctToServer();
            //}

            Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
            try
            {
                HttpResponseMessage response = await client.GetAsync(i_Url);

                Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
                using (HttpContent content = response.Content)
                {
                    Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
                    result = await content.ReadAsStringAsync();
                }


                Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        private async Task<string> PostRequestAsync(string i_Url, HttpContent i_Content)
        {
            string result = String.Empty;
            try
            {


                
                if (client.DefaultRequestHeaders.ConnectionClose == true)
                {
                    connerctToServer();
                }
                HttpResponseMessage response = await client.PostAsync(i_Url, i_Content);

                using (HttpContent content = response.Content)
                {
                    result = await content.ReadAsStringAsync();
                }



                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }


        private async Task<string> PutRequestAsync(string i_Url, HttpContent i_Content)
        {
            string result = String.Empty;
            if (client.DefaultRequestHeaders.ConnectionClose == true)
            {
                connerctToServer();
            }
            using (HttpResponseMessage response = await client.PutAsync(i_Url, i_Content))
                {
                    using (HttpContent content = response.Content)
                    {
                        result = await content.ReadAsStringAsync();
                    }
                }

            return result;
        }

        private async Task<string> DeleteRequestAsync(string i_Url)
        {
            string result = String.Empty;
            //if(client.DefaultRequestHeaders.ConnectionClose == true)
            //{
            //    connerctToServer();
            //}

            Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(i_Url);

                Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
                using (HttpContent content = response.Content)
                {
                    Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
                    result = await content.ReadAsStringAsync();
                }


                Console.WriteLine(client.DefaultRequestHeaders.ConnectionClose);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public string Login(string i_Email, string i_Password)
        {
            try
            {
                var values = new JObject();

                values.Add("Name", null);
                values.Add("Email", i_Email);
                values.Add("Password", i_Password);

                // HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
                //string result = PostRequestAsync(BaseURL + "api/LogIn", content).Result;
                string result = postReq(values.ToString(), BaseURL + "api/LogIn","POST").Result;
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public List<FurnitureInstance> GetCasinoFurnitureInstances(string i_CasinoId)
        {
            try
            {
                string result = getReq(BaseURL + "api/Casino?i_CasinoId=" + i_CasinoId,"GET").Result;
                var val = JArray.Parse(result);

                List<FurnitureInstance> list = val.ToObject<List<FurnitureInstance>>();
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string UpdatePosition(string i_CainoId, string i_Email, string name, int LastXPos, int LastYPos, int CurrentXPos, int CurrentYPos, int direction, int skin)
        {
            try
            {
                var values = new JObject();
                values.Add("Name", name);
                values.Add("CasinoId", i_CainoId);
                values.Add("Email", i_Email);
                values.Add("LastXPos", LastXPos);
                values.Add("LastYPos", LastYPos);
                values.Add("CurrentXPos", CurrentXPos);
                values.Add("CurrentYPos", CurrentYPos);
                values.Add("Direction", direction);
                values.Add("Skin", skin);


                string result = postReq(values.ToString(), BaseURL + "api/UserLocation", "POST").Result;

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        
        public List<CharacterInstance> GetPosition(string i_CasinoId, string i_Email)
        {
            try
            {
                string result = getReq(BaseURL + "api/UserLocation?i_CasinoId=" + i_CasinoId + "&&i_Email=" + i_Email,"GET").Result;
                var val = JArray.Parse(result);

                List<CharacterInstance> players = val.ToObject<List<CharacterInstance>>();
                return players;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string MakeAnAction(string i_TableId,string i_CasinoId, string i_Email ,string i_Signature, int i_Action, int i_Amount)
        {
            try
            {
                var values = new JObject();
                values.Add("TableId", i_TableId);
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);
                values.Add("PlayerSignature", i_Signature);
                values.Add("Action", i_Action);
                values.Add("RaiseAmount", i_Amount);

       

                string result = postReq(values.ToString(), BaseURL + "api/PokerAction", "POST").Result;

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void ConfirmEndRound(string i_TableId,string i_CasinoId, string i_Email)
        {
            try
            {
                var values = new JObject();
                values.Add("TableId", i_TableId);
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);

                string result = postReq(values.ToString(), BaseURL + "api/PokerRound", "POST").Result;

            }
            catch (Exception e)
            {

            }
        }

        public Round GetRoundByTableId(string i_TableId, string i_CasinoId)
        {
            try
            {
                string result = getReq(BaseURL + "api/PokerRound?CasinoId=" + i_CasinoId + "&&TableId=" + i_TableId).Result;
                var val = JObject.Parse(result);

                Round round = val.ToObject<Round>();
                return round;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Table GetTableById(string i_TableId, string i_CasinoId, string i_Email)
        {
            try
            {
                string result = getReq(BaseURL + "api/PokerTable?CasinoId=" + i_CasinoId + "&&TableId=" + i_TableId+"&&Email=" + i_Email).Result;
                Console.WriteLine(result);
                var val = JObject.Parse(result);
                Console.WriteLine(val);

                Table table = val.ToObject<Table>();
                return table;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<PokerPlayer> GetPlayersByTableId(string i_TableId, string i_CasinoId)
        {
            try
            {
                string result = getReq("api/PokerTablePlayer?CasinoId=" + i_CasinoId + "&&TableId=" + i_TableId).Result;
                var val = JArray.Parse(result);

                List<PokerPlayer> players = val.ToObject<List<PokerPlayer>>();
                return players;
            }
            catch (Exception e)
            {
                return null;
            }
        }                                                                           

        public PokerPlayer GetPlayerByPlayerEmailAndTableId(string i_Email, string i_TableId,string i_CasinoId)
        {
            try
            {
                string result = getReq(BaseURL + "api/PokerTablePlayer?CasinoId=" + i_CasinoId + "&&TableId=" + i_TableId + "&&email=" + i_Email).Result;
                var val = JObject.Parse(result);

                PokerPlayer player = val.ToObject<PokerPlayer>();
                return player;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string AddPlayerToTable(string i_TableId, string i_CasinoId, string i_Email,string i_Name, int i_Money, int i_Index)
        {
            try
            {
                var values = new JObject();
                values.Add("TableId", i_TableId);
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);
                values.Add("Name", i_Name);
                values.Add("Money", i_Money);
                values.Add("Index", i_Index);

                string result = postReq(values.ToString(), BaseURL + "api/PokerTablePlayer", "POST").Result;

                return result.Substring(1, 15);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SitOut(string i_CasinoId,string i_TableId,string i_Email, string i_Signature, bool i_Now)
        {
            try
            {
                string result = getReq(BaseURL + "api/PokerTablePlayer?CasinoId=" + i_CasinoId +"&&TableId="+ i_TableId+  "&&Email="+i_Email+"&&Signature=" + i_Signature,"DELETE").Result;
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<Message> GetTableChatMessages(string i_TableId, string i_CasinoId)
        {
            try
            {
                string result = getReq(BaseURL + "api/TablePokerChat?CasinoId=" + i_CasinoId + "&&TableId=" + i_TableId).Result;
                var val = JArray.Parse(result);

                List<Message> messages = val.ToObject<List<Message>>();
                return messages;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SendMessageToTableChat(string i_TableId, string i_CasinoId, string i_Email, string i_Signature, string i_UserName, string i_Body)
        {
            try
            {
                var values = new JObject();
                values.Add("TableId", i_TableId);
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);
                values.Add("Signature", i_Signature);
                values.Add("UserName", i_UserName);
                values.Add("Body", i_Body);

                string result = postReq(values.ToString(), BaseURL + "api/TablePokerChat", "POST").Result;

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public Stats GetStats(string i_Email)
        {
            try
            {
                string result = getReq(BaseURL + "api/Stats?" + "i_Email=" + i_Email).Result;
                var val = JObject.Parse(result);

                Stats stats = val.ToObject<Stats>();
                return stats;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public User GetUserDetails(string i_Email)
        {
            try
            {
                string result = getReq(BaseURL + "api/User?" + "i_Email=" + i_Email).Result;
                var val = JObject.Parse(result);

                User user = val.ToObject<User>();
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SignUp(string i_UserName, string i_Email, string i_Password)
        {
            try
            {
                var values = new JObject();

                values.Add("Name", i_UserName);
                values.Add("Email", i_Email);
                values.Add("Password", i_Password);

                string result = postReq(values.ToString(), BaseURL + "api/User", "POST").Result;

                if (result.Equals("\"Successed\""))
                {
                    createStatsForPlayer(i_Email);
                    return result;
                }

                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ChangeUserDetails(string i_LastEmail, User i_User)
        {
            try
            {
                var values = JObject.FromObject(i_User);

                string result = postReq(values.ToString(), BaseURL + BaseURL + "api/User?i_Email=" + i_LastEmail, "PUT").Result;
            }
            catch (Exception e)
            {
               
            }
        }

        private void createStatsForPlayer(string i_Email)
        {
            try
            {
                var values = new JObject();

                HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
                string result = postReq(values.ToString(), BaseURL + "api/Stats?i_Email=" + i_Email, "POST").Result;

            }
            catch (Exception e)
            {

            }
        }

        public void AddOnReBuy(string i_TableId, string i_CasinoId, string i_Email, string i_Signature, int i_Money)
        {
            try
            {
                var values = new JObject();
                values.Add("TableId", i_TableId);
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);
                values.Add("Signature", i_Signature);
                values.Add("Amount", i_Money);

               
                string result = postReq(values.ToString(), BaseURL + "api/PokerTablePlayer", "PUT").Result;

            }
            catch (Exception e)
            {
               
            }
        }

        private void deleteSignatures(List<PokerPlayer> i_Players)
        {
            try
            {
                foreach (var player in i_Players)
                {
                    if (player != null)
                    {
                        player.Signature = "";
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public List<Message> getCasinoMessages(string i_CasinoId)
        {
            try
            {
                string result = getReq(BaseURL + "api/CasinoChat?i_CasinoId=" + i_CasinoId).Result;
                var val = JArray.Parse(result);

                List<Message> messages = val.ToObject<List<Message>>();
                return messages;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SendMessageToCasinoChat(string i_CasinoId, string i_Email, string i_Signature, string i_UserName, string i_Body)
        {
            try
            {
                var values = new JObject();
                values.Add("TableId", null);
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);
                values.Add("Signature", i_Signature);
                values.Add("UserName", i_UserName);
                values.Add("Body", i_Body);

                
                string result = postReq(values.ToString(), BaseURL + "api/CasinoChat", "POST").Result;
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Chest getChest(string i_CasinoId)
        {
            try
            {
                string result = getReq(BaseURL + "api/Chest?" + "CasinoId=" + i_CasinoId).Result;
                var val = JObject.Parse(result);

                Chest chest = val.ToObject<Chest>();
                return chest;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string CollectChest(string i_CasinoId, string i_Email)
        {
            try
            {
                var values = new JObject();
                values.Add("CasinoId", i_CasinoId);
                values.Add("Email", i_Email);

         
                string result = postReq(values.ToString(), BaseURL + "api/Chest", "POST").Result;
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }


    }
}