using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using UnityEngine;

public class ImageManager : SingletonMonoBehaviour<ImageManager>
{
    [SerializeField] private SpriteAtlasSO _spriteAtlasSO;

    public Sprite GetSprite(string atlasName, string spriteName)
    {
        return _spriteAtlasSO.GetSprite(atlasName, spriteName);
    }

    public Sprite GetCharacterIcon(int characterID)
    {
        string iconName = $"UI_Cha_{characterID}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetPerkIcon(int perkID)
    {
        string iconName = $"Icon_Perk_{perkID}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetPerkIcon(PerkType type)
    {
        var perkData = SpecDataManager.Instance.GetPerkData(type);
        if (perkData == null) return null;

        string iconName = $"Icon_Perk_{perkData.perk_id}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetGameItemIcon(int GameItemID)
    {
        // 공용 스프라이트 처리
        var gameItemData = SpecDataManager.Instance.GetGameItemData(GameItemID);
        if (gameItemData == null) return null;

        string itemName = GameItemID.ToString();
        if (gameItemData.game_item_type == GameItemType.Hammer)
        {
            itemName = gameItemData.game_item_type.ToString();
        }

        string iconName = $"Icon_game_item_{itemName}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetGameItemIcon(GameItemType type)
    {
        string itemName = type.ToString();

        string iconName = $"Icon_game_item_{itemName}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetRuleIcon(int ruleID)
    {
        string iconName = $"Icon_Rule_{ruleID}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetPrisonDiceIcon(int diceNumber)
    {
        string iconName = $"img_Prison_dice_{diceNumber}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_UI_MAIN, iconName);
    }

    public Sprite GetSocialProfileIcon(string playerID)
    {
        bool isComputer = playerID == "0";
        playerID = isComputer  ? "2" : playerID;

        string iconName = $"Social_Profile_Img_{playerID}";

        return _spriteAtlasSO.GetSprite(Define.ATLAS_SOCIAL, iconName);
    }
}
