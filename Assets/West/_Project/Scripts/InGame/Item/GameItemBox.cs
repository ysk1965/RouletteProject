using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemBox : MonoBehaviour
{
    [SerializeField]
    private float _hideObjectDelayTime = 2f;

    [SerializeField]
    private float _showGetEffectDelayTime = 0.2f;

    [Space]
    [SerializeField]
    private Animator _itemBoxAnimator;

    [Space]
    [SerializeField]
    private ParticleSystem _itemGetEffect;

    public void PlayGetAnimation()
    {
        _itemBoxAnimator.SetTrigger("Item_Get");
        _itemBoxAnimator.Play("Item_Get", -1, 0f);

        //Invoke(nameof(PlayGetEffect), _showGetEffectDelayTime);
        PlayGetEffect();

        Invoke(nameof(HideGameObject), _hideObjectDelayTime);
    }

    public void ResetItemBox()
    {
        // [상건] Spawn 애님 추가로 인한 주석처리
        // _itemBoxAnimator.SetTrigger("Item_Idle");
        // _itemBoxAnimator.Play("Item_Idle", -1, 0f);

        gameObject.SetActive(true);
    }

    private void PlayGetEffect()
    {
        _itemGetEffect.gameObject.SetActive(true);
        _itemGetEffect.Clear();
        _itemGetEffect.Play();
    }

    private void HideGameObject()
    {
        _itemGetEffect.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
