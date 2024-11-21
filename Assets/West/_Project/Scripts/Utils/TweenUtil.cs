using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public static class TweenUtil
{

    #region ////////다이스 관련 트윈////////
    //다이스 던질때 백판 움직임
    public static Tween TweenDiceRollBehind(Transform pos)
    {
        return pos.DOScale(0.6f, 0.4f).SetEase(Ease.OutFlash).SetAutoKill(true).SetLoops(2, LoopType.Yoyo);
    }
    #endregion

    #region ////////토스트팝업 트윈////////

    #region 하단 토스트 팝업
    public static Sequence OpenToastBottomTween(Transform pos, CanvasGroup cg)
    {
        return DOTween.Sequence()
            .OnStart(() =>
            {
                pos.gameObject.SetActive(true);
                cg.alpha = 0f;
            })
            .Append(pos.DOLocalMoveY(-100f, 0.25f).SetAutoKill(true).SetEase(Ease.OutFlash).From())
            .Append(cg.DOFade(1f, 0.25f).SetAutoKill(true).SetEase(Ease.OutFlash));
    }

    public static Sequence CloseToastBottomTween(Transform pos, CanvasGroup cg)
    {
        return DOTween.Sequence()
            .Append(pos.DOLocalMoveY(-100f, 0.25f).SetAutoKill(true).SetEase(Ease.OutFlash))
            .Append(cg.DOFade(0f, 0.25f).SetAutoKill(true).SetEase(Ease.OutFlash))
            .OnComplete(() =>
            {
                pos.gameObject.SetActive(false);
            });
    }

    #endregion

    #region 상단 토스트 팝업

    public static Sequence OpenToastTopTween(Transform pos, Action completeCallback)
    {
        return DOTween.Sequence()
            //.OnStart(() =>  pos.DOLocalMoveY(100f, 0f))
            .Append(pos.DOLocalMoveY(130f, 0f).SetRelative())
            .Append(pos.DOLocalMoveY(-130f, 0.2f).SetRelative().SetEase(Ease.InQuad))
            .AppendInterval(2.0f)
            // .Append(pos.DOLocalMoveY(-60f, 0f).SetRelative())
            .Append(pos.DOLocalMoveY(130f, 0.2f).SetRelative().SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                completeCallback?.Invoke();
            });
    }

    public static Sequence CloseToastTopTween(Transform pos)
    {
        return DOTween.Sequence()
            .Append(pos.DOLocalMoveY(-60f, 0f).SetRelative())
            .Append(pos.DOLocalMoveY(130f, 0.2f).SetRelative().SetEase(Ease.InQuad));
    }

    #endregion

    #endregion

    #region////////턴 카운터 숫자 전환 트윈///////

    public static Sequence NumbChangeTween(Transform pos)
    {
        return DOTween.Sequence()
            .Prepend(pos.DOScale(new Vector3(5, 0, 1), 0f))
            .Append(pos.DOScale(new Vector3(0.5f, 1.85f, 1), 0.06f).SetEase(Ease.OutQuad))
            .Append(pos.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutQuad).SetDelay(0.06f));
    }

    #endregion

    #region /////////인게임 바텀 바 전환 트윈 /////////

    public static Tween BottomBarChangeTween(Transform pos)
    {
        return pos.DOScaleY(1.06f, 0.075f).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
    }


    #endregion

    #region ///////// 아이템 사용 말풍선 트윈 //////////

    public static Sequence UseItemBubbleTween(Transform pos, float delayTime)
    {
        return DOTween.Sequence()
            .Prepend(pos.DOScale(0f, 0f))
            .Append(pos.DOScale(1.8f, 0.18f).SetEase(Ease.InOutFlash))
            .Append(pos.DOScale(1.5f, 0.13f).SetEase(Ease.InSine))
            .Append(pos.DOScale(1.9f,0.2f).SetEase(Ease.InOutFlash).SetDelay(delayTime))
            .Append(pos.DOScale(0f, 0.15f).SetEase(Ease.InFlash))
            .OnComplete(() =>
            {
                pos.gameObject.SetActive(false);
            });
    }

    #endregion

    #region ////////// 아이템 카드 슬롯 사용 트윈 //////////

    public static Sequence UseItemCardSlotFadeTween(Transform pos, CanvasGroup cg)
    {
        return DOTween.Sequence()
            .Prepend(pos.DOLocalMoveY(70f, 0.5f).SetEase(Ease.OutQuad))
            .Join(cg.DOFade(0f, 0.3f).SetEase(Ease.OutQuad))
            .AppendCallback(() =>
            {
                InGameManager.Instance.Refresh(InGameRefreshType.RefreshItem);
            })
            .Append(pos.DOLocalMoveY(0f, 0.5f).SetEase(Ease.OutQuad))
            .Join(cg.DOFade(1f, 0.3f).SetEase(Ease.OutQuad));
    }

    #endregion

    #region ////////////아이템 카드 슬롯 선택 트윈/////////////

    public static Sequence SelectItemCardSlotTween(Transform pos)
    {
        pos.DOKill();

        pos.localPosition = Vector3.zero;
        pos.localScale = Vector3.one;
        pos.localRotation = Quaternion.Euler(0,0,0);

        return DOTween.Sequence()
            .Append(pos.DOScale(0.1f, 0.08f).SetEase(Ease.OutQuad).SetRelative(true))
            .Join(pos.DOLocalMoveY(88f, 0.08f).SetEase(Ease.OutQuad).SetRelative(true))
            .Join(pos.DOLocalRotate(new Vector3(0f, 0f, -3.2f), 0.08f).SetEase(Ease.OutQuad).SetRelative(true));
    }


    #endregion
}
