using System;
using UnityEngine;
using UnityEngine.U2D;

public class MoveAlongSpline : MonoBehaviour
{
    public SpriteShapeController snakeShape;
    public float speed = 2f;
    public AnimationCurve movementCurve;

    [Header("Opacity Curve")]
    public AnimationCurve opacityCurve; // 오퍼시티 조정을 위한 커브

    [Header("Scale Curve")]
    public AnimationCurve scaleCurve;   // 스케일 조정을 위한 커브

    private float t = 0; // Spline 진행 비율
    private int segmentCount;
    private Vector3 shapeOffset;
    private Vector3 shapeScale;
    private SpriteRenderer spriteRenderer;

    public bool IsPlaySwallowAnim { get; private set; } = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void PlaySwallowAnim()
    {
        IsPlaySwallowAnim = true;

        segmentCount = snakeShape.spline.GetPointCount() - 1;
        shapeOffset = snakeShape.transform.position;
        shapeScale = snakeShape.transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // 기본 커브 설정 (필요시 Inspector에서 수정 가능)
        if (movementCurve == null || movementCurve.keys.Length == 0)
        {
            movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        if (opacityCurve == null || opacityCurve.keys.Length == 0)
        {
            opacityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 시작에서 1, 끝에서 0으로 감소
        }

        if (scaleCurve == null || scaleCurve.keys.Length == 0)
        {
            scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 시작에서 1, 끝에서 0으로 감소
        }
    }

    private void Update()
    {
        if (IsPlaySwallowAnim == false) return;

        t += Time.deltaTime * speed / segmentCount;
        if (t > 1)
        {
            t = 0;
            IsPlaySwallowAnim = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
        }

        float adjustedT = movementCurve.Evaluate(t);
        Vector3 positionOnSpline = GetSmoothPositionOnSpline(adjustedT);
        transform.position = Vector3.Scale(positionOnSpline, shapeScale) + shapeOffset;

        UpdateOpacityAndScale(adjustedT);
    }

    private Vector3 GetSmoothPositionOnSpline(float t)
    {
        int startPointIndex = Mathf.FloorToInt(t * segmentCount);
        int endPointIndex = Mathf.Min(startPointIndex + 1, segmentCount);

        Vector3 p0 = snakeShape.spline.GetPosition(Mathf.Max(startPointIndex - 1, 0));
        Vector3 p1 = snakeShape.spline.GetPosition(startPointIndex);
        Vector3 p2 = snakeShape.spline.GetPosition(endPointIndex);
        Vector3 p3 = snakeShape.spline.GetPosition(Mathf.Min(endPointIndex + 1, segmentCount));

        float localT = (t * segmentCount) - startPointIndex;
        return CatmullRom(p0, p1, p2, p3, localT);
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * ((2 * p1) +
                       (-p0 + p2) * t +
                       (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                       (-p0 + 3 * p1 - 3 * p2 + p3) * t3);
    }

    private void UpdateOpacityAndScale(float adjustedT)
    {
        // 오퍼시티 계산
        Color color = spriteRenderer.color;
        color.a = opacityCurve.Evaluate(adjustedT); // 커브를 통해 오퍼시티 조정
        spriteRenderer.color = color;

        // 스케일 조정
        float scale = scaleCurve.Evaluate(adjustedT); // 커브를 통해 스케일 조정
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
