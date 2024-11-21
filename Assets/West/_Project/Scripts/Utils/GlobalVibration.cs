using UnityEngine;

public class GlobalVibration : Singleton<GlobalVibration>
{
    /// <summary>
    /// 진동을 발생시킵니다.
    /// </summary>
    /// <param name="duration">진동 지속 시간 (초 단위)</param>
    /// <param name="amplitude">진동 강도 (0~255, Android API 26 이상에서만 지원)</param>
    public void Vibrate(float duration, int amplitude = 128)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (amplitude < 0 || amplitude > 255)
        {
            Debug.LogWarning("Amplitude 값은 0에서 255 사이여야 합니다.");
            amplitude = Mathf.Clamp(amplitude, 0, 255);
        }

        long milliseconds = (long)(duration * 1000);

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (vibrator.Call<bool>("hasVibrator"))
        {
            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            AndroidJavaObject vibrationEffect = null;

            // Android API 26 이상인 경우 VibrationEffect 사용
            if (AndroidVersion() >= 26)
            {
                vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", vibrationEffect);
            }
            else
            {
                // Android API 26 미만은 amplitude를 지원하지 않음
                vibrator.Call("vibrate", milliseconds);
            }
        }
        else
        {
            Debug.LogWarning("이 기기는 진동을 지원하지 않습니다.");
        }
#else
        Debug.Log($"진동 실행: 시간={duration}s, 강도={amplitude} (Android 환경에서만 실행됩니다)");
#endif
    }

    /// <summary>
    /// 실행 중인 진동을 중지합니다.
    /// </summary>
    public void CancelVibration()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        vibrator.Call("cancel");
#else
        Debug.Log("진동 취소: Android 환경에서만 작동합니다.");
#endif
    }

    /// <summary>
    /// 현재 Android API 레벨을 가져옵니다.
    /// </summary>
    /// <returns>Android API 레벨</returns>
    private int AndroidVersion()
    {
        using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }
}
