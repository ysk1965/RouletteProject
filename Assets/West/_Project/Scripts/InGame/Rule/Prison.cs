using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Prison : MonoBehaviour
{
    [SerializeField] private Animator _prisonAnimator;

    [SerializeField] private GameObject _prisonDiceUIObject;
    [SerializeField] private SpriteRenderer _prisonDiceSprite;
    [SerializeField] private TextMeshPro _escapeCountText;

    public void SetPrisonDice(int diceNumber)
    {
        OnOffPrisonDiceUI(true);

        _prisonDiceSprite.sprite = ImageManager.Instance.GetPrisonDiceIcon(diceNumber);
    }

    public void PlayPrisonEscapeAnim()
    {
        _prisonAnimator.SetTrigger("Prison_End");
    }

    public void UpdatePrisonEscapeCount(int count)
    {
        _escapeCountText.text = count.ToString();
    }

    public void OnOffPrisonDiceUI(bool isOn)
    {
        _prisonDiceUIObject.SetActive(isOn);
    }
}
