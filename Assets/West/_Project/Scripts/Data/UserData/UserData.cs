using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
using UnityEngine;

[Serializable]
public class UserCharacter
{
    public int CharacterID; // 캐릭터 ID
    public int Level; // 캐릭터 레벨 (0 이면 미보유, 1 이상이면 보유)
}

public class UserData
{
    // Base Data
    public string PlayerID;
    public string PlayerName;

    // Game Data
    public int RankPoint;
    public int TutorialPlayCount;

    // Currency Data
    public int Gold;
    public int Jewel;

    // Character Data
    public int MainCharacterID; // 현재 선택된 캐릭터 ID
    public Dictionary<int, UserCharacter> CharacterDic = new(); // 보유한 캐릭터 목록
}

public partial class UserDataManager
{
    public void SetMainCharacter(int characterID)
    {
        UserData.MainCharacterID = characterID;
        SaveUserData();
    }

    public void AddCharacter(int characterID, bool needSave)
    {
        UserData.CharacterDic.TryAdd(characterID, new UserCharacter
        {
            CharacterID = characterID,
            Level = 1
        });

        if (needSave)
        {
            SaveUserData();
        }
    }

    public void IncreaseRankPoint(int amount, bool needSave)
    {
        UserData.RankPoint += amount;

        if (needSave)
        {
            SaveUserData();
        }
    }

    public void DecreaseRankPoint(int amount, bool needSave)
    {
        UserData.RankPoint -= amount;

        if (needSave)
        {
            SaveUserData();
        }
    }

    public void SetTutorialPlayCount(int setValue, bool needSave)
    {
        UserData.TutorialPlayCount = setValue;

        if (needSave)
        {
            SaveUserData();
        }
    }

    public void InceraseTutorialPlayCount(bool needSave)
    {
        UserData.TutorialPlayCount++;

        if (needSave)
        {
            SaveUserData();
        }
    }

    public void IncreaseItem(ItemType targetType, int amount, bool needSave)
    {
        switch (targetType)
        {
            case ItemType.Gold:
                UserData.Gold += amount;
                break;
            case ItemType.Jewel:
                UserData.Jewel += amount;
                break;
        }

        if (needSave)
        {
            SaveUserData();
        }
    }

    public void DecreaseItem(ItemType targetType, int amount, bool needSave)
    {
        switch (targetType)
        {
            case ItemType.Gold:
                UserData.Gold -= amount;
                break;
            case ItemType.Jewel:
                UserData.Jewel -= amount;
                break;
        }

        if (needSave)
        {
            SaveUserData();
        }
    }

    public bool CheckEnoughItem(ItemType type, int amount, bool showToast = true)
    {
        var msg = "";
        var isEnough = false;

        switch (type)
        {
            case ItemType.Gold:
                isEnough = UserData.Gold >= amount;
                msg = "골드가 부족합니다.";
                break;
            case ItemType.Jewel:
                isEnough = UserData.Jewel >= amount;
                msg = "보석이 부족합니다.";
                break;
        }

        if (isEnough == false && showToast)
        {
            //ToastManager.Instance.OnMessage(msg);
        }

        return isEnough;
    }

    // 튜토리얼 플레이가 가능한 플레이 카운트인지 체크
    public bool IsValidTutorialPlay()
    {
        int maxPlayCount = SpecDataManager.Instance.SpecRuleScenarioList.Max(scenario => scenario.play_count);

        return UserData.TutorialPlayCount <= maxPlayCount;
    }
}
