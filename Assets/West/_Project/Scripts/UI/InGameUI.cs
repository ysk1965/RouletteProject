using System.Collections.Generic;
using CookApps.BM.MVPWest;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// DOTween 네임스페이스

public class InGameUI : SingletonMonoBehaviour<InGameUI>
{
    [SerializeField] private Transform _popupTransform;

    [Header("Common")]
    [SerializeField] private InGameCameraController _cameraController;

    [SerializeField] private Image _diceBGImage;
    [SerializeField] private GameObject _playerEquipSlotPrefab;
    [SerializeField] private List<TextMeshProUGUI> _itemSlotTextList;

    [Header("Player Profile")]
    [SerializeField] private Animator _player1ProfileAnimator;

    [SerializeField] private GameObject _player1BottomBarObject;
    [SerializeField] private Image _player1ProfileImage;
    [SerializeField] private TextMeshProUGUI _player1NameText;
    [SerializeField] private Color _player1DiceBGColor;
    [SerializeField] private Color _player1ItemSlotTextColor;
    [SerializeField] private IngamePerkSlot _player1PerkSlot;
    [SerializeField] private Transform _player1EquipLayout;

    [Space]
    [SerializeField] private Animator _player2ProfileAnimator;

    [SerializeField] private GameObject _player2BottomBarObject;
    [SerializeField] private Image _player2ProfileImage;
    [SerializeField] private TextMeshProUGUI _player2NameText;
    [SerializeField] private Color _player2DiceBGColor;
    [SerializeField] private Color _player2ItemSlotTextColor;
    [SerializeField] private IngamePerkSlot _player2PerkSlot;
    [SerializeField] private Transform _player2EquipLayout;

    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI _turnCountText;

    [SerializeField] private GameObject _turnAlertEffectObject; // 게임 종료 턴 임박 시 연출 오브젝트
    [SerializeField] private GameObject _waitTurnBlockObject;
    [SerializeField] private GameObject _missEffectObject; // 퍼펙트 랜딩 실패 이펙트 오브젝트
    [SerializeField] private GameObject _itemSlotFullEffectObject; // 퍼펙트 랜딩 실패 이펙트 오브젝트
    [SerializeField] private Transform _bottomBarTransform;
    [SerializeField] private float _moveDistance = 100f; // 내려갈 거리
    [SerializeField] private float _duration = 0.4f; // 애니메이션 시간

    [Header("Time Limit Counter")]
    [SerializeField] private TurnTimeCounter _turnTimeCounter;

    [SerializeField] private Animator _turnTimeCounterAnimator;

    [Header("Perk")]
    [SerializeField] private PerkAppearEffect _perkAppearEffect;

    [Header("Rule")]
    [SerializeField] private List<RuleTipSlot> _ruleTipSlotList;

    private GamePlayerData _player1Data;
    private GamePlayerData _player2Data;

    public InGameCameraController CameraController => _cameraController;

    private void OnEnable()
    {
        PopupManager.CloseAllPopup();

        PopupManager.Instance.popupLayerTr = _popupTransform;
    }

    protected override void OnDestroy()
    {
        // InGameUI 오버라이드하여 파괴시키지 않음 처리
    }

    public void Init()
    {
        Clear();

        SetPlayerData();
        SetPlayerInfo();
    }

    public void Refresh(bool isInit)
    {
        UpdateTurnCount();
        UpdateRuleTip();

        if (isInit == false)
        {
            UpdatePlayerTurn();
            UpdateCamera();
        }
    }

    public void PlayMissEffect()
    {
        if (_missEffectObject == null)
        {
            return;
        }

        _missEffectObject.SetActive(true);
        SoundManager.Instance.PlaySFX("sfx_miss");
    }

    public void PlayPerkEffect(SpecPerk perkData)
    {
        if (_perkAppearEffect == null)
        {
            return;
        }

        _perkAppearEffect.SetPerk(perkData);
        _perkAppearEffect.gameObject.SetActive(true);
    }

    public void PlayItemSlotFullEffect()
    {
        if (_itemSlotFullEffectObject == null)
        {
            return;
        }

        _itemSlotFullEffectObject.SetActive(true);
    }

    public void OnOffWaitTurnBlock(bool isOn)
    {
        if (isOn)
        {
            _bottomBarTransform.DOMoveY(_bottomBarTransform.position.y - _moveDistance, _duration).SetEase(Ease.InQuad);
        }
        else
        {
            // 원래 위치로 이동 (위로 올라가기)
            _bottomBarTransform.DOMoveY(_bottomBarTransform.position.y + _moveDistance, _duration)
                .SetEase(Ease.OutBack);
        }
    }

    public void OnOffTurnTimeCounter(bool isOn)
    {
        if (isOn)
        {
            _turnTimeCounter.gameObject.SetActive(true);
            _turnTimeCounterAnimator.SetTrigger("isEntry");
        }
        else
        {
            _turnTimeCounterAnimator.SetTrigger("isOut");
        }
    }

    public void SetTurnTimeCount(int timeCount)
    {
        _turnTimeCounter.SetNumberCount(timeCount);
    }

    private void SetPlayerData()
    {
        List<PlayerController> playerList = InGameManager.Instance.GamePlayerList;

        foreach (PlayerController player in playerList)
        {
            switch (player.PlayerData.TurnIndex)
            {
                case 0:
                    _player1Data = player.PlayerData;
                    break;
                case 1:
                    _player2Data = player.PlayerData;
                    break;
            }
        }
    }

