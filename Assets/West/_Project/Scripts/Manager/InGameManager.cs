using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
using UnityEngine;

public class GameResultData
{
    public PlayerController CurrentWinnerPlayer;
    public bool IsPerfectWin;
    public GameResultType ResultType = GameResultType.None;
}

// 인게임 내 전체적인 게임 시스템을 관리하는 매니저 (모드 공용)
public class InGameManager : SingletonMonoBehaviour<InGameManager>
{
    [SerializeField] private Transform _stageParentTransform;

    [SerializeField] private DiceController _diceController;
    [SerializeField] private ItemController _itemController;
    [SerializeField] private RuleController _ruleController;

    [Header("Game Effect Control")]
    [SerializeField] private float _characterMoveBaseDelayTime = 1.5f;

    [SerializeField] private float _extraMoveDelayTime = 2.0f;
    private bool _isShowTurnCountObject;

    // 시간제한 관련
    private float _playerTurnLimitTime;

    private SpecStage _specStageData;

    public GameModeType CurrentGameMode { get; private set; } = GameModeType.Normal_AI;
    public int CurrentTurnIndex { get; private set; } // 현재 턴 인덱스
    public int GameTurnCount { get; set; } = 1; // 현재 게임 턴 수 (모든 플레이어가 1회씩 플레이하면 증가)
    public int GameEndTurnCount { get; set; } // 게임 종료 턴 수
    public int CurrentPlayerCount { get; private set; } // 플레이 유저 수
    public GamePlayStateType CurrentGamePlayState { get; set; } = GamePlayStateType.None; // 현재 게임 진행 상태
    public GameResultData CurrentGameResult { get; private set; } // 현재 게임 결과 데이터

    public List<PlayerController> GamePlayerList { get; } = new();

    public StageController StageController { get; private set; }

    public DiceController DiceController => _diceController;
    public ItemController ItemController => _itemController;
    public RuleController RuleController => _ruleController;

    public bool IsEndGameTurn => GameEndTurnCount < GameTurnCount;

    // 추가로 확실하게 초기화해야하는 부분 Awake에서 처리
    private void Awake()
    {
        Clear();
    }

    protected override void OnDestroy()
    {
        // IngameManager는 오버라이드하여 파괴시키지 않음 처리
    }

    public PlayerController GetCurrentTurnPlayer()
    {
        return GamePlayerList[CurrentTurnIndex];
    }

    public PlayerController GetPlayer(int turnIndex)
    {
        return GamePlayerList[turnIndex];
    }

    public PlayerController GetOpponentPlayer(int turnIndex)
    {
        return GamePlayerList[turnIndex == 0 ? 1 : 0];
    }

    // 모든 플레이어의 액션이 끝났는지 체크
    public bool IsAllPlayerActionEnd()
    {
        return !GamePlayerList.Exists(player => player.CheckAllActionEnd == false);
    }

    // 새로운 게임을 초기화
    public void InitGame(GameModeType type, int stageID, int playerCount)
    {
        Clear();

        _specStageData = SpecDataManager.Instance.GetStageData(stageID);

        LoadStage();

        CurrentGameMode = type;
        CurrentPlayerCount = playerCount;

        GameEndTurnCount = SpecDataManager.Instance.GetGameConfig<int>("END_GAME_TURN_COUNT");

        _playerTurnLimitTime = SpecDataManager.Instance.GetGameConfig<float>("PLAYER_TURN_LIMIT_TIME");

        switch (CurrentGameMode)
        {
            case GameModeType.Normal_AI:
                SetNormalModePlayerData();
                break;
            case GameModeType.Normal_User:
                SetNormalModePlayerData();
                break;
        }

        /*** 게임 시작! ***/

        // 게임 시작 연출 재생
        PlayGameStartEffect();

        // 시간 제한 카운트 시작
        SwitchPlayerTurnLimitTimeCount(false);
        //SwitchPlayerTurnLimitTimeCount(true); // 선후공 팝업으로 이동

        //PopupManager.OpenPopup<TurnAlertPop>();

        //CurrentGamePlayState = GamePlayStateType.ReadyToRoll;

        // UI 갱신
        InGameUI.Instance.Init();

        // 컨트롤러 초기화
        _ruleController.Init(); // 해금 룰 초기화 및 세팅

        // 캐릭터 소팅 그룹 변경
        GetCurrentTurnPlayer().SetCharacterSortingOrder(1);

        // 컨트롤러 갱신
        Refresh(InGameRefreshType.GameStart);

        // bgm 재생
        SoundManager.Instance.PlayBGM("bgm_ingame");
    }

