//#define USE_AUDIOCONTROLLER

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CookApps.BM.TTT.Util
{
    public class EnableWithBouncingDOTween : MonoBehaviour
    {
        [SerializeField]
        private bool withSound = true;

        [SerializeField]
        private string soundKey = "object_appear";

        private void OnEnable()
        {
#if USE_AUDIOCONTROLLER
            if (withSound)
            {
                if (AudioController.DoesInstanceExist())
                    AudioController.Play(soundKey);
            }
#endif
            Transform tr = transform;
            tr.localScale = Vector3.zero;
            tr.DOKill();
            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true);
            seq.Append(tr.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.1f));
            seq.Append(tr.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f));
            seq.Append(tr.DOScale(Vector3.one, 0.1f));
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}
