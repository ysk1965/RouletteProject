using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using CookApps.Obfuscator;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] private Transform _blockParentTransform; // 블록 부모 트랜스폼
    [SerializeField] private Transform _snakeParentTransform; // 뱀 오브젝트 부모 트랜스폼
    [SerializeField] private Transform _ladderParentTransform; // 사다리 오브젝트 부모 트랜스폼
    [SerializeField] private Transform _characterParentTransform; // 캐릭터 부모 트랜스폼

    private readonly List<InGameBlock> _inGameBlockList = new(); // 전체 블록 리스트
    private readonly List<InGameLadder> _inGameLadderList = new(); // 전체 사다리 리스트
    private readonly List<InGameSnake> _inGameSnakeList = new(); // 전체 뱀 리스트

    public Transform CharacterParentTransform => _characterParentTransform;
    public InGameBlock StartBlock { get; private set; }

    public InGameBlock EndBlock { get; private set; }

    public void Init()
    {
        Clear();

        LoadBlock();
        LoadSnake();
        LoadLadder();
    }

    public void Refresh()
    {
        ClearBlockPin();
    }

    public InGameBlock GetInGameBlock(int blockIndex)
    {
        if (_inGameBlockList == null || _inGameBlockList.Count == 0)
        {
            return null;
        }

        if (blockIndex < StartBlock.BlockData.BlockIndex)
        {
            return null;
        }

        if (blockIndex > EndBlock.BlockData.BlockIndex)
        {
            return null;
        }

        return _inGameBlockList[blockIndex];
    }

    public InGameBlock GetSnakeBlock(bool isStart, int snakeID)
    {
        if (_inGameBlockList == null || _inGameBlockList.Count == 0)
        {
            return null;
        }

        InGameBlock resultBlock = null;

        if (isStart)
        {
            resultBlock = _inGameBlockList.Find(block => block.BlockData.SnakeStartID == snakeID);
        }
        else
        {
            resultBlock = _inGameBlockList.Find(block => block.BlockData.SnakeEndID == snakeID);
        }

        return resultBlock;
    }

    public InGameBlock GetLadderBlock(bool isStart, int ladderID)
    {
        if (_inGameBlockList == null || _inGameBlockList.Count == 0)
        {
            return null;
        }

        InGameBlock resultBlock = null;

        if (isStart)
        {
            resultBlock = _inGameBlockList.Find(block => block.BlockData.LadderStartID == ladderID);
        }
        else
        {
            resultBlock = _inGameBlockList.Find(block => block.BlockData.LadderEndID == ladderID);
        }

        return resultBlock;
    }

    public InGameSnake GetSnake(int snakeID)
    {
        if (_inGameSnakeList == null || _inGameSnakeList.Count == 0)
        {
            return null;
        }

        return _inGameSnakeList.Find(snake => snake.SnakeID == snakeID);
    }

    public InGameLadder GetLadder(int ladderID)
    {
        if (_inGameLadderList == null || _inGameLadderList.Count == 0)
        {
            return null;
        }

        return _inGameLadderList.Find(ladder => ladder.LadderID == ladderID);
    }

    // 해당 블럭에 플레이어가 있는지 확인
    public bool IsAnyPlayerOnBlock(int blockIndex)
    {
        List<PlayerController> playerList = InGameManager.Instance.GamePlayerList;

        return playerList.Exists(player => player.PlayerData.CurrentBlockIndex == blockIndex);
    }

    // 해당 블럭에 플레이어가 있는지 확인 (해당 플레이어 아이디 제외)
    public bool IsAnyPlayerOnBlockExceptTarget(int blockIndex, int targetTurnIndex)
    {
        List<PlayerController> playerList = InGameManager.Instance.GamePlayerList;

        return playerList.Exists(player =>
            player.PlayerData.CurrentBlockIndex == blockIndex && player.PlayerData.TurnIndex != targetTurnIndex);
    }

    public void PlaySnakeSwallowHeadAnim(int snakeID)
    {
        if (_inGameSnakeList == null || _inGameSnakeList.Count == 0)
        {
            return;
        }

        InGameSnake snake = _inGameSnakeList.Find(s => s.SnakeID == snakeID);
        snake?.PlaySnakeSwallowHeadAnim();
    }

    public void PlaySnakeSwallowAnim(int snakeID)
    {
        if (_inGameSnakeList == null || _inGameSnakeList.Count == 0)
        {
            return;
        }

        InGameSnake snake = _inGameSnakeList.Find(s => s.SnakeID == snakeID);
        snake?.PlaySnakeSwallowAnim();
    }

    // 목적지 플래그 설정
    public void SwitchDestinationPin(int destBlockIndex, bool isOn)
    {
        var destBlock = GetInGameBlock(destBlockIndex);
        if (destBlock == null) return;

        destBlock.SetDestinationPin(isOn);
    }

    public void UpdateBlockPin()
    {
        // 하이라이트 상태 초기화
        ClearBlockPin();

        int currentUseItemID = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.UseGameItemID;
        if (currentUseItemID <= 0)
        {
            return;
        }

        SpecGameItem currentUseItemData = SpecDataManager.Instance.GetGameItemData(currentUseItemID);
        if (currentUseItemData == null)
        {
            return;
        }

        if (currentUseItemData.game_item_type != GameItemType.Dice)
        {
            return;
        }

        // 하이라이트 블럭 설정
        int currentUserBlockIndex = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData.CurrentBlockIndex;

        ObfuscatorFloat[] diceResultList = currentUseItemData.game_item_value;
        for (var i = 0; i < diceResultList.Length; ++i)
        {
            var diceResult = (int) diceResultList[i];

            int targetBlockIndex = currentUserBlockIndex + diceResult;

            if (targetBlockIndex < 0 || targetBlockIndex >= _inGameBlockList.Count)
            {
                continue;
            }

            InGameBlock targetBlock = _inGameBlockList[targetBlockIndex];
            targetBlock?.SetBlockPin(true, diceResult);
        }
    }

    // 모든 아이템 박스 리셋
    public void ResetItemBox(InGameIndexType indexType)
    {
        // [상건] Spawn 애님 추가 및 리셋 딜레이 추가
        StartCoroutine(ResetItemsWithDelay(indexType, 0.04f));
    }

    public IEnumerator ResetItemsWithDelay(InGameIndexType indexType, float second)
    {
        foreach (InGameBlock block in _inGameBlockList)
        {
            // 현재 캐릭터가 위치한 블럭인 경우 제외
            if (IsAnyPlayerOnBlock(block.BlockData.BlockIndex))
            {
                continue;
            }

            // 아이템 리셋 진행
            if (indexType == InGameIndexType.Odd)
            {
                if (block.BlockData.BlockIndex % 2 == 1)
                {
                    block.ResetItemBox();
                }
            }
            else if (indexType == InGameIndexType.Even)
            {
                if (block.BlockData.BlockIndex % 2 == 0)
                {
                    block.ResetItemBox();
                }
            }
            else
            {
                block.ResetItemBox();
            }

            yield return new WaitForSeconds(second);
        }
    }

    // 사다리 오브젝트 숨기기
    public void HideAllLadder()
    {
        _inGameLadderList.ForEach(ladder => ladder.StartFadeOut());
    }

    // 뱀 오브젝트 숨기기
    public void HideAllSnake()
    {
        _inGameSnakeList.ForEach(snake => snake.StartFadeOut());
    }

    public void Clear()
    {
        StartBlock = null;
        EndBlock = null;

        BMUtil.RemoveChildObjects(CharacterParentTransform);
    }

    // 자식 오브젝트 하위에 있는 모든 블럭리스트를 로드
    private void LoadBlock()
    {
        if (_blockParentTransform == null)
        {
            return;
        }

        for (var i = 0; i < _blockParentTransform.childCount; i++)
        {
            var block = _blockParentTransform.GetChild(i).GetComponent<InGameBlock>();
            if (block == null)
            {
                Debug.LogError($"Block is null. Index : {i}");
                continue;
            }

            // 시작 블럭 캐싱
            if (block.BlockData.IsStartBlock)
            {
                StartBlock = block;
            }

            // 끝 블럭 캐싱
            if (block.BlockData.IsEndBlock)
            {
                EndBlock = block;
            }

            _inGameBlockList.Add(block);
        }
    }

    // 자식 오브젝트 하위에 있는 모든 뱀 리스트를 로드
    private void LoadSnake()
    {
        if (_snakeParentTransform == null)
        {
            return;
        }

        for (var i = 0; i < _snakeParentTransform.childCount; i++)
        {
            var snake = _snakeParentTransform.GetChild(i).GetComponent<InGameSnake>();
            if (snake == null)
            {
                Debug.LogError($"Snake is null. Index : {i}");
                continue;
            }

            _inGameSnakeList.Add(snake);
        }
    }

    // 자식 오브젝트 하위에 있는 모든 사다리 리스트를 로드
    private void LoadLadder()
    {
        if (_ladderParentTransform == null)
        {
            return;
        }

        for (var i = 0; i < _ladderParentTransform.childCount; i++)
        {
            var ladder = _ladderParentTransform.GetChild(i).GetComponent<InGameLadder>();
            if (ladder == null)
            {
                Debug.LogError($"Ladder is null. Index : {i}");
                continue;
            }

            _inGameLadderList.Add(ladder);
        }
    }

    private void ClearBlockPin()
    {
        _inGameBlockList.ForEach(block => block.ClearPin());
    }
}
