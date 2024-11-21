using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자동으로 비활성화 되는 오브젝트
public class SelfInactiveObject : MonoBehaviour
{
    [SerializeField] private float _inactiveDelayTime = 2.0f;

    private void OnEnable()
    {
        Invoke(nameof(OffEffect), _inactiveDelayTime);
    }

    private void OffEffect()
    {
        gameObject.SetActive(false);
    }
}
