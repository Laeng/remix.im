using log4net;
using Newtonsoft.Json.Linq;
using MiNET;
using MiNET.Effects;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Worlds;
using REMIX_Freighter.Extension;
using System;
using System.Collections.Concurrent;
using System.Linq;
using MiNET.Utils;
using MiNET.Net;
using MiNET.Items;

namespace REMIX_Freighter
{
    [Plugin(PluginName = "Freigter", Description = "", PluginVersion = "1.0", Author = "Laeng")]
    public class Freighter : Plugin
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Freighter));

        protected override void OnEnable()
        {
            Context.Server.PlayerFactory.PlayerCreated += (o, e) =>
            {
                Player player = e.Player;
                player.PlayerJoin += OnPlayeJoinEvent;
                player.PlayerLeave += OnPlayeLeaveEvent;
            };
        }

        private void OnPlayeJoinEvent(Object o, PlayerEventArgs e)
        {
            try
            {
                Player player = e.Player;
                JObject datas = player.GetPlayerDatas();
                ItemGather inv = datas["Inventory"].ToObject<ItemGather>();
                ItemGather arm = datas["Clothes"].ToObject<ItemGather>();
                PlayerInventory slot = player.Inventory;
                LevelManager l = player.Level.LevelManager;
                Level lastSeen = l.Levels.First(lv => lv.LevelName.ToLower() == datas["LastSeen"].ToString().ToLower());

                player.SpawnLevel(lastSeen, datas["Location"].ToObject<PlayerLocation>());
                player.GameMode = datas["GameMode"].ToObject<GameMode>();
                player.Effects = datas["Effect"].ToObject<ConcurrentDictionary<EffectType, Effect>>();
                player.HealthManager.Health = datas["Health"].ToObject<Int32>();
                player.HungerManager.Hunger = datas["Hunger"].ToObject<Int32>();
                player.ExperienceLevel = datas["LV"].ToObject<Int32>();
                player.Experience = datas["XP"].ToObject<Int32>();
                
                for (Int32 i = 0; i < inv.Count(); i++)
                {
                    slot.SetInventorySlot(i, inv[i]);
                }

                for (Int32 i = 0; i < arm.Count(); i++)
                {
                    CustomItem armor = arm[i];

                    if (i == 0) slot.Helmet = armor;
                    if (i == 1) slot.Chest = armor;
                    if (i == 2) slot.Leggings = armor;
                    if (i == 3) slot.Boots = armor;
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ex.InnerException + "\r" +  ex.StackTrace);
            }
        }

        private void OnPlayeLeaveEvent(Object o, PlayerEventArgs e)
        {
            Player player = e.Player;

            player.SetPlayerDatas(player.PlayerDatasForm(), true);
        }

    }
}
