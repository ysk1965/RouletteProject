using System;
using System.Collections.Generic;
using CookApps.BM.TTT.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class RouletteController : MonoBehaviour
{
    public RouletteRotateData RotateData => _rotateData;

    public float PinOffset => _pinOffset;
    public Transform RouletteFrame => _rouletteFrame;
    public Transform RouletteSpinFrame => _rouletteSpinFrame;

    public RouletteState CurrentState => _currentState;

    [SerializeField]
    private Transform _rouletteFrame;

    [SerializeField]
    private Transform _rouletteSpinFrame;

    [SerializeField]
    private GameObject _resultObject;

    [Range(-180, 180)]
    [SerializeField]
    private float _pinOffset;

    private RouletteState _currentState;
    public List<RouletteItemData> RouletteItemDataList { get; private set; } = new();
    public RouletteType RouletteType { get; private set; }
    private RouletteRotateData _rotateData;

    private void Start()
    {
        // Result Object 초기 비활성화
        if (_resultObject != null)
        {
            _resultObject.SetActive(false);
        }
    }

    public void Init(RouletteType rouletteType, RouletteRotateData rotateData)
    {
        RouletteType = rouletteType;
        _rotateData = rotateData;

        SetState(new RouletteIdleState(this));

        List<SpecRouletteSet> rouletteSetList = SpecDataManager.Instance.GetRouletteSetList(1);
        switch (rouletteType)
        {
            case RouletteType.Reward:
            {
                var index = 0;
                for (var i = 0; i < rouletteSetList.Count; ++i)
                {
                    if (rouletteSetList[i].roulette_id == 1)
                    {
                        RouletteItemDataList.Add(new RouletteItemData(index,
                            _rouletteSpinFrame.transform.Find($"Dart_Board_Piece_{index + 1}").gameObject,
                            60f * index));
                        ++index;
                    }
                }

                break;
            }
            case RouletteType.Multiply:
            {
                var index = 0;
                for (var i = 0; i < rouletteSetList.Count; ++i)
                {
                    if (rouletteSetList[i].roulette_id == 2)
                    {
                        RouletteItemDataList.Add(new RouletteItemData(index,
                            _rouletteSpinFrame.transform.Find($"Dart_Board_Piece_{index + 1}").gameObject,
                            60f * index));
                        ++index;
                    }
                }

                break;
            }
            case RouletteType.Method:
            {
                var index = 0;
                for (var i = 0; i < rouletteSetList.Count; ++i)
                {
                    if (rouletteSetList[i].roulette_id == 3)
                    {
                        RouletteItemDataList.Add(new RouletteItemData(index,
                            _rouletteSpinFrame.transform.Find($"Dart_Board_Piece_{index + 1}").gameObject,
                            60f * index));
                        ++index;
                    }
                }

                break;
            }
        }
    }

    public void RefreshRotateData(RouletteRotateData rouletteRotateData)
    {
        _rotateData = rouletteRotateData;
    }

    public void StartRoulette()
    {
        SetState(new RouletteLoopState(this));
    }

    public void StopRoulette()
    {
        SetState(new RouletteStopState(this));
    }

    public void SetState(RouletteState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    private void Update()
    {
        _currentState?.OnUpdate();
    }
}

[Serializable]
public class RouletteRotateData
{
    [Header("Start State Rotate Data")]
    [Range(0, 100)]
    [Tooltip("돌아가기 전 반시계 방향으로 돌아갈 각도")]
    public float StartRotationAngle = 30f;

    [Range(0.1f, 1)]
    [Tooltip("돌아가기 전 반시계 방향으로 회전하는데 걸리는 시간")]
    public float StartRotationDuration = 0.5f;

    [Space(10)]
    [Header("Loop State Rotate Data")]
    [Range(1f, 100f)]
    [Tooltip("루프 상태일 때 회전 속도 (초당 회전 바퀴 수)")]
    public float LoopRotationSpeed = 2f;

    [Space(10)]
    [Header("Stop State Rotate Data")]
    [Range(0.1f, 1)]
    [Tooltip("룰렛이 멈추기 전에 돌아가는 시간")]
    public float StopDuration = 0.3f;

    [Range(0, 20)]
    [Tooltip("룰렛이 완전히 멈추기 전에 감속하면서 회전할 횟수")]
    public int AdditionalSpinCount = 3;

    [Range(0.01f, 0.5f)]
    [Tooltip("룰렛이 감속 할 때 각 회전 수 마다 추가되는 회전 시간 => 가속도 조절용, AdditionalSpinCount가 0보다 클 때만 적용")]
    public float AdditionalSpinDuration = 0.5f;
}

public class RouletteItemData
{
    public RouletteItemData(int index, GameObject rouletteObject, float rotateAngle)
    {
        Index = index;
        RotateAngle = rotateAngle;
        RouletteObject = rouletteObject;
    }

    public int Index;
    public float RotateAngle;
    public GameObject RouletteObject;
}
