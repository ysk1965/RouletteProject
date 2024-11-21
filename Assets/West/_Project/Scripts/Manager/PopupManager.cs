using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    public Transform popupLayerTr = null;

    Dictionary<Type, Popup> popupDic = new Dictionary<Type, Popup>();
    Stack<Popup> popupStack = new Stack<Popup>();

    public static T GetPopup<T>() where T : Popup
    {
        if (Instance.popupDic == null || Instance.popupDic.Count < 1) { return null; }

        var t = typeof(T);
        if (Instance.popupDic.ContainsKey(t) == false) { return null; }

        return Instance.popupDic[t] as T;
    }

    public static T OpenPopup<T>() where T : Popup
    {
        if (IsPopupOpened<T>()) { return GetPopup<T>(); }

        string popupPath = string.Concat(Define.GetFilePath(FilePath.PoupPath), typeof(T).ToString());

        GameObject popupPrefab = ResourceManager.GetResource<GameObject>(popupPath);
        if (popupPrefab == null) { return null; }

        GameObject newPopup = Instantiate(popupPrefab, Instance.popupLayerTr);
        if (newPopup == null) { return null; }

        T result = newPopup.GetComponent<T>();
        if (result == null) { return null; }

        result.Init();
        Instance.popupDic.Add(typeof(T), result);

        Instance.popupStack.Push(result);

        return result;
    }

    public static T OpenPopup<T>(string popupName) where T : Popup
    {
        if (IsPopupOpened<T>()) { return GetPopup<T>(); }

        string popupPath = string.Concat(Define.GetFilePath(FilePath.PoupPath), popupName);

        GameObject popupPrefab = ResourceManager.GetResource<GameObject>(popupPath);
        if (popupPrefab == null) { return null; }

        GameObject newPopup = Instantiate(popupPrefab, Instance.popupLayerTr);
        if (newPopup == null) { return null; }

        T result = newPopup.GetComponent<T>();
        if (result == null) { return null; }

        result.Init();
        Instance.popupDic.Add(typeof(T), result);

        Instance.popupStack.Push(result);

        return result;
    }

    public static void RefreshPopup<T>() where T : Popup
    {
        T targetPopup = GetPopup<T>();
        if (targetPopup != null)
        {
            targetPopup.Refresh();
        }
    }

    public static void RefreshAllPopup()
    {
        foreach (var popup in Instance.popupDic)
        {
            popup.Value.Refresh();
        }
    }

    public static bool IsPopupOpened<T>() where T : Popup
    {
        return GetPopup<T>() != null;
    }

    public static bool ClosePopup<T>() where T : Popup
    {
        T targetPopup = GetPopup<T>();
        if (targetPopup == null) { return false; }

        Instance.popupDic.Remove(typeof(T));
        //targetPopup.ClosePopup();

        Instance.popupStack.Pop();

        Destroy(targetPopup.gameObject);

        return true;
    }

    // 남은 팝업의 갯수를 리턴
    public static int CloseLatestPopup()
    {
        int remainCount = Instance.popupStack.Count;
        if (remainCount > 0)
        {
            Popup popup = Instance.popupStack.Pop();
            popup.ClosePopup();

            Instance.popupDic.Remove(popup.GetType());
        }

        return remainCount;
    }

    public static bool CloseAllPopup()
    {
        if (Instance.popupDic == null || Instance.popupDic.Count < 1) { return false; }

        foreach (KeyValuePair<Type, Popup> popup in Instance.popupDic)
        {
            popup.Value.ClosePopup();
        }

        Instance.popupDic.Clear();
        Instance.popupStack.Clear();

        return true;
    }
}