    private void SetPlayerInfo()
    {
        if (_player1Data != null)
        {
            _player1NameText.text = _player1Data.PlayerName;
            _player1PerkSlot.SetSlot(_player1Data.PerkData.PerkID);
        }

        if (_player2Data != null)
        {
            _player2NameText.text = _player2Data.PlayerName;
            _player2PerkSlot.SetSlot(_player2Data.PerkData.PerkID);
        }
    }

    // 해당 플레이어 턴 관련 처리 갱신
    public void UpdatePlayerTurn()
    {
        int currentTurnIndex = InGameManager.Instance.CurrentTurnIndex;

        int targetPlayerIndex = currentTurnIndex + 1;

        _player1BottomBarObject.SetActive(targetPlayerIndex == 1);
        _player2BottomBarObject.SetActive(targetPlayerIndex == 2);

        switch (targetPlayerIndex)
        {
            case 1:
                _player1ProfileAnimator.SetTrigger("TurnOn");
                _player2ProfileAnimator.SetTrigger("TurnOff");

                _diceBGImage.color = _player1DiceBGColor;
                _itemSlotTextList.ForEach(slotText => slotText.color = _player1ItemSlotTextColor);

                TweenUtil.BottomBarChangeTween(_player1BottomBarObject.transform);
                break;
            case 2:
                _player1ProfileAnimator.SetTrigger("TurnOff");
                _player2ProfileAnimator.SetTrigger("TurnOn");

                _diceBGImage.color = _player2DiceBGColor;
                _itemSlotTextList.ForEach(slotText => slotText.color = _player2ItemSlotTextColor);

                TweenUtil.BottomBarChangeTween(_player2BottomBarObject.transform);
                break;
        }
    }

    // 카메라 관련 갱신
    public void UpdateCamera()
    {
        if (_cameraController == null)
        {
            return;
        }

        PlayerController currentTurnPlayer = InGameManager.Instance.GetCurrentTurnPlayer();

        // 카메라 관련 처리
        _cameraController.SetPlayerFocus(currentTurnPlayer.gameObject.transform);
    }

    public void UpdateItemEquipSlot()
    {
        BMUtil.RemoveChildObjects(_player1EquipLayout);
        BMUtil.RemoveChildObjects(_player2EquipLayout);

        if (_player1Data != null)
        {
            // 해머 아이템 체크
            if (_player1Data.IsActiveHammer)
            {
                GameObject newSlot = Instantiate(_playerEquipSlotPrefab, _player1EquipLayout);
                var equipSlot = newSlot.GetComponent<IngameItemEquipSlot>();

                equipSlot.SetHammerSlot(_player1Data.HammerAmount);
            }

            // 뱀 쉴드 체크
            if (_player1Data.PerkData.IsActiveTargetPerk(PerkType.SnakeShield))
            {
                GameObject newSlot = Instantiate(_playerEquipSlotPrefab, _player1EquipLayout);
                var equipSlot = newSlot.GetComponent<IngameItemEquipSlot>();

                equipSlot.SetSnakeSlot(_player1Data.PerkData.PerkRemainTurnValue);
            }
        }

        if (_player2Data != null)
        {
            // 해머 아이템 체크
            if (_player2Data.IsActiveHammer)
            {
                GameObject newSlot = Instantiate(_playerEquipSlotPrefab, _player2EquipLayout);
                var equipSlot = newSlot.GetComponent<IngameItemEquipSlot>();

                equipSlot.SetHammerSlot(_player2Data.HammerAmount);
            }

            // 뱀 쉴드 체크
            if (_player2Data.PerkData.IsActiveTargetPerk(PerkType.SnakeShield))
            {
                GameObject newSlot = Instantiate(_playerEquipSlotPrefab, _player2EquipLayout);
                var equipSlot = newSlot.GetComponent<IngameItemEquipSlot>();

                equipSlot.SetSnakeSlot(_player2Data.PerkData.PerkRemainTurnValue);
            }
        }
    }

    private void UpdateTurnCount()
    {
        int currentTurnCount = InGameManager.Instance.GameTurnCount;
        int endTurnCount = InGameManager.Instance.GameEndTurnCount;

        bool isNearEndTurn = endTurnCount - currentTurnCount <= 3; // 종료 턴이 3턴 이내일 경우

        string textColorCode = isNearEndTurn ? "#C3513F" : "#FFF886";

        _turnCountText.text = $"<color={textColorCode}>{currentTurnCount}</color>/{endTurnCount}";
        _turnAlertEffectObject.SetActive(isNearEndTurn);
    }

    private void UpdateRuleTip()
    {
        List<RuleData> ruleDataList = InGameManager.Instance.RuleController.GetAllRuleData();

        if (ruleDataList == null || ruleDataList.Count <= 0)
        {
            return;
        }

        if (ruleDataList.Count != _ruleTipSlotList.Count)
        {
            return;
        }

        for (var i = 0; i < _ruleTipSlotList.Count; ++i)
        {
            if (ruleDataList[i].IsRuleActivate == false)
            {
                continue;
            }

            _ruleTipSlotList[i].SetSlot(ruleDataList[i].RuleID);
        }
    }

    private void Clear()
    {
        _player1Data = null;
        _player2Data = null;

        _turnTimeCounter.gameObject.SetActive(false);
    }
}
