using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSync : MonoBehaviour
{
    [SerializeField]
    private Transform _targetTransform;

    [SerializeField]
    private bool _updatePosition = true;

    [SerializeField]
    private bool _updateRotation = true;

    private Transform _cachedTransform;

    private void Awake()
    {
        _cachedTransform = transform;
    }

    private void FixedUpdate()
    {
        SyncTransform();
    }

    private void SyncTransform()
    {
        if (!_targetTransform)
            return;

        if (_updatePosition)
            _cachedTransform.position = _targetTransform.position;
        if (_updateRotation)
            _cachedTransform.rotation = _targetTransform.rotation;
    }
}
