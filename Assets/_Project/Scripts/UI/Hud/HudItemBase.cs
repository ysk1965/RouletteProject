using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Playgrounds.UI;
using CookApps.Playgrounds.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CookApps.BM.TTT.UI.Hud
{
    public abstract class HudItemBase : CachedUIBehaviour
    {
        public enum State
        {
            Open = -1,
            Hide = 1,
        }

        [SerializeField]
        protected bool isActive = true;

        [SerializeField]
        protected HudItemGroup whichGroup = HudItemGroup.None;

        [SerializeField]
        protected HudStyle whichStyle = HudStyle.None;

        [SerializeField]
        protected Transform animationRoot = null;

        [SerializeField]
        protected Transform receiveTargetTrans = null;

        [SerializeField]
        protected ParticleSystem receiveEffect;

        public Transform ReceiveTargetTrans => receiveTargetTrans;
        public Transform AnimationRoot => animationRoot;
        public HudStyle WhichStyle => whichStyle;
        public HudItemGroup WhichGroup => whichGroup;
        public State CurrentState { get; private set; }

        protected HudItemController controller;
        protected bool deactiveWhenHide;

        protected override void Awake()
        {
            if (animationRoot == null)
            {
                animationRoot = transform;
            }

            var layoutGroup = transform.parent.GetComponent<LayoutGroup>();
            deactiveWhenHide = layoutGroup;
        }

        protected override void OnEnable()
        {
            OnAddEvent();
        }

        protected override void OnDisable()
        {
            OnRemoveEvent();
        }

        public void Initialize(HudItemController controller)
        {
            this.controller = controller;
            OnInitialize();
        }

        public void OnClickHudItem()
        {
            OnClickHudItemEvent();
        }

        public void PlayReceiveEffect()
        {
            if (receiveEffect == null)
            {
                return;
            }

            receiveEffect.gameObject.SetActive(true);
            receiveEffect.Clear(true);
            receiveEffect.Play(true);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnClickHudItemEvent()
        {
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnHide()
        {
        }

        public virtual void OnSequenceComplete(State state)
        {
            if (state == State.Hide && deactiveWhenHide)
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnAddEvent()
        {
        }

        protected virtual void OnRemoveEvent()
        {
        }

        protected virtual bool CheckCanOpen()
        {
            return true;
        }

        // protected virtual bool CheckTutorial(bool includeProcessing = true)
        // {
        //     if (tutorialId == 0)
        //         return true;
        //
        //     if (!TutorialManager.IsExist)
        //         return false;
        //
        //     if (includeProcessing && TutorialManager.Instance.CurrentTutorialId == tutorialId)
        //         return true;
        //
        //     return TutorialManager.Instance.IsComplete(tutorialId);
        // }

        public virtual void Open(bool immediately = false)
        {
            if (!isActive)
            {
                Hide(immediately);
                return;
            }

            bool match = controller.CurrentStyle.IsSet(whichStyle);
            if (!match || !CheckCanOpen())
            {
                Hide(immediately);
                return;
            }

            if (CurrentState == State.Open)
            {
                return;
            }

            if (deactiveWhenHide)
            {
                gameObject.SetActive(true);
            }

            CurrentState = State.Open;
            PlayStateAnimation(State.Open, immediately ? 0 : 0.2f);
            OnOpen();
        }

        public virtual void Hide(bool immediately = false)
        {
            if (CurrentState == State.Hide)
            {
                return;
            }

            CurrentState = State.Hide;
            PlayStateAnimation(State.Hide, immediately ? 0 : 0.2f);
            OnHide();
        }

        private void PlayStateAnimation(State state, float duration = 0.1f)
        {
            Transform targetTransform = animationRoot;
            targetTransform.DOKill();
            if (state == State.Open)
            {
                if (whichGroup == HudItemGroup.None)
                {
                    targetTransform.gameObject.SetActive(true);
                }
                else
                {
                    targetTransform.DOLocalMove(Vector3.zero, duration);
                }

                return;
            }

            Rect sa = Screen.safeArea;
            float safeAreaSizeBottom = Screen.height - sa.yMax;

            const int multiplier = 350;
            switch (whichGroup)
            {
                case HudItemGroup.Top:
                    targetTransform.DOLocalMove(Vector3.up * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.RightTop:
                    targetTransform.DOLocalMove(new Vector3(1, 1) * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.Right:
                    targetTransform.DOLocalMove(Vector3.right * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.RightBottom:
                    targetTransform.DOLocalMove(new Vector3(1, -1) * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.Bottom:
                    targetTransform.DOLocalMove(Vector3.down * (multiplier + safeAreaSizeBottom), duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.LeftBottom:
                    targetTransform.DOLocalMove(new Vector3(-1, -1) * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.Left:
                    targetTransform.DOLocalMove(Vector3.left * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.LeftTop:
                    targetTransform.DOLocalMove(new Vector3(-1, 1) * multiplier, duration)
                        .OnComplete(() => OnSequenceComplete(state));
                    break;
                case HudItemGroup.None:
                    targetTransform.gameObject.SetActive(false);
                    OnSequenceComplete(state);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (transform is RectTransform rt)
            {
                rt.pivot = new Vector2(0.5f, 0.5f);
            }
        }
#endif
    }
}
