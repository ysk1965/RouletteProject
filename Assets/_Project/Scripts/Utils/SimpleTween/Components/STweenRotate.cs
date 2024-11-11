/*********************************************
 * NHN StarFish - Simple Tween
 * CHOI YOONBIN
 *
 *********************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STweenRotate : STweenBase<Vector3>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public override void Restore()
    {
        base.Restore();
        this.transform.localRotation = Quaternion.Euler(this.start);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides STweenBase

    protected override void PlayTween()
    {
        this.transform.localRotation = Quaternion.Euler(this.start);
        base.PlayTween();

        base.tweenValue = this.tweener.CreateTween(this.start, this.end);
    }

    protected override void UpdateValue(Vector3 value)
    {
        base.UpdateValue(value);
        this.transform.localRotation = Quaternion.Euler(value);

//		Vector3 rotateValue = this.lastValue - value;
//		this.transform.Rotate(rotateValue);
//		this.lastValue = value;
    }
}

public class STweenFlip : MonoBehaviour
{
    /// <summary>
    /// 플립 돌려
    /// </summary>
    /// <param name="t">회전시킬 트랜스폼 </param>
    /// <param name="startVec">시작 벡터</param>
    /// <param name="endVec">끝 벡터</param>
    /// <param name="duration">연출시간</param>
    /// <param name="delay">연출 딜레이</param>
    /// <param name="easingType_start">시작 이징 타입</param>
    /// <param name="easingType_end">끝 이징 타입</param>
    /// <param name="middleCallback">시작과 끝 사이 콜백 </param>
    /// <param name="endCallback">다 끝나고 콜백 </param>
    public static void PlayFlip(Transform t, Vector3 startVec, Vector3 endVec, float duration, float delay, SimpleTweenerEx.EasingType easingType_start = SimpleTweenerEx.EasingType.QuadEasingIn,
        SimpleTweenerEx.EasingType easingType_end = SimpleTweenerEx.EasingType.QuadEasingOut, Action middleCallback = null, Action endCallback = null)
    {
        var prevComp = t.GetComponents<STweenRotate>();

        if (prevComp.Length < 2)
        {
            for (var i = prevComp.Length; i < 2; i++)
            {
                t.gameObject.AddComponent<STweenRotate>();
            }

            prevComp = t.GetComponents<STweenRotate>();
        }


        var startComp = prevComp[0];
        var endComp = prevComp[1];


        startComp.LoopCount = 1;
        startComp.TweenLoopType = SimpleTweener.TweenLoopType.Default;
        startComp.Delay = delay;
        startComp.Duration = duration;
        startComp.StartVec = startVec;
        startComp.EndVec = endVec;
        startComp.EasingType = easingType_start;


        endComp.LoopCount = 1;
        endComp.TweenLoopType = SimpleTweener.TweenLoopType.Default;
        endComp.Delay = delay;
        endComp.Duration = duration;
        endComp.StartVec = endVec;
        endComp.EndVec = startVec;
        endComp.EasingType = easingType_end;


        // startComp
        startComp.SetOnComplete(() =>
        {
            middleCallback?.Invoke();
            endComp.Begin();
        });

        endComp.SetOnComplete(() => { endCallback?.Invoke(); });


        startComp.Begin();
    }

    /// <summary>
    /// 플립 돌려
    /// </summary>
    /// <param name="t">회전시킬 트랜스폼 </param>
    /// <param name="startVec">시작 벡터</param>
    /// <param name="endVec">끝 벡터</param>
    /// <param name="duration">연출시간</param>
    /// <param name="delay">연출 딜레이</param>
    /// <param name="easingType_start">시작 이징 타입</param>
    /// <param name="easingType_end">끝 이징 타입</param>
    /// <param name="middleCallback">시작과 끝 사이 콜백 </param>
    /// <param name="endCallback">다 끝나고 콜백 </param>
    public static void PlayFlip(Transform t, Vector3 startVec, Vector3 endVec, float duration, float delay, SimpleTweenerEx.EasingType easingType_start = SimpleTweenerEx.EasingType.QuadEasingIn,
        SimpleTweenerEx.EasingType easingType_end = SimpleTweenerEx.EasingType.QuadEasingOut, Action<int> middleCallback = null, Action<int> endCallback = null, int index = 0)
    {
        var prevComp = t.GetComponents<STweenRotate>();

        if (prevComp.Length < 2)
        {
            for (var i = prevComp.Length; i < 2; i++)
            {
                t.gameObject.AddComponent<STweenRotate>();
            }

            prevComp = t.GetComponents<STweenRotate>();
        }


        var startComp = prevComp[0];
        var endComp = prevComp[1];


        startComp.LoopCount = 1;
        startComp.TweenLoopType = SimpleTweener.TweenLoopType.Default;
        startComp.Delay = delay;
        startComp.Duration = duration;
        startComp.StartVec = startVec;
        startComp.EndVec = endVec;
        startComp.EasingType = easingType_start;


        endComp.LoopCount = 1;
        endComp.TweenLoopType = SimpleTweener.TweenLoopType.Default;
        endComp.Delay = delay;
        endComp.Duration = duration;
        endComp.StartVec = endVec;
        endComp.EndVec = startVec;
        endComp.EasingType = easingType_end;


        // startComp
        startComp.SetOnComplete(() =>
        {
            middleCallback?.Invoke(index);
            endComp.Begin();
        });

        endComp.SetOnComplete(() => { endCallback?.Invoke(index); });


        startComp.Begin();
    }
}
