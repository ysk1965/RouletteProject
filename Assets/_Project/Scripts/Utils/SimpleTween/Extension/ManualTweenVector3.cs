using System;
using UnityEngine;

[Serializable]
public class ManualTweenVector3 : ManualTween<Vector3>
{
    public override void PlayTween()
    {
        base.PlayTween();
        _tweenValue = _tweener.CreateTween(_start, _end);
    }
}