using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IngamePerkSlot : MonoBehaviour
{
    [SerializeField] private Image _perkIconImage;
    [SerializeField] private CanvasGroup _cg;

    private SpecPerk _specPerkData;

    public void SetSlot(int perkID)
    {
        _specPerkData = SpecDataManager.Instance.GetPerkData(perkID);

        _perkIconImage.sprite = ImageManager.Instance.GetPerkIcon(perkID);
    }

    public void OnClickPerkSlot()
    {
        var perkTipPopup = PopupManager.OpenPopup<PerkTipPopup>();
        perkTipPopup.InitPop(_specPerkData.perk_id);
    }

    private void OnEnable()
    {
        IngameEntrySet();
    }

    private Sequence IngameEntrySet()
    {
        transform.DOKill();
        _cg.alpha = 0;

        return DOTween.Sequence()
            .Prepend(transform.DOLocalMoveY(140f, 0f).SetRelative(true))
            .Prepend(transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0f))
            .AppendInterval(7.8f)
            .Append(transform.DOLocalMoveY(-150f, 0.25f).SetRelative(true).SetEase(Ease.OutQuad))
            .Join(_cg.DOFade(1f, 0.2f).SetEase(Ease.OutQuad))
            .Join(transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad))
            .Append(transform.DOLocalMoveY(12f, 0.25f).SetRelative(true).SetEase(Ease.OutQuad));
           
            
    }
}
