using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.U2D;


public class SnakeBreatheAnim : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    public Spline spline;

    public float initHeight = 0.26f;
    public float targetHeight = 0.4f;
    public float duration = 2f;
    public float delayOffset = 0.1f;

    List<Tween> activeTweens = new List<Tween>();

    private void OnEnable()
    {
        //Init();

    }

    private void OnDisable()
    {
        //StopAllTweens();
    }

    public void Init()
    {
        spriteShapeController = GetComponent<SpriteShapeController>();
        if (spriteShapeController is null)
        {
            Debug.LogError("SpriteShapeController is Null");
            return;
        }

        spline = spriteShapeController.spline;

        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            spline.SetHeight(i, initHeight);
        }

        //TweenSnakeBearthe();
    }

    public void TweenSnakeBearthe()
    {

        int allPointCount = spline.GetPointCount();
        if (allPointCount <= 0)
        {
            Debug.LogWarning("Spline에 Control Points가 없음");
            return;
        }
        StopAllTweens();

        for (int i = 0; i < allPointCount; i++)
        {
            int pointIndex = i;
            float initialHeight = spline.GetHeight(pointIndex);
            activeTweens.Add(SnakeBodyTween(initialHeight, pointIndex));
        }


    }

    Tween SnakeBodyTween(float initialHeight, int pointIndex)
    {
        float delay = pointIndex*duration* delayOffset; // 각 포인트에 순차적인 지연 시간 부여

        return DOTween.To(() => initialHeight,
                value =>
                {
                    if (pointIndex < spline.GetPointCount())
                    {
                        spline.SetHeight(pointIndex, value);
                        spriteShapeController.BakeMesh(); // 변경사항 반영
                    }
                }, targetHeight, duration)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(delay);
    }

    void StopAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween.Kill();
        }
        activeTweens.Clear();
    }
}
