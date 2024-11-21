using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Animator _diceAnimator;
    [SerializeField] private Animator _diceAlertAnimator;
    [SerializeField] private GameObject _diceBackImageObject;

    [SerializeField] private TextMeshProUGUI _diceCountText;

    public void Init()
    {
        Refresh();
    }

    public void Refresh()
    {
        int currentDiceCount = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.DiceCount;
        int maxDiceCount = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.MaxDiceCount;

        var diceCountString = $"{currentDiceCount}<color=#ABC2CF>/{maxDiceCount}</color>";

        _diceCountText.text = diceCountString;
    }

    public void OnClickRollDiceButton()
    {
        if (InGameManager.Instance.CurrentGamePlayState != GamePlayStateType.ReadyToRoll) return;
        if (InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.IsComputerPlayer) return;
        if (InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.IsEnoughDiceCount == false) return;

        Debug.Log("Roll Dice Button Press!!");

        RollDice();
    }

    // 자동으로 주사위 굴리기 (컴퓨터 등)
    public void AutoRollDice()
    {
        if (InGameManager.Instance.CurrentGamePlayState != GamePlayStateType.ReadyToRoll) return;

        Debug.Log("Auto Roll Dice Activate!!");

        RollDice();
    }

    // 주사위 굴리기 노티 처리
    public void OnOffRollDiceAlert(bool isOn)
    {
        if (_diceAlertAnimator == null) return;

        _diceAlertAnimator.enabled = isOn;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (InGameManager.Instance.CurrentGamePlayState != GamePlayStateType.ReadyToRoll) return;
        //
        // //todo.. 주사위 게이지 처리
        // Debug.Log("Roll Dice Button Down!!");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // if (InGameManager.Instance.CurrentGamePlayState != GamePlayStateType.ReadyToRoll) return;
        //
        // Debug.Log("Roll Dice Button Up!!");
        //
        // int diceResult = DecideDiceResult();
        //
        // InGameManager.Instance.AdjustDiceResult(diceResult);
    }

    // 주사위 굴리기
    private void RollDice()
    {
        InGameManager.Instance.ClearUI();

        int diceResult = DecideDiceResult();

        PlayDiceAnim(diceResult);

        InGameManager.Instance.AdjustDiceResult(diceResult);

        TweenUtil.TweenDiceRollBehind(_diceBackImageObject.transform);

        SoundManager.Instance.PlaySFX("sfx_dice");
    }

    // 아이템, 부스터 등 요소를 고려하여 주사위 결과값 결정
    private int DecideDiceResult()
    {
        int resultNumber = Random.Range(1, 7);

        var currentPlayer = InGameManager.Instance.GetCurrentTurnPlayer();

        // 아이템 사용 여부 체크
        int useItemID = currentPlayer.PlayerData.UseGameItemID;
        if (useItemID != 0)
        {
            // 주사위 형태 아이템 결과값 받아오기
            resultNumber = InGameManager.Instance.ItemController.GetDiceItemResult(useItemID);

            // 아이템 사용 처리
            InGameManager.Instance.UseItem(currentPlayer.PlayerData.TurnIndex, useItemID, false);
        }

        return resultNumber;
    }

    private void PlayDiceAnim(int resultNumber)
    {
        _diceAnimator.Play("Idle", -1, 0f);
        // _diceAnimator.SetTrigger("Dice_Roll");

        var animKeyNumber = Mathf.Abs(resultNumber);

        var animKeyString = $"Dice_{animKeyNumber}";

        _diceAnimator.Play(animKeyString, -1, 0f);
        _diceAnimator.SetTrigger(animKeyString);
    }
}
