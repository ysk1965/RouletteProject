using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.TTT.Data;
using CookApps.BM.TTT.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace CookApps.BM.TTT.InGame.Object
{
    public abstract class UpgradableObjectBase : WorldObjectBase
    {
        [Header("=========== UpgradableObjectBase ===========")]
        [SerializeField]
        protected List<GameObject> _upgradeObjects;

        protected int _level;
        protected List<SpecUpgradeObject> _specList;

        protected override void OnAwake()
        {
            base.OnAwake();
            SetLevel(0);
        }

        protected override void OnInitializeData()
        {
        }

        public void Initialize(List<SpecUpgradeObject> specList)
        {
            _specList = specList;
            base.Initialize();
        }

        public void SetLevel(int level)
        {
            if (level > _upgradeObjects.Count)
            {
                Debug.LogWarning("Level is out of range");
                return;
            }

            for (var index = 0; index < _upgradeObjects.Count; index++)
            {
                GameObject uo = _upgradeObjects[index];
                bool isActive = index < level;
                if (isActive)
                {
                    var t = uo.GetComponent<EnableWithBouncingDOTween>();
                    if (!t)
                    {
                        t = uo.AddComponent<EnableWithBouncingDOTween>();
                    }
                }

                uo.SetActive(isActive);
            }

            _level = level;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetLevel(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetLevel(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetLevel(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetLevel(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetLevel(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetLevel(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetLevel(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SetLevel(7);
            }
        }
#endif
    }
}
