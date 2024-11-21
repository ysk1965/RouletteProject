using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
using UnityEngine;

public class PlayerPerkData
{
    public int PerkID; // 퍽 ID

    public bool IsPerkReady; // 퍽 효과 활성화 준비 여부

    public int PerkAmountValue; // 퍽 효과 갯수 관련 값
    public int PerkRemainTurnValue; // 퍽 효과 남은 턴 수

    private SpecPerk _perkSpecData;

    public SpecPerk GetSpecPerkData()
    {
        if (_perkSpecData == null)
        {
            _perkSpecData = SpecDataManager.Instance.GetPerkData(PerkID);
        }

        return _perkSpecData;
    }

    public PerkType GetPerkType => GetSpecPerkData().perk_type;

    public bool IsActiveSelfPerk => PerkAmountValue > 0; // 자기 자신의 퍽 활성화 여부

    // 타겟 타입을 기준으로 퍽 활성화 여부 체크
    public bool IsActiveTargetPerk(PerkType type)
    {
        if (GetPerkType != type) return false;

        switch (type)
        {
            case PerkType.Reroll:
                return IsPerkReady;
            case PerkType.SnakeShield:
                return PerkAmountValue > 0;
        }

        return false;
    }

    public bool IsShowPerkEffect()
    {
        bool result = true;

        switch (GetPerkType)
        {
            case PerkType.Reroll:
                int limitValue = (int)GetSpecPerkData().perk_value.First();
                result = PerkAmountValue < limitValue;
                break;
            case PerkType.SnakeShield:
                break;
        }

        return result;
    }

    public void ReducePerkRemainTurn()
    {
        // 턴 차감
        if (PerkRemainTurnValue > 0)
        {
            PerkRemainTurnValue--;
        }

        // 턴이 0이 됐을경우 처리
        if (PerkRemainTurnValue == 0)
        {
            switch (GetPerkType)
            {
                case PerkType.Reroll:
                    break;
                case PerkType.SnakeShield:
                    PerkAmountValue = 0;
                    break;
            }
        }
    }
}

// 인게임 플레이에서 사용되는 게임 플레이어 데이터
public class GamePlayerData
{
    // 기본 데이터
    public string PlayerID; // 유저 ID (0 = 컴퓨터)
    public string PlayerName;
    public int CharacterID; // 유저 캐릭터 ID

    // 보드 관련
    public int TurnIndex; // 유저 플레이 턴(순서) 인덱스
    public int CurrentBlockIndex; // 현재 위치 블록 인덱스

    // 주사위 관련
    public int DiceCount; // 주사위 갯수
    public int MaxDiceCount; // 최대 주사위 갯수

    // 아이템 관련
    public int MaxItemCount; // 최대 아이템 보유 개수
    public List<int> GameItemList; // 획득한 아이템 ID
    public int UseGameItemID; // 현재 사용한 아이템 인덱스

    // 퍽 관련
    public PlayerPerkData PerkData;

    //public int CurrentBoostGauge; // 현재 부스트 게이지
    //public int MaxBoostGauge; // 최대 부스트 게이지

    // 추가 효과 관련
    public int HammerAmount;  // 해머 효과 값 (0이면 미사용, 0보다크면 해당 값만큼 뒤로 보냄)
    public int PrisonEscapeNumber = 0; // 감옥 탈출에 필요한 숫자 값 (0이면 감옥 상태 아님)
    public int PrisonEscapeCount = 0; // 감옥 탈출 시도 횟수

    private SpecCharacter _characterSpecData;

    public SpecCharacter GetSpecCharacterData => _characterSpecData;


    public bool IsComputerPlayer => PlayerID == "0";
    public bool IsUseItemState => UseGameItemID > 0; // 현재 아이템을 사용한 상태인지 체크
    public bool IsEnoughDiceCount => DiceCount > 0; // 주사위 갯수가 충분한지 체크
    public bool IsActiveHammer => HammerAmount != 0; // 해머 활성화 여부
    public bool IsPrisonState => PrisonEscapeNumber > 0; // 감옥 상태 여부

    public int GetABSHammerAmount => Mathf.Abs(HammerAmount);

    // 유저 데이터를 기반으로 새로운 플레이어 데이터 생성
    public static GamePlayerData CreateUserPlayerData()
    {
        bool isAIMode = InGameManager.Instance.CurrentGameMode == GameModeType.Normal_AI;

        GamePlayerData newPlayerData = CreateCommonPlayerData();
        newPlayerData.PlayerID = UserDataManager.Instance.UserData.PlayerID;
        //newPlayerData.PlayerName = UserDataManager.Instance.UserData.PlayerName;
        newPlayerData.PlayerName = isAIMode ? "ME" : "1P";
        newPlayerData.CharacterID = UserDataManager.Instance.UserData.MainCharacterID;

        newPlayerData._characterSpecData = SpecDataManager.Instance.GetCharacterData(newPlayerData.CharacterID);
        newPlayerData.PerkData = new PlayerPerkData();
        newPlayerData.PerkData.PerkID = newPlayerData.GetSpecCharacterData.perk_id;

        return newPlayerData;
    }

