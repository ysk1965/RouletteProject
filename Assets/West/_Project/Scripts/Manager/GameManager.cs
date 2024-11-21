using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitManager();
    }

    private void InitManager()
    {
        // 수동 매니저 Init
        UserDataManager.Instance.Init();

        SpecDataManager.Instance.Initialize(1);
    }
}
