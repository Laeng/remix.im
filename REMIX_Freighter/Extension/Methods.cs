using log4net;
using Newtonsoft.Json.Linq;
using MiNET;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using fNbt;
using System.Reflection;

namespace REMIX_Freighter.Extension
{
    public static class Methods
    {
        private static Dictionary<String, JObject> FastLoad = new Dictionary<String, JObject>();
        private static readonly ILog Log = LogManager.GetLogger(typeof(Freighter));

        public static JObject GetPlayerDatas(this Player player, String name = null)
        {
            String laeng = ReturnUsername(player, name);
            String paths = PathPlayerDatas(laeng);
            JObject res = JObject.Parse("{ }");

            if (!File.Exists(paths)) SetPlayerDatas(player, PlayerDatasForm(player), true);
            if (!FastLoad.ContainsKey(laeng))
            {
                res = JObject.Parse(File.ReadAllText(paths));
                FastLoad.Add(laeng, res);
            }
            else res = FastLoad[laeng];

            Log.Error(res.ToString());
            return res;
        }

        public static void SetPlayerDatas(this Player player, JObject datas, Boolean save = false, String name = null)
        {
            String laeng = ReturnUsername(player, name);
            FastLoad[laeng] = datas;

            if (save)
            {
                File.WriteAllText(PathPlayerDatas(laeng), FastLoad[laeng].ToString());
            }

        }

        public static void RemovePlayerDatas(this Player player, String name = null)
        {
            String laeng = ReturnUsername(player, name);
            String path = PathPlayerDatas(laeng);

            if (File.Exists(path)) File.Delete(path);
        }

        public static JObject PlayerDatasForm(this Player player, Boolean local = false)
        {
            String laeng = player.Username.ToLower();
            JObject datas = new JObject();

            if (File.Exists(PathPlayerDatas(laeng)))
            {
                datas = (local) ? GetPlayerDatas(player) : FastLoad[laeng];  
            }

            datas["Username"] = player.Username;
            datas["GameMode"] = JToken.FromObject(player.GameMode);
            datas["LastSeen"] = player.Level.LevelName;
            datas["Location"] = JToken.FromObject(player.KnownPosition);
            datas["Inventory"] = JToken.FromObject(player.Inventory.GetSlots());
            datas["Clothes"] = JToken.FromObject(player.Inventory.GetArmor());
            datas["Effect"] = JToken.FromObject(player.Effects);
            datas["Health"] = player.HealthManager.Health;
            datas["Hunger"] = player.HungerManager.Hunger;
            datas["LV"] = JToken.FromObject(player.ExperienceLevel);
            datas["XP"] = JToken.FromObject(player.Experience);

            return datas;
        }

        internal static MiNetServer GetServer(this Player player)
        {
            PropertyInfo info = player.GetType().GetProperty("Server", BindingFlags.Instance | BindingFlags.NonPublic);
            return info.GetValue(player) as MiNetServer;
        }

        #region Private Area | Devloped by Laeng

        private static String PathPlayerDatas(String name = null)
        {
            String re = Path.Combine(Directory.GetCurrentDirectory(), "players");
            
            if(!String.IsNullOrEmpty(name))
            {
                String laeng = name.ToLower(), di;
                re = Path.Combine(re, laeng[0].ToString(), laeng + ".json");
                di = Path.GetDirectoryName(re);

                if (!Directory.Exists(re)) Directory.CreateDirectory(di);
            }
            return re;
        }

        private static String ReturnUsername(Player player, String name)
        {
            return ((!String.IsNullOrEmpty(name)) ? name : player.Username).ToLower();
        }

        #endregion
    }
}


