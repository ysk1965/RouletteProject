using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class InGameSnake : MonoBehaviour
{
    [SerializeField] private int _snakeID; // 스네이크 ID

    [SerializeField] private Animator _headAnimator; // 머리 애니메이터
    [SerializeField] private MoveAlongSpline _snakeAnimComponent;
    [SerializeField] private SpriteShapeRenderer _snakeSpriteShapeRenderer;

    public GameObject GetSwallowObject => _snakeAnimComponent.gameObject;

    public bool IsPlayingSwallowAnim => _snakeAnimComponent.IsPlaySwallowAnim;

    public int SnakeID => _snakeID;

    public void PlaySnakeSwallowHeadAnim()
    {
        if (_headAnimator == null) return;

        _headAnimator?.SetTrigger("New_Eat");
        _headAnimator?.Play("New_Eat", -1, 0f);
    }

    public void PlaySnakeHeadDisappearAnim()
    {
        if (_headAnimator == null) return;

        _headAnimator?.SetTrigger("Snake_Head_Disappear");
        _headAnimator?.Play("Snake_Head_Disappear", -1, 0f);
    }

    public void PlaySnakeSwallowAnim()
    {
        if (_snakeAnimComponent == null) return;

        _snakeAnimComponent?.PlaySwallowAnim();
    }

    public void StartFadeOut()
    {
        if (_snakeSpriteShapeRenderer == null)
        {
            _snakeSpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        }

        StopCoroutine(nameof(FadeToAlpha));
        StartCoroutine(FadeToAlpha(0, 2.0f));

        // 머리 애니메이션 재생
        PlaySnakeHeadDisappearAnim();
    }

    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        // 현재 알파값을 가져옵니다.
        float startAlpha = _snakeSpriteShapeRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Lerp를 사용하여 알파 값을 점진적으로 변경합니다.
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);

            // 새로운 알파 값을 설정합니다.
            Color newColor = _snakeSpriteShapeRenderer.color;
            newColor.a = newAlpha;
            _snakeSpriteShapeRenderer.color = newColor;

            yield return null; // 다음 프레임까지 대기
        }

        // 목표 알파값으로 설정 (마지막 프레임에서 정확하게 설정)
        Color finalColor = _snakeSpriteShapeRenderer.color;
        finalColor.a = targetAlpha;
        _snakeSpriteShapeRenderer.color = finalColor;

        // 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}
