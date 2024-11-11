#define USE_DOTTWEEN
//#define USE_AUDIOCONTROLLER
using System;
using System.Collections;
using System.Collections.Generic;
#if USE_DOTTWEEN
using DG.Tweening;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CookApps.BM.TTT.UI
{
    public class UIButtonScale : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Transform targetTransform = null;

        [SerializeField]
        private bool playSound = true;

        [SerializeField]
        private string soundKey = "button_click";

        [Header("↓ 버튼을 누를 때의 스케일")]
        [SerializeField]
        private float downScale = 0.1f;

        [Header("↑ 버튼을 튕길 때의 스케일")]
        [SerializeField]
        private float bounceUpScale = 0.15f;

        [SerializeField]
        private float bounceDownScale = 0.1f;

        private Selectable selectable = null;
        private Vector3 initialScale;
        private bool touchDown = false;

        private void Awake()
        {
            if (targetTransform == null)
            {
                targetTransform = TryGetComponent(out selectable) ? selectable.transform : transform;
            }

            initialScale = targetTransform.localScale;

            Init();
        }

        private void Init()
        {
            if (selectable == null)
            {
                return;
            }

            if (selectable is Button btn)
            {
                return;
            }

            if (selectable is Toggle toggle)
            {
                return;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

            BoundingScale();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

#if USE_AUDIOCONTROLLER
            if (playSound && !string.IsNullOrEmpty(soundKey))
                AudioController.Play(soundKey);
#endif

            touchDown = true;

#if USE_DOTTWEEN
            targetTransform.DOScale(new Vector3(initialScale.x - downScale, initialScale.y - downScale, 1), 0.15f)
                .SetUpdate(true);
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

            if (!touchDown)
            {
                return;
            }

            touchDown = false;

            // haptic?
            OnPointerClick(eventData);
        }

        private void BoundingScale()
        {
#if USE_DOTTWEEN
            targetTransform.DOKill();
            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true);
            seq.Append(targetTransform.DOScale(
                new Vector3(initialScale.x + bounceUpScale, initialScale.y + bounceUpScale, 1), 0.1f));
            seq.Append(targetTransform.DOScale(
                new Vector3(initialScale.x - bounceDownScale, initialScale.y - bounceDownScale, 1), 0.1f));
            seq.Append(targetTransform.DOScale(initialScale, 0.1f));
#endif
        }
    }
}
