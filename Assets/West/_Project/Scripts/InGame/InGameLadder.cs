using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class InGameLadder : MonoBehaviour
{
    [SerializeField] private int _ladderID;
    [SerializeField] private InGameLadderType _ladderType;
    [SerializeField] private SpriteShapeRenderer _ladderSpriteShapeRenderer;

    private bool _isPlaySFXSound = false;

    public int LadderID => _ladderID;

    public InGameLadderType LadderType => _ladderType;

    public void StartFadeOut()
    {
        if (_ladderSpriteShapeRenderer == null)
        {
            _ladderSpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        }

        StopCoroutine(nameof(FadeToAlpha));
        StartCoroutine(FadeToAlpha(0, 2.0f));
    }

    public void PlayLadderUpSFXSound()
    {
        _isPlaySFXSound = true;
        StartCoroutine(nameof(PlayLadderSFX));
    }

    IEnumerator PlayLadderSFX()
    {
        while (_isPlaySFXSound)
        {
            switch (LadderType)
            {
                case InGameLadderType.Wood_Ladder:
                    SoundManager.Instance.PlaySFX("sfx_ladder_climb");
                    break;
                case InGameLadderType.Rope:
                    SoundManager.Instance.PlaySFX("sfx_rope_climb");
                    break;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    public void StopLadderUpSFXSound()
    {
        _isPlaySFXSound = false;
        StopCoroutine(nameof(PlayLadderSFX));
    }

    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        // 현재 알파값을 가져옵니다.
        float startAlpha = _ladderSpriteShapeRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Lerp를 사용하여 알파 값을 점진적으로 변경합니다.
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);

            // 새로운 알파 값을 설정합니다.
            Color newColor = _ladderSpriteShapeRenderer.color;
            newColor.a = newAlpha;
            _ladderSpriteShapeRenderer.color = newColor;

            yield return null; // 다음 프레임까지 대기
        }

        // 목표 알파값으로 설정 (마지막 프레임에서 정확하게 설정)
        Color finalColor = _ladderSpriteShapeRenderer.color;
        finalColor.a = targetAlpha;
        _ladderSpriteShapeRenderer.color = finalColor;
    }
}
