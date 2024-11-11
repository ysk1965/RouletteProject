using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;
    [FormerlySerializedAs("DartObject")]
    [SerializeField] private GameObject _dartObject;

    [SerializeField] private List<Transform> DartPinList = new List<Transform>();

    [SerializeField] private List<GameObject> _dartBoard;

    public List<GameObject> DartBoard => _dartBoard;

    private GameObject _currentDartPin;

    public void CreateDart()
    {
        int index = Random.Range(0, DartPinList.Count);
        GameObject dart = Instantiate(_dartObject, DartPinList[index].position, Quaternion.Euler(0, 180, 0), DartPinList[index]);
        _currentDartPin = dart;
    }

    public void DestroyDart()
    {
        if(_currentDartPin != null)
            Destroy(_currentDartPin);
    }

    public void OnClickShot()
    {
        _director.Play();
    }
}
