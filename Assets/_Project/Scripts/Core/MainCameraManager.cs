using System;
using UnityEngine;

public class MainCameraManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _rotationSpeed = 0.1f;
    [SerializeField] private float _zoomSpeed = 0.01f;
    [SerializeField] private float _minDistance = 10f;

    // 회전 제한 값 추가
    [SerializeField] private float _maxHorizontalAngle = 30f;

    private Vector3 _cameraPinPosition;
    private float _maxDistance;
    private float _currentDistance;
    private float _currentHorizontalAngle;
    private float _currentVerticalAngle;

    // 초기 수평 각도를 저장할 변수 추가
    private float _initialHorizontalAngle;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        _cameraPinPosition = transform.position + new Vector3(0, -transform.position.y, 60);
        _maxDistance = Vector3.Distance(transform.position, _cameraPinPosition);
        _currentDistance = _maxDistance;

        Vector3 directionToCamera = transform.position - _cameraPinPosition;
        _currentHorizontalAngle = Mathf.Atan2(directionToCamera.x, directionToCamera.z) * Mathf.Rad2Deg;
        _initialHorizontalAngle = _currentHorizontalAngle; // 초기 각도 저장
        _currentVerticalAngle = Mathf.Asin(directionToCamera.y / directionToCamera.magnitude) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        if(Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 delta = touch.deltaPosition;

            // 수평 회전 계산 및 제한
            float horizontalRotation = delta.x * _rotationSpeed * 0.05f;

            // 초기 각도를 기준으로 좌우 30도로 제한
            float minAngle = _initialHorizontalAngle - _maxHorizontalAngle;
            float maxAngle = _initialHorizontalAngle + _maxHorizontalAngle;
            _currentHorizontalAngle = Mathf.Clamp(_currentHorizontalAngle + horizontalRotation, minAngle, maxAngle);

            // 수직 줌
            float verticalZoom = delta.y * _zoomSpeed * 0.1f;
            _currentDistance = Mathf.Clamp(_currentDistance - verticalZoom, _minDistance, _maxDistance);

            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        float horizontalRadius = _currentDistance * Mathf.Cos(_currentVerticalAngle * Mathf.Deg2Rad);
        float y = _currentDistance * Mathf.Sin(_currentVerticalAngle * Mathf.Deg2Rad);
        float x = horizontalRadius * Mathf.Sin(_currentHorizontalAngle * Mathf.Deg2Rad);
        float z = horizontalRadius * Mathf.Cos(_currentHorizontalAngle * Mathf.Deg2Rad);

        Vector3 newPosition = _cameraPinPosition + new Vector3(x, y, z);
        _mainCamera.transform.position = newPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _cameraPinPosition);

        Vector3 cameraGroundPosition = _mainCamera.transform.position;
        cameraGroundPosition.y = 0;
        Gizmos.DrawLine(_mainCamera.transform.position, cameraGroundPosition);
        Gizmos.DrawLine(cameraGroundPosition, _cameraPinPosition);
    }
}
