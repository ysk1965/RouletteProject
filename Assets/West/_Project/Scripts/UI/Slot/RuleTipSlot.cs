using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using UnityEngine;
using UnityEngine.UI;

public class RuleTipSlot : MonoBehaviour
{
    [SerializeField] private GameObject _onSlotObject;
    [SerializeField] private GameObject _offSlotObject;

    [SerializeField] private Image _ruleIconImage;

    private SpecRule _specRuleData;

    private bool _isActivate = false;

    private void Start()
    {
        Clear();
    }

    public void SetSlot(int ruleID)
    {
        Clear();

        _isActivate = true;

        _specRuleData = SpecDataManager.Instance.GetRuleData(ruleID);

        _ruleIconImage.sprite = ImageManager.Instance.GetRuleIcon(_specRuleData.rule_id);

        _onSlotObject.SetActive(_isActivate);
        _offSlotObject.SetActive(!_isActivate);
    }

    public void OnClickSlot()
    {
        if (_isActivate == false) return;

        var ruleTipPopup = PopupManager.OpenPopup<RuleTipPopup>();
        ruleTipPopup.InitPop(_specRuleData.rule_id);
    }

    private void Clear()
    {
        _isActivate = false;

        _onSlotObject.SetActive(false);
        _offSlotObject.SetActive(true);
    }
}
