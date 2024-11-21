using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BiniLab;
using CookApps.BM.MVPWest;
using UnityEngine;

public class RuleData
{
    public int RuleID = 0;

    public bool IsRuleActivate = false;

    private SpecRule _specRuleData;
    public SpecRule SpecRuleData
    {
        get
        {
            if (_specRuleData == null)
            {
                _specRuleData = SpecDataManager.Instance.GetRuleData(RuleID);
            }

            return _specRuleData;
        }
        set
        {
            _specRuleData = value;
        }
    }
}

public class RuleController : MonoBehaviour
{
    private const int MAX_RULE_COUNT = 2;

    private int _firstOpenTurnCount = 0;
    private int _secondOpenTurnCount = 0;
    private int _thirdOpenTurnCount = 0;

    private Dictionary<int, RuleData> _ruleDataDic = new();

    public bool CheckRuleActionEnd { get; set; } = false; // 룰 관련 연출 종료 체크

    public void Init()
    {
        Clear();

        _firstOpenTurnCount = SpecDataManager.Instance.GetGameConfig<int>("FIRST_RULE_OPEN_TURN");
        _secondOpenTurnCount = SpecDataManager.Instance.GetGameConfig<int>("SECOND_RULE_OPEN_TURN");
        _thirdOpenTurnCount = SpecDataManager.Instance.GetGameConfig<int>("THIRD_RULE_OPEN_TURN");

        SetRuleData();
    }

    public void Refresh()
    {
        UpdateRuleState();
    }

    // 룰 데이터 설정
    public void SetRuleData()
    {
        for (int i = 0; i < MAX_RULE_COUNT; ++i)
        {
            int appearOrder = i + 1; // 해금 등장 순서

            RuleData newRuleData = new RuleData();

            if (UserDataManager.Instance.IsValidTutorialPlay())
            {
                var specRuleScenarioData = SpecDataManager.Instance.GetRuleScenarioData(UserDataManager.Instance.UserData.TutorialPlayCount);
                if (specRuleScenarioData != null)
                {
                    newRuleData.RuleID = specRuleScenarioData.rule_id_list[i];

                    var specRuleData = SpecDataManager.Instance.GetRuleData(newRuleData.RuleID);
                    newRuleData.SpecRuleData = specRuleData;
                }
            }
            else
            {
                SpecRule pickRuleData = PickRandomRuleData(appearOrder);

                newRuleData.RuleID = pickRuleData.rule_id;
                newRuleData.SpecRuleData = pickRuleData;

                // newRuleData.RuleID = 1008;
                // newRuleData.SpecRuleData = SpecDataManager.Instance.GetRuleData(newRuleData.RuleID);
            }

            _ruleDataDic.TryAdd(appearOrder, newRuleData);
        }
    }

    public List<RuleData> GetAllRuleData()
    {
        if (_ruleDataDic == null || _ruleDataDic.Count <= 0) return null;

        return _ruleDataDic.Values.ToList();
    }

    public RuleData GetRuleData(int appearOrder)
    {
        if (_ruleDataDic == null || _ruleDataDic.Count <= 0) return null;
        if (_ruleDataDic.ContainsKey(appearOrder) == false) return null;

        return _ruleDataDic[appearOrder];
    }

    // 현재 턴에 따른 룰 오픈 여부 갱신
    private void UpdateRuleState()
    {
        if (_ruleDataDic == null || _ruleDataDic.Count <= 0) return;

        var currentTurnCount = InGameManager.Instance.GameTurnCount;

        if (currentTurnCount == _firstOpenTurnCount)
        {
            if (_ruleDataDic.ContainsKey(1) && _ruleDataDic[1].IsRuleActivate == false)
            {
                _ruleDataDic[1].IsRuleActivate = true;

                RequestRule(1);
            }
        }
        else if (currentTurnCount == _secondOpenTurnCount)
        {
            if (_ruleDataDic.ContainsKey(2) && _ruleDataDic[2].IsRuleActivate == false)
            {
                _ruleDataDic[2].IsRuleActivate = true;

                RequestRule(2);
            }
        }
        else if (currentTurnCount == _thirdOpenTurnCount)
        {
            if (_ruleDataDic.ContainsKey(3) && _ruleDataDic[3].IsRuleActivate == false)
            {
                _ruleDataDic[3].IsRuleActivate = true;

                RequestRule(3);
            }
        }
    }

    // 와일드 룰 효과 적용 요청
    public void RequestRule(int appearOrder)
    {
        if (_ruleDataDic == null || _ruleDataDic.Count <= 0) return;
        if (_ruleDataDic.ContainsKey(appearOrder) == false) return;

        var ruleData = _ruleDataDic[appearOrder];
        if (ruleData == null) return;

        InGameManager.Instance.SwitchPlayerTurnLimitTimeCount(false);

        // 연출 재생
        var ruleEntryPop = PopupManager.OpenPopup<RuleEntryPop>();
        ruleEntryPop.SetRuleEntryPop(ruleData.SpecRuleData);

        GlobalVibration.Instance.Vibrate(1.0f);

        Run.After(0.1f, () =>
        {
            SoundManager.Instance.PlaySFX("sfx_wildrule_open");
        });

        // 와일드 룰 효과 실제 적용 (팝업을 끄는 타이밍을 늦춰 조작을 막고있으므로 팝업이 꺼지는 타이밍을 함께 고려해야함)
        Run.After(3.0f, () =>
        {
            AdjustRule(ruleData);

            InGameManager.Instance.SwitchPlayerTurnLimitTimeCount(true);
        });
    }

