using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.TTT.UI.Hud;
using CookApps.Playgrounds.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CookApps.BM.TTT.UI
{
    public class MainUI : SceneUIBase
    {
        [SerializeField]
        protected HudItemController hudItemController;

        // private const int MAX_DART_AMOUNT = 60;
        //
        // [Header("User Data Info")]
        // [SerializeField]
        // private Text _userLevelText;
        //
        // [SerializeField]
        // private Text _userCoinAmountText;
        //
        // [SerializeField]
        // private Text _userDartAmountText;
        //
        // [SerializeField]
        // private Text _userDartExtraAmountText;
        //
        // [SerializeField]
        // private Slider _userDartAmountSlider;
        //
        // [SerializeField]
        // private List<GameObject> _userShieldObjects;
        //
        // [Space(10)]
        // [Header("Bottom Buttons")]
        // [SerializeField]
        // private GameObject _shotButtonObject;
        //
        // [SerializeField]
        // private GameObject _autoShotButtonObject;
        //
        // [SerializeField]
        // private GameObject _multipleButtonObject;
        //
        // [SerializeField]
        // private GameObject _maxMultipleButtonObject;
        //
        // [SerializeField]
        // private Slider _shotButtonPressedSlider;
        //
        // [SerializeField]
        // private Slider _multipleButtonPressedSlider;
        //
        // private void Awake()
        // {
        // }
        //
        // public void OnClickShotButton()
        // {
        //     Debug.Log("Click Shot Button!");
        //
        //     if (_autoShotButtonObject.activeSelf)
        //     {
        //         _autoShotButtonObject.SetActive(false);
        //         _shotButtonObject.SetActive(true);
        //         return;
        //     }
        // }
        //
        // public void OnPressedShotButton()
        // {
        //     Debug.Log("Pressed Shot Button!");
        //
        //     _shotButtonPressedSlider.DOValue(1, 2).OnComplete(() =>
        //     {
        //         _shotButtonObject.SetActive(false);
        //         _autoShotButtonObject.SetActive(true);
        //     });
        // }
        //
        // public void OnClickMultipleButton()
        // {
        //     Debug.Log("Click Multiple Button!");
        //
        //     if (_maxMultipleButtonObject.activeSelf)
        //     {
        //         _maxMultipleButtonObject.SetActive(false);
        //         _multipleButtonObject.SetActive(true);
        //         return;
        //     }
        // }
        //
        // public void OnPressedMultipleButton()
        // {
        //     Debug.Log("Pressed Multiple Button!");
        //
        //     _multipleButtonPressedSlider.DOValue(1, 2).OnComplete(() =>
        //     {
        //         _multipleButtonObject.SetActive(false);
        //         _maxMultipleButtonObject.SetActive(true);
        //     });
        // }
    }
}
