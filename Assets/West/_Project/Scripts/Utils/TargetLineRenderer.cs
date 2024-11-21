using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetLineRenderer : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _minHeight = 2f;
    [SerializeField] private float _maxHeight = 5f;
    [SerializeField] private float _durationTime = 1f;

    private WaitForEndOfFrame _waitForEndOfFrame;

    private void Awake()
    {
        _waitForEndOfFrame = new WaitForEndOfFrame();
    }

    public void DrawLineObjectToUI(Transform startTransform, RectTransform targetUI, Action OnComplete = null)
    {
        // TrailRenderer 완전 초기화
        StartCoroutine(ResetTrailRendererAndStart(startTransform, targetUI, OnComplete));
    }

    private IEnumerator ResetTrailRendererAndStart(Transform startTransform, RectTransform targetUI, Action OnComplete)
    {
        _particleSystem.transform.position = startTransform.position;
        _trailRenderer.transform.position = startTransform.position;
        yield return null; // 한 프레임 대기하여 TrailRenderer 비활성화 반영

        _particleSystem.Clear();
        _trailRenderer.Clear();

        StartCoroutine(DrawParabolicPath(startTransform, targetUI, _durationTime, OnComplete));
    }

    private IEnumerator DrawParabolicPath(Transform startTransform, RectTransform targetUI, float duration,
        Action OnComplete = null)
    {
        Camera mainCamera = Camera.main;

        var time = 0f;
        float randomHeight = Random.Range(_minHeight, _maxHeight);
        Vector3 startPosition = startTransform.position;

        // Canvas의 실제 화면 깊이를 계산
        float canvasPlaneZ = mainCamera.nearClipPlane + 1f;

        while (time < duration)
        {
            // 매 프레임마다 UI의 월드 좌표를 계산
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetUI.position);

            // Z값을 명확히 지정하여 월드 좌표로 변환
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, canvasPlaneZ));

            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);

            // 포물선 경로 계산
            Vector3 currentPos = CalculateParabola(startPosition, worldPos, randomHeight, t);
            _trailRenderer.transform.position = currentPos;
            _particleSystem.transform.position = currentPos;

            yield return _waitForEndOfFrame;
        }

        // 최종 위치 보정
        Vector2 finalScreenPos = RectTransformUtility.WorldToScreenPoint(null, targetUI.position);
        Vector3 finalWorldPos =
            mainCamera.ScreenToWorldPoint(new Vector3(finalScreenPos.x, finalScreenPos.y, canvasPlaneZ));

        _trailRenderer.transform.position = finalWorldPos;
        _particleSystem.transform.position = finalWorldPos;
        OnComplete?.Invoke();
    }

    private Vector3 CalculateParabola(Vector3 start, Vector3 end, float height, float t)
    {
        // 두 점 사이의 중간 지점을 기준으로 최고점을 계산하여 포물선을 생성
        float parabolaHeight = height * 4f * t * (1 - t);
        Vector3 flatPosition = Vector3.Lerp(start, end, t);
        return new Vector3(flatPosition.x, flatPosition.y + parabolaHeight, flatPosition.z);
    }
}
