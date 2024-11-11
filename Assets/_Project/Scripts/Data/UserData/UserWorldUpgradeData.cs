using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace CookApps.BM.TTT.Data
{
    public interface ISerializableWorldObject
    {
        public int MapId { get; }
        public (int, UpgradeObjectType, int) Serialize();
        public void Deserialize(int level);
    }

    public record UserWorldUpgrade
    {
        public int MapId { get; set; }
        public UpgradeObjectType UpgradeObjectType { get; set; }
        public int Level { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserWorldUpgradeData : UserDataBase
    {
    }
}
