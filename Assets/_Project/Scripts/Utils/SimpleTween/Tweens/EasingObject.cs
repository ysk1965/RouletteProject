using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EasingObject
{
    private const float TWO_PI = Mathf.PI * 2.0f;

    public static Dictionary<EasingPosition, float[]> DEF_PARAM = new Dictionary<EasingPosition, float[]>()
    {
        { InternalElasticEasingIn, new float[2] { 0.1f, 0.12f } },
        { InternalElasticEasingOut, new float[2] { 0.1f, 0.12f } },
        { InternalElasticEasingInOut, new float[2] { 0.1f, 0.12f } },
        { InternalBackEasingIn, new float[2] { 1.70158f, 0.0f } },
        { InternalBackEasingOut, new float[2] { 1.70158f, 0.0f } },
        { InternalBackEasingInOut, new float[2] { 1.70158f, 0.0f } }
    };

    public delegate float EasingPosition(float s, float e, float deltaTime, float duration, float overshoot_amplitude, float period);

    public static readonly EasingPosition LinearEasing = InternalLinearEasing;
    public static readonly EasingPosition LinearEasingInOut = InternalLinearEasingInOut;
    public static readonly EasingPosition QuadEasingIn = InternalQuadEasingIn;
    public static readonly EasingPosition QuadEasingOut = InternalQuadEasingOut;
    public static readonly EasingPosition QuadEasingInOut = InternalQuadEasingInOut;
    public static readonly EasingPosition CircEasingIn = InternalCircEasingIn;
    public static readonly EasingPosition CircEasingOut = InternalCircEasingOut;
    public static readonly EasingPosition CircEasingInOut = InternalCircEasingInOut;
    public static readonly EasingPosition ExpoEasingIn = InternalExpoEasingIn;
    public static readonly EasingPosition ExpoEasingOut = InternalExpoEasingOut;
    public static readonly EasingPosition ExpoEasingInOut = InternalExpoEasingInOut;
    public static readonly EasingPosition ElasticEasingIn = InternalElasticEasingIn;
    public static readonly EasingPosition ElasticEasingOut = InternalElasticEasingOut;
    public static readonly EasingPosition ElasticEasingInOut = InternalElasticEasingInOut;
    public static readonly EasingPosition BackEasingIn = InternalBackEasingIn;
    public static readonly EasingPosition BackEasingOut = InternalBackEasingOut;
    public static readonly EasingPosition BackEasingInOut = InternalBackEasingInOut;
    public static readonly EasingPosition BounceEasingIn = InternalBounceEasingIn;
    public static readonly EasingPosition BounceEasingOut = InternalBounceEasingOut;
    public static readonly EasingPosition BounceEasingInOut = InternalBounceEasingInOut;

    private static float InternalLinearEasing(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        return (e - s) * deltaTime / duration + s;
    }

    private static float InternalLinearEasingInOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration / 2.0f;

        if (deltaTime < 1.0f)
            return (e - s) * deltaTime / duration + s;

        return (e - s) * (2.0f - deltaTime) / duration + s;
    }

    private static float InternalQuadEasingIn(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration;
        return (e - s) * deltaTime * deltaTime + s;
    }

    private static float InternalQuadEasingOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration;
        return (s - e) * deltaTime * (deltaTime - 2.0f) + s;
    }

    private static float InternalQuadEasingInOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration / 2.0f;

        if (deltaTime < 1.0f)
            return (e - s) / 2.0f * deltaTime * deltaTime + s;

        deltaTime--;
        return (s - e) / 2.0f * (deltaTime * (deltaTime - 2.0f) - 1) + s;
    }

    private static float InternalCircEasingIn(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration;
        return (s - e) * (Mathf.Sqrt(1.0f - deltaTime * deltaTime) - 1.0f) + s;
    }

    private static float InternalCircEasingOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration;
        deltaTime--;
        return (e - s) * Mathf.Sqrt(1 - deltaTime * deltaTime) + s;
    }

    private static float InternalCircEasingInOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration / 2.0f;
        if (deltaTime < 1.0f)
            return (s - e) / 2.0f * (Mathf.Sqrt(1.0f - deltaTime * deltaTime) - 1.0f) + s;

        deltaTime -= 2.0f;
        return (e - s) / 2.0f * (Mathf.Sqrt(1.0f - deltaTime * deltaTime) + 1.0f) + s;
    }

    private static float InternalExpoEasingIn(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        return (e - s) * Mathf.Pow(2.0f, 10.0f * (deltaTime / duration - 1.0f)) + s;
    }

    private static float InternalExpoEasingOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        return (e - s) * (-Mathf.Pow(2.0f, -10.0f * deltaTime / duration) + 1.0f) + s;
    }

    private static float InternalExpoEasingInOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        deltaTime /= duration / 2.0f;
        if (deltaTime < 1.0f)
            return (e - s) / 2.0f * Mathf.Pow(2.0f, 10.0f * (deltaTime - 1.0f)) + s;

        deltaTime--;
        return (e - s) / 2.0f * (-Mathf.Pow(2.0f, -10.0f * deltaTime) + 2.0f) + s;
    }

    private static float InternalElasticEasingIn(float s, float e, float deltaTime, float duration, float amplitude, float period)
    {
        float ss, d;

        d = e - s;

        if (deltaTime == 0.0f)
            return s;

        if ((deltaTime /= duration) == 1.0f)
            return e;

        if (period == 0)
            period = duration * 0.3f;

        if (amplitude == 0 || (d > 0 && amplitude < d) || (d < 0 && amplitude < -d))
        {
            amplitude = d;
            ss = period / 4;
        }
        else
            ss = period / TWO_PI * Mathf.Asin(d / amplitude);

        return -(amplitude * Mathf.Pow(2, 10 * (deltaTime -= 1.0f)) * Mathf.Sin((deltaTime * duration - ss) * TWO_PI / period)) + s;
    }

    private static float InternalElasticEasingOut(float s, float e, float deltaTime, float duration, float amplitude, float period)
    {
        float ss, d;

        d = e - s;

        if (deltaTime == 0.0f)
            return s;

        if ((deltaTime /= duration) == 1.0f)
            return e;

        if (period == 0)
            period = duration * 0.3f;

        if (amplitude == 0 || (d > 0 && amplitude < d) || (d < 0 && amplitude < -d))
        {
            amplitude = d;
            ss = period / 4;
        }
        else
            ss = period / TWO_PI * Mathf.Asin(d / amplitude);

        return (amplitude * Mathf.Pow(2, -10 * deltaTime) * Mathf.Sin((deltaTime * duration - ss) * TWO_PI / period) + e);
    }

    private static float InternalElasticEasingInOut(float s, float e, float deltaTime, float duration, float amplitude, float period)
    {
        float ss, d;

        d = e - s;

        if (deltaTime == 0)
            return s;

        if ((deltaTime /= duration * 0.5f) == 2)
            return e;

        if (period == 0)
            period = duration * (0.3f * 1.5f);

        if (amplitude == 0 || (d > 0 && amplitude < d) || (d < 0 && amplitude < -d))
        {
            amplitude = d;
            ss = period / 4;
        }
        else
            ss = period / TWO_PI * Mathf.Asin(d / amplitude);

        if (deltaTime < 1.0f)
            return -0.5f * (amplitude * Mathf.Pow(2, 10 * (deltaTime -= 1)) * Mathf.Sin((deltaTime * duration - ss) * TWO_PI / period)) + s;

        return amplitude * Mathf.Pow(2, -10 * (deltaTime -= 1)) * Mathf.Sin((deltaTime * duration - ss) * TWO_PI / period) * 0.5f + e;
    }

    private static float InternalBackEasingIn(float s, float e, float deltaTime, float duration, float overshoot, float unused2)
    {
        return (e - s) * (deltaTime /= duration) * deltaTime * ((overshoot + 1) * deltaTime - overshoot) + s;
    }

    private static float InternalBackEasingOut(float s, float e, float deltaTime, float duration, float overshoot, float unused2)
    {
        return (e - s) * ((deltaTime = deltaTime / duration - 1) * deltaTime * ((overshoot + 1) * deltaTime + overshoot) + 1) + s;
    }

    private static float InternalBackEasingInOut(float s, float e, float deltaTime, float duration, float overshoot, float unused2)
    {
        if ((deltaTime /= duration * 0.5f) < 1)
        {
            return (e - s) * 0.5f * (deltaTime * deltaTime * (((overshoot *= 1.525f) + 1) * deltaTime - overshoot)) + s;
        }

        return (e - s) / 2 * ((deltaTime -= 2) * deltaTime * (((overshoot *= 1.525f) + 1) * deltaTime + overshoot) + 2) + s;
    }

    private static float InternalBounceEasingIn(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        return (e - s) - InternalBounceEasingOut(0.0f, e - s, duration - deltaTime, duration, unused1, unused2) + s;
    }

    private static float InternalBounceEasingOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        if ((deltaTime /= duration) < (1.0f / 2.75f))
            return (e - s) * (7.5625f * deltaTime * deltaTime) + s;

        if (deltaTime < (2.0f / 2.75f))
            return (e - s) * (7.5625f * (deltaTime -= (1.5f / 2.75f)) * deltaTime + 0.75f) + s;

        if (deltaTime < (2.5f / 2.75f))
            return (e - s) * (7.5625f * (deltaTime -= (2.25f / 2.75f)) * deltaTime + 0.9375f) + s;

        return (e - s) * (7.5625f * (deltaTime -= (2.625f / 2.75f)) * deltaTime + 0.984375f) + s;
    }

    private static float InternalBounceEasingInOut(float s, float e, float deltaTime, float duration, float unused1, float unused2)
    {
        if (deltaTime < duration * 0.5f)
            return InternalBounceEasingIn(0.0f, e - s, deltaTime * 2.0f, duration, unused1, unused2) * 0.5f + s;

        return InternalBounceEasingOut(0.0f, e - s, deltaTime * 2.0f - duration, duration, unused1, unused2) * 0.5f + (e - s) * 0.5f + s;
    }
}