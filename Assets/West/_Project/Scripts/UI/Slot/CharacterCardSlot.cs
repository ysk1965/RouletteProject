using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterCardSlot : MonoBehaviour
{
    [SerializeField] private Animator _slotAnimator;

    [Header("Chracter Info")]
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterNameText;

    [Header("Perk Info")]
    [SerializeField] private Image _perkImage;
    [SerializeField] private TextMeshProUGUI _perkNameText;

    private SpecCharacter _specCharacterData;
    private SpecPerk _specPerkData;

    public bool IsSlotFlip { get; set; } = false;

    public void SetSlot(SpecCharacter data)
    {
        if (data == null) return;

        Clear();

        _specCharacterData = data;
        _specPerkData = SpecDataManager.Instance.GetPerkData(_specCharacterData.perk_id);

        _characterImage.sprite = ImageManager.Instance.GetCharacterIcon(_specCharacterData.character_id);

        _characterNameText.text = _specCharacterData.character_name_key;

        _perkImage.sprite = ImageManager.Instance.GetPerkIcon(_specPerkData.perk_id);
        _perkNameText.text = _specPerkData.perk_name_key;
    }

    public void SetFlipAnimation(bool isFlip)
    {
        IsSlotFlip = isFlip;

        if (IsSlotFlip)
        {
            _slotAnimator.SetTrigger("isFlip_R");
        }
        else
        {
            _slotAnimator.SetTrigger("isFlip_L");
        }
    }

    private void Clear()
    {
        _specCharacterData = null;

        //IsSlotFlip = false;
    }
}
