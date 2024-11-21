using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : SingletonMonoBehaviour<LobbyUI>
{
    [SerializeField] private Transform _popupTransform;

    [Header("User Profile")]
    [SerializeField] private Image _userCharacterImage;
    [SerializeField] private TextMeshProUGUI _userNameText;

    [Header("User Game Info")]
    [SerializeField] private TextMeshProUGUI _userRankPointText;

    [Header("User Character")]
    [SerializeField] private CharacterCardSlot _characterCardSlot;

    private List<SpecCharacter> _allCharacterList = new();
    private SpecCharacter _selectedCharacterData;

    private int _selectedCharacterListIndex = 0;

    private GameModeType _currentGameMode = GameModeType.None;

    private bool _gameStartButtonFlag = false;

    public int SelectedStageID { get; set; } = 1;

    private void OnEnable()
    {
        PopupManager.CloseAllPopup();

        PopupManager.Instance.popupLayerTr = _popupTransform;

        Clear();

        SoundManager.Instance.PlayBGM("bgm_lobby");
    }

    private void Start()
    {
        Init();

        SetUserProfile();
        SetUserCharacter();
        SetUserGameInfo();
    }

    public void OnClickSettingButton()
    {
        PopupManager.OpenPopup<SettingPopup>();
    }

    public void OnClickAIMatchStartButton()
    {
        if (_gameStartButtonFlag) return;

        _gameStartButtonFlag = true;

        _currentGameMode = GameModeType.Normal_AI;

        GameStart();
    }

    public void OnClickUserMatchStartButton()
    {
        if (_gameStartButtonFlag) return;

        _gameStartButtonFlag = true;

        _currentGameMode = GameModeType.Normal_User;

        GameStart();
    }

    public void OnClickCharacterCardFlipButton()
    {
        _characterCardSlot.SetFlipAnimation(!_characterCardSlot.IsSlotFlip);
    }

    public void OnClickCharacterCardLeftButton()
    {
        _selectedCharacterListIndex--;

        if (_selectedCharacterListIndex < 0)
        {
            _selectedCharacterListIndex = _allCharacterList.Count - 1;
        }

        _selectedCharacterData = _allCharacterList[_selectedCharacterListIndex];

        SetUserCharacter();
    }

    public void OnClickCharacterCardRightButton()
    {
        _selectedCharacterListIndex++;

        if (_selectedCharacterListIndex >= _allCharacterList.Count)
        {
            _selectedCharacterListIndex = 0;
        }

        _selectedCharacterData = _allCharacterList[_selectedCharacterListIndex];

        SetUserCharacter();
    }

    private void Init()
    {
        _allCharacterList = SpecDataManager.Instance.SpecCharacterList;

        _selectedCharacterData = SpecDataManager.Instance.GetCharacterData(UserDataManager.Instance.UserData.MainCharacterID);
        _selectedCharacterListIndex = _allCharacterList.FindIndex(0, data => data.character_id == _selectedCharacterData.character_id);
    }

    private void SetUserProfile()
    {
        _userNameText.text = UserDataManager.Instance.UserData.PlayerName;
    }

    private void SetUserCharacter()
    {
        if (_selectedCharacterData == null) return;

        _characterCardSlot.SetSlot(_selectedCharacterData);

        if (_characterCardSlot.IsSlotFlip)
        {
            _characterCardSlot.SetFlipAnimation(false);
        }
    }

    private void SetUserGameInfo()
    {
        _userRankPointText.text = UserDataManager.Instance.UserData.RankPoint.ToString();
    }

    private void GameStart()
    {
        // 메인 캐릭터 설정
        UserDataManager.Instance.SetMainCharacter(_selectedCharacterData.character_id);

        SceneManager.LoadSceneAsync("Play");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬 로드 완료: " + scene.name);

        _gameStartButtonFlag = false;

        // 여기서 콜백 로직 실행
        InGameManager.Instance.InitGame(_currentGameMode, SelectedStageID, 2);

        // 콜백 실행 후, 이벤트에서 제거 (한번만 호출되도록)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Clear()
    {
        _gameStartButtonFlag = false;
    }
}
