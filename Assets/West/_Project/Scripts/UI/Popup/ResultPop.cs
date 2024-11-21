using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPop : Popup
{
    [SerializeField] private GameObject _crownImageObject; // 퍼펙트 우승시 사용
    [SerializeField] private Image _playerImage;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerNameShadowText;

    public override void Init()
    {
        base.Init();

        SetResultPop();
    }

    public void SetResultPop()
    {
        var resultData = InGameManager.Instance.CurrentGameResult;

        if (resultData.ResultType == GameResultType.Draw)
        {
            _playerNameText.text = "Draw";
            _playerNameShadowText.text = "Draw";
        }
        else
        {
            _playerImage.sprite = ImageManager.Instance.GetSocialProfileIcon(resultData.CurrentWinnerPlayer.PlayerData.PlayerID);
            _playerNameText.text = resultData.CurrentWinnerPlayer.PlayerData.PlayerName;
            _playerNameShadowText.text = resultData.CurrentWinnerPlayer.PlayerData.PlayerName;
        }

        _crownImageObject.SetActive(resultData.IsPerfectWin);

        SoundManager.Instance.PlaySFX("sfx_win");
    }

    public void OnClickGoHomeButton()
    {
        SceneManager.LoadSceneAsync("Lobby");

        //PopupManager.ClosePopup<ResultPop>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬 로드 완료: " + scene.name);

        // 여기서 콜백 로직 실행
        PopupManager.ClosePopup<ResultPop>();

        // 콜백 실행 후, 이벤트에서 제거 (한번만 호출되도록)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
