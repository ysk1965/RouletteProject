using CookApps.Playgrounds.Utility;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;

public enum RouletteState
{
    Idle, // 대기 상태 (버튼이 대기 중)
    Spinning, // 룰렛 회전 중
    Result, // 결과 표시 중
}

public class InGameManager : SingletonMonoBehaviour<InGameManager>
{
    [SerializeField] private Button _spinButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _selectedItemImage;
    [SerializeField] private Animator _rouletteAnimator;
    [SerializeField] private Animator _uiAnimator;
    [SerializeField] private Animator _popupAnimator;

    [SerializeField] private List<Sprite> _sprites;

    [SerializeField] private AudioSource _clickSfx;
    [SerializeField] private AudioSource _pinSoundSfx;
    [SerializeField] private AudioSource _resultSfx;
    [SerializeField] private AudioSource _rouletteStartSfx;
    [SerializeField] private AudioSource _rouletteEndSfx;

    private RouletteState _currentState = RouletteState.Idle;
    private Item _selectedItem;

    public void OnClickPressedButton()
    {
        UpdateState(RouletteState.Spinning);
        _clickSfx.Play();
    }

    public void OnClickResultButton()
    {
        _popupAnimator.SetTrigger("PopupClose");
        UpdateState(RouletteState.Idle);
        _clickSfx.Play();
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
                _rouletteAnimator.SetTrigger("Restart");
                RouletteApiManager.Instance.GetConsumeRouletteItem((onComplete) =>
                {
                    if (onComplete)
                    {
                        bool isExistsItem = RouletteApiManager.Instance.CachedItems.Exists(l => l.count > 0);
                        if (isExistsItem)
                        {
                            _spinButton.interactable = true;
                            _selectedItem = GetRandomItemByWeight();

                            int findIndex = RouletteApiManager.Instance.CachedItems.FindIndex(l => l.name == _selectedItem.name);
                            _rouletteAnimator.SetInteger("Number", findIndex);
                        }
                        else
                        {
                            // [TODO] 현재 상품이 모두 소진되었습니다.
                        }
                    }
                });
                // 상태 변경 : OnClickPressedButton()
                break;
            case RouletteState.Spinning:
                _rouletteStartSfx.Play();
                _spinButton.interactable = false;

                _rouletteAnimator.SetTrigger("Start");
                _uiAnimator.SetTrigger("Start");
                // 상태 변경 : ChangeStateToResult()
                break;
            case RouletteState.Result:
                _rouletteEndSfx.Play();
                _spinButton.interactable = false;
                _uiAnimator.SetTrigger("Restart");
                RouletteApiManager.Instance.PostConsumeRouletteItem(_selectedItem, (onComplete) =>
                {
                    if (onComplete)
                    {
                        _popupAnimator.SetTrigger("PopupOpen");
                        int findIndex = RouletteApiManager.Instance.CachedItems.FindIndex(l => l.name == _selectedItem.name);
                        SetPopUp(findIndex);
                    }
                });
                // 상태 변경 : OnClickResultButton()
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

    private void SetPopUp(int findIndex)
    {
        switch (findIndex)
        {
            case 0 :
                _titleText.text = "담요";
                _selectedItemImage.sprite = _sprites[findIndex];
                break;
            case 1 :
                _titleText.text = "스티커";
                _selectedItemImage.sprite = _sprites[findIndex];
                break;
            case 2 :
                _titleText.text = "엽서";
                _selectedItemImage.sprite = _sprites[findIndex];
                break;
            case 3 :
                _titleText.text = "인생네컷";
                _selectedItemImage.sprite = _sprites[findIndex];
                break;
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

    public void ChangeStateToResult()
    {
        UpdateState(RouletteState.Result);
    }
}
