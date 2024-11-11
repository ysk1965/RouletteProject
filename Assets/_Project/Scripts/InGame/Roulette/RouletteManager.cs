using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.TTT.Data;
using CookApps.Playgrounds.Utility;
using UnityEngine;

public class RouletteManager : SingletonMonoBehaviour<RouletteManager>
{
    public bool IsAuto => _isAuto;

    [SerializeField] private List<InGameRouletteData> _inGameRouletteDataList;
    [Range(0, 1f)] [Tooltip("각 룰렛 별 시작 딜레이 값")]
    [SerializeField] private float _rouletteStartDelay = 0.5f;
    [Range(0, 1f)] [Tooltip("각 룰렛 별 멈추는 딜레이 값")]
    [SerializeField] private float _rouletteStopDelay = 0.5f;
    [SerializeField] private RouletteRotateData _rotateData;

    private bool _isAuto;

    protected override void OnAwakeEvent()
    {
        foreach (InGameRouletteData inGameRouletteData in _inGameRouletteDataList)
        {
            inGameRouletteData.RouletteController.Init(inGameRouletteData.RouletteType, _rotateData);
        }
    }

    public RouletteController GetRouletteControllerByType(RouletteType rouletteType)
    {
        return _inGameRouletteDataList.Find(rouletteData => rouletteData.RouletteType == rouletteType).RouletteController;
    }

    public void StartRouletteAll()
    {
        RouletteController methodRoulette = GetRouletteControllerByType(RouletteType.Method);

        if(methodRoulette.CurrentState is not RouletteIdleState) return;

        StartCoroutine(SpinRouletteGroup());
    }

    public void StopRouletteAll()
    {
        RouletteController methodRoulette = GetRouletteControllerByType(RouletteType.Method);

        if(methodRoulette.CurrentState is not RouletteLoopState) return;

        StartCoroutine(StopRouletteGroup());
    }

    public void SetAutoState(bool isAuto)
    {
        _isAuto = isAuto;
    }


    private IEnumerator SpinRouletteGroup()
    {
        _inGameRouletteDataList.Sort();

        for (var i = 0; i < _inGameRouletteDataList.Count; i++)
        {
            InGameRouletteData inGameRouletteData = _inGameRouletteDataList[i];
            inGameRouletteData.RouletteController.StartRoulette();

            if (_rouletteStartDelay > 0)
            {
                yield return new WaitForSeconds(_rouletteStartDelay);
            }
        }
    }

    private IEnumerator StopRouletteGroup()
    {
        for (var i = 0; i < _inGameRouletteDataList.Count; i++)
        {
            InGameRouletteData inGameRouletteData = _inGameRouletteDataList[i];
            inGameRouletteData.RouletteController.StopRoulette();

            if (_rouletteStopDelay > 0)
            {
                yield return new WaitForSeconds(_rouletteStopDelay);
            }
        }
    }

#if UNITY_EDITOR
    public void RefreshRotateData()
    {
        if(Application.isPlaying == false) return;

        foreach (InGameRouletteData inGameRouletteData in _inGameRouletteDataList)
        {
            inGameRouletteData.RouletteController.RefreshRotateData(_rotateData);
        }
    }
#endif
}

[Serializable]
public class InGameRouletteData : Comparer<InGameRouletteData>, IComparable<InGameRouletteData>
{
    public RouletteType RouletteType;
    public RouletteController RouletteController;

    public override int Compare(InGameRouletteData x, InGameRouletteData y)
    {
        if (x == null)
        {
            return 0;
        }

        return y == null ? 0 : x.RouletteType.CompareTo(y.RouletteType);
    }

    public int CompareTo(InGameRouletteData other)
    {
        return other == null ? 0 : RouletteType.CompareTo(other.RouletteType);
    }
}
