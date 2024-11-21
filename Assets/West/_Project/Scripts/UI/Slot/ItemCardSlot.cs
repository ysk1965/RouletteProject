using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardSlot : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private int _slotIndex;
    [SerializeField] private GameObject _selectObject;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private CanvasGroup _slotCanvasGroup;

    [Header("Detail")]
    [SerializeField] private GameObject _valueInfoObject;
    [SerializeField] private TextMeshProUGUI _valueInfoText_1;
    [SerializeField] private TextMeshProUGUI _valueInfoText_2;

    private SpecGameItem _specGameItemData;

    public void SetEmptyCardSlot()
    {
        //ClearSlot();

        gameObject.SetActive(false);
    }

    public void SetItemCardSlot(int gameItemID)
    {
        ClearSlot();

        _specGameItemData = SpecDataManager.Instance.GetGameItemData(gameItemID);

        _itemImage.sprite = ImageManager.Instance.GetGameItemIcon(_specGameItemData.game_item_id);
        _itemNameText.text = _specGameItemData.game_item_name_key;

        // 수치 정보 표시 여부
        bool isShowValueInfo = _specGameItemData.game_item_type == GameItemType.Hammer;
        if (isShowValueInfo)
        {
            var itemValue = _specGameItemData.game_item_value.First();
            itemValue = Mathf.Abs(itemValue); // 요청에 따라 부호 제거
            _valueInfoText_1.text = itemValue.ToString();
            _valueInfoText_2.text = itemValue.ToString();
        }

        gameObject.SetActive(true);
        _valueInfoObject.SetActive(isShowValueInfo);
    }

    public void Refresh()
    {
        if (gameObject.activeInHierarchy == false) return;
        if (_specGameItemData == null) return;

        bool isSelected = _slotIndex == InGameManager.Instance.ItemController.CurrentSelectedItemSlot;
        _selectObject.SetActive(isSelected);

        transform.DOKill();

        if (isSelected)
        {
            TweenUtil.SelectItemCardSlotTween(transform);
        }
        else
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(0,0,0);
        }
    }

    // 아이템 카드 슬롯이 사라지는 연출시 사용
    public void PlayDisapperAnim()
    {
        if (_slotCanvasGroup == null) return;

        TweenUtil.UseItemCardSlotFadeTween(transform, _slotCanvasGroup);
    }

    private void ClearSlot()
    {
        _specGameItemData = null;

        _itemImage.sprite = null;
        _itemNameText.text = "Empty";

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(0,0,0);

        _selectObject.SetActive(false);
        _valueInfoObject.SetActive(false);
    }
}
