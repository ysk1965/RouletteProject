using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class RawImageTiling : MonoBehaviour
{
    public RawImage rawImage;    // 타겟 RawImage
    public float xSpeed = 0.1f;  // X축 이동 속도
    public float ySpeed = 0.1f;  // Y축 이동 속도
    public bool isRunning = true; // 애니메이션 실행 여부

    private CancellationTokenSource cancellationTokenSource;

    private void OnEnable()
    {
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();

        // CancellationTokenSource 생성
        cancellationTokenSource = new CancellationTokenSource();

        // 비동기 애니메이션 시작 (CancellationToken 전달)
        StartTilingAnimation(cancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid StartTilingAnimation(CancellationToken token)
    {
        try
        {
            while (isRunning && !token.IsCancellationRequested)
            {
                // 현재 UV Rect를 가져와서 X와 Y 값을 업데이트
                Rect uvRect = rawImage.uvRect;
                uvRect.x += xSpeed * Time.deltaTime;
                uvRect.y += ySpeed * Time.deltaTime;

                // 업데이트된 uvRect를 적용
                rawImage.uvRect = uvRect;

                // 다음 프레임까지 대기, 취소 가능하게 만듦
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Tiling animation canceled.");
        }
    }

    private void OnDisable()
    {
        // 애니메이션 중지 및 CancellationTokenSource 해제
        isRunning = false;
        cancellationTokenSource?.Cancel(); // 작업 취소
        cancellationTokenSource?.Dispose(); // 리소스 해제
    }
}
