using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameItemEquipSlot : MonoBehaviour
{
    [SerializeField] private GameObject _onItemObject;
    [SerializeField] private Image _amountBGImage;

    [SerializeField] private Image _itemIconImage;
    [SerializeField] private TextMeshProUGUI _itemNameText_1;
    [SerializeField] private TextMeshProUGUI _itemNameText_2;

    [Space(6)]
    [Header("BackFrame 컬러 프리셋")]
    [SerializeField] private Color _common;
    [SerializeField] private Color _snakeShield;
    [SerializeField] private Color _hammer;

    private GamePlayerData currentGamePlayerData;

    public void SetHammerSlot(int hammerAmount)
    {
        Clear();

        var resultHammerAmount = Mathf.Abs(hammerAmount);

        _itemNameText_1.text = resultHammerAmount.ToString();
        _itemNameText_2.text = resultHammerAmount.ToString();

        _itemIconImage.sprite = ImageManager.Instance.GetGameItemIcon(GameItemType.Hammer);

        _amountBGImage.color = _hammer;

        _onItemObject.SetActive(true);
    }

    public void SetSnakeSlot(int remainTurn)
    {
        Clear();

        _itemNameText_1.text = remainTurn.ToString();
        _itemNameText_2.text = remainTurn.ToString();

        _itemIconImage.sprite = ImageManager.Instance.GetPerkIcon(PerkType.SnakeShield);

        _amountBGImage.color = _snakeShield;

        _onItemObject.SetActive(true);
    }

    private void Clear()
    {
        _onItemObject.SetActive(false);
    }
}
