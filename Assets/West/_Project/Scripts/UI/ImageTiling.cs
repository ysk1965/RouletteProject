using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ImageTiling : MonoBehaviour
{
    public Image tileImage; // 타일링할 이미지
    public float speed = 1.0f; // 타일링 속도
    public Vector2 direction = new Vector2(1, 0); // 타일링 방향

    public Material tileMaterial;
    private CancellationTokenSource cts;

    void Start()
    {
        // 비동기 타일링 시작
        cts = new CancellationTokenSource();
        StartTiling(cts.Token).Forget();
    }

    // 비동기적으로 타일링 애니메이션 실행
    private async UniTaskVoid StartTiling(CancellationToken token)
    {
        Vector2 currentOffset = Vector2.zero;

        try
        {
            while (!token.IsCancellationRequested)
            {
                // 오프셋 업데이트
                currentOffset += direction * speed * Time.deltaTime;
                currentOffset.x = Mathf.Repeat(currentOffset.x, 1);
                currentOffset.y = Mathf.Repeat(currentOffset.y, 1);

                // Material의 Offset 속성 업데이트
                tileMaterial.SetVector("_Offset", new Vector4(currentOffset.x, currentOffset.y, 0, 0));

                // 한 프레임 대기
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("타일링 작업이 취소되었습니다.");
        }
    }

   
}
