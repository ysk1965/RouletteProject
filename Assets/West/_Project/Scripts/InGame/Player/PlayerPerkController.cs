using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
using UnityEngine;

public class PlayerPerkController : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _effectPivotTransform;

    [Header("Snake Shield")]
    [SerializeField] private GameObject _snakeShieldPrefab;
    [SerializeField] private GameObject _snakeShieldBreakPrefab;
    private GameObject _snakeShieldObject;

    public void Init()
    {
        Clear();
    }

    // 퍽 활성화 여부 갱신
    public void UpdatePerk(int targetValue)
    {
        var currentPerkData = _playerController.PlayerData.PerkData;
        var currentSpecPerkData = _playerController.PlayerData.PerkData.GetSpecPerkData();

        bool checkPerkCondition = targetValue == currentSpecPerkData.perk_cond_value;

        switch (currentSpecPerkData.perk_type)
        {
            case PerkType.Reroll:
                int limitValue = (int)currentSpecPerkData.perk_value.First();
                bool isLimit = currentPerkData.PerkAmountValue >= limitValue;

                _playerController.PlayerData.PerkData.IsPerkReady = checkPerkCondition && !isLimit;

                if (checkPerkCondition && isLimit == false)
                {
                    _playerController.PlayerData.PerkData.PerkAmountValue++;
                }

                break;
            case PerkType.SnakeShield:
                _playerController.PlayerData.PerkData.IsPerkReady = checkPerkCondition;

                if (checkPerkCondition)
                {
                    _playerController.PlayerData.PerkData.PerkAmountValue = 1;
                    _playerController.PlayerData.PerkData.PerkRemainTurnValue = (int)currentSpecPerkData.perk_value.First();
                }

                break;
        }

        // 퍽 활성화 조건 체크
        // switch (currentPerkData.perk_cond_type)
        // {
        //     case PerkCondType.Dice:
        //         _playerController.PlayerData.PerkData.IsPerkReady = targetValue == currentPerkData.perk_cond_value;
        //         break;
        // }
        //
        // // 퍽 활성화 추가 처리
        // if (_playerController.PlayerData.PerkData.IsPerkReady)
        // {
        //     switch (currentPerkData.perk_type)
        //     {
        //         case PerkType.Reroll:
        //             break;
        //         case PerkType.SnakeShield:
        //             _playerController.PlayerData.PerkData.PerkAmountValue = 1;
        //             _playerController.PlayerData.PerkData.PerkRemainTurnValue = (int)currentPerkData.perk_value.First();
        //             break;
        //     }
        // }

        // 추가 퍽 연출 효과 처리
        UpdateSnakeShieldObject();

        // UI 갱신
        InGameManager.Instance.Refresh(InGameRefreshType.RefreshEquip);
    }

    // 턴 차감 방식 퍽 갱신
    public void UpdatePerkRemainTurn()
    {
        // 퍽 관련 데이터 처리 (턴 차감)
        _playerController.PlayerData.PerkData.ReducePerkRemainTurn();

        // 추가 퍽 연출 효과 처리
        UpdateSnakeShieldObject();

        // UI 갱신
        InGameManager.Instance.Refresh(InGameRefreshType.RefreshEquip);
    }

    public void UpdateSnakeShieldObject()
    {
        if (_playerController.PlayerData.PerkData.IsActiveTargetPerk(PerkType.SnakeShield))
        {
            if (_snakeShieldObject == null)
            {
                _snakeShieldObject = Instantiate(_snakeShieldPrefab, _effectPivotTransform);
            }
        }
        else
        {
            Destroy(_snakeShieldObject);
            _snakeShieldObject = null;
        }
    }

    public void PlaySnakeShieldBreakEffect()
    {
        var snakeShieldBreakObject = Instantiate(_snakeShieldBreakPrefab, _effectPivotTransform);
        Destroy(snakeShieldBreakObject, 0.5f);
    }

    // 초기화가 필요한 퍽데이터 리셋
    public void ResetPerkData()
    {
        if (_playerController.PlayerData.PerkData.GetPerkType == PerkType.Reroll)
        {
            _playerController.PlayerData.PerkData.PerkAmountValue = 0;
        }
    }

    private void Clear()
    {
        if (_snakeShieldObject != null)
        {
            Destroy(_snakeShieldObject);
            _snakeShieldObject = null;
        }

        //BMUtil.RemoveChildObjects(_effectPivotTransform);
    }
}
