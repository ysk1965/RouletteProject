using CookApps.BM.TTT.Data;
using CookApps.BM.TTT.UI.Hud;
using CookApps.Playgrounds.Utility;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public interface IRouletteState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}

public abstract class RouletteState : IRouletteState
{
    protected readonly RouletteController _controller;

    protected RouletteState(RouletteController controller)
    {
        _controller = controller;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}

public class RouletteIdleState : RouletteState
{
    public RouletteIdleState(RouletteController controller) : base(controller)
    {
    }

    public override void OnEnter()
    {
        if (RouletteManager.Instance.IsAuto)
        {
            CoroutineTaskManager.RunDeferred(new WaitForSeconds(0.3f),
                () => { _controller.SetState(new RouletteLoopState(_controller)); });
        }
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
    }
}

public class RouletteLoopState : RouletteState
{
    // private float _startZAngle = 0f;

    // private Tweener _rotationTween;
    // private int _spinCount = 0;

    public RouletteLoopState(RouletteController controller) : base(controller)
    {
    }

    public override void OnEnter()
    {
        Debug.Log($"{_controller.gameObject.name} :: RouletteLoopState OnEnter");

        // _spinCount = 0;

        // 시계 방향으로 일정한 속도로 무한 회전
        // _startZAngle = _controller.RouletteSpinFrame.localEulerAngles.z;
        // _rotationTween = _controller.RouletteSpinFrame
        //     .DOLocalRotate(
        //         new Vector3(0, 0, currentZAngle + 360f), // 시계방향 회전을 위해 음수 사용
        //         _controller.RotateData.LoopRotationSpeed,
        //         RotateMode.FastBeyond360
        //     )
        //     .SetEase(Ease.Linear)
        //     .SetLoops(-1)
        //     .OnStepComplete(() => { ++_spinCount; });

        if (HudItemController.Instance.SpinButtonTween.IsActive() == false &&
            _controller.RouletteType == RouletteType.Method)
        {
            HudItemController.Instance.ChangeButtonState(this);
        }
    }

    public override void OnUpdate()
    {
        float deltaZ = 360f * _controller.RotateData.LoopRotationSpeed * Time.deltaTime;
        var nextRotation = new Vector3(0, 0, _controller.RouletteSpinFrame.localEulerAngles.z + deltaZ);
        _controller.RouletteSpinFrame.localRotation = Quaternion.Euler(nextRotation);
        // if (RouletteManager.Instance.IsAuto && _spinCount > 3)
        // {
        //     _controller.SetState(new RouletteStopState(_controller));
        // }
    }

    public override void OnExit()
    {
        Debug.Log($"{_controller.gameObject.name} :: RouletteLoopState OnExit");
        // _rotationTween?.Kill();
        // _rotationTween = null;
    }
}

public class RouletteStopState : RouletteState
{
    private float _targetRotationZ = 0f;
    private Quaternion _targetRotation;
    private float _targetDuration = 1f;
    private float _elapsedTime = 0f;

    public RouletteStopState(RouletteController controller) : base(controller)
    {
    }

    public override void OnEnter()
    {
        Debug.Log($"{_controller.gameObject.name} :: RouletteStopState OnEnter");

        _elapsedTime = 0f;
        _targetDuration = 1f;
        _targetRotationZ = CalculateTargetRotation();

        // 감속 시퀀스 생성
        // Sequence sequence = DOTween.Sequence();

        // 1. 점진적 감속 구간
        // for (var i = 1; i <= _controller.RotateData.AdditionalSpinCount; ++i)
        // {
        //     sequence.Append(_controller.RouletteSpinFrame.DOLocalRotate(
        //         new Vector3(0, 0, _controller.RouletteSpinFrame.localEulerAngles.z + 360f),
        //         _controller.RotateData.LoopRotationDuration + (i * _controller.RotateData.AdditionalSpinDuration),
        //         RotateMode.FastBeyond360
        //     ).SetEase(Ease.Linear));
        // }

        // 현재 각도보다 목표 각도가 작다면, 360도를 더해서 시계방향 회전 보장
        float currentRotation = _controller.RouletteSpinFrame.localEulerAngles.z;
        // float adjustedTargetRotation = _targetRotation;
        if (_targetRotationZ < currentRotation)
        {
            _targetRotationZ += 360f;
        }

        _targetRotation = Quaternion.Euler(0f, 0f, _targetRotationZ);

        // 2. 목표 지점으로 회전
        // sequence.Append(_controller.RouletteSpinFrame.DOLocalRotate(
        //     new Vector3(0, 0, _targetRotation),
        //     _controller.RotateData.StopDuration + (_controller.RotateData.AdditionalSpinCount *
        //                                            _controller.RotateData.AdditionalSpinDuration),
        //     RotateMode.FastBeyond360
        // ).SetEase(Ease.Linear));
        //
        // sequence.OnComplete(() =>
        // {
        //     sequence.Kill();
        //     _controller.SetState(new RouletteShowResultState(_controller));
        // });
    }