    public void Refresh(InGameRefreshType refreshType)
    {
        switch (refreshType)
        {
            case InGameRefreshType.All:
            case InGameRefreshType.TurnChange:
                _diceController.Refresh();
                _itemController.Refresh();
                StageController.Refresh();
                _ruleController.Refresh();

                InGameUI.Instance.Refresh(false);
                break;
            case InGameRefreshType.GameStart:
                _diceController.Refresh();
                _itemController.Refresh();
                StageController.Refresh();
                _ruleController.Refresh();

                InGameUI.Instance.Refresh(true);
                break;
            case InGameRefreshType.AdjustDice:
                _diceController.Refresh();
                _itemController.Refresh();
                StageController.Refresh();
                _ruleController.Refresh();
                break;
            case InGameRefreshType.AdjustPerk:
                _diceController.Refresh();
                _itemController.Refresh();
                StageController.Refresh();
                _ruleController.Refresh();
                break;
            case InGameRefreshType.RefreshItem:
                _itemController.Refresh();
                break;
            case InGameRefreshType.RefreshCamera:
                InGameUI.Instance.UpdateCamera();
                break;
            case InGameRefreshType.RefreshEquip:
                InGameUI.Instance.UpdateItemEquipSlot();
                break;
        }
    }

    public void RequestNextTurn()
    {
        StopCoroutine(nameof(ProcessNextTurn));
        StartCoroutine(nameof(ProcessNextTurn));
    }

    // 다음 턴으로 넘어갈 때 호출
    public void NextTurn()
    {
        // 게임 종료 컨디션 업데이트 (keep - 주사위 카운트 방식)
        // UpdateGameEndCondition();
        //
        // if (CurrentGameResult.ResultType != GameResultType.None)
        // {
        //     EndGame();
        //     return;
        // }

        // 턴 인덱스 증가
        UpdateNextPlayer();

        // 남은 주사위 갯수가 있는지 체크 - 없으면 다음 턴 (keep - 주사위 카운트 방식)
        // if (GetCurrentTurnPlayer().PlayerData.DiceCount <= 0)
        // {
        //     UpdateNextPlayer();
        // }

        // 게임 종료 컨디션 업데이트
        UpdateGameEndCondition();

        if (CurrentGameResult.ResultType != GameResultType.None)
        {
            EndGame();
            return;
        }

        // 다음 턴 시작!

        // 현재 1등 플레이어 처리
        UpdateWinnerPlayer();

        // 현재 플레이어 퍽 관련 데이터 업데이트
        GetCurrentTurnPlayer().PerkController.ResetPerkData();
        GetCurrentTurnPlayer().PerkController.UpdatePerkRemainTurn();

        // 시간제한 카운트 시작
        _isShowTurnCountObject = false;
        SwitchPlayerTurnLimitTimeCount(false);
        SwitchPlayerTurnLimitTimeCount(true);

        // 주사위 알림 연출
        DiceController.OnOffRollDiceAlert(true);

        // 캐릭터 소팅 그룹 변경
        GetCurrentTurnPlayer().SetCharacterSortingOrder(1);

        //PopupManager.OpenPopup<TurnAlertPop>();

        CurrentGamePlayState = GamePlayStateType.ReadyToRoll;

        bool checkComputerPlayer = CurrentGameMode == GameModeType.Normal_AI &&
                                   GetCurrentTurnPlayer().PlayerData.IsComputerPlayer;
        if (checkComputerPlayer)
        {
            Run.After(0.75f, () =>
            {
                PlayComputerPlayerTurn();
            });
        }

        // UI 갱신
        InGameUI.Instance.Refresh(false);
        if (CurrentGameMode == GameModeType.Normal_AI)
        {
            InGameUI.Instance.OnOffWaitTurnBlock(checkComputerPlayer);
        }

        // 컨트롤러 갱신
        Refresh(InGameRefreshType.TurnChange);
    }

