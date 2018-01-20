using log4net;
using Newtonsoft.Json.Linq;
using MiNET;
using MiNET.Entities;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using MiNET.Worlds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace REMIX_Freighter
{
    [Plugin(PluginName = "Freigter", Description = "", PluginVersion = "1.0", Author = "Laeng")]
    public class Freighter : Plugin
    {
        internal Dictionary<String, JObject> FastLoad = new Dictionary<String, JObject>();
        internal static readonly ILog Log = LogManager.GetLogger("Freighter");
        internal static String Local = Path.Combine(Assembly.GetExecutingAssembly().GetName().CodeBase, "Players");

        public Player[] GetOnlinePlayers()
        {
            List<Level> levels = Context.LevelManager.Levels;
            List<Player> re = new List<Player>();

            foreach (Level level in levels)
            {
                re.AddRange(level.GetAllPlayers());
            }

            return re.ToArray();
        }

        public JObject InitPlayerData(Player player)
        {
            return new JObject(
                    new JProperty("Username", player.Username),
                    new JProperty("GameMode", player.GameMode),
                    new JProperty("LastSeen", player.Level.LevelName),
                    new JProperty("Location", player.KnownPosition.GetDirection()),
                    new JProperty("Clothing", player.Inventory.GetArmor()),
                    new JProperty("Inventory", player.Inventory.GetSlots()),
                    new JProperty("Hunger", player.Experience),
                    new JProperty("Health", player.ExperienceLevel),
                    new JProperty("XP", player.Experience),
                    new JProperty("LV", player.ExperienceLevel)
                );
        }

        public JObject GetPlayerDatas(Player player) => GetPlayerDatas(player.Username);
        public JObject GetPlayerDatas(String player)
        {
            String laeng = player.ToLower();
            String local = Path.Combine(Local, player.ToLower());
            JObject data = new JObject();

            try
            {
                if (!File.Exists(local))
                {
                    Player user = null;
                    foreach (Player loop in GetOnlinePlayers())
                    {
                        if (laeng.Equals(loop.Username.ToLower()))
                        {
                            user = loop;
                            break;
                        }
                    }

                    if (user.Equals(null))
                    {
                        Log.Fatal("CAN NOT FIND USER DATA!!! - GetPlayerData");
                        return data;
                    }

                    JObject init = InitPlayerData(user);

                    SetPlayerDatas(laeng, init, true);
                    FastLoad.Add(laeng, init);
                }

                if (!FastLoad.TryGetValue(laeng, out data))
                {
                    FastLoad.Add(laeng, JObject.Parse(File.ReadAllText(local)));
                    data = FastLoad[laeng];
                }

                return data;
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " - GetPlayerData");
                return new JObject { };
            }
        }

        public Boolean SetPlayerDatas(Player player, JObject data, Boolean forceSave = false) => SetPlayerDatas(player.Username, data, forceSave);
        public Boolean SetPlayerDatas(String player, JObject data, Boolean forceSave = false)
        {
            String laeng = player.ToLower();
            try
            {
                FastLoad[laeng] = data;

                if (forceSave)
                {
                    Directory.CreateDirectory(Local);
                    File.WriteAllText(Path.Combine(Local, player.ToLower()), data.ToString(), Encoding.UTF8);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " - SetPlayerData");
                return false;
            }
        }

        public Object GetPlayerData(Player player, String key) => GetPlayerData(player.Username, key);
        public Object GetPlayerData(String player, String key)
        {
            String laeng = player.ToLower();
            JObject load = GetPlayerDatas(player);

            if (!load.TryGetValue(key, out JToken re))
            {
                Log.Error("The key is entered does not exist - GetPlayerData");
                return null;
            }

            return re.ToObject<Object>();
        }

        public Boolean SetPlayerData(String player, String key, Object value)
        {
            String laeng = player.ToLower();
            JObject load = GetPlayerDatas(player);

            try
            {
                if (!load.TryGetValue(key, out JToken re))
                {
                    load.Add(key, JToken.FromObject(value));
                }

                return true;
            }
            catch(Exception e)
            {
                Log.Error(e + " - SetPlayerData");
                return false;
            }
        }

        public Boolean IsPlayerDatas(Player player) => IsPlayerDatas(player.Username.ToLower());
        public Boolean IsPlayerDatas(String player)
        {
            return (File.Exists(Path.Combine(Local, player.ToLower()))) ? true : false;
        }

        public Boolean SaveDefaultPlayerData(Player player, Boolean forceSave = false)
        {
            try
            {
                JObject load = GetPlayerDatas(player);

                load["Username"] = JToken.FromObject(player.Username);
                load["Gamename"] = JToken.FromObject(player.GameMode);
                load["LastSeen"] = JToken.FromObject(player.Level.LevelName);
                load["Location"] = JToken.FromObject(player.KnownPosition.GetDirection());
                load["Clothing"] = JToken.FromObject(player.Inventory.GetArmor());
                load["Inventory"] = JToken.FromObject(player.Inventory.GetSlots());
                load["Hunger"] = JToken.FromObject(player.HungerManager.Hunger);
                load["Health"] = JToken.FromObject(player.HealthManager.Health);
                load["XP"] = JToken.FromObject(player.Experience);
                load["LV"] = JToken.FromObject(player.ExperienceLevel);

                SetPlayerDatas(player, load, forceSave);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e + " - SaveDefaultPlayerData");
                return false;
            }
        }
    }
}
