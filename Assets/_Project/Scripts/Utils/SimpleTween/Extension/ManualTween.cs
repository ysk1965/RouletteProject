using UnityEngine;

public class ManualTween<T>
{
    public bool Playing => _tweener.Playing;

    public T Value => _tweenValue.Value;

    public float Duration
    {
        get => _duration;
        set => _duration = value;
    }

    public float Delay
    {
        get => _delay;
        set => _delay = value;
    }

    public T Start
    {
        get => _start;
        set => _start = value;
    }
    
    public T End
    {
        get => _end;
        set => _end = value;
    }
    
    public SimpleTweenerEx.EasingType EasingType
    {
        get => _easingType;
        set => _easingType = value;
    }

    [SerializeField] protected T _start;
    [SerializeField] protected T _end;

    [SerializeField] private float _delay;
    [SerializeField] private float _duration;

    [SerializeField] private SimpleTweenerEx.EasingType _easingType = SimpleTweenerEx.EasingType.LinearEasing;

    [SerializeField] private SimpleTweener.TweenLoopType _tweenLoopType = SimpleTweener.TweenLoopType.Default;
    [SerializeField] private int _loopCount = 1;

    protected TweenLerp<T> _tweenValue;
    protected SimpleTweenerEx _tweener = new SimpleTweenerEx();

    public virtual void PlayTween()
    {
        _tweener.Reset(_duration, _easingType);
        _tweener.LoopType(_tweenLoopType, _loopCount);
        _tweener.Delay(this._delay);
    }

    public virtual void Update(float dt)
    {
        _tweener.Update(dt);
    }
}