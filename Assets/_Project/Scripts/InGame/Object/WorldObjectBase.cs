using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using CookApps.BM.TTT.Data;
using UnityEngine;

namespace CookApps.BM.TTT.InGame.Object
{
    public abstract class WorldObjectBase : MonoBehaviour
    {
        [Header("=========== WorldObjectBase ===========")]
        [SerializeField]
        protected UpgradeObjectType _upgradeObjectType;

        [SerializeField]
        protected CinemachineVirtualCamera _virtualCamera;

        public UpgradeObjectType UpgradeObjectType => _upgradeObjectType;

        #region Unity Event Methods

        private void Awake()
        {
            OnAwake();
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnInitializeData()
        {
        }

        #endregion

        public void Initialize()
        {
            OnInitialize();
            OnInitializeData();
        }

        public void SetActiveVirtualCamera(bool active)
        {
            if (!_virtualCamera)
            {
                Debug.LogWarning("Virtual Camera is null");
                return;
            }

            _virtualCamera.gameObject.SetActive(active);
        }
    }
}
