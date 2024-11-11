using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Playgrounds.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CookApps.BM.TTT.UI.Hud
{
    public static class HudStyleExtensions
    {
        public static bool IsSet(this HudStyle style, HudStyle flag)
        {
            return (style & flag) == flag;
        }
    }

    [Flags]
    public enum HudStyle
    {
        None = 0,

        Assets = 1 << 0,
        Setting = 1 << 1,
        Exp = 1 << 2,
        Top = Assets | Setting | Exp,

        Left = 1 << 10,
        Right = 1 << 11,
        Bottom = 1 << 12,

        All = Top | Left | Right | Bottom,

        Hide = 1 << 31,
    }

    public class HudItemController : SingletonMonoBehaviour<HudItemController>
    {
        [Header("Bottom Component")]
        [SerializeField]
        private GameObject _spinButtonObject;

        [SerializeField]
        private GameObject _stopButtonObject;

        [SerializeField]
        private GameObject _autoSpinButtonObject;

        [SerializeField]
        private Slider _spinPressButtonSlider;

        private Tween _spinButtonTween;

        private HudItemBase[] hudItemBaseArray;
        private Dictionary<HudStyle, List<HudItemBase>> hudItemStyle = new();

        public HudStyle CurrentStyle { get; private set; }
        public Tween SpinButtonTween => _spinButtonTween;

        protected override void OnAwakeEvent()
        {
            base.OnAwakeEvent();
            GroupHudItems();
        }

        private void GroupHudItems()
        {
            hudItemBaseArray = GetComponentsInChildren<HudItemBase>(true);
            if (hudItemBaseArray == null)
            {
                return;
            }

            Array styleValues = Enum.GetValues(typeof(HudStyle));
            foreach (HudItemBase hudItem in hudItemBaseArray)
            {
                hudItem.Initialize(this);
                HudStyle style = hudItem.WhichStyle;
                if (style == HudStyle.None)
                {
                    AddToStyle(HudStyle.None, hudItem);
                    continue;
                }

                foreach (HudStyle hudStyle in styleValues)
                {
                    bool contain = style.IsSet(hudStyle);
                    if (!contain)
                    {
                        continue;
                    }

                    AddToStyle(hudStyle, hudItem);
                }
            }
        }

        private void AddToStyle(HudStyle style, HudItemBase item)
        {
            if (!hudItemStyle.TryGetValue(style, out List<HudItemBase> list))
            {
                list = new List<HudItemBase>();
                hudItemStyle.Add(style, list);
            }

            list.Add(item);
        }

        public void SetHud(HudStyle style, bool immediately = false)
        {
            CurrentStyle = style;
            foreach ((HudStyle _, List<HudItemBase> list) in hudItemStyle)
            {
                foreach (HudItemBase hudItem in list)
                {
                    if (style == HudStyle.Hide)
                    {
                        hudItem.Hide();
                    }
                    else if (style.IsSet(hudItem.WhichStyle))
                    {
                        hudItem.Open(immediately);
                    }
                    else
                    {
                        hudItem.Hide(immediately);
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKey(KeyCode.PageUp))
            {
                SetHud(HudStyle.All);
            }
            else if (Input.GetKey(KeyCode.PageDown))
            {
                SetHud(HudStyle.None);
            }
        }
#endif

        public void OnClickSpinButton()
        {
            RouletteManager.Instance.StartRouletteAll();
        }

        public void OnClickStopButton()
        {
            RouletteManager.Instance.StopRouletteAll();
        }

        public void OnClickAutoButton()
        {
            RouletteManager.Instance.SetAutoState(false);

            _autoSpinButtonObject.SetActive(false);
        }

        public void OnPressSpinButton()
        {
            _spinButtonTween = _spinPressButtonSlider.DOValue(1f, 1.5f)
                .OnComplete(() =>
                {
                    _stopButtonObject.SetActive(false);
                    _spinButtonObject.SetActive(false);
                    _autoSpinButtonObject.SetActive(true);
                    RouletteManager.Instance.SetAutoState(true);
                });

            _spinButtonTween.OnPause(() =>
            {
                _spinPressButtonSlider.value = 0;
                _spinButtonObject.SetActive(false);
                _stopButtonObject.SetActive(true);
            });
        }

        public void CheckPressing()
        {
            if (_spinButtonTween != null && _spinButtonTween.IsPlaying())
            {
                _spinButtonTween.Kill();
            }
        }

        public void ChangeButtonState(RouletteState rouletteState)
        {
            if (RouletteManager.Instance.IsAuto || (_spinButtonTween != null && _spinButtonTween.IsPlaying()))
            {
                return;
            }

            switch (rouletteState)
            {
                case RouletteLoopState:
                    _spinButtonObject.SetActive(false);
                    _stopButtonObject.SetActive(true);
                    break;
                case RouletteShowResultState:
                    _spinButtonObject.SetActive(true);
                    _stopButtonObject.SetActive(false);
                    break;
            }
        }
    }
}
