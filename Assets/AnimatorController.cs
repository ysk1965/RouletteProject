using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
// Animator Controller에서 애니메이션 클립에 이벤트 추가 (애니메이션 클립에서 직접 설정해야 함)
    public void OnSpinAnimationEnd()
    {
        // 애니메이션이 끝나는 시점에 실행하고 싶은 로직
        Debug.Log("Spin Animation Ended!");
        // 결과 상태로 변경
        InGameManager.Instance.ChangeStateToResult();
    }
}
