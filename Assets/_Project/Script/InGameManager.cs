using CookApps.Playgrounds.Utility;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum RouletteState
{
    Idle, // 대기 상태 (버튼이 대기 중)
    Spinning, // 룰렛 회전 중
    Result, // 결과 표시 중
}

public class InGameManager : Singleton<InGameManager>
{
    public RouletteState _currentState = RouletteState.Idle;
    public Button _spinButton;
    public GameObject _rewardPopup;
    public Text _descText;
    public Text _titleText;
    public Image _selectedItemImage;
    public Animator _rouletteAnimator;
    public Item _selectedItem;

    public void OnClickPressedButton()
    {
        UpdateState(RouletteState.Spinning);
    }

    public void OnClickResultButton()
    {
        _rewardPopup.SetActive(false);
    }

    protected void Start()
    {
        _spinButton.onClick.AddListener(OnSpinButtonPressed);
        UpdateState(_currentState);
    }

    private void UpdateState(RouletteState newState)
    {
        _currentState = newState;

        switch (_currentState)
        {
            case RouletteState.Idle:
                UpdateState(RouletteState.Idle);
                _rouletteAnimator.SetTrigger("ReStart");
                RouletteApiManager.Instance.GetConsumeRouletteItem((onComplete) =>
                {
                    if (onComplete)
                    {
                        _spinButton.interactable = true;
                    }
                });
                break;
            case RouletteState.Spinning:
                _spinButton.interactable = false;
                _selectedItem = GetRandomItemByWeight();

                _rouletteAnimator.SetTrigger("Start");
                // 룰렛 시작
                // 끝난 후 Result로 옮기기
                break;
            case RouletteState.Result:
                _spinButton.interactable = false;
                RouletteApiManager.Instance.PostConsumeRouletteItem(_selectedItem, (onComplete) =>
                {
                    if (onComplete)
                    {
                        _rewardPopup.SetActive(true);
                    }
                    else
                    {
                        // [TODO] 여기서 오류났을 때 처리
                    }
                });
                break;
        }
    }

    private void OnSpinButtonPressed()
    {
        if (_currentState == RouletteState.Idle)
        {
            UpdateState(RouletteState.Spinning);
        }
    }

    public Item GetRandomItemByWeight()
    {
        // count가 0보다 큰 아이템만 필터링
        List<Item> availableItems = RouletteApiManager.Instance.CachedItems.FindAll(item => item.count > 0);

        if (availableItems.Count == 0)
        {
            Debug.LogWarning("No items available with count > 0.");
            return null;
        }

        // 총 weight 계산
        int totalWeight = 0;
        foreach (var item in availableItems)
        {
            totalWeight += item.weight;
        }

        // 랜덤 포인트 선택
        int randomPoint = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // 랜덤 포인트에 해당하는 아이템 선택
        foreach (var item in availableItems)
        {
            cumulativeWeight += item.weight;
            if (randomPoint < cumulativeWeight)
            {
                return item;
            }
        }

        return null;
    }
}
