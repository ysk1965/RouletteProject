using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;

public class PerkAppearEffect : MonoBehaviour
{
    [SerializeField] private float _perkDurationTime = 2f;

    [Space]
    [SerializeField] private TextMeshProUGUI _perkNameText;
    [SerializeField] private TextMeshProUGUI _perkDescText;

    private SpecPerk _specPerkData;

    public void SetPerk(SpecPerk data)
    {
        if (data == null) return;

        _specPerkData = data;

        _perkNameText.text = $"<bounce><rainb>{_specPerkData.perk_name_key}</rainb></bounce>";
        //_perkDescText.text = _specPerkData.perk_desc_key;

        Invoke(nameof(OffEffect), _perkDurationTime);
    }

    private void OffEffect()
    {
        gameObject.SetActive(false);
    }
}
