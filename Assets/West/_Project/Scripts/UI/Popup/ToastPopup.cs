using UnityEngine;
using UnityEngine.UI;

public class ToastPopup : MonoBehaviour
{
    [SerializeField]
    private Text _msgText;

    private float _duration;

    public void InitToast(string msg, float time)
    {
        _msgText.text = msg;
        _duration = time;

        Invoke(nameof(OffToast), time);
    }

    private void OffToast()
    {
        ToastManager.Instance.IsShowingToast = false;

        Destroy(gameObject, _duration);
    }
}
