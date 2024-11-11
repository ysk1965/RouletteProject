/*********************************************
 * CHOI YOONBIN
 *********************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class STweenSlide : STweenBase<float>
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public float Value
    {
        get { return tweenValue.Value; }
    }

    public override void Restore()
    {
        base.Restore();

        if (this.slider != null)
            this.slider.value = this.start;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides STweenBase 

    protected override void Start()
    {
        this.slider = this.GetComponent<Slider>();
        base.Start();
    }

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

    private Slider slider;

    private void SetValue(float value)
    {
        if (this.slider != null)
            this.slider.value = value;
    }

}
