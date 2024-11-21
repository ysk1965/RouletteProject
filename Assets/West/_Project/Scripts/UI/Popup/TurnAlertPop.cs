using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnAlertPop : Popup
{
    [SerializeField] float normalWinPopupDuration = 1.5f; //
    [SerializeField] float perfectWinPopupDuration = 2.5f; //

    [Space]
    [SerializeField] private GameObject _drawLayerObject;
    [SerializeField] private GameObject _normalWinLayerObject;
    [SerializeField] private TextMeshProUGUI _turnAlertText;

    private bool _isLockOpenResultPop = true;

    private float _targetPopupDuration;

    public override void Init()
    {
        base.Init();

        Clear();

        //SetTurnAlertPop();
        SetWinnerAlertPop();

        Invoke(nameof(UnlockOpenResultPop), _targetPopupDuration);
        //Invoke(nameof(ClosePopup), popupDuration);
    }

    public void OnClickOpenResultPop()
    {
        if (_isLockOpenResultPop) return;

        PopupManager.OpenPopup<ResultPop>();
    }

    private void SetTurnAlertPop()
    {
        var currentUserData = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData;

        _turnAlertText.text = $"{currentUserData.PlayerName}'s Turn!";
    }

    // 임시 - 우승 연출로 사용
    private void SetWinnerAlertPop()
    {
        var resultData = InGameManager.Instance.CurrentGameResult;

        if (resultData.ResultType == GameResultType.Draw)
        {
            _turnAlertText.text = $"Draw Game!";
            _targetPopupDuration = normalWinPopupDuration;
            _drawLayerObject.SetActive(true);
        }
        else
        {
            _turnAlertText.text = $"{resultData.CurrentWinnerPlayer.PlayerData.PlayerName}'s Win!";
            _targetPopupDuration = resultData.IsPerfectWin ? perfectWinPopupDuration : normalWinPopupDuration;
            _normalWinLayerObject.SetActive(resultData.IsPerfectWin == false);
        }
    }

    private void UnlockOpenResultPop()
    {
        _isLockOpenResultPop = false;
    }

    private void ClosePopup()
    {
        PopupManager.ClosePopup<TurnAlertPop>();
    }

    private void Clear()
    {
        _isLockOpenResultPop = true;

        _drawLayerObject.SetActive(false);
        _normalWinLayerObject.SetActive(false);

        _targetPopupDuration = normalWinPopupDuration;
    }
}