    // 주사위 결과를 적용
    public void AdjustDiceResult(int diceValue)
    {
        if (GetCurrentTurnPlayer() == null)
        {
            return;
        }

        // (keep - 주사위 카운트 방식)
        //GetCurrentTurnPlayer().PlayerData.DiceCount--;

        // 시간제한 카운트 중지
        if (_isShowTurnCountObject)
        {
            InGameUI.Instance.OnOffTurnTimeCounter(false);
        }

        SwitchPlayerTurnLimitTimeCount(false);

        // 주사위 알림 연출
        DiceController.OnOffRollDiceAlert(false);

        CurrentGamePlayState = GamePlayStateType.PlayMoveAction;

        // 최종 목적지
        int destinationBlockIndex = GetCurrentTurnPlayer().PlayerData.CurrentBlockIndex + diceValue;

        // 기본 이동 딜레이 설정
        float moveDelayTime = _characterMoveBaseDelayTime;

        // 이동 전 필요 연출 (퍽)
        bool checkLandingFail = destinationBlockIndex > StageController.EndBlock.BlockData.BlockIndex;
        bool checkShowPerkEffect = GetCurrentTurnPlayer().PlayerData.PerkData.IsShowPerkEffect();
        bool isPrisonState = GetCurrentTurnPlayer().PlayerData.IsPrisonState;
        if (checkLandingFail == false && checkShowPerkEffect && !isPrisonState) // 퍼펙트 랜딩 실패가 아닐 경우, 감옥 상태가 아닐경우
        {
            if (diceValue == 6)
            {
                moveDelayTime += _extraMoveDelayTime;

                Run.After(_characterMoveBaseDelayTime, () =>
                {
                    GetCurrentTurnPlayer().AnimController.SetAnimeState(PlayerAnimState.Six);
                });

                Run.After(_characterMoveBaseDelayTime, () =>
                {
                    InGameUI.Instance.PlayPerkEffect(GetCurrentTurnPlayer().PlayerData.PerkData.GetSpecPerkData());

                    SoundManager.Instance.PlaySFX("sfx_perk_open");

                    GlobalVibration.Instance.Vibrate(0.5f);
                });
            }
        }

        // 실제 이동 시작
        Run.After(moveDelayTime, () =>
        {
            // 최종 목적지 표시
            StageController.SwitchDestinationPin(destinationBlockIndex, true);

            GetCurrentTurnPlayer().MoveToBlock(diceValue, true);
        });

        // 컨트롤러 갱신
        Refresh(InGameRefreshType.AdjustDice);
        Refresh(InGameRefreshType.RefreshCamera);
    }

    // 퍽 효과를 적용
    public void AdjustPerkResult()
    {
        if (GetCurrentTurnPlayer() == null)
        {
            return;
        }

        SpecPerk currentSpecPerkData = GetCurrentTurnPlayer().PlayerData.PerkData.GetSpecPerkData();
        switch (currentSpecPerkData.perk_type)
        {
            case PerkType.Reroll:
                CurrentGamePlayState = GamePlayStateType.ReadyToRoll;

                // 시간제한 카운트 시작
                SwitchPlayerTurnLimitTimeCount(false);
                SwitchPlayerTurnLimitTimeCount(true);

                // 주사위 알림 연출
                DiceController.OnOffRollDiceAlert(true);

                bool checkComputerPlayer = CurrentGameMode == GameModeType.Normal_AI &&
                                           GetCurrentTurnPlayer().PlayerData.IsComputerPlayer;
                if (checkComputerPlayer)
                {
                    Run.After(0.75f, () =>
                    {
                        PlayComputerPlayerTurn();
                    });
                }

                break;
            case PerkType.SnakeShield:
                break;
        }

        // 컨트롤러 갱신
        Refresh(InGameRefreshType.AdjustPerk);
    }

