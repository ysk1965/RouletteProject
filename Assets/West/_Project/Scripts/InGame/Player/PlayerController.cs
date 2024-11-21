using System.Collections;
using BiniLab;
using CookApps.BM.MVPWest;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("Anim Control")]
    [SerializeField] private float _characterMoveSpeed = 3f;

    [SerializeField] private float _characterJumpStartDelay = 1.0f;
    [SerializeField] private float _characterJumpEndDelay = 0.15f;
    [SerializeField] private float _characterJumpDuration = 1.0f;
    [SerializeField] private float _characterBaseJumpPower = 0.75f;
    [SerializeField] private float _characterBaseJumpPowerValue = 0.05f;

    [Space]
    [SerializeField] private float _characterFallSpeed = 4.5f;

    [Header("Common")]
    [SerializeField] private GameObject _characterObject;

    [SerializeField] private GameObject _characterNameObject;
    [SerializeField] private SortingGroup _characterSortingGroup;

    private InGameBlock _endBlock;
    private InGameBlock _moveDestinationBlock;

    private int _goalBlockIndex; // 도달 목표 인덱스
    private bool _isFinishNormalMoveAction; // 한칸 한칸 개별 이동에 관련된 체크
    private bool _isFinishUserItemEvent; // 유저 아이템 이벤트 처리 체크
    private bool _isForceMove; // 강제 이동 여부

    private bool _isMoveBack; // 이동 방향
    private bool _isPerfectLandingMiss; // 퍼펙트 랜딩 미스 여부
    private bool _isPlayingGetItem; // 아이템 획득 액션 체크
    private bool _isPlayingLadderMove; // 사다리 액션 이동 체크

    private bool _isPlayingNormalMove; // 전체 기본 이동과 관련된 체크

    private bool _isPlayingSnakeMove; // 뱀 액션 이동 체크

    private Tween _moveJumpTween;

    public GamePlayerData PlayerData { get; private set; }

    public PlayerAnimController AnimController { get; private set; }

    public PlayerPerkController PerkController { get; private set; }

    public PlayerName PlayerNameController { get; private set; }

    public bool CheckAllActionEnd { get; set; } = true;

    public void InitPlayer(GamePlayerData playerData)
    {
        Clear();

        AnimController = GetComponent<PlayerAnimController>();
        PerkController = GetComponent<PlayerPerkController>();

        PlayerData = playerData;

        // 플레이어 이름 설정
        SetPlayerName();

        // 캐릭터 초기 위치 설정
        InGameBlock startBlock = InGameManager.Instance.StageController.StartBlock;
        transform.position = startBlock.CharacterPivotTransform.position;

        // 마지막 블럭 캐싱
        _endBlock = InGameManager.Instance.StageController.EndBlock;

        // 컨트롤러 초기화
        AnimController?.Init();
        PerkController?.Init();
    }

    // 캐릭터 소팅 그룹 설정
    public void SetCharacterSortingOrder(int order)
    {
        if (_characterSortingGroup == null)
        {
            _characterSortingGroup = GetComponent<SortingGroup>();
        }

        _characterSortingGroup.sortingOrder = order;
    }

    // 기본 블록 이동 (주사위나 아이템 등 일반적인 이동 시 사용)
    public void MoveToBlock(int moveValue, bool isDiceMove)
    {
        if (PlayerData == null)
        {
            return;
        }

        _isMoveBack = moveValue < 0;
        _isForceMove = false;

        CheckAllActionEnd = false;

        _goalBlockIndex = PlayerData.CurrentBlockIndex + moveValue;
        _goalBlockIndex = Mathf.Clamp(_goalBlockIndex, 0, _goalBlockIndex);

        _moveDestinationBlock = InGameManager.Instance.StageController.GetInGameBlock(_goalBlockIndex);

        // 애니메이션 초기화
        AnimController.SetAnimeState(PlayerAnimState.Idle);

        // 마지막 블럭 퍼펙트 랜딩 체크
        _isPerfectLandingMiss = _goalBlockIndex > _endBlock.BlockData.BlockIndex;
        if (_isPerfectLandingMiss)
        {
            InGameUI.Instance.PlayMissEffect(); // 미스 연출

            EndPlayerMove();
            return;
        }

        // 퍽 활성화 여부 체크
        if (isDiceMove && !PlayerData.IsPrisonState) // 주사위 이동일 경우 , 감옥 상태가 아닐 경우
        {
            PerkController.UpdatePerk(moveValue);
        }

        // 감옥 상태 체크
        if (PlayerData.IsPrisonState)
        {
            int resultMoveValue = Mathf.Abs(moveValue); // 음수 값 주사위도 허용
            if (isDiceMove && PlayerData.PrisonEscapeNumber == resultMoveValue)
            {
                EscapePrisionState();
                // 감옥 해제 연출 후 이동
                Run.After(1.0f, () =>
                {
                    StopCoroutine(StartMove());
                    StartCoroutine(StartMove());
                });
                return;
            }
            else
            {
                ReducePrisionEscapeCount();
                InGameUI.Instance.PlayMissEffect();
                EndPlayerMove();
                return;
            }
        }

        StopCoroutine(StartMove());
        StartCoroutine(StartMove());
    }

    // 강제 블록 이동 (백해머, 강제 이동 바꾸기 등 특수 이동 시 사용)
    public void ForceMoveToBlock(int moveValue)
    {
        if (PlayerData == null)
        {
            return;
        }

        _isMoveBack = moveValue < 0;
        _isForceMove = true;
        CheckAllActionEnd = false;

        _goalBlockIndex = PlayerData.CurrentBlockIndex + moveValue;
        _goalBlockIndex = Mathf.Clamp(_goalBlockIndex, 0, _goalBlockIndex);

        // 애니메이션 초기화
        AnimController.SetAnimeState(PlayerAnimState.Idle);

        StopCoroutine(ForceStartMove());
        StartCoroutine(ForceStartMove());
    }

    // 캐릭터의 바라보는 방향 처리
    public void CheckPlayerDirection()
    {
        // 마지막 블럭 체크
        if (IsEndBlock(PlayerData.CurrentBlockIndex))
        {
            return;
        }

        InGameBlock currentBlock = InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex);
        InGameBlock nextBlock = InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex + 1);

        if (currentBlock == null || nextBlock == null)
        {
            return;
        }

        bool isLeft = nextBlock.transform.position.x < currentBlock.transform.position.x;

        Vector3 currentScale = _characterObject.transform.localScale;
        if (isLeft)
        {
            float resultX = currentScale.x < 0 ? currentScale.x : -currentScale.x;
            resultX = _isMoveBack ? resultX * -1 : resultX; // 뒤로 이동 시 처리
            _characterObject.transform.localScale = new Vector3(resultX, currentScale.y, currentScale.z);
        }
        else
        {
            float resultX = currentScale.x < 0 ? -currentScale.x : currentScale.x;
            resultX = _isMoveBack ? resultX * -1 : resultX; // 뒤로 이동 시 처리
            _characterObject.transform.localScale = new Vector3(resultX, currentScale.y, currentScale.z);
        }
    }

    // 감옥 탈출 가능 데이터 차감
    public void ReducePrisionEscapeCount()
    {
        if (PlayerData.IsPrisonState)
        {
            PlayerData.PrisonEscapeCount--;

            AnimController.UpdatePrisonObject(PlayerData.PrisonEscapeCount);

            if (PlayerData.PrisonEscapeCount <= 0)
            {
                EscapePrisionState();
            }
        }
    }

    // 특수 상태 설정 (감옥)
    public void SendPrisonState()
    {
        int prisonNumber = Random.Range(1, 7);

        PlayerData.PrisonEscapeNumber = prisonNumber;
        PlayerData.PrisonEscapeCount = SpecDataManager.Instance.GetGameConfig<int>("RULE_PRISON_ESCAPE_COUNT");

        // 캐릭터 이름표 위치 조정
        Vector3 currentNamePos = _characterNameObject.transform.localPosition;
        var newNamePos = new Vector3(currentNamePos.x, currentNamePos.y + 2.5f, currentNamePos.z);
        _characterNameObject.transform.localPosition = newNamePos;

        AnimController.OnOffPrision(true, prisonNumber);

        AnimController.UpdatePrisonObject(PlayerData.PrisonEscapeCount);

        SoundManager.Instance.PlaySFX("sfx_prison_lock");

        Debug.Log("Send Prison!! ---> Escape Number : " + prisonNumber);
    }

    public void EscapePrisionState()
    {
        PlayerData.PrisonEscapeNumber = 0;
        PlayerData.PrisonEscapeCount = 0;

        // 캐릭터 이름표 원위치
        _characterNameObject.transform.localPosition = Vector3.zero;

        AnimController.OnOffPrision(false, 0);

        SoundManager.Instance.PlaySFX("sfx_prison_unlock");
    }

    // 도착한 블럭의 아이템 이벤트 체크 (해머 등)
    private void CheckItemEvent(int goalBlockIndex)
    {
        InGameBlock currentBlock = InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex);

        bool isLastBlock = currentBlock.BlockData.BlockIndex == goalBlockIndex; // 목표 이동 마지막 블럭

        if (isLastBlock)
        {
            if (PlayerData.IsActiveHammer)
            {
                PlayerController oppnentPlayer = InGameManager.Instance.GetOpponentPlayer(PlayerData.TurnIndex);

                bool isOpponentOnBlock = PlayerData.CurrentBlockIndex == oppnentPlayer.PlayerData.CurrentBlockIndex;
                bool isOpponentPrisonState = oppnentPlayer.PlayerData.IsPrisonState;

                if (isOpponentOnBlock && !isOpponentPrisonState)
                {
                    StopCoroutine(nameof(StartUseItemAction));
                    StartCoroutine(StartUseItemAction(GameItemType.Hammer));
                }
            }
            else
            {
                _isFinishUserItemEvent = true;
            }

            // 주사위 목적지에 도달하면 도착 pin 제거
            if (_moveDestinationBlock != null)
            {
                InGameManager.Instance.StageController.SwitchDestinationPin(_moveDestinationBlock.BlockData.BlockIndex, false);
            }
        }
        else
        {
            _isFinishUserItemEvent = true;
        }
    }

    // 도착한 블럭의 이벤트 체크 (첫 블럭, 이동 중 블럭, 마지막 블럭을 구분 or 뱀, 사다리 아이템 등)
    private void CheckBlockEvent()
    {
        InGameBlock currentBlock = InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex);

        bool isEndBlock = IsEndBlock(currentBlock.BlockData.BlockIndex); // 게임 종료 블럭
        bool isLastBlock = currentBlock.BlockData.BlockIndex == _goalBlockIndex; // 목표 이동 마지막 블럭

        if (isEndBlock)
        {
            EndPlayerMove();
        }
        else if (isLastBlock) // 마지막 블럭 이벤트 처리
        {
            bool isSnakeBlock = currentBlock.BlockData.SnakeStartID > 0;
            bool isLadderBlock = currentBlock.BlockData.LadderStartID > 0;

            // 특수 블럭 처리
            if (isSnakeBlock) // 뱀 블럭 처리
            {
                StopCoroutine(nameof(StartSnakeMoveBlock));

                // 뱀 쉴드 효과 체크
                if (PlayerData.PerkData.IsActiveTargetPerk(PerkType.SnakeShield))
                {
                    PlayerData.PerkData.PerkAmountValue = 0;

                    // 연출 관련 처리
                    InGameManager.Instance.StageController.PlaySnakeSwallowHeadAnim(currentBlock.BlockData
                        .SnakeStartID);
                    PerkController.PlaySnakeShieldBreakEffect();
                    PerkController.UpdateSnakeShieldObject();

                    EndPlayerMove();
                }
                // 사다리 무효화 룰 체크
                else if (InGameManager.Instance.RuleController.IsActivateRule(RuleType.Hide_Snake))
                {
                    EndPlayerMove();
                }
                else
                {
                    StartCoroutine(StartSnakeMoveBlock(currentBlock));
                }
            }
            else if (isLadderBlock) // 사다리 블럭 처리
            {
                StopCoroutine(nameof(StartLadderMoveBlock));

                // 사다리 무효화 룰 체크
                if (InGameManager.Instance.RuleController.IsActivateRule(RuleType.Hide_Ladder))
                {
                    EndPlayerMove();
                }
                else
                {
                    StartCoroutine(StartLadderMoveBlock(currentBlock));
                }
            }
            else // 아무 블럭도 없는 경우 별도 추가 처리
            {
                if (_isForceMove) // 기본 이동이 없는 강제 이동의 경우에는 해당 부분에서 이동 종료 처리
                {
                    EndPlayerMove();
                }
            }

            if (currentBlock.BlockData.IsGameItemBlock) // 아이템 처리
            {
                bool checkEndMove = !(isSnakeBlock || isLadderBlock); // 뱀, 사다리 블럭이 아닐 경우만 이동 종료 처리

                StopCoroutine(nameof(StartGetItemBlock));
                StartCoroutine(StartGetItemBlock(currentBlock, checkEndMove));
            }

            // 주사위 목적지에 도달하면 도착 pin 제거
            if (_moveDestinationBlock != null)
            {
                InGameManager.Instance.StageController.SwitchDestinationPin(_moveDestinationBlock.BlockData.BlockIndex, false);
            }
        } // 이동 중 블럭 이벤트 처리
    }

    // 게임 종료 컨디션 체크
    private bool CheckGameEndCondition()
    {
        bool isArriveEndBlock = IsEndBlock(PlayerData.CurrentBlockIndex);

        return isArriveEndBlock;
    }

    // 플레이어의 모든 행동이 종료된 시점에 호출
    private void EndPlayerMove()
    {
        AnimController.SetAnimeState(PlayerAnimState.Idle);

        _isPlayingNormalMove = false;
        StopCoroutine(nameof(StartMove));

        _isMoveBack = false;

        PlayerData.ClearUseItem();

        if (_moveDestinationBlock != null)
        {
            InGameManager.Instance.StageController.SwitchDestinationPin(_moveDestinationBlock.BlockData.BlockIndex, false);
        }

        CheckPlayerDirection();

        SetCharacterSortingOrder(0);

        // 게임 종료 우선 체크
        if (CheckGameEndCondition() && !_isForceMove)
        {
            InGameManager.Instance.SetWinnerPlayer(PlayerData.TurnIndex);
            InGameManager.Instance.RequestNextTurn();
            CheckAllActionEnd = true;
            return;
        }

        // 모든 행동 종료 후 퍽효과 처리
        if (PlayerData.PerkData.IsPerkReady &&
            PlayerData.PerkData.GetSpecPerkData().perk_active_type == PerkActiveType.AfterMove &&
            !_isPerfectLandingMiss)
        {
            PlayerData.PerkData.IsPerkReady = false;

            InGameManager.Instance.AdjustPerkResult();
        }
        else
        {
            if (!_isForceMove)
            {
                InGameManager.Instance.RequestNextTurn();
            }
        }

        CheckAllActionEnd = true;
    }

    private bool IsEndBlock(int targetBlockIndex)
    {
        return targetBlockIndex >= _endBlock.BlockData.BlockIndex;
    }

    private void SetPlayerName()
    {
        string namePath = Define.GetFilePath(FilePath.CharacterNamePath) + $"PlayerName_{PlayerData.TurnIndex + 1}P";
        GameObject newNameObject = ResourceManager.Instantiate(namePath, _characterNameObject.transform);
        PlayerNameController = newNameObject.GetComponent<PlayerName>();

        PlayerNameController.SetPlayerName(PlayerData.PlayerName);
    }

    private void OnOffCharacterNameObject(bool isOn)
    {
        _characterNameObject.SetActive(isOn);
    }

    private void Clear()
    {
        _goalBlockIndex = 0;

        _moveDestinationBlock = null;

        CheckAllActionEnd = true;

        BMUtil.RemoveChildObjects(_characterNameObject.transform);
    }

    // 캐릭터의 이동 시작 (일반적인 이동)
    private IEnumerator StartMove()
    {
        // 이동할 블럭이 없을 경우
        if (PlayerData.CurrentBlockIndex == _goalBlockIndex)
        {
            EndPlayerMove();
            yield break;
        }

        _isPlayingNormalMove = true;

        while (_isPlayingNormalMove)
        {
            if (_isMoveBack)
            {
                PlayerData.CurrentBlockIndex--;
            }
            else
            {
                PlayerData.CurrentBlockIndex++;
            }

            // 마지막 블럭 체크 (마지막 블럭에는 도달하고 종료되어야 하므로 별도 체크 처리)
            if (PlayerData.CurrentBlockIndex > _endBlock.BlockData.BlockIndex)
            {
                EndPlayerMove();
                yield break;
            }

            InGameBlock currentBlock =
                InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex);

            if (_isMoveBack)
            {
                CheckPlayerDirection();
            }

            StopCoroutine(nameof(StartNormalMoveBlock));
            StartCoroutine(nameof(StartNormalMoveBlock));

            yield return new WaitUntil(() => _isFinishNormalMoveAction);

            CheckItemEvent(_goalBlockIndex);

            yield return new WaitUntil(() => _isFinishUserItemEvent);

            CheckBlockEvent();

            //yield return new WaitForSeconds(0.5f);
            CheckPlayerDirection();

            // 마지막 블럭 체크 처리
            bool isLastBlock = PlayerData.CurrentBlockIndex == _goalBlockIndex;
            bool isSnakeBlock = currentBlock.BlockData.SnakeStartID > 0;
            bool isLadderBlock = currentBlock.BlockData.LadderStartID > 0;
            bool isItemBlock = currentBlock.BlockData.IsGameItemBlock;
            if (isLastBlock && !isSnakeBlock && !isLadderBlock && !isItemBlock) // 뱀, 사다리, 아이템 블럭은 해당 부분에서 직접 처리
            {
                _isPlayingNormalMove = false;
                EndPlayerMove();
            }
        }
    }

    private IEnumerator ForceStartMove()
    {
        _isPlayingNormalMove = true;

        while (_isPlayingNormalMove)
        {
            PlayerData.CurrentBlockIndex = _goalBlockIndex;

            InGameBlock goalBlock = InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex);

            Vector3 targetPos = goalBlock.CharacterPivotTransform.position;

            // 강제 이동 액션
            AnimController.SetAnimeState(PlayerAnimState.Fall);

            float distanceDelta = Mathf.Abs(targetPos.y - transform.position.y);
            float resultJumpPower = _characterBaseJumpPower + (distanceDelta * _characterBaseJumpPowerValue);

            var isJumpTweenComplete = false;
            DOTween.Sequence().Append(transform.DOJump(targetPos, resultJumpPower, 1, _characterJumpDuration))
                .AppendCallback(() =>
                {
                    AnimController.OnLandingEffect();
                }).OnComplete(() =>
                {
                    isJumpTweenComplete = true;
                });

            yield return new WaitUntil(() => isJumpTweenComplete);

            // ex - 직선 이동
            // while (Vector3.Distance(transform.position, goalBlock.CharacterPivotTransform.position) > 0.05f)
            // {
            //     transform.position = Vector3.MoveTowards(transform.position, goalBlock.CharacterPivotTransform.position, _characterFallSpeed * Time.deltaTime);
            //     yield return null;
            // }

            transform.position = targetPos; // 이동 후 위치보정

            AnimController.SetAnimeState(PlayerAnimState.Snake_Out);
            yield return new WaitUntil(() => AnimController.IsAnimationFinish(PlayerAnimState.Snake_Out));

            CheckBlockEvent();

            AnimController.OnOffHammerObject(PlayerData.IsActiveHammer);

            CheckPlayerDirection();

            _isPlayingNormalMove = false;

            yield break;
        }
    }

    // 기본적인 블럭 이동
    private IEnumerator StartNormalMoveBlock()
    {
        _isFinishNormalMoveAction = false;
        AnimController.SetAnimeState(PlayerAnimState.Walk);

        InGameBlock currentBlock = InGameManager.Instance.StageController.GetInGameBlock(PlayerData.CurrentBlockIndex);

        Vector3 targetPos = currentBlock.CharacterPivotTransform.position;

        if (Mathf.Abs(transform.position.y - targetPos.y) > 0.1f)
        {
            //_animController.SetAnimeState(PlayerAnimState.Idle);

            float distanceDelta = Mathf.Abs(targetPos.y - transform.position.y);
            float resultJumpPower = _characterBaseJumpPower + (distanceDelta * _characterBaseJumpPowerValue);

            var isJumpTweenComplete = false;

            AnimController.SetAnimeState(PlayerAnimState.Jump); // 애니메이션을 먼저 플레이

            DOTween.Sequence().PrependInterval(_characterJumpStartDelay)
                .Append(transform.DOJump(targetPos, resultJumpPower, 1, _characterJumpDuration))
                .JoinCallback(() =>
                {
                    SoundManager.Instance.PlaySFX("sfx_move_jump");
                })
                .AppendCallback(() =>
                {
                    AnimController.OnLandingEffect();
                    SoundManager.Instance.PlaySFX("sfx_move_land");
                    currentBlock.ProcessBlockAnimation(1.1f);
                }).AppendInterval(_characterJumpEndDelay)
                // .JoinCallback(() =>
                // {
                //     _animController.SetAnimeState(PlayerAnimState.Jump);
                // })
                .OnComplete(() =>
                {
                    isJumpTweenComplete = true;
                });

            yield return new WaitUntil(() => isJumpTweenComplete);

            AnimController.SetAnimeState(PlayerAnimState.Idle);
        }
        else
        {
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPos, _characterMoveSpeed * Time.deltaTime);

                yield return null;
            }

            currentBlock.ProcessBlockAnimation(0.3f);
        }

        transform.position = currentBlock.CharacterPivotTransform.position; // 이동 후 위치보정

        //CheckPlayerDirection();

        _isFinishNormalMoveAction = true;
    }

    // 캐릭터의 뱀 블럭 이동
    private IEnumerator StartSnakeMoveBlock(InGameBlock targetBlock)
    {
        _isPlayingNormalMove = false;
        _isPlayingSnakeMove = true;

        InGameBlock endBlock =
            InGameManager.Instance.StageController.GetSnakeBlock(false, targetBlock.BlockData.SnakeStartID);

        // 뱀 이동 액션 시작
        AnimController.SetAnimeState(PlayerAnimState.Snake);
        InGameManager.Instance.StageController.PlaySnakeSwallowHeadAnim(targetBlock.BlockData.SnakeStartID);

        SoundManager.Instance.PlaySFX("sfx_snake_mouth");

        yield return new WaitUntil(() => AnimController.IsAnimationFinish(PlayerAnimState.Snake));

        InGameSnake snake = InGameManager.Instance.StageController.GetSnake(targetBlock.BlockData.SnakeStartID);

        // 카메라 타겟 변경 (뱀 삼키기 오브젝트)
        InGameUI.Instance.CameraController.SetTarget(snake.GetSwallowObject.transform);

        // 캐릭터 이름표 숨김
        OnOffCharacterNameObject(false);

        SoundManager.Instance.PlaySFX("sfx_snake_swallow");

        InGameManager.Instance.StageController.PlaySnakeSwallowAnim(targetBlock.BlockData.SnakeStartID);

        // while (Vector3.Distance(transform.position, endBlock.CharacterPivotTransform.position) > 0.05f)
        // {
        //     transform.position = Vector3.MoveTowards(transform.position, endBlock.CharacterPivotTransform.position, _characterMoveSpeed * Time.deltaTime);
        //     yield return null;
        // }

        yield return new WaitUntil(() => snake.IsPlayingSwallowAnim == false);

        // 카메라 타겟 변경 (뱀 삼키기 오브젝트)
        InGameUI.Instance.CameraController.SetTarget(transform);

        // 캐릭터 이름표 켜기
        OnOffCharacterNameObject(true);

        // 뱀에서 나왔을 경우는 유저 방향을 먼저 처리
        PlayerData.CurrentBlockIndex = endBlock.BlockData.BlockIndex;
        transform.position = endBlock.CharacterPivotTransform.position; // 이동 후 위치보정

        CheckPlayerDirection(); // 뱀이 나온방향으로 바로 처리

        AnimController.SetAnimeState(PlayerAnimState.Snake_Out);
        SoundManager.Instance.PlaySFX("sfx_snake_out");

        yield return new WaitUntil(() => AnimController.IsAnimationFinish(PlayerAnimState.Snake_Out));

        // 뱀 이동 액션 종료 처리
        _isPlayingSnakeMove = false;

        AnimController.OnOffHammerObject(PlayerData.IsActiveHammer);

        // 망치 아이템 체크
        CheckItemEvent(endBlock.BlockData.BlockIndex);
        yield return new WaitUntil(() => _isFinishUserItemEvent);

        // 아이템 블럭이 있는지 체크
        if (endBlock.BlockData.IsGameItemBlock)
        {
            StopCoroutine(nameof(StartGetItemBlock));
            StartCoroutine(StartGetItemBlock(endBlock, true));
        }
        else
        {
            EndPlayerMove();
        }
    }

    // 캐릭터의 사다리 블럭 이동
    private IEnumerator StartLadderMoveBlock(InGameBlock targetBlock)
    {
        _isPlayingNormalMove = false;
        _isPlayingLadderMove = true;

        InGameBlock endBlock = InGameManager.Instance.StageController.GetLadderBlock(false, targetBlock.BlockData.LadderStartID);
        InGameLadder ladder = InGameManager.Instance.StageController.GetLadder(targetBlock.BlockData.LadderStartID);

        // 사다리 이동 액션
        AnimController.SetAnimeState(PlayerAnimState.Ladder_Up);
        ladder.PlayLadderUpSFXSound();

        while (Vector3.Distance(transform.position, endBlock.CharacterPivotTransform.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endBlock.CharacterPivotTransform.position,
                _characterMoveSpeed * Time.deltaTime);
            yield return null;
        }

        ladder.StopLadderUpSFXSound();

        // 사다리 이동 종료 처리
        _isPlayingLadderMove = false;

        PlayerData.CurrentBlockIndex = endBlock.BlockData.BlockIndex;
        transform.position = endBlock.CharacterPivotTransform.position; // 이동 후 위치보정

        //CheckPlayerDirection();

        // 망치 아이템 체크
        CheckItemEvent(endBlock.BlockData.BlockIndex);
        yield return new WaitUntil(() => _isFinishUserItemEvent);

        // 아이템 블럭이 있는지 체크
        if (endBlock.BlockData.IsGameItemBlock)
        {
            StopCoroutine(nameof(StartGetItemBlock));
            StartCoroutine(StartGetItemBlock(endBlock, true));
        }
        else
        {
            EndPlayerMove();
        }
    }

    // 캐릭터의 아이템 획득 관련 처리
    private IEnumerator StartGetItemBlock(InGameBlock targetBlock, bool isEndMove)
    {
        _isPlayingNormalMove = false;
        _isPlayingGetItem = true;

        bool isValidItemBlock = targetBlock.IsAvailGetItem; // 획득 가능한 유요한 아이템 블럭인지 체크
        bool haveEnoughItemSlot = PlayerData.CheckItemListByItemID(0); // 아이템 슬롯이 충분한지 체크

        if (isValidItemBlock)
        {
            // 아이템 사용 타입에 따른 분기 처리
            int pickItemID = targetBlock.BlockData.GameItemIDList.RandomRatePick(x => 1);
            SpecGameItem specGameItemData = SpecDataManager.Instance.GetGameItemData(pickItemID);
            if (specGameItemData.game_item_active_type == GameItemActiveType.Immediate)
            {
                // todo.. 즉시 획득 아이템 처리
            }
            else
            {
                // 아이템 슬롯 공간 체크
                if (haveEnoughItemSlot)
                {
                    int slotNumber = PlayerData.AddItem(pickItemID);
                    targetBlock.ProcessGetItem();

                    // 아이템 획득 트레일 연출 재생
                    // InGameManager.Instance.ItemController.PlayGetItemTrailEffect(targetBlock.gameObject.transform,
                    //     slotNumber, () =>
                    //     {
                    //         InGameManager.Instance.Refresh(InGameRefreshType.RefreshItem);
                    //     });
                    InGameManager.Instance.Refresh(InGameRefreshType.RefreshItem);

                    SoundManager.Instance.PlaySFX("sfx_itembox_open");
                }
                else
                {
                    InGameUI.Instance.PlayItemSlotFullEffect();
                }
            }
        }

        _isPlayingGetItem = false;

        if (isEndMove)
        {
            EndPlayerMove();
        }

        yield break;
    }

    // 아이템 사용 연출 처리 (해머 등)
    private IEnumerator StartUseItemAction(GameItemType itemType)
    {
        _isFinishUserItemEvent = false;

        if (itemType == GameItemType.Hammer)
        {
            var targetAnimState = PlayerAnimState.Hammer_Atk;
            if (PlayerData.GetABSHammerAmount >= 10)
            {
                targetAnimState = PlayerAnimState.GoldenHammer_Atk;
            }

            AnimController.SetAnimeState(targetAnimState);

            InGameUI.Instance.CameraController.SetHammerZoomState(true);

            SoundManager.Instance.PlaySFX("sfx_hammer_and_land");

            yield return new WaitForSeconds(1.0f);

            InGameManager.Instance.AdjustForceMove(PlayerData.HammerAmount);

            // 망치 진동 분기 처리
            if (PlayerData.GetABSHammerAmount <= 3)
            {
                GlobalVibration.Instance.Vibrate(0.5f, 64);
            }
            else if (PlayerData.GetABSHammerAmount < 6)
            {
                GlobalVibration.Instance.Vibrate(0.5f, 128);
            }
            else
            {
                GlobalVibration.Instance.Vibrate(0.5f, 192);
            }

            // 카메라 쉐이크 적용
            InGameUI.Instance.CameraController.TriggerShake(0.5f);

            InGameUI.Instance.CameraController.SetHammerZoomState(false);

            yield return new WaitUntil(() => AnimController.IsAnimationFinish(targetAnimState));

            PlayerData.HammerAmount = 0;

            AnimController.OnOffHammerObject(PlayerData.IsActiveHammer);

            InGameManager.Instance.Refresh(InGameRefreshType.RefreshItem);
            InGameManager.Instance.Refresh(InGameRefreshType.RefreshEquip);
        }

        _isFinishUserItemEvent = true;
    }
}
