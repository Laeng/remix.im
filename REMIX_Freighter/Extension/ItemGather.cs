using fNbt;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace REMIX_Freighter.Extension
{
    public class ItemGather : List<CustomItem>
    {
        
    }

    public class CustomItem : Item
    {
        [JsonConstructor]
        public CustomItem(Int16 id, Int16 metadata = 0, Byte count = 1, NbtCompound extraData = null) : base(id, metadata, count)
        {
            Id = id;
            Metadata = metadata;
            Count = count;
            ExtraData = extraData;
        }
    }






}
