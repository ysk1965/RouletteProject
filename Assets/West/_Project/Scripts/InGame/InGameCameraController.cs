using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class InGameCameraController : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;

    [Header("Common")]
    [SerializeField] private float _followSpeed = 2.0f;
    [SerializeField] private float _firstInitfollowSpeed = 2.0f;
    [SerializeField] private Vector3 _offsetPos;      // 대상과 카메라 간의 거리(오프셋)

    [Header("Zoom")]
    [SerializeField] private float _originProjectionSize = 8.2f;
    [SerializeField] private float _zoomProjectionSize = 6.2f;
    [SerializeField] private float _normalWinZoomProjectionSize = 3.2f;
    [SerializeField] private float _perfectWinZoomProjectionSize = 3.2f;
    [SerializeField] private float _hammerZoomProjectionSize = 5.2f;
    [SerializeField] private float _zoomSpeed = 2.0f;
    [SerializeField] private float _firstInitZoomSpeed = 0.25f;

    [Header("Shake")]
    [SerializeField] private float shakeMagnitude = 0.2f; // 흔들림 강도

    private Vector3 initialPosition; // 카메라 초기 위치
    private Coroutine shakeCoroutine; // 현재 실행 중인 코루틴 참조

    // 카메라 이동 제한 영역
    [Header("Camera Move Limit")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Transform _targetTransform;    // 따라갈 대상
    private float _targetFollowSpeed;
    private float _targetZoomSize;         // 줌인할 카메라 사이즈
    private float _targetZoomSpeed;

    private float _smoothSpeed = 0.125f; // 카메라 이동의 부드러움 정도


    private bool _isDragging = false;    // 드래그 중인지 확인
    private Vector3 _dragOriginPos;         // 드래그 시작 위치
    private Coroutine _focusCoroutine;   // 현재 실행 중인 포커싱 코루틴
    private float _dragThraeshold = 0.5f; // 드래그 감지 거리

    private bool _isNowFocusing = false; // 타겟 포커싱 중인지 확인
    public bool IsLockDrag { get; set; }= false;

    private void Awake()
    {
        if (_targetCamera == null)
        {
            _targetCamera = Camera.main;
        }

        IsLockDrag = true;
        _targetFollowSpeed = _followSpeed;
        _targetZoomSpeed = _zoomSpeed;
    }

    public void SetCameraZoomSize(float size)
    {
        _targetZoomSize = size;
    }

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }

    public void SetZoomSpeed(float speed)
    {
        _targetZoomSpeed = speed;
    }

    public void SetFirstInitFollowSpeed(bool isInit)
    {
        _targetFollowSpeed = isInit ? _firstInitfollowSpeed : _followSpeed;
    }

    public void SetFirstInitZoomSpeed(bool isInit)
    {
        _targetZoomSpeed = isInit ? _firstInitZoomSpeed : _zoomSpeed;
    }

    public void SetZoomState(bool isZoom)
    {
        _targetZoomSize = isZoom ? _zoomProjectionSize : _originProjectionSize;
    }

    public void SetHammerZoomState(bool isZoom)
    {
        _targetZoomSize = isZoom ? _hammerZoomProjectionSize : _originProjectionSize;
    }

    public void SetWinnerZoomState(Transform target, bool isPerfectWin)
    {
        IsLockDrag = true;

        SetTarget(target);

        _targetZoomSize = isPerfectWin ? _perfectWinZoomProjectionSize : _normalWinZoomProjectionSize;

        minX = -10f;
        maxX = 10f;
        minY = -10f;
        maxY = 10f;
    }

    // 플레이어에게 포커싱 설정
    public void SetPlayerFocus(Transform target)
    {
        SetTarget(target);
        SetZoomState(true);

        ClearCamera();

        StartFocus();
    }

    private void StartFocus()
    {
        // 이미 실행 중인 코루틴이 있으면 중지
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);

        // 포커싱 시작 코루틴 실행
        _focusCoroutine = StartCoroutine(FocusOnTarget());
    }

    private void StopFocus()
    {
        // 이미 실행 중인 코루틴이 있으면 중지
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);

        // 포커싱 해제 코루틴 실행
        _focusCoroutine = StartCoroutine(FocusOut());
    }

    // 흔들림을 트리거
    public void TriggerShake(float duration)
    {
        // 기존 코루틴이 실행 중이라면 멈춤
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        initialPosition = _targetCamera.transform.localPosition;
        shakeCoroutine = StartCoroutine(Shake(duration));
    }

    private void LateUpdate()
    {
        // 드래그 이동 감지
        if (IsLockDrag == false)
        {
            HandleDragInput();
        }
    }

    // 드래그 이동 감지
    private void HandleDragInput()
    {
        // 터치나 마우스 왼쪽 버튼을 누르면 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            _dragOriginPos = _targetCamera.ScreenToWorldPoint(Input.mousePosition);

            StopCoroutine(nameof(ResetCamera));
        }

        // 터치나 마우스 왼쪽 버튼을 누르고 있을 때 드래그 중
        if (Input.GetMouseButton(0)/* && _isDragging == false*/)
        {
            Vector3 currentPos = _targetCamera.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(_dragOriginPos, currentPos);

            if (_isDragging == false)
            {
                _isDragging = distance > _dragThraeshold;
            }
            else
            {
                Vector3 difference = _dragOriginPos - currentPos;

                Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0);

                // 새로운 위치를 제한된 범위 내로 고정
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

                //transform.position += difference;
                transform.position = newPosition;

                //SetZoomState(false);
                _isNowFocusing = false;
                _targetCamera.orthographicSize = _originProjectionSize;
                //StopFocus();
            }

            // if (distance > _dragThraeshold)
            // {
            //     _isDragging = true;
            //
            //     Vector3 difference = _dragOriginPos - currentPos;
            //
            //     Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0);
            //
            //     // 새로운 위치를 제한된 범위 내로 고정
            //     newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            //     newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            //
            //     //transform.position += difference;
            //     transform.position = newPosition;
            //
            //     //SetZoomState(false);
            //     _isNowFocusing = false;
            //     _targetCamera.orthographicSize = _originProjectionSize;
            //     //StopFocus();
            // }
        }

        // 터치를 놓거나 마우스 왼쪽 버튼을 놓으면 드래그 종료
        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            //isDragging = false;
            _dragOriginPos = Vector3.zero;

            _focusCoroutine = StartCoroutine(nameof(ResetCamera));
        }
    }

    private void ClearCamera()
    {
        StopCoroutine(nameof(ResetCamera));
        _isDragging = false;
        _isNowFocusing = false;
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(2.0f);

        _isDragging = false;

        SetZoomState(true);
        StartFocus();
    }

    IEnumerator FocusOnTarget()
    {
        _isNowFocusing = true;

        // 타겟을 포커싱하여 줌 인하는 과정
        //while (Vector2.Distance(transform.position, _targetTransform.position) > 0.1f /*&& Mathf.Abs(_targetCamera.orthographicSize - _targetZoomSize) > 0.1f*/)
        while (_isNowFocusing)
        {
            // 타겟 위치로 부드럽게 이동
            Vector3 targetPosition = new Vector3(_targetTransform.position.x, _targetTransform.position.y, transform.position.z);

            Vector3 newPosition = targetPosition;

            // 새로운 위치를 제한된 범위 내로 고정
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            //transform.position += difference;
            //transform.position = newPosition;

            transform.position = Vector3.Lerp(transform.position, newPosition, _targetFollowSpeed * Time.deltaTime);

            // 타겟 줌 값으로 부드럽게 조정
            _targetCamera.orthographicSize = Mathf.Lerp(_targetCamera.orthographicSize, _targetZoomSize, _targetZoomSpeed * Time.deltaTime);

            yield return null;
        }

        _isNowFocusing = false;
    }

    private IEnumerator FocusOut()
    {
        // 기본 줌으로 돌아가는 과정
        while (Mathf.Abs(_targetCamera.orthographicSize - _originProjectionSize) > 0.05f)
        {
            // 기본 위치를 유지하며 줌 조정
            _targetCamera.orthographicSize = Mathf.Lerp(_targetCamera.orthographicSize, _originProjectionSize, _targetZoomSpeed * Time.deltaTime);

            yield return null;
        }
    }

    // 흔들림 효과 코루틴
    private IEnumerator Shake(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 카메라를 랜덤한 위치로 이동
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 원래 위치로 복구
        transform.localPosition = initialPosition;
        shakeCoroutine = null;
    }
}
