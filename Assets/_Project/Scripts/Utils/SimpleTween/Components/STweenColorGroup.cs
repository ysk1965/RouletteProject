using UnityEngine;
using UnityEngine.UI;

public class STweenColorGroup : STweenBase<float>
{
    [SerializeField] private Graphic[] _graphics;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public float Value
    {
        get { return tweenValue.Value; }
    }

    public override void Restore()
    {
        base.Restore();

        foreach (Graphic gr in _graphics)
        {
            if (gr)
            {
                Color color = gr.color;
                color.a = start;
                gr.color = color;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides STweenBase

    protected override void PlayTween()
    {
        base.PlayTween();
        this.SetValue(this.start);
        base.tweenValue = this.tweener.CreateTween(this.start, this.end);
    }

    protected override void UpdateValue(float value)
    {
        base.UpdateValue(value);
        this.SetValue(value);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // private 

    private void SetValue(float alpha)
    {
        foreach (Graphic gr in _graphics)
        {
            if (gr)
            {
                Color color = gr.color;
                color.a = alpha;
                gr.color = color;
            }
        }
    }
}