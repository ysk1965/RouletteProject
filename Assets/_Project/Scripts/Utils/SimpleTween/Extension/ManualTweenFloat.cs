using System;

[Serializable]
public class ManualTweenFloat : ManualTween<float>
{
    public override void PlayTween()
    {
        base.PlayTween();
        _tweenValue = _tweener.CreateTween(_start, _end);
    }
}