using CookApps.Playgrounds.Utility;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public enum RouletteState
{
    Idle,       // 대기 상태 (버튼이 대기 중)
    Spinning,   // 룰렛 회전 중
    Result,     // 결과 표시 중
    Reset       // 초기화 상태
}

public class InGameManager : Singleton<InGameManager>
{
    public RouletteState currentState = RouletteState.Idle;
    public Button spinButton;
    public GameObject rewardPopup;

    private void Start()
    {
        spinButton.onClick.AddListener(OnSpinButtonPressed);
        UpdateState(RouletteState.Idle);
    }

    private void UpdateState(RouletteState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case RouletteState.Idle:
                spinButton.interactable = true;
                rewardPopup.SetActive(false);
                break;

            case RouletteState.Spinning:
                spinButton.interactable = false;
                StartRouletteAnimation();
                break;

            case RouletteState.Result:
                ShowReward();
                break;

            case RouletteState.Reset:
                ResetRoulette();
                break;
        }
    }

    private void OnSpinButtonPressed()
    {
        if (currentState == RouletteState.Idle)
        {
            UpdateState(RouletteState.Spinning);
        }
    }

    private void StartRouletteAnimation()
    {
        // 애니메이션 시작 (코루틴 또는 애니메이션 클립 사용 가능)
        // 애니메이션이 끝난 후, 상태를 Result로 변경
        // StartCoroutine(RouletteSpinCoroutine());
    }

    private System.Collections.IEnumerator RouletteSpinCoroutine()
    {
        // 룰렛 애니메이션 지속 시간
        yield return new WaitForSeconds(2.0f);
        UpdateState(RouletteState.Result);
    }

    private void ShowReward()
    {
        // 보상 표시
        rewardPopup.SetActive(true);

        // 일정 시간 후에 Reset 상태로 전환
        // Invoke("EndRewardDisplay", 2.0f);
    }

    private void EndRewardDisplay()
    {
        UpdateState(RouletteState.Reset);
    }

    private void ResetRoulette()
    {
        // 초기화하고 Idle 상태로 돌아감
        UpdateState(RouletteState.Idle);
    }
}

