using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class StageEditor : MonoBehaviour
{
    #if UNITY_EDITOR

    [Header("Preview Mode")]
    [SerializeField] private Transform _previewItemParentTransform; // 프리뷰 아이템 부모 트랜스폼
    [SerializeField] private Transform _previewSnakeParentTransform; // 프리뷰 뱀 부모 트랜스폼
    [SerializeField] private Transform _previewLadderParentTransform; // 프리뷰 사다리 부모 트랜스폼

    [SerializeField] private GameObject _itemPreviewObject; // 아이템 프리뷰 오브젝트
    [SerializeField] private GameObject _snakePreviewObject; // 뱀 프리뷰 오브젝트
    [SerializeField] private GameObject _ladderPreviewObject; // 사다리 프리뷰 오브젝트

    [Header("Common")]
    [SerializeField] private Transform _blockParentTransform; // 블록 부모 트랜스폼

    [Header("Block Align")]
    [SerializeField] private List<Transform> _alignObjectList; // 정렬할 오브젝트 목록
    [SerializeField] private Vector3 startPosition = Vector3.zero; // 첫 번째 오브젝트의 위치
    [SerializeField] private float spacing = 2.0f; // 오브젝트 간 간격

    private List<InGameBlock> _inGameBlockList = new();

    private void Start()
    {
        LoadBlock();
    }

    private void Clear()
    {
        _inGameBlockList.Clear();
    }

    private void LoadBlock()
    {
        if (_blockParentTransform == null) return;

        Clear();
        for (int i = 0; i < _blockParentTransform.childCount; i++)
        {
            InGameBlock block = _blockParentTransform.GetChild(i).GetComponent<InGameBlock>();
            if (block == null)
            {
                Debug.LogError($"Block is null. Index : {i}");
                continue;
            }

            _inGameBlockList.Add(block);
        }
    }

    [ContextMenu("[Common] Align Object Horizontal")]
    private void AlignObjectHorizontal()
    {
        LoadBlock();

        if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;

        for (int i = 0; i < _alignObjectList.Count; i++)
        {
            // 각 오브젝트의 위치 계산
            // Vector3 newPosition;
            // if (alignVertically)
            // {
            //     // 세로 방향 정렬
            //     newPosition = startPosition + new Vector3(0, -i * spacing, 0);
            // }
            // else
            // {
            //     // 가로 방향 정렬
            //     newPosition = startPosition + new Vector3(i * spacing, 0, 0);
            // }

            // 가로 방향 정렬
            var newPosition = startPosition + new Vector3(i * spacing, 0, 0);

            _alignObjectList[i].position = newPosition;
        }
    }

    [ContextMenu("[Common] Show Preview Item Object")]
    public void ShowPreviewItemObject()
    {
        LoadBlock();

        if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;

        ClearPreviewItemObject();

        for (int i = 0; i < _inGameBlockList.Count; i++)
        {
            // 아이템 블럭 그리기
            if (_inGameBlockList[i].BlockData.IsGameItemBlock)
            {
                var newItemPreviewObject = Instantiate(_itemPreviewObject, _previewItemParentTransform);
                newItemPreviewObject.transform.position = _inGameBlockList[i].transform.position;
                newItemPreviewObject.SetActive(true);
            }
        }
    }

    [ContextMenu("[Common] Show Preview Snake Object")]
    public void ShowPreviewSnakeObject()
    {
        LoadBlock();

        if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;

        ClearPreviewSnakeObject();

        for (int i = 0; i < _inGameBlockList.Count; i++)
        {
            // 뱀 블럭 그리기
            if (_inGameBlockList[i].BlockData.SnakeStartID > 0 || _inGameBlockList[i].BlockData.SnakeEndID > 0)
            {
                var newSnakePreviewObject = Instantiate(_snakePreviewObject, _previewSnakeParentTransform);
                newSnakePreviewObject.transform.position = _inGameBlockList[i].transform.position;
                int snakeID = _inGameBlockList[i].BlockData.SnakeStartID > 0 ? _inGameBlockList[i].BlockData.SnakeStartID : _inGameBlockList[i].BlockData.SnakeEndID;
                newSnakePreviewObject.GetComponentInChildren<TextMeshPro>().text = snakeID.ToString();
                newSnakePreviewObject.SetActive(true);
            }
        }
    }

    [ContextMenu("[Common] Show Preview Ladder Object")]
    public void ShowPreviewLadderObject()
    {
        LoadBlock();

        if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;

        ClearPreviewLadderObject();

        for (int i = 0; i < _inGameBlockList.Count; i++)
        {
            // 사다리 블럭 그리기
            if (_inGameBlockList[i].BlockData.LadderStartID > 0 || _inGameBlockList[i].BlockData.LadderEndID > 0)
            {
                var newLadderPreviewObject = Instantiate(_ladderPreviewObject, _previewLadderParentTransform);
                newLadderPreviewObject.transform.position = _inGameBlockList[i].transform.position;
                int laderID = _inGameBlockList[i].BlockData.LadderStartID > 0 ? _inGameBlockList[i].BlockData.LadderStartID : _inGameBlockList[i].BlockData.LadderEndID;
                newLadderPreviewObject.GetComponentInChildren<TextMeshPro>().text = laderID.ToString();
                newLadderPreviewObject.SetActive(true);
            }
        }
    }

    // [ContextMenu("[Common] Show Preview Object")]
    // public void ShowPreviewObject()
    // {
    //     LoadBlock();
    //
    //     if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;
    //
    //     ClearPreviewItemObject();
    //
    //     for (int i = 0; i < _inGameBlockList.Count; i++)
    //     {
    //         // 아이템 블럭 그리기
    //         if (_inGameBlockList[i].BlockData.IsGameItemBlock)
    //         {
    //             var newItemPreviewObject = Instantiate(_itemPreviewObject, _previewItemParentTransform);
    //             newItemPreviewObject.transform.position = _inGameBlockList[i].transform.position;
    //             newItemPreviewObject.SetActive(true);
    //         }
    //
    //         // 뱀 블럭 그리기
    //         if (_inGameBlockList[i].BlockData.SnakeStartID > 0 || _inGameBlockList[i].BlockData.SnakeEndID > 0)
    //         {
    //             var newSnakePreviewObject = Instantiate(_snakePreviewObject, _previewSnakeParentTransform);
    //             newSnakePreviewObject.transform.position = _inGameBlockList[i].transform.position;
    //             int snakeID = _inGameBlockList[i].BlockData.SnakeStartID > 0 ? _inGameBlockList[i].BlockData.SnakeStartID : _inGameBlockList[i].BlockData.SnakeEndID;
    //             newSnakePreviewObject.GetComponentInChildren<TextMeshPro>().text = snakeID.ToString();
    //             newSnakePreviewObject.SetActive(true);
    //         }
    //
    //         // 사다리 블럭 그리기
    //         if (_inGameBlockList[i].BlockData.LadderStartID > 0 || _inGameBlockList[i].BlockData.LadderEndID > 0)
    //         {
    //             var newLadderPreviewObject = Instantiate(_ladderPreviewObject, _previewLadderParentTransform);
    //             newLadderPreviewObject.transform.position = _inGameBlockList[i].transform.position;
    //             int laderID = _inGameBlockList[i].BlockData.LadderStartID > 0 ? _inGameBlockList[i].BlockData.LadderStartID : _inGameBlockList[i].BlockData.LadderEndID;
    //             newLadderPreviewObject.GetComponentInChildren<TextMeshPro>().text = laderID.ToString();
    //             newLadderPreviewObject.SetActive(true);
    //         }
    //     }
    // }

    [ContextMenu("[Common] Clear Preview Item Object")]
    public void ClearPreviewItemObject()
    {
        BMUtil.RemoveChildObjectsImmdiate(_previewItemParentTransform);
    }

    [ContextMenu("[Common] Clear Preview Snake Object")]
    public void ClearPreviewSnakeObject()
    {
        BMUtil.RemoveChildObjectsImmdiate(_previewSnakeParentTransform);
    }

    [ContextMenu("[Common] Clear Preview Ladder Object")]
    public void ClearPreviewLadderObject()
    {
        BMUtil.RemoveChildObjectsImmdiate(_previewLadderParentTransform);
    }

    [ContextMenu("[Common] Redraw All Block")]
    public void RedrawAllBlock()
    {
        LoadBlock();

        if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;

        for (int i = 0; i < _inGameBlockList.Count; i++)
        {
            // 블럭 시작, 끝 데이터 설정
            if (i == 0)
            {
                _inGameBlockList[i].BlockData.IsStartBlock = true;
            }
            else if (i == _inGameBlockList.Count - 1)
            {
                _inGameBlockList[i].BlockData.IsEndBlock = true;
            }
            else
            {
                _inGameBlockList[i].BlockData.IsStartBlock = false;
                _inGameBlockList[i].BlockData.IsEndBlock = false;
            }

            // 블럭 이름 설정
            _inGameBlockList[i].BlockData.BlockIndex = i;
            _inGameBlockList[i].name = $"Block_{i}";
            _inGameBlockList[i].SetBlockIndexText();

            // 아이템 생성
            _inGameBlockList[i].CreateItemObject(true);
        }

        // foreach (var block in _inGameBlockList)
        // {
        //     block.Clear();
        //
        //     // 블럭 이름 설정
        //     block.BlockData.BlockIndex
        //     block.name = $"Block_{block.BlockData.BlockIndex}";
        //     block.SetBlockIndexText();
        //
        //     // 아이템 생성
        //     block.CreateItemObject();
        // }
    }

    [ContextMenu("[Common] Check Block Error")]
    public void CheckBlockError()
    {
        // 블럭 로드 하면서 index 누락 체크
        LoadBlock();

        if (_inGameBlockList == null || _inGameBlockList.Count <= 0) return;

        Dictionary<int, int> _snakeBlockIDDic = new();
        Dictionary<int, int> _ladderBlockIDDic = new();

        int startBlockCheckCount = 0;
        int endBlockCheckCount = 0;
        foreach (var block in _inGameBlockList)
        {
            // 블럭 오브젝트가 프리팹인지 아닌지 확인
#if UNITY_EDITOR
            bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(block.gameObject) || PrefabUtility.IsAnyPrefabInstanceRoot(block.gameObject);
            if (isPrefab == false)
            {
                Debug.LogError($"#### 프리팹이 아닌 블럭이 존재합니다. 오류 블럭 인덱스 : {block.BlockData.BlockIndex} ####");
                return;
            }
#endif

            if (block.BlockData.IsStartBlock)
            {
                startBlockCheckCount++;
                if (startBlockCheckCount > 1)
                {
                    Debug.LogError($"시작 블럭이 1개가 아닙니다. 오류 블럭 인덱스 : {block.BlockData.BlockIndex}");
                    return;
                }
            }

            if (block.BlockData.IsEndBlock)
            {
                endBlockCheckCount++;
                if (endBlockCheckCount > 1)
                {
                    Debug.LogError($"마지막 블럭이 1개가 아닙니다. 오류 블럭 인덱스 : {block.BlockData.BlockIndex}");
                    return;
                }
            }

            // 뱀 블럭 체크
            if (block.BlockData.SnakeStartID > 0 || block.BlockData.SnakeEndID > 0)
            {
                int targetID = block.BlockData.SnakeStartID > 0 ? block.BlockData.SnakeStartID : block.BlockData.SnakeEndID;

                if (_snakeBlockIDDic.TryAdd(targetID, 1) == false)
                {
                    _snakeBlockIDDic[targetID]++;
                }
            }

            // 사다리 블럭 체크
            if (block.BlockData.LadderStartID > 0 || block.BlockData.LadderEndID > 0)
            {
                int targetID = block.BlockData.LadderStartID > 0 ? block.BlockData.LadderStartID : block.BlockData.LadderEndID;

                if (_ladderBlockIDDic.TryAdd(targetID, 1) == false)
                {
                    _ladderBlockIDDic[targetID]++;
                }
            }
        }

        if (startBlockCheckCount == 0)
        {
            Debug.LogError($"시작 블럭이 설정되지 않았습니다.");
        }

        if (endBlockCheckCount == 0)
        {
            Debug.LogError($"마지막 블럭이 설정되지 않았습니다.");
        }

        // 뱀 블럭 오류 체크
        foreach (var snake in _snakeBlockIDDic)
        {
            if (snake.Value != 2)
            {
                Debug.LogError($"뱀 블럭의 시작 or 끝 ID 오류 --> 오류 뱀 ID : {snake.Key}");
            }
        }

        // 사다리 블럭 오류 체크
        foreach (var ladder in _ladderBlockIDDic)
        {
            if (ladder.Value != 2)
            {
                Debug.LogError($"사다리 블럭의 시작 or 끝 ID 오류 --> 오류 사다리 ID : {ladder.Key}");
            }
        }
    }

    #endif
}
