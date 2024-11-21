using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshPro _mainNameText;
    [SerializeField] private TextMeshPro _subNameText;

    [Space]
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private GameObject _crownObject;

    public void SetPlayerName(string name)
    {
        Clear();

        _mainNameText.text = name;
        _subNameText.text = name;
    }

    public void OnOffPlayerCrown(bool isOn)
    {
        if (isOn && _crownObject.activeInHierarchy)
        {
            return;
        }

        float targetX = isOn ? 1.0f : 0;
        _rectTransform.DOAnchorPosX(targetX, 0.4f).SetEase(Ease.OutBack);

        _crownObject.SetActive(isOn);
    }

    private void Clear()
    {
        _crownObject.SetActive(false);
    }
}
