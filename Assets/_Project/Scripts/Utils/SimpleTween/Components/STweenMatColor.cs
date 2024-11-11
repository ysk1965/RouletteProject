using UnityEngine;
using UnityEngine.UI;

public class STweenMatColor : STweenBase<Color>
{
    [SerializeField] private string _key;
    [SerializeField] private Renderer _targetRenderer;
    //[SerializeField] private string _key;

    private MaterialPropertyBlock _block;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public Color Value
    {
        get { return tweenValue.Value; }
    }

    public override void Restore()
    {
        base.Restore();

        SetValue(this.start);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides STweenBase 

    protected override void PlayTween()
    {
        base.PlayTween();
        this.SetValue(this.start);
        base.tweenValue = this.tweener.CreateTween(this.start, this.end);
    }

    protected override void UpdateValue(Color value)
    {
        base.UpdateValue(value);
        this.SetValue(value);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // private 


    private void SetValue(Color color)
    {
        if (_targetRenderer == false) return;

        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
        }
        
        _block.SetColor(_key, color);
        _targetRenderer.SetPropertyBlock(_block);
    }
}