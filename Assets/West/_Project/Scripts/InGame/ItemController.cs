using System;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
using CookApps.Obfuscator;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemController : MonoBehaviour
{
    [Header("Item")]
    [Header("Item List")]
    [SerializeField] private GameObject _gameItemListContentObject;

    [SerializeField] private GameObject _gameItemPrefab;

    [SerializeField] private GameObject _selectItemUIObject;

    [Space]
    [SerializeField] private List<ItemCardSlot> _itemCardSlotList = new();

    [FormerlySerializedAs("_itemCardSlotRectTransform")]
    [SerializeField] private List<RectTransform> _itemCardSlotRectTransformList = new();

    [Header("Effect")]
    [SerializeField] private TargetLineRenderer _targetLineRenderer;

    private bool _itemSelectedDelayFlag = false;

    public int CurrentSelectedItemSlot { get; set; } = -1; // -1: 선택된 아이템 없음

    public void Refresh()
    {
        ClearUI();

        SetGameItemCardSlotList();
    }

    public void RefreshGameItemCardSlotList()
    {
        if (_itemCardSlotList == null || _itemCardSlotList.Count <= 0)
        {
            return;
        }

        _itemCardSlotList.ForEach(gameItem => gameItem.Refresh());
    }

    public void OnClickCardSlotItemButton(int slotIndex)
    {
        if (InGameManager.Instance.CurrentGamePlayState != GamePlayStateType.ReadyToRoll) return;
        if (InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.IsComputerPlayer) return;
        if (InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.GameItemList.Count <= 0) return;

        // 연출 중복 방지 체크
        if (_itemSelectedDelayFlag) return;

        // 아이템 카드슬롯 선택 딜레이 적용 (연출 중복 방지)
        _itemSelectedDelayFlag = true;
        Run.After(0.5f, () =>
        {
            _itemSelectedDelayFlag = false;
        });


        int currentSelectItemID = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.GetGameItem(slotIndex);

        var targetGameItemID = 0;

        // 같은 슬롯 클릭 체크
        if (CurrentSelectedItemSlot == slotIndex)
        {
            CurrentSelectedItemSlot = -1;
            targetGameItemID = 0;
        }
        else
        {
            CurrentSelectedItemSlot = currentSelectItemID > 0 ? slotIndex : -1;
            targetGameItemID = currentSelectItemID;
        }

        UseGameItem(targetGameItemID);
    }

    public void OnClickCloseItemListButton()
    {
        _selectItemUIObject.SetActive(false);
    }

    // 주사위 타입 아이템의 사용 결과를 반환
    public int GetDiceItemResult(int gameItemID)
    {
        var result = 0;

        SpecGameItem gameItemData = SpecDataManager.Instance.GetGameItemData(gameItemID);

        if (gameItemData == null)
        {
            return result;
        }

        if (gameItemData.game_item_type != GameItemType.Dice)
        {
            return result;
        }

        // 주사위 결과 계산
        result = (int) BMUtil.GetRandomValue(gameItemData.game_item_value);

        return result;
    }

    public void SetGameItemCardSlotList()
    {
        List<int> currentPlayerItemList = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.GameItemList;

        for (var i = 0; i < _itemCardSlotList.Count; i++)
        {
            if (currentPlayerItemList[i] > 0)
            {
                _itemCardSlotList[i].SetItemCardSlot(currentPlayerItemList[i]);
            }
            else
            {
                _itemCardSlotList[i].SetEmptyCardSlot();
            }
        }
    }

    // 아이템 자동 사용 (ai) - 사용여부 리턴
    public bool AutoUseItem()
    {
        bool isItemUsed = false;

        PlayerController currentPlayer = InGameManager.Instance.GetCurrentTurnPlayer();

        // 사용 가능한 아이템 체크
        List<int> gameItemList = currentPlayer.PlayerData.GameItemList;

        if (gameItemList.Count <= 0)
        {
            return isItemUsed;
        }

        // 아이템 우선순위 기준에 맞춰 정렬 (todo.. 추후 고도화 필요)
        gameItemList = gameItemList.OrderBy(item => item).ToList();

        // ai 아이템 사용 체크 시작
        var useHammerItemFlag = false; // 해머 아이템은 1턴에 1회만 사용제한

        foreach (int gameItem in gameItemList)
        {
            if (gameItem <= 0)
            {
                continue;
            }

            SpecGameItem gameItemData = SpecDataManager.Instance.GetGameItemData(gameItem);

            // 해머 타입 아이템
            if (gameItemData.game_item_type == GameItemType.Hammer && useHammerItemFlag == false)
            {
                useHammerItemFlag = true;
                UseGameItem(gameItem);
                isItemUsed = true;
            }

            // 주사위 타입 아이템
            if (gameItemData.game_item_type == GameItemType.Dice)
            {
                // 명시적 숫자 타입
                if (gameItemData.game_item_value.Length == 1)
                {
                    var targetNumber = (int) gameItemData.game_item_value.First();

                    int targetBlockIndex = currentPlayer.PlayerData.CurrentBlockIndex + targetNumber;
                    InGameBlock targetBlock = InGameManager.Instance.StageController.GetInGameBlock(targetBlockIndex);

                    if (targetBlock == null)
                    {
                        continue;
                    }

                    // 1.사다리 도달 가능 여부 체크
                    bool isHideLadderRuleActivate = InGameManager.Instance.RuleController.IsActivateRule(RuleType.Hide_Ladder); // 사다리 숨기기 와일드 룰 체크
                    bool isFartherMove = targetBlockIndex > currentPlayer.PlayerData.CurrentBlockIndex; // 현재 칸 보다 더 멀리갈수 있는지 체크

                    if (isHideLadderRuleActivate == false && isFartherMove)
                    {
                        if (targetBlock.BlockData.IsLadderStartBlock)
                        {
                            UseGameItem(gameItem);
                            isItemUsed = true;
                            break;
                        }
                    }

                    // 2. 퍼펙트 랜딩 여부 체크
                    var endBlock = InGameManager.Instance.StageController.EndBlock;
                    if (endBlock.BlockData.BlockIndex == targetBlockIndex)
                    {
                        UseGameItem(gameItem);
                        isItemUsed = true;
                        break;
                    }
                }

                // 리스트형 숫자 타입
                if (gameItemData.game_item_value.Length > 1)
                {
                    foreach (ObfuscatorFloat diceValue in gameItemData.game_item_value)
                    {
                        var targetNumber = (int) diceValue;

                        int targetBlockIndex = currentPlayer.PlayerData.CurrentBlockIndex + targetNumber;
                        InGameBlock targetBlock = InGameManager.Instance.StageController.GetInGameBlock(targetBlockIndex);

                        if (targetBlock == null)
                        {
                            continue;
                        }

                        // 1.사다리 도달 가능 여부 체크
                        bool isHideLadderRuleActivate = InGameManager.Instance.RuleController.IsActivateRule(RuleType.Hide_Ladder); // 사다리 숨기기 와일드 룰 체크
                        bool isFartherMove = targetBlockIndex > currentPlayer.PlayerData.CurrentBlockIndex; // 현재 칸 보다 더 멀리갈수 있는지 체크

                        if (isHideLadderRuleActivate == false && isFartherMove)
                        {
                            if (targetBlock.BlockData.IsLadderStartBlock)
                            {
                                UseGameItem(gameItem);
                                isItemUsed = true;
                                break;
                            }
                        }

                        // 2. 퍼펙트 랜딩 여부 체크
                        var endBlock = InGameManager.Instance.StageController.EndBlock;
                        if (endBlock.BlockData.BlockIndex == targetBlockIndex)
                        {
                            UseGameItem(gameItem);
                            isItemUsed = true;
                            break;
                        }
                    }
                }
            }
        }

        return isItemUsed;
    }

    // 아이템 획득 트레일 연출 재생
    public void PlayGetItemTrailEffect(Transform startTransform, int targetSlotNumber, Action completeCallback)
    {
        if (_targetLineRenderer == null)
        {
            return;
        }

        if (targetSlotNumber < 0)
        {
            return;
        }

        RectTransform itemSlot = _itemCardSlotRectTransformList[targetSlotNumber];

        if (itemSlot != null)
        {
            _targetLineRenderer.DrawLineObjectToUI(startTransform, itemSlot, completeCallback);
        }
    }

    // 아이템 사라지는 연출 재생 (타겟)
    public void PlayDisappearItemEffect(int slotIndex)
    {
        if (slotIndex < 0)
        {
            return;
        }

        if (_itemCardSlotList == null || _itemCardSlotList.Count <= 0)
        {
            return;
        }

        ItemCardSlot itemSlot = _itemCardSlotList[slotIndex];
        if (itemSlot == null)
        {
            return;
        }

        itemSlot.PlayDisapperAnim();
    }

    // 아이템 사라지는 연출 재생 (전체)
    public void PlayAllDisappearItemEffect()
    {
        if (_itemCardSlotList == null || _itemCardSlotList.Count <= 0)
        {
            return;
        }

        _itemCardSlotList.ForEach(item => item.PlayDisapperAnim());
    }

    public void ClearUI()
    {
        CurrentSelectedItemSlot = -1;

        _itemSelectedDelayFlag = false;
    }

    private void UseGameItem(int gameItemID)
    {
        PlayerController currentPlayer = InGameManager.Instance.GetCurrentTurnPlayer();

        // 같은 슬롯 클릭에 대한 처리
        if (gameItemID == 0)
        {
            currentPlayer.PlayerData.ClearUseItem();
            RefreshGameItemCardSlotList();
            InGameManager.Instance.StageController.UpdateBlockPin();
            return;
        }

        SpecGameItem specGameItemData = SpecDataManager.Instance.GetGameItemData(gameItemID);
        if (specGameItemData.game_item_active_type == GameItemActiveType.Hold)
        {
            currentPlayer.PlayerData.SelectItem(gameItemID);

            RefreshGameItemCardSlotList();

            InGameManager.Instance.StageController.UpdateBlockPin();
        }
        else if (specGameItemData.game_item_active_type == GameItemActiveType.HoldAuto)
        {
            // 아이템 사용 처리
            InGameManager.Instance.UseItem(currentPlayer.PlayerData.TurnIndex, specGameItemData.game_item_id, false);
        }

        SoundManager.Instance.PlaySFX("sfx_card_use");
    }
}
