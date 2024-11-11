using UnityEditor;
using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

#if UNITY_EDITOR
        SafeAreaPanel[] panels = GetComponentsInParent<SafeAreaPanel>();
        if (panels.Length > 1)
        {
            UnityEngine.Debug.LogError("SafeArea가 중복 적용되었습니다. 확인 후 제거해주세요.", this);

            foreach (SafeAreaPanel safeAreaPanel in panels)
            {
                Transform parent = safeAreaPanel.transform;
                string path = transform.name;
                while (parent != null)
                {
                    parent = parent.parent;
                    path = parent.name + "/" + path;
                }

                Debug.LogError(path);
            }
        }
#endif
    }

    private void OnEnable()
    {
        // SafeAreaDetection.OnSafeAreaChanged += RefreshPanel;
        RefreshPanel(Screen.safeArea);
    }

    // private void OnDisable()
    // {
    //     SafeAreaDetection.OnSafeAreaChanged -= RefreshPanel;
    //     //RefreshPanel(SafeAreaDetection.safeArea);
    // }

    private void RefreshPanel(Rect safeArea)
    {
        if (!_rectTransform)
        {
            return;
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        Vector2 gameViewSize = GetMainGameViewSize();
        var width = (int) gameViewSize.x;
        var height = (int) gameViewSize.y;

        anchorMin.x /= width;
        anchorMin.y /= height;
        anchorMax.x /= width;
        anchorMax.y /= height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }

    private static Vector2 GetMainGameViewSize()
    {
#if UNITY_EDITOR
        //  EDITOR에서 Screen.width를 사용할 경우 마지막 선택한 창의 Size가 넘어온다.
        //  그 마지막 창이 GameView가 아닐 수 있음!
        //  ex : SROption을 클릭하여 SafeArea가 호출된 경우 SROptionWindow의 Size가 넘어온다.
        string[] res = UnityStats.screenRes.Split('x');
        return new Vector2(int.Parse(res[0]), int.Parse(res[1]));
#else
        return new Vector2(Screen.width, Screen.height);
#endif
    }
}