    // 타겟 유저 강제 이동 관련 처리
    public void AdjustForceMove(int moveValue)
    {
        PlayerController opponent = GetCurrentTurnPlayer().PlayerData.TurnIndex == 0 ? GetPlayer(1) : GetPlayer(0);

        opponent.ForceMoveToBlock(moveValue);
    }

    // 아이템 사용 처리 (소지형 아이템)
    public void UseItem(int playerTurnIndex, int gameItemID, bool isForceUse)
    {
        if (gameItemID <= 0)
        {
            return;
        }

        PlayerController targetPlayer = GetPlayer(playerTurnIndex);

        // 아이템 사용 처리
        targetPlayer.PlayerData.UseItem(gameItemID, isForceUse);

        // 아이템 사용 연출 적용
        targetPlayer.AnimController.PlayGameItemAction(gameItemID);

        Refresh(InGameRefreshType.RefreshItem);
        Refresh(InGameRefreshType.RefreshEquip);
    }

    // 승자 즉시 데이터 세팅 (종료 조건 바로 달성 시 사용)
    public void SetWinnerPlayer(int winnerPlayerTurnIndex)
    {
        CurrentGameResult.ResultType = GameResultType.Win;
        CurrentGameResult.CurrentWinnerPlayer = GamePlayerList[winnerPlayerTurnIndex];
        CurrentGameResult.IsPerfectWin = true;
    }

    // 시간 타이머 제어
    public void SwitchPlayerTurnLimitTimeCount(bool isStart)
    {
        if (isStart)
        {
            StartCoroutine(nameof(StartPlayerTurnLimitTimeCount));
        }
        else
        {
            StopCoroutine(nameof(StartPlayerTurnLimitTimeCount));
        }
    }

    // UI 관련 정리
    public void ClearUI()
    {
        ItemController.ClearUI();
    }

    // 게임 종료 시 호출
    public void EndGame()
    {
        //Clear();

        SwitchPlayerTurnLimitTimeCount(false);

        CurrentGamePlayState = GamePlayStateType.GameEnd;

        // test
        if (CurrentGameResult.ResultType == GameResultType.Draw)
        {
            Debug.Log("Draw Game!!");
        }
        else
        {
            Debug.Log("Winner is ===> " + CurrentGameResult.CurrentWinnerPlayer.PlayerData.PlayerName);
        }

        // 왕관 상태 업데이트
        UpdateWinnerPlayer();

        // 게임 종료 인게임 연출
        PopupManager.OpenPopup<TurnAlertPop>(); // 턴 알림 팝업을 승리 연출 팝업으로 임시 사용
        if (CurrentGameResult.ResultType != GameResultType.Draw)
        {
            PlayerController winnerPlayer = CurrentGameResult.CurrentWinnerPlayer;

            // 우승 플레이어 캐릭터 연출
            winnerPlayer.AnimController.ForceOffHammerObejct();
            winnerPlayer.AnimController.SetAnimeState(PlayerAnimState.Victory);

            // 우승 플레이어 카메라 연출
            InGameUI.Instance.CameraController.SetWinnerZoomState(winnerPlayer.gameObject.transform, CurrentGameResult.IsPerfectWin);

            // 폭죽 연출 (퍼펙트 승리 시)
            if (CurrentGameResult.IsPerfectWin)
            {
                StageController.EndBlock.PlayFireworksAnimation();
            }

            // 게임 종료 관련 데이터 처리
            if (CurrentGameResult.CurrentWinnerPlayer.PlayerData.PlayerID == UserDataManager.Instance.UserData.PlayerID)
            {
                if (CurrentGameResult.IsPerfectWin)
                {
                    var rankPoint = SpecDataManager.Instance.GetGameConfig<int>("PERFECT_WIN_RANK_POINT");
                    UserDataManager.Instance.IncreaseRankPoint(rankPoint, true);
                }
                else
                {
                    var rankPoint = SpecDataManager.Instance.GetGameConfig<int>("NORMAL_WIN_RANK_POINT");
                    UserDataManager.Instance.IncreaseRankPoint(rankPoint, true);
                }
            }
        }

        // 게임 종료 팝업 UI 생성
        // Run.After(2.5f, () =>
        // {
        //     PopupManager.OpenPopup<ResultPop>();
        // });

        // 관련 데이터 처리
        UserDataManager.Instance.InceraseTutorialPlayCount(true);
    }

