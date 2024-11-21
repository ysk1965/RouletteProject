using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTimeCounter : MonoBehaviour
{
    [SerializeField] private GameObject _numberParentObject;

    [SerializeField] private List<GameObject> _numberObjectList = new ();

    public void SetNumberCount(int targetNumber)
    {
        if (targetNumber < 0) return;
        if (targetNumber > _numberObjectList.Count) return;

        Clear();

        _numberParentObject.SetActive(true);

        _numberObjectList[targetNumber].SetActive(true);
        TweenUtil.NumbChangeTween(_numberObjectList[targetNumber].transform);
    }

    private void Clear()
    {
        _numberParentObject.SetActive(false);

        _numberObjectList.ForEach(number => number.SetActive(false));
    }
}
