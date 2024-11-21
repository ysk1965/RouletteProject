using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuleChangeToastPopup : Popup
{
    [Space]
    [SerializeField] private Transform _rootTransform;
    [SerializeField] private Image _ruleIconImage;
    [SerializeField] private TextMeshProUGUI _ruleDescText;

    private SpecRule _specRuleData;

    public void InitPop(int ruleID)
    {
        if (ruleID <= 0) return;

        _specRuleData = SpecDataManager.Instance.GetRuleData(ruleID);

        _ruleIconImage.sprite = ImageManager.Instance.GetRuleIcon(_specRuleData.rule_id);
        _ruleDescText.text = _specRuleData.rule_desc_key;

        TweenUtil.OpenToastTopTween(_rootTransform, ClosePopup);

        //Invoke(nameof(CloseTween), 2.0f);
        //Invoke(nameof(ClosePopup), 3.0f);
    }

    private void ClosePopup()
    {
        PopupManager.ClosePopup<RuleChangeToastPopup>();
    }

    private void CloseTween()
    {
        TweenUtil.CloseToastTopTween(_rootTransform);
    }
}
