using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuleTipPopup : Popup
{
    [SerializeField] private Image _ruleIconImage;
    [SerializeField] private TextMeshProUGUI _ruleDescText;

    private SpecRule _specRuleData;

    public void InitPop(int ruleID)
    {
        _specRuleData = SpecDataManager.Instance.GetRuleData(ruleID);

        _ruleIconImage.sprite = ImageManager.Instance.GetRuleIcon(_specRuleData.rule_id);
        _ruleDescText.text = _specRuleData.rule_desc_key;
    }

    public void OnClickCloseButton()
    {
        PopupManager.ClosePopup<RuleTipPopup>();
    }
}