    public override void OnUpdate()
    {
        // tweener 사용하지 않고, update 에서 직접 처리하도록 변경해보았다.
        // 위의 1. 점진적 감속 구간은 실질적으로 사용하지 않는다.

        // 감속은 로테이션 델타가 줄어들어야 한다.

        _elapsedTime += Time.deltaTime;
        // float t = _elapsedTime / _targetDuration;
        // float currentRotation = _controller.RouletteSpinFrame.localEulerAngles.z;
        // float nextRotation = Mathf.Lerp(currentRotation, _targetRotation, t);

        float deltaZ = 360f * _controller.RotateData.LoopRotationSpeed * Time.deltaTime;
        float nextZ = _controller.RouletteSpinFrame.localEulerAngles.z + deltaZ;
        // nextZ = Mathf.Lerp(nextZ, _targetRotation, t);
        var nextRotation = new Vector3(0, 0, nextZ);

        Quaternion currentRotation = Quaternion.Euler(nextRotation);
        _controller.RouletteSpinFrame.localRotation = currentRotation;
        float deltaAngle = Quaternion.Angle(_targetRotation, currentRotation);

        // float deltaAngle = Mathf.DeltaAngle(Mathf.Abs(nextRotation.z), _targetRotationZ);
        if (deltaAngle < 20f)
        {
            _controller.SetState(new RouletteShowResultState(_controller));
        }

        // _elapsedTime += Time.deltaTime;
        // float t = _targetRotation * Time.deltaTime;
        // float currentRotation = _controller.RouletteSpinFrame.localEulerAngles.z;
        // // var rotationDelta = Mathf.Lerp(0, _targetRotation, t);
        // float nextZ = Mathf.Clamp(currentRotation + t, 0, _targetRotation);
        // var nextRotation = new Vector3(0, 0, nextZ);
        // _controller.RouletteSpinFrame.localRotation = Quaternion.Euler(nextRotation);
        //
        // if (nextZ >= _targetRotation)
        // {
        //     _controller.SetState(new RouletteShowResultState(_controller));
        // }
    }

    public override void OnExit()
    {
    }

    private float CalculateTargetRotation()
    {
        RouletteItemData selectedData =
            _controller.RouletteItemDataList[Random.Range(0, _controller.RouletteItemDataList.Count)];

        var targetRotation = 0f;
        float pinOffset = _controller.PinOffset;

        for (var i = 0; i < _controller.RouletteItemDataList.Count; i++)
        {
            if (_controller.RouletteItemDataList[i].Index == selectedData.Index)
            {
                targetRotation = pinOffset - _controller.RouletteItemDataList[i].RotateAngle + 360f;
                break;
            }
        }

        Debug.LogWarning(
            $"{_controller.gameObject.name} :: Selected Data : {selectedData.RouletteObject.name}\nTarget Rotation : {targetRotation}");

        return targetRotation /* + Random.Range(-10, 10)*/;
    }
}

public class RouletteShowResultState : RouletteState
{
    public RouletteShowResultState(RouletteController controller) : base(controller)
    {
    }

    public override void OnEnter()
    {
        Debug.Log($"{_controller.gameObject.name} :: RouletteShowResultState OnEnter");

        // 결과 오브젝트 활성화 및 애니메이션

        // 덜컹?
        _controller.RouletteSpinFrame.DOPunchRotation(new Vector3(0, 0, 15f), 0.3f);

        CoroutineTaskManager.RunDeferred(new WaitForSeconds(1f),
            () => { _controller.SetState(new RouletteIdleState(_controller)); });

        // 마지막 룰렛인 경우 스핀 조건 활성화
        if (_controller.RouletteType == RouletteType.Method)
        {
            HudItemController.Instance.ChangeButtonState(this);
        }
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
        Debug.Log($"{_controller.gameObject.name} :: RouletteShowResultState OnExit");
    }
}
