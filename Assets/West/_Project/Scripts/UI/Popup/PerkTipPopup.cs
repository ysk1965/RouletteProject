using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkTipPopup : Popup
{
    [SerializeField] private Image _perkIconImage;
    [SerializeField] private TextMeshProUGUI _perkNameText;
    [SerializeField] private TextMeshProUGUI _perkDescText;

    private SpecPerk _specPerkData;

    public void InitPop(int perkID)
    {
        _specPerkData = SpecDataManager.Instance.GetPerkData(perkID);

        _perkIconImage.sprite = ImageManager.Instance.GetPerkIcon(_specPerkData.perk_id);

        _perkNameText.text = _specPerkData.perk_name_key;
        _perkDescText.text = _specPerkData.perk_desc_key;
    }

    public void OnClickCloseButton()
    {
        PopupManager.ClosePopup<PerkTipPopup>();
    }
}
