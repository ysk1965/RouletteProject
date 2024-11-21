using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstPlayerPickPop : Popup
{
    [Header("Common")]
    [SerializeField] private Animator _popupAnimator;
    [SerializeField] private CanvasGroup _popupCanvasGroup;

    [Header("Player 1")]
    [SerializeField] private Image _player1ProfileImage;
    [SerializeField] private TextMeshProUGUI _player1NameText;
    [SerializeField] private GameObject _player1FirstTurnObject;
    [SerializeField] private GameObject _player1SecondTurnObject;

    private PlayerController _player1Controller;

    [Header("Player 2")]
    [SerializeField] private Image _player2ProfileImage;
    [SerializeField] private TextMeshProUGUI _player2NameText;
    [SerializeField] private GameObject _player2FirstTurnObject;
    [SerializeField] private GameObject _player2SecondTurnObject;

    [Header("Effect")]
    [SerializeField] private float _popupDuration = 2.0f;
    [SerializeField] private float _popupFadeDuration = 0.75f;

    private PlayerController _player2Controller;

    public override void Init()
    {
        base.Init();

        Run.After(0.2f, () =>
        {
            SoundManager.Instance.PlaySFX("sfx_draw_for_turn");
        });

        _player1Controller = InGameManager.Instance.GetPlayer(0);
        _player2Controller = InGameManager.Instance.GetPlayer(1);
    }

    public void StartFirstPlayerPick(int firstPlayerTurnIndex)
    {
        // 플레이어1 처리
        bool isPlayer1FirstTurn = _player1Controller.PlayerData.TurnIndex == firstPlayerTurnIndex;
        _player1NameText.text = _player1Controller.PlayerData.PlayerName;
        _player1FirstTurnObject.SetActive(isPlayer1FirstTurn);
        _player1SecondTurnObject.SetActive(!isPlayer1FirstTurn);

        // 플레이어2 처리
        bool isPlayer2FirstTurn = _player2Controller.PlayerData.TurnIndex == firstPlayerTurnIndex;
        _player2NameText.text = _player2Controller.PlayerData.PlayerName;
        _player2FirstTurnObject.SetActive(isPlayer2FirstTurn);
        _player2SecondTurnObject.SetActive(!isPlayer2FirstTurn);

        Invoke(nameof(ClosePopup), _popupDuration);
    }

    public void ClosePopup()
    {
        DOTween.Sequence()
            .Append(_popupCanvasGroup.DOFade(0f, _popupFadeDuration)).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                InGameUI.Instance.CameraController.IsLockDrag = false;
                InGameUI.Instance.UpdatePlayerTurn();

                InGameManager.Instance.CurrentGamePlayState = GamePlayStateType.ReadyToRoll;

                InGameManager.Instance.SwitchPlayerTurnLimitTimeCount(true);
                // 주사위 알림 연출
                InGameManager.Instance.DiceController.OnOffRollDiceAlert(true);

                PopupManager.ClosePopup<FirstPlayerPickPop>();
            });
    }
}
