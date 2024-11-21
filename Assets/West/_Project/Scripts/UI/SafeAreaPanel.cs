using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private Vector2 lastScreenSize;

    [Header("Offset Settings")]
    public float topOffset = 0f;  // Top 오프셋
    public float bottomOffset = 0f;  // Bottom 오프셋

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafeArea ||
            new Vector2(Screen.width, Screen.height) != lastScreenSize)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        lastScreenSize = new Vector2(Screen.width, Screen.height);

        // Safe Area를 기준으로 비율 계산
        Vector2 anchorMin = safeArea.position / new Vector2(Screen.width, Screen.height);
        Vector2 anchorMax = (safeArea.position + safeArea.size) / new Vector2(Screen.width, Screen.height);

        // 오프셋을 반영 (화면 크기에 비례)
        float screenHeight = Screen.height;
        anchorMin.y += bottomOffset / screenHeight;
        anchorMax.y -= topOffset / screenHeight;

        // RectTransform에 반영
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
