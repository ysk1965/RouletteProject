using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuleEntryPop : Popup
{
    private const float POPUP_CLOSE_DELAY_TIME = 5.0f;

    [SerializeField] private Image _ruleIconImage;
    [SerializeField] private TextMeshProUGUI _ruleNameText;
    [SerializeField] private TextMeshProUGUI _ruleDescText;

    private SpecRule _ruleData;

    public void SetRuleEntryPop(SpecRule data)
    {
        if (data == null) return;

        _ruleData = data;

        _ruleIconImage.sprite = ImageManager.Instance.GetRuleIcon(_ruleData.rule_id);
        _ruleNameText.text = _ruleData.rule_name_key;
        _ruleDescText.text = _ruleData.rule_desc_key;

        Invoke(nameof(ClosePopup), POPUP_CLOSE_DELAY_TIME);
    }

    private void ClosePopup()
    {
        PopupManager.ClosePopup<RuleEntryPop>();
    }
}
