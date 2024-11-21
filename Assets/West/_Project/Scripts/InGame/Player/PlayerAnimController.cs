using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private Transform objectPivotTransform;

    [Header("Item")]
    [SerializeField] private GameObject _bubbleUseItemObject;
    [SerializeField] private SpriteRenderer _bubbleUseItemIconSprite;
    [SerializeField] private GameObject _hammerObject;
    [SerializeField] private GameObject _normalHammerObject;
    [SerializeField] private GameObject _goldenHammerObject;

    [Header("Effect")]
    [SerializeField] private GameObject _idleEffectObject;
    [SerializeField] private GameObject _walkEffectObject;
    [SerializeField] private ParticleSystem _landingEffect;

    [Header("Rule")]
    [SerializeField] private GameObject _prisonPrefab;

    private GameObject _prisonObject;

    public void Init()
    {
        Clear();
    }

    public void PlayGameItemAction(int gameItemID)
    {
        var gameItemData = SpecDataManager.Instance.GetGameItemData(gameItemID);
        if (gameItemData == null) return;

        var currentPlayer = InGameManager.Instance.GetCurrentTurnPlayer();

        switch (gameItemData.game_item_type)
        {
            case GameItemType.Hammer:

                if (currentPlayer.PlayerData.GetABSHammerAmount >= 10)
                {
                    SetAnimeState(PlayerAnimState.GoldenHammer_Rdy);
                }
                else
                {
                    SetAnimeState(PlayerAnimState.Hammer_Rdy);
                }

                break;
        }

        ShowBubbleUseItem(gameItemID);
    }

    public void SetAnimeState(PlayerAnimState targetState)
    {
        switch (targetState)
        {
            case PlayerAnimState.Idle:
                _characterAnimator.SetBool("Idle", true);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                ClearTrigger();

                _idleEffectObject.SetActive(true);
                _walkEffectObject.SetActive(false);
                break;
            case PlayerAnimState.Walk:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", true);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                ClearTrigger();

                _idleEffectObject.SetActive(false);
                _walkEffectObject.SetActive(true);

                SoundManager.Instance.PlaySFX("sfx_move_run");
                break;
            case PlayerAnimState.Ladder_Up:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", true);
                _characterAnimator.SetBool("Fall", false);

                _idleEffectObject.SetActive(false);
                _walkEffectObject.SetActive(false);
                break;
            case PlayerAnimState.Snake:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Snake");
                _characterAnimator.Play("Snake", -1, 0f);

                _idleEffectObject.SetActive(false);
                _walkEffectObject.SetActive(false);
                break;
            case PlayerAnimState.Snake_Out:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Snake_Out");
                _characterAnimator.Play("Snake_Out", -1, 0f);

                _idleEffectObject.SetActive(true);
                _walkEffectObject.SetActive(true);

                OnOffHammerObject(false);
                OnLandingEffect();
                break;
            case PlayerAnimState.Jump:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Jump");
                _characterAnimator.Play("Jump", -1, 0f);

                _idleEffectObject.SetActive(false);
                _walkEffectObject.SetActive(false);
                break;
            case PlayerAnimState.Six:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Six");
                _characterAnimator.Play("Six", -1, 0f);
                break;
            case PlayerAnimState.Hammer_Rdy:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Hammer_Rdy");
                _characterAnimator.Play("Hammer_Rdy", -1, 0f);

                OnOffHammerObject(true);

                break;
            case PlayerAnimState.Hammer_Atk:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Hammer_Atk");
                _characterAnimator.Play("Hammer_Atk", -1, 0f);

                break;
            case PlayerAnimState.GoldenHammer_Rdy:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("GoldenHammer_Rdy");
                _characterAnimator.Play("GoldenHammer_Rdy", -1, 0f);

                OnOffHammerObject(true);

                break;
            case PlayerAnimState.GoldenHammer_Atk:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("GoldenHammer_Atk");
                _characterAnimator.Play("GoldenHammer_Atk", -1, 0f);

                break;
            case PlayerAnimState.Fall:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", true);

                OnOffHammerObject(false);
                _walkEffectObject.SetActive(true);
                break;
            case PlayerAnimState.Victory:
                _characterAnimator.SetBool("Idle", false);
                _characterAnimator.SetBool("Walk", false);
                _characterAnimator.SetBool("Ladder_Up", false);
                _characterAnimator.SetBool("Fall", false);

                _characterAnimator.SetTrigger("Victory");
                _characterAnimator.Play("Victory", -1, 0f);
                break;
        }
    }

    // 현재 플레이중인 애니메이션이 끝났는지 체크
    public bool IsAnimationFinish(PlayerAnimState targetAnimState)
    {
        bool isFinish = false;

        AnimatorStateInfo stateInfo = _characterAnimator.GetCurrentAnimatorStateInfo(0);

        // 현재 애니메이션이 지정한 상태인지 확인
        if (stateInfo.IsName(targetAnimState.ToString()))
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                isFinish = true;
            }
        }

        return isFinish;
    }

    // 점프 착지 오브젝트 제어
    public void OnLandingEffect()
    {
        _landingEffect.gameObject.SetActive(true);

        _landingEffect.Clear();
        _landingEffect.Play();
    }

    // 아이템 사용 버블 오브젝트 제어
    public void ShowBubbleUseItem(int gameItemID)
    {
        var gameItemData = SpecDataManager.Instance.GetGameItemData(gameItemID);
        if (gameItemData == null) return;

        _bubbleUseItemIconSprite.sprite = ImageManager.Instance.GetGameItemIcon(gameItemData.game_item_id);
        _bubbleUseItemObject.SetActive(true);

        TweenUtil.UseItemBubbleTween(_bubbleUseItemObject.transform, 0.8f);
    }

    // 해머 오브젝트 제어
    public void OnOffHammerObject(bool isOn)
    {
        if (_hammerObject == null) return;

        _hammerObject.SetActive(isOn);

        var currentPlayer = InGameManager.Instance.GetCurrentTurnPlayer();

        _normalHammerObject.SetActive(isOn && currentPlayer.PlayerData.GetABSHammerAmount < 10);
        _goldenHammerObject.SetActive(isOn && currentPlayer.PlayerData.GetABSHammerAmount >= 10);
    }

    // 해머 강제 끄기
    public void ForceOffHammerObejct()
    {
        _hammerObject.SetActive(true);

        _normalHammerObject.SetActive(false);
        _goldenHammerObject.SetActive(false);
    }

    // 감옥 오브젝트 제어
    public void OnOffPrision(bool isOn, int diceNumber)
    {
        if (isOn && _prisonObject == null)
        {
            _prisonObject = Instantiate(_prisonPrefab, objectPivotTransform);
            _prisonObject.GetComponent<Prison>().SetPrisonDice(diceNumber);
        }

        if (!isOn && _prisonObject != null)
        {
            _prisonObject.GetComponent<Prison>().PlayPrisonEscapeAnim();
            Destroy(_prisonObject, 1.8f);
        }
    }

    public void UpdatePrisonObject(int count)
    {
        if (_prisonObject != null)
        {
            _prisonObject.GetComponent<Prison>().UpdatePrisonEscapeCount(count);
        }
    }

    private void Clear()
    {
        _bubbleUseItemObject.SetActive(false);
        _hammerObject.SetActive(false);
    }

    private void ClearTrigger()
    {
        _characterAnimator.ResetTrigger("Jump");
        _characterAnimator.ResetTrigger("Six");
        _characterAnimator.ResetTrigger("Hammer_Rdy");
        _characterAnimator.ResetTrigger("GoldenHammer_Rdy");
    }
}