    // 해당 룰이 활성화 되어있는지 체크
    public bool IsActivateRule(RuleType ruleType)
    {
        if (_ruleDataDic == null || _ruleDataDic.Count <= 0) return false;

        foreach (var ruleData in _ruleDataDic)
        {
            if (ruleData.Value.SpecRuleData.rule_type == ruleType && ruleData.Value.IsRuleActivate)
            {
                return true;
            }
        }

        return false;
    }

    // 와일드 룰 효과 실제 적용
    private void AdjustRule(RuleData ruleData)
    {
        if (ruleData == null) return;

        var playerList = InGameManager.Instance.GamePlayerList;
        var currentPlayer = InGameManager.Instance.GetCurrentTurnPlayer();
        var opponentPlayer = InGameManager.Instance.GetOpponentPlayer(currentPlayer.PlayerData.TurnIndex);

        switch (ruleData.SpecRuleData.rule_type)
        {
            case RuleType.Item_Refill_Odd_Block:
                InGameManager.Instance.StageController.ResetItemBox(InGameIndexType.Odd);
                break;
            case RuleType.Hide_Ladder:
                InGameManager.Instance.StageController.HideAllLadder();
                break;
            case RuleType.Hide_Snake:
                InGameManager.Instance.StageController.HideAllSnake();
                break;
            case RuleType.Item_Exchange:
                var tempItemList = currentPlayer.PlayerData.GameItemList;

                currentPlayer.PlayerData.GameItemList = opponentPlayer.PlayerData.GameItemList;
                opponentPlayer.PlayerData.GameItemList = tempItemList;

                // 아이템 룰 관련 연출 재생 (연출 내부에서 Refresh 처리)
                InGameManager.Instance.ItemController.PlayAllDisappearItemEffect();

                //InGameManager.Instance.Refresh(InGameRefreshType.RefreshItem);

                break;
            case RuleType.Use_Hammer_Six:
                var hammerItemData = SpecDataManager.Instance.GetGameItemData(GameItemType.Hammer, -6);
                if (hammerItemData != null)
                {
                    playerList.ForEach(player =>
                    {
                        InGameManager.Instance.UseItem(player.PlayerData.TurnIndex, hammerItemData.game_item_id, true);
                    });
                }
                break;
            case RuleType.Character_Pos_Change:

                var currentBlock = InGameManager.Instance.StageController.GetInGameBlock(currentPlayer.PlayerData.CurrentBlockIndex);
                var opponentBlock = InGameManager.Instance.StageController.GetInGameBlock(opponentPlayer.PlayerData.CurrentBlockIndex);

                int currentPlayerMoveAmount = opponentBlock.BlockData.BlockIndex - currentBlock.BlockData.BlockIndex;
                int opponentPlayerMoveAmount = currentBlock.BlockData.BlockIndex - opponentBlock.BlockData.BlockIndex;

                currentPlayer.ForceMoveToBlock(currentPlayerMoveAmount);
                opponentPlayer.ForceMoveToBlock(opponentPlayerMoveAmount);

                break;
            case RuleType.Item_Refresh:
                int maxItemSlotCount = SpecDataManager.Instance.GetGameConfig<int>("DEFAULT_GAME_ITEM_COUNT");

                playerList.ForEach(player =>
                {
                    player.PlayerData.ClearAllItem();

                    for (int i = 0; i < maxItemSlotCount; ++i)
                    {
                        var randomItem = SpecDataManager.Instance.GetRandomItem();
                        player.PlayerData.AddItem(randomItem.game_item_id);
                    }
                });

                // 아이템 룰 관련 연출 재생 (연출 내부에서 Refresh 처리)
                InGameManager.Instance.ItemController.PlayAllDisappearItemEffect();

                //InGameManager.Instance.Refresh(InGameRefreshType.RefreshItem);
                break;
            case RuleType.Prison:
                currentPlayer.SendPrisonState();
                opponentPlayer.SendPrisonState();
                break;
        }
    }

    // 해금 등장 순서 값에 따른 랜덤 룰 데이터 반환
    private SpecRule PickRandomRuleData(int appearOrder)
    {
        SpecRule resultData = null;

        var currentRuleList = GetAllRuleData();
        var apperSpecRuleList = SpecDataManager.Instance.GetRuleList(appearOrder);

        // 룰 추첨
        if (currentRuleList == null || currentRuleList.Count <= 0) // 첫 룰
        {
            resultData = apperSpecRuleList.RandomRatePick(x => x.rate);
        }
        else
        {
            // 현제 룰 리스트에서 스펙 데이터 리스트 추출
            var currentSpecRuleList = currentRuleList.Select(x => x.SpecRuleData).ToList();

            // 중복없는 리스트를 추출
            var exceptList = currentSpecRuleList.Except(apperSpecRuleList).Union(apperSpecRuleList.Except(currentSpecRuleList)).ToList();

            resultData = exceptList.RandomRatePick(x => x.rate);
        }

        return resultData;
    }

    # region !!!! 치트 전용 !!!
    // !!!! 치트 전용 !!!
    public void CheatChangeActivateTurnCount(int order, int targetTurn)
    {
        switch (order)
        {
            case 1:
                _firstOpenTurnCount = targetTurn;
                break;
            case 2:
                _secondOpenTurnCount = targetTurn;
                break;
            case 3:
                _thirdOpenTurnCount = targetTurn;
                break;
        }
    }

    // !!!! 치트 전용 !!!
    public void CheatChangeRule(int order, int ruleID)
    {
        if (_ruleDataDic == null || _ruleDataDic.Count <= 0) return;
        if (_ruleDataDic.ContainsKey(order) == false) return;

        _ruleDataDic[order].RuleID = ruleID;
        _ruleDataDic[order].SpecRuleData = SpecDataManager.Instance.GetRuleData(ruleID);
    }
    #endregion

    private void Clear()
    {
        _ruleDataDic.Clear();
        CheckRuleActionEnd = true;
    }
}