    // 스테이지 로드 및 생성
    private void LoadStage()
    {
        string stageResourcePath = Define.GetFilePath(FilePath.StagePath) + $"Stage_{_specStageData.stage_id}";

        GameObject newStageObject = ResourceManager.Instantiate(stageResourcePath, _stageParentTransform);
        StageController = newStageObject.GetComponent<StageController>();

        StageController.Init();
    }

    // 플레이더 데이터 세팅
    private void SetNormalModePlayerData()
    {
        string characterResourcePath = Define.GetFilePath(FilePath.CharacterPath);

        for (var i = 0; i < CurrentPlayerCount; i++)
        {
            // 첫번째는 유저 세팅
            if (i == 0)
            {
                var newUserData = GamePlayerData.CreateUserPlayerData();
                newUserData.SetPlayerTurnIndex(i);

                string resultUserCharacterPath = characterResourcePath + $"Character_{newUserData.CharacterID}";
                GameObject newChracterObject = ResourceManager.Instantiate(resultUserCharacterPath, StageController.CharacterParentTransform);
                var newCharacter = newChracterObject.GetComponent<PlayerController>();
                newCharacter.InitPlayer(newUserData);

                GamePlayerList.Add(newCharacter);

                continue;
            }

            GamePlayerData newPlayerData = null;
            if (CurrentGameMode == GameModeType.Normal_AI)
            {
                newPlayerData = GamePlayerData.CreateComputerPlayerData();
            }
            else
            {
                newPlayerData = GamePlayerData.CreateLocalUserPlayerData();
            }

            newPlayerData.SetPlayerTurnIndex(i);

            string resultComputerCharacterPath = characterResourcePath + $"Character_{newPlayerData.CharacterID}";
            GameObject newComputerObject =
                ResourceManager.Instantiate(resultComputerCharacterPath, StageController.CharacterParentTransform);
            var newComputer = newComputerObject.GetComponent<PlayerController>();
            newComputer.InitPlayer(newPlayerData);

            GamePlayerList.Add(newComputer);
        }
    }

    // 게임의 종료 컨디션 업데이트 (keep - 주사위 카운트 방식)
    // private void UpdateGameEndCondition()
    // {
    //     // 모든 플레이어의 주사위가 0개일 경우 게임 종료
    //     int emptyDicePlayerCount = _gamePlayerList.Count(player => player.PlayerData.DiceCount <= 0);
    //     bool isAllPlayerDiceEmpty = emptyDicePlayerCount == CurrentPlayerCount;
    //
    //     if (isAllPlayerDiceEmpty)
    //     {
    //         int highScore = _gamePlayerList.Max(player => player.PlayerData.CurrentBlockIndex);
    //         int highScorePlayerCount = _gamePlayerList.Count(player => player.PlayerData.CurrentBlockIndex == highScore);
    //
    //         if (highScorePlayerCount == 1) // 승자 1명
    //         {
    //             int winnerIndex = _gamePlayerList.FindIndex(player => player.PlayerData.CurrentBlockIndex == highScore);
    //
    //             CurrentGameResult.ResultType = GameResultType.Win;
    //             CurrentGameResult.CurrentWinnerPlayer = _gamePlayerList[winnerIndex];
    //         }
    //         else if (highScorePlayerCount > 1) // 승자 1명 이상 (무승부)
    //         {
    //             CurrentGameResult.ResultType = GameResultType.Draw;
    //             CurrentGameResult.CurrentWinnerPlayer = null;
    //         }
    //     }
    // }

