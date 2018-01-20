using MiNET;
using MiNET.Plugins;
using MiNET.Worlds;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REMIX_Freighter
{
    class Laeng : Freighter
    {
        protected override void OnEnable()
        {
            Directory.CreateDirectory(Local);
            Context.Server.PlayerFactory.PlayerCreated += (sender, e) =>
            {
                e.Player.PlayerJoin += PlayerJoin;
                e.Player.PlayerLeave += PlayerLeave;
            };


        }

        public void PlayerJoin(Object sender, PlayerEventArgs e)
        {
            JObject load = GetPlayerDatas(e.Player);

        }

        public void PlayerLeave(Object sender, PlayerEventArgs e)
        {
            SaveDefaultPlayerData(e.Player);
            FastLoad.Remove(e.Player.Username.ToLower());
        }







    }
}