    // 로컬 플레이를 위한 새로운 플레이어 데이터 생성
    public static GamePlayerData CreateLocalUserPlayerData()
    {
        GamePlayerData newPlayerData = CreateCommonPlayerData();
        //newPlayerData.PlayerID = UserDataManager.Instance.UserData.PlayerID;
        //newPlayerData.PlayerName = UserDataManager.Instance.UserData.PlayerName;
        newPlayerData.PlayerID = "2";
        newPlayerData.PlayerName = "2P";

        // 랜덤 캐릭터 설정
        var randomCharacter = SpecDataManager.Instance.GetRandomCharacter();
        newPlayerData.CharacterID = randomCharacter.character_id;

        newPlayerData._characterSpecData = SpecDataManager.Instance.GetCharacterData(newPlayerData.CharacterID);
        newPlayerData.PerkData = new PlayerPerkData();
        newPlayerData.PerkData.PerkID = newPlayerData.GetSpecCharacterData.perk_id;

        return newPlayerData;
    }

    // 새로운 컴퓨터 플레이어 데이터 생성
    public static GamePlayerData CreateComputerPlayerData()
    {
        GamePlayerData newPlayerData = CreateCommonPlayerData();
        newPlayerData.PlayerID = "0";
        newPlayerData.PlayerName = "COM";

        // 랜덤 캐릭터 설정
        var randomCharacter = SpecDataManager.Instance.GetRandomCharacter();
        newPlayerData.CharacterID = randomCharacter.character_id;

        newPlayerData._characterSpecData = SpecDataManager.Instance.GetCharacterData(newPlayerData.CharacterID);
        newPlayerData.PerkData = new PlayerPerkData();
        newPlayerData.PerkData.PerkID = newPlayerData.GetSpecCharacterData.perk_id;

        return newPlayerData;
    }

    private static GamePlayerData CreateCommonPlayerData()
    {
        GamePlayerData newCommonPlayerData = new();

        newCommonPlayerData.TurnIndex = 0;
        newCommonPlayerData.CurrentBlockIndex = 0;

        newCommonPlayerData.DiceCount = SpecDataManager.Instance.GetGameConfig<int>("DEFAULT_DICE_COUNT");
        //newCommonPlayerData.DiceCount = 2;
        newCommonPlayerData.MaxDiceCount = SpecDataManager.Instance.GetGameConfig<int>("DEFAULT_MAX_DICE_COUNT");

        newCommonPlayerData.MaxItemCount = SpecDataManager.Instance.GetGameConfig<int>("DEFAULT_GAME_ITEM_COUNT");
        newCommonPlayerData.GameItemList = Enumerable.Repeat(0, newCommonPlayerData.MaxItemCount).ToList();
        newCommonPlayerData.UseGameItemID = 0;

        return newCommonPlayerData;
    }

    // 플레이어 턴 설정
    public void SetPlayerTurnIndex(int turnIndex)
    {
        TurnIndex = turnIndex;
    }

    // 아이템 추가 (아이템을 추가하고 추가된 슬롯 번호 반환)
    public int AddItem(int itemID)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return -1;
        if (CheckItemListByItemID(0) == false) return -1;

        int listIndex = GameItemList.FindIndex(0, item => item == 0);
        if (listIndex < 0) return listIndex;

        GameItemList[listIndex] = itemID;

        return listIndex;
    }

    public void SelectItem(int itemID)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return;
        if (CheckItemListByItemID(itemID) == false) return;

        UseGameItemID = GetGameItemByID(itemID);
    }

    public void ClearUseItem()
    {
        UseGameItemID = 0;
    }

    public void UseItem(int itemID, bool isForceUse)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return;
        if (!isForceUse && CheckItemListByItemID(itemID) == false) return;

        var gameItemData = SpecDataManager.Instance.GetGameItemData(itemID);
        if (gameItemData == null) return;

        // 아이템 타입에 따른 처리
        if (gameItemData.game_item_active_type == GameItemActiveType.Hold)
        {
            ClearUseItem();
        }
        else if (gameItemData.game_item_active_type == GameItemActiveType.HoldAuto)
        {
            switch (gameItemData.game_item_type)
            {
                case GameItemType.Hammer:
                    HammerAmount += (int)gameItemData.game_item_value.First();
                    break;
            }
        }

        // 게임 아이템 리스트에 적용
        if (!isForceUse)
        {
            var listIndex = GetGameItemIndexByID(itemID);
            GameItemList[listIndex] = 0;
        }
    }

    public void ClearAllItem()
    {
        GameItemList = Enumerable.Repeat(0, MaxItemCount).ToList();
    }

    // 게임 아이템 리스트에 해당 아이템이 있는지 확인 ([0 =] 빈칸 체크, [0 >] 해당 아이템 ID 체크)
    public bool CheckItemListByItemID(int itemID)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return false;

        return GameItemList.Contains(itemID);
    }

    // 게임 아이템 리스트 인덱스에 어떤 아이템이 있는지 확인
    public int GetGameItem(int listIndex)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return 0;

        return GameItemList[listIndex];
    }

    public int GetGameItemByID(int itemID)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return 0;

        return GameItemList.Find(item => item == itemID);
    }

    public int GetGameItemIndexByID(int itemID)
    {
        if (GameItemList == null || GameItemList.Count <= 0) return -1;

        return GameItemList.FindIndex(item => item == itemID);
    }
}