    // 게임의 종료 컨디션 업데이트 (턴 카운트 방식)
    private void UpdateGameEndCondition()
    {
        if (IsEndGameTurn)
        {
            int highScore = GamePlayerList.Max(player => player.PlayerData.CurrentBlockIndex);
            int highScorePlayerCount = GamePlayerList.Count(player => player.PlayerData.CurrentBlockIndex == highScore);

            if (highScorePlayerCount == 1) // 승자 1명
            {
                int winnerIndex = GamePlayerList.FindIndex(player => player.PlayerData.CurrentBlockIndex == highScore);

                CurrentGameResult.ResultType = GameResultType.Win;
                CurrentGameResult.CurrentWinnerPlayer = GamePlayerList[winnerIndex];
                CurrentGameResult.IsPerfectWin = false;
            }
            else if (highScorePlayerCount > 1) // 승자 1명 이상 (무승부)
            {
                CurrentGameResult.ResultType = GameResultType.Draw;
                CurrentGameResult.CurrentWinnerPlayer = null;
                CurrentGameResult.IsPerfectWin = false;
            }
        }
    }

    // 현재 1등 플레이어 업데이트
    private void UpdateWinnerPlayer()
    {
        int highScore = GamePlayerList.Max(player => player.PlayerData.CurrentBlockIndex);
        int highScorePlayerCount = GamePlayerList.Count(player => player.PlayerData.CurrentBlockIndex == highScore);

        if (highScorePlayerCount == 1)
        {
            GamePlayerList.ForEach(player =>
            {
                bool isHighScorePlayer = player.PlayerData.CurrentBlockIndex == highScore;
                player.PlayerNameController.OnOffPlayerCrown(isHighScorePlayer);
            });
        }
        else if (highScorePlayerCount > 1)
        {
            GamePlayerList.ForEach(player => player.PlayerNameController.OnOffPlayerCrown(false));
        }
    }

    // 다음 플레이어 데이터 업데이트
    private void UpdateNextPlayer()
    {
        CurrentTurnIndex++;
        if (CurrentTurnIndex >= CurrentPlayerCount)
        {
            GameTurnCount++;
            CurrentTurnIndex = 0;

            // 마지막턴 알림
            if (GameTurnCount == GameEndTurnCount)
            {
                PopupManager.OpenPopup<LastTurnPop>();
            }

            Debug.Log($"***** Game Turn Count : {GameTurnCount} *****");
        }
    }

    private void PlayComputerPlayerTurn()
    {
        bool isItemUsed = ItemController.AutoUseItem();

        if (isItemUsed)
        {
            Run.After(1.0f, () =>
            {
                _diceController.AutoRollDice();
            });
        }
        else
        {
            _diceController.AutoRollDice();
        }
    }

