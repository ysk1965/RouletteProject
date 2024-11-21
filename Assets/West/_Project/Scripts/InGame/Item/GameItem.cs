using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using TMPro;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    [SerializeField] private GameObject _defaultObject;
    [SerializeField] private GameObject _itemImageObject;
    [SerializeField] private GameObject _selectObject;
    [SerializeField] private TextMeshProUGUI _itemNameText;

    private SpecGameItem _specGameItemData;

    private GamePlayerData _currentPlayerData;

    public void SetGameItem(int gameItemID)
    {
        _currentPlayerData = InGameManager.Instance.GetCurrentTurnPlayer().PlayerData;

        bool haveItem = gameItemID > 0;

        if (haveItem) // 아이템이 있는 칸 처리
        {
            _specGameItemData = SpecDataManager.Instance.GetGameItemData(gameItemID);

            _itemNameText.text = _specGameItemData.game_item_id.ToString();
        }
        else // 아이템이 없는 칸 처리
        {
            _itemNameText.text = "Empty";
        }

        _defaultObject.SetActive(!haveItem);
        _itemImageObject.SetActive(haveItem);
        _selectObject.SetActive(false);

        Refresh();
    }

    public void Refresh()
    {
        if (_specGameItemData == null) return;

        //bool isSelected = _specGameItemData.game_item_id == InGameManager.Instance.ItemController.CurrentSelectedItemID;
        //_selectObject.SetActive(isSelected);
    }

    public void OnClickGameItemButton()
    {
        if (_currentPlayerData == null) return;
        if (_specGameItemData == null) return;

        //bool isAlreadySelected = _specGameItemData.game_item_id == InGameManager.Instance.ItemController.CurrentSelectedItemID;

        //InGameManager.Instance.ItemController.CurrentSelectedItemID = isAlreadySelected ? 0 : _specGameItemData.game_item_id;

        InGameManager.Instance.ItemController.RefreshGameItemCardSlotList();
    }
}
