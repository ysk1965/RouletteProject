using UnityEngine;

public class STweenRotateGroup : STweenBase<Vector3>
{
    [SerializeField] private Transform[] _transforms;
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public override void Restore()
    {
        base.Restore();
        foreach (Transform tr in _transforms)
        {
            if (tr == false) continue;
            tr.localRotation = Quaternion.Euler(this.start);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides STweenBase 

    protected override void PlayTween()
    {
        foreach (Transform tr in _transforms)
        {
            if (tr == false) continue;
            tr.localRotation = Quaternion.Euler(this.start);
        }

        base.PlayTween();

        base.tweenValue = this.tweener.CreateTween(this.start, this.end);
    }

    protected override void UpdateValue(Vector3 value)
    {
        base.UpdateValue(value);
        foreach (Transform tr in _transforms)
        {
            if (tr == false) continue;
            tr.localRotation = Quaternion.Euler(value);
        }
    }
}