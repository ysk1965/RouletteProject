using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using UnityEngine;

public class ToastManager : SingletonMonoBehaviour<ToastManager>
{
    private const float DEFAULT_SHOW_DURATION = 1.5f;

    [SerializeField]
    private Transform _parentTransform;

    [SerializeField]
    private GameObject _toastPrefab;

    public bool IsShowingToast { get; set; } = false;

    public void ShowToast(string tokenKey, float duration = DEFAULT_SHOW_DURATION, bool isForce = false)
    {
        if (IsShowingToast && !isForce)
        {
            return;
        }

        if (isForce)
        {
            OffToast();
        }

        string message = LanguageManager.Instance.GetLanguageText(tokenKey);

        GameObject newToast = Instantiate(_toastPrefab, _parentTransform);
        var toast = newToast.GetComponent<ToastPopup>();

        toast.InitToast(message, duration);

        IsShowingToast = true;
    }

    private void OffToast()
    {
        IsShowingToast = false;

        BMUtil.RemoveChildObjects(_parentTransform.transform);
    }
}
