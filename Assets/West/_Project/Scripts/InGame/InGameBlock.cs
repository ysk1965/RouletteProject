using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using DG.Tweening;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class InGameBlockData
{
    [Header("[Block]")]
    public bool IsStartBlock;

    public bool IsEndBlock;
    public int BlockIndex;

    [Header("[Snake]")]
    public int SnakeStartID;

    public int SnakeEndID;

    [Header("[Ladder]")]
    public int LadderStartID;

    public int LadderEndID;

    [Header("[Game Item]")]
    public List<int> GameItemIDList;

    public bool IsGameItemBlock => GameItemIDList.Count > 0;

    public bool IsSnakeStartBlock => SnakeStartID > 0;
    public bool IsSnakeEndBlock => SnakeEndID > 0;
    public bool IsSnakeBlock => IsSnakeStartBlock || IsSnakeEndBlock;

    public bool IsLadderStartBlock => LadderStartID > 0;
    public bool IsLadderEndBlock => LadderEndID > 0;
    public bool IsLadderBlock => IsLadderStartBlock || IsLadderEndBlock;
}

public class InGameBlock : MonoBehaviour
{
    [Header("Block Data")]
    [SerializeField] private InGameBlockData _blockData;

    [Header("Object - Block")]
    [SerializeField] private TextMeshPro _blockIndexText; // 블록 인덱스 텍스트
    [SerializeField] private Transform _characterPivotTransform; // 캐릭터가 위치할 트랜스폼

    [Space]
    [SerializeField] private GameObject _startFlagObject;
    [SerializeField] private GameObject _endFlagObject;

    [Space]
    [SerializeField] private Animator _fireworksAnimator_1;
    [SerializeField] private Animator _fireworksAnimator_2;

    [Header("Object - Pin")]
    [SerializeField] private GameObject _blockPinObject;
    [SerializeField] private TextMeshPro _blockPinText;

    [Space]
    [SerializeField] private GameObject _destinationPinObject;

    [Header("Object - Item")]
    [SerializeField] private GameObject _ItemLayerObject;
    [SerializeField] private GameObject _ItemPrefab;

    private GameItemBox _gameItemBox;
    private bool _isAnimating;
    private bool _isPlaySFXSound = false;

    public bool IsAvailGetItem { get; private set; } // 아이템 획득 가능 여부

    public InGameBlockData BlockData => _blockData;
    public Transform CharacterPivotTransform => _characterPivotTransform;

    private void OnDisable()
    {
        StopSFXSound();
    }

    private void Start()
    {
        Clear();

        CreateItemObject(true);

        _startFlagObject?.SetActive(_blockData.IsStartBlock);
        _endFlagObject?.SetActive(_blockData.IsEndBlock);
    }

    public void CreateItemObject(bool isInit)
    {
        if (isInit && _blockData.IsGameItemBlock == false)
        {
            return;
        }

        _ItemLayerObject.SetActive(true);

        GameObject newItemBox = Instantiate(_ItemPrefab, _ItemLayerObject.transform);
        _gameItemBox = newItemBox.GetComponent<GameItemBox>();

        // 인위적으로 생성 되는 경우
        if (isInit == false)
        {
            SpecGameItem randomItemData = SpecDataManager.Instance.GetRandomItem(GameItemType.Dice);
            _blockData.GameItemIDList.Add(randomItemData.game_item_id);
        }

        IsAvailGetItem = true;
    }

    public void SetBlockIndexText()
    {
        if (_blockIndexText == null)
        {
            return;
        }

        _blockIndexText.text = BlockData.BlockIndex.ToString();

        // 에디터 모드로 변경한 경우 처리
#if UNITY_EDITOR
        SaveEditorChange();
#endif
    }

    // 아이템을 먹었을 경우 처리
    public void ProcessGetItem()
    {
        //_ItemLayerObject.SetActive(false);

        _gameItemBox.PlayGetAnimation();

        IsAvailGetItem = false;
    }

    public void ProcessBlockAnimation(float power)
    {
        if (_isAnimating)
        {
            return;
        }

        _isAnimating = true;

        Vector3 originalPosition = transform.position;
        Vector3 downPosition = originalPosition + (Vector3.down * 0.09f * power);
        Vector3 upPosition = originalPosition + (Vector3.down * 0.03f * power);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(downPosition, 0.05f).SetEase(Ease.InOutQuad));
        sequence.Append(transform.DOMove(upPosition, 0.07f).SetEase(Ease.InOutQuad));
        sequence.Append(transform.DOMove(originalPosition, 0.02f).SetEase(Ease.InOutQuad));

        sequence.OnComplete(() =>
        {
            _isAnimating = false;
        });
    }

    // 아이템 박스를 다시 획득 가능하도록 리셋
    public void ResetItemBox()
    {
        if (_blockData.IsGameItemBlock == false)
        {
            CreateItemObject(false);
        }

        _gameItemBox.ResetItemBox();

        IsAvailGetItem = true;
    }

    public void SetBlockPin(bool isActive, int pinValue)
    {
        _blockPinObject?.SetActive(isActive);

        _blockPinText.text = pinValue.ToString();
    }

    public void SetDestinationPin(bool isActive)
    {
        _destinationPinObject?.SetActive(isActive);
    }

    public void PlayFireworksAnimation()
    {
        if (_fireworksAnimator_1 == null) return;
        if (_fireworksAnimator_2 == null) return;

        _fireworksAnimator_1?.SetTrigger("FireWorks");
        _fireworksAnimator_1?.Play("FireWorks", -1, 0f);

        _fireworksAnimator_2?.SetTrigger("FireWorks");
        _fireworksAnimator_2?.Play("FireWorks", -1, 0f);

        PlaySFXSound();
    }

    public void PlaySFXSound()
    {
        _isPlaySFXSound = true;
        StartCoroutine(nameof(PlayFireworksSFX));
    }

    public void StopSFXSound()
    {
        _isPlaySFXSound = false;
        StopCoroutine(nameof(PlayFireworksSFX));
    }

    public void Clear()
    {
        _gameItemBox = null;

        _ItemLayerObject.SetActive(false);

        _blockPinObject.SetActive(false);
        _destinationPinObject.SetActive(false);

        _isPlaySFXSound = false;

        BMUtil.RemoveChildObjects(_ItemLayerObject.transform);
    }

    public void ClearPin()
    {
        _blockPinObject.SetActive(false);
    }

    IEnumerator PlayFireworksSFX()
    {
        while (_isPlaySFXSound)
        {
            SoundManager.Instance.PlaySFX("sfx_firework");

            yield return new WaitForSeconds(4.47f);
        }
    }

    private void SaveEditorChange()
    {
#if UNITY_EDITOR
        Undo.RecordObject(_blockIndexText, "Change TextMeshPro Text");
        EditorUtility.SetDirty(_blockIndexText);
#endif
    }
}
