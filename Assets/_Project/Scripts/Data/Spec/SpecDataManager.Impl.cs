using System.Collections.Generic;
using System.Linq;
using CookApps.Playgrounds.Utility;

namespace CookApps.BM.TTT.Data
{
    public partial class SpecDataManager : Singleton<SpecDataManager>
    {
        private Dictionary<string, SpecGameConfig> _configMap = new();
        private Dictionary<int, SpecRouletteItem> _rouletteItemMap = new();
        private Dictionary<int, List<SpecRouletteSet>> _rouletteSetMap = new();
        private Dictionary<int, List<SpecUpgradeObject>> _upgradeObjectMap = new();

        public void LoadSpecData()
        {
            LoadFromResource();
            PostProcess();
        }

        private void PostProcess()
        {
            foreach (SpecGameConfig record in SpecGameConfig.All)
            {
                _configMap.TryAdd(record.config_key, record);
            }

            SpecGameConfig = null;

            foreach (SpecRouletteItem record in SpecRouletteItem.All)
            {
                _rouletteItemMap.TryAdd(record.id, record);
            }

            SpecRouletteItem = null;

            _rouletteSetMap = SpecRouletteSet.All.GroupBy(data => data.roulette_set_id)
                .ToDictionary(data => data.Key, data => data.ToList());
            SpecRouletteSet = null;

            _upgradeObjectMap = SpecUpgradeObject.All.GroupBy(data => data.map_id)
                .ToDictionary(data => data.Key, data => data.ToList());
            SpecUpgradeObject = null;
        }

        public List<SpecRouletteSet> GetRouletteSetList(int rouletteSetId)
        {
            if (_rouletteSetMap.TryGetValue(rouletteSetId, out List<SpecRouletteSet> list))
            {
                return list;
            }

            return new List<SpecRouletteSet>();
        }

        public int GetIntConfig(string key)
        {
            if (_configMap.TryGetValue(key, out SpecGameConfig config))
            {
                if (int.TryParse(config.config_value, out int value))
                {
                    return value;
                }
            }

            return 0;
        }

        public float GetFloatConfig(string key)
        {
            if (_configMap.TryGetValue(key, out SpecGameConfig config))
            {
                if (float.TryParse(config.config_value, out float value))
                {
                    return value;
                }
            }

            return 0;
        }

        public string GetStringConfig(string key)
        {
            if (_configMap.TryGetValue(key, out SpecGameConfig config))
            {
                return config.config_value;
            }

            return string.Empty;
        }

        public List<SpecUpgradeObject> GetUpgradeObjectList(int mapId)
        {
            if (_upgradeObjectMap.TryGetValue(mapId, out List<SpecUpgradeObject> list))
            {
                return list;
            }

            return new List<SpecUpgradeObject>();
        }
    }
}