    private void PlayGameStartEffect()
    {
        CurrentGamePlayState = GamePlayStateType.GameStart;

        DiceController.OnOffRollDiceAlert(false);

        // 등장 연출 카메라 설정
        InGameUI.Instance.CameraController.SetFirstInitZoomSpeed(true);
        InGameUI.Instance.CameraController.SetFirstInitFollowSpeed(true);
        InGameUI.Instance.CameraController.IsLockDrag = true;

        Run.After(1.5f, () =>
        {
            InGameUI.Instance.UpdateCamera();
        });

        // 캐릭터 off
        GamePlayerList.ForEach(player => player.gameObject.SetActive(false));

        // 캐릭터 on
        Run.After(4f, () =>
        {
            GamePlayerList[0].gameObject.SetActive(true);
        });
        Run.After(4.5f, () =>
        {
            GamePlayerList[1].gameObject.SetActive(true);
        });

        // 플레이어 선후공 정하기 연출 (팝업 내부에서 게임 스타트 관련 처리)
        Run.After(5.5f, () =>
        {
            InGameUI.Instance.CameraController.SetFirstInitZoomSpeed(false);
            InGameUI.Instance.CameraController.SetFirstInitFollowSpeed(false);

            // todo.. 추후 선후공 관련 정식 처리 필요 (현재는 무조건 0번 유저가 선 플레이)
            CurrentGamePlayState = GamePlayStateType.SelectFirstPlayer;
            var pickPopup = PopupManager.OpenPopup<FirstPlayerPickPop>();
            pickPopup.StartFirstPlayerPick(0);
        });
    }

    private void Clear()
    {
        CurrentGameMode = GameModeType.None;
        GameTurnCount = 1;
        CurrentPlayerCount = 0;
        CurrentTurnIndex = 0;
        CurrentGamePlayState = GamePlayStateType.None;
        CurrentGameResult = new GameResultData();

        GamePlayerList.Clear();

        BMUtil.RemoveChildObjects(_stageParentTransform);
    }

    // 다음 턴 진행 프로세스
    private IEnumerator ProcessNextTurn()
    {
        yield return new WaitUntil(() => IsAllPlayerActionEnd());

        // 다음턴 시작 전 딜레이
        yield return new WaitForSeconds(0.5f);

        NextTurn();
    }

    // 플레이어 플레이 제한 시간 카운트 시작
    private IEnumerator StartPlayerTurnLimitTimeCount()
    {
        float currentTime = _playerTurnLimitTime;
        //float currentTime = 5000;

        while (currentTime > 0)
        {
            Debug.Log("Player Time Limit =====> " + currentTime);

            yield return new WaitForSeconds(1.0f);

            currentTime -= 1.0f;

            if (currentTime <= 5)
            {
                if (_isShowTurnCountObject == false)
                {
                    _isShowTurnCountObject = true;
                    InGameUI.Instance.OnOffTurnTimeCounter(true);
                }

                InGameUI.Instance.SetTurnTimeCount((int) currentTime - 1);

                SoundManager.Instance.PlaySFX("sfx_countdown_1");
            }
        }

        // 시간이 다 되었을 경우 -> 강제 주사위 굴리기
        Debug.Log("Player Time Limit Over!!");
        if (_isShowTurnCountObject)
        {
            _isShowTurnCountObject = false;
            InGameUI.Instance.OnOffTurnTimeCounter(false);
        }

        DiceController.AutoRollDice();
        SoundManager.Instance.PlaySFX("sfx_countdown_2");
    }

    #region Cheat

    public void CheatSetGameEndCondition()
    {
        int highScore = GamePlayerList.Max(player => player.PlayerData.CurrentBlockIndex);
        int highScorePlayerCount = GamePlayerList.Count(player => player.PlayerData.CurrentBlockIndex == highScore);

        if (highScorePlayerCount == 1) // 승자 1명
        {
            int winnerIndex = GamePlayerList.FindIndex(player => player.PlayerData.CurrentBlockIndex == highScore);

            CurrentGameResult.ResultType = GameResultType.Win;
            CurrentGameResult.CurrentWinnerPlayer = GamePlayerList[winnerIndex];
            CurrentGameResult.IsPerfectWin = false;
        }
        else if (highScorePlayerCount > 1) // 승자 1명 이상 (무승부)
        {
            CurrentGameResult.ResultType = GameResultType.Draw;
            CurrentGameResult.CurrentWinnerPlayer = null;
            CurrentGameResult.IsPerfectWin = false;
        }
    }

    #endregion
}
