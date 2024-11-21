//#define USE_SERVER_SPEC

#if USE_SERVER_SPEC
using CookApps.LocalData;
#endif
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using CookApps.BM.MVPWest;
using CookApps.SpecData;
using BiniLab;
using Unity.VisualScripting;

public partial class SpecDataManager : SingletonMonoBehaviour<SpecDataManager>
{
    private ISpecDataManager _specData;
    public ISpecDataManager SpecData => _specData;

    public async UniTask Initialize(uint serverSpecVersion)
    {
#if USE_SERVER_SPEC
        var localData = new CookAppsLocalData(SecretKey.GetKey());
        string json;
        var localSpecVersion = Preference.LoadPreference(Pref.LOCAL_SPEC_VERSION, 0);
        if (localSpecVersion != serverSpecVersion)
        {
            do
            {
                json = await GrpcManager.Instance.Spec.GetSpecDataAsync(serverSpecVersion);
            } while (string.IsNullOrEmpty(json));

            localData.Save(json, "SpecData");
        }
        else
        {
            if (!localData.TryLoad("SpecData", out json))
            {
                do
                {
                    json = await GrpcManager.Instance.Spec.GetSpecDataAsync(serverSpecVersion);
                } while (string.IsNullOrEmpty(json));

                localData.Save(json, "SpecData");
            }
        }

        Preference.SavePreference(Pref.LOCAL_SPEC_VERSION, serverSpecVersion);
#else
        string json = SpecDataResourceLoader.LoadSpecData();
        await UniTask.Yield();
#endif
        _specData = new CookApps.BM.MVPWest.SpecDataManager();
        _specData.Load(json);
        await UniTask.Yield();
        GenerateCacheSpecData();
        CustomizeSpecData();
    }

    // SpecData Dictionary Cache Data
    private Dictionary<string, SpecLanguage> _languageDic = new(); // key : token_key, value : language data
    private Dictionary<string, SpecGameConfig> _configDic = new(); // key : config_key, value : game config data

    private void CustomizeSpecData()
    {
        // Language
        _languageDic.Clear();
        foreach (SpecLanguage language in _specData.SpecLanguage.All)
        {
            if (!_languageDic.ContainsKey(language.token_key))
            {
                _languageDic.Add(language.token_key, language);
            }
        }

        // Game Config
        _configDic.Clear();
        foreach (SpecGameConfig config in _specData.SpecGameConfig.All)
        {
            if (!_configDic.ContainsKey(config.config_key))
            {
                _configDic.Add(config.config_key, config);
            }
        }
    }

    public T GetGameConfig<T>(string key)
    {
        if (!_configDic.TryGetValue(key, out SpecGameConfig configData))
        {
            return default;
        }

        if (typeof(T) == typeof(int) && configData.config_value_type == ConfigValueType.INT)
        {
            return int.Parse(configData.config_value).ConvertTo<T>();
        }

        if (typeof(T) == typeof(float) && configData.config_value_type == ConfigValueType.FLOAT)
        {
            return float.Parse(configData.config_value).ConvertTo<T>();
        }

        if (typeof(T) == typeof(string) && configData.config_value_type == ConfigValueType.STRING)
        {
            return configData.config_value.ConvertTo<T>();
        }

        return configData.config_value.ConvertTo<T>();
    }

    public string GetLanguageText(string tokenKey, LanguageType targetLanguageType)
    {
        if (_languageDic.TryGetValue(tokenKey, out SpecLanguage languageData))
        {
            switch (targetLanguageType)
            {
                case LanguageType.EN:
                    return languageData.language_en;
            }
        }

        return string.Empty;
    }

    public SpecCharacter GetCharacterData(int characterID)
    {
        return SpecCharacterList.Find(character => character.character_id == characterID);
    }

    public SpecCharacter GetRandomCharacter()
    {
        return SpecCharacterList.RandomRatePick(x => 1);
    }

    public SpecPerk GetPerkData(int perkID)
    {
        return SpecPerkList.Find(perk => perk.perk_id == perkID);
    }

    public SpecPerk GetPerkData(PerkType perkType)
    {
        return SpecPerkList.Find(perk => perk.perk_type == perkType);
    }

    public SpecStage GetStageData(int stageID)
    {
        return SpecStageList.Find(stage => stage.stage_id == stageID);
    }

    public SpecGameItem GetGameItemData(int gameItemID)
    {
        return SpecGameItemList.Find(item => item.game_item_id == gameItemID);
    }

    public SpecGameItem GetGameItemData(GameItemType itemType, float itemValue)
    {
        return SpecGameItemList.Find(item => item.game_item_type == itemType && item.game_item_value.Contains(itemValue));
    }

    public List<SpecGameItem> GetGameItemList(GameItemType itemType)
    {
        return SpecGameItemList.FindAll(item => item.game_item_type == itemType);
    }

    public SpecGameItem GetRandomItem()
    {
        return SpecGameItemList.RandomRatePick(x => 1);
    }

    public SpecGameItem GetRandomItem(GameItemType itemType)
    {
        var itemList = GetGameItemList(itemType);
        return itemList.RandomRatePick(x => 1);
    }

    public SpecRule GetRuleData(int ruleID)
    {
        return SpecRuleList.Find(rule => rule.rule_id == ruleID);
    }

    // 해당 룰 오더값에 속한 룰 리스트 반환
    public List<SpecRule> GetRuleList(int appearOrder)
    {
        return SpecRuleList.FindAll(rule => rule.appear_turn_order.Contains(appearOrder));
    }

    public SpecRuleScenario GetRuleScenarioData(int playCount)
    {
        return SpecRuleScenarioList.Find(scenario => scenario.play_count == playCount);
    }
}
