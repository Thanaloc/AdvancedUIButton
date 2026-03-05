using UnityEngine;

namespace AdvancedUI
{
    internal static class EasingFunctions
    {
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1f;
        private const float c4 = (2f * Mathf.PI) / 3f;
        private const float n1 = 7.5625f;
        private const float d1 = 2.75f;

        public static float Evaluate(EasingType type, float t)
        {
            t = Mathf.Clamp01(t);
            switch (type)
            {
                case EasingType.Linear:     return t;
                case EasingType.EaseIn:     return t * t;
                case EasingType.EaseOut:    return 1f - (1f - t) * (1f - t);
                case EasingType.EaseInOut:  return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case EasingType.BackIn:     return c3 * t * t * t - c1 * t * t;
                case EasingType.BackOut:    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                case EasingType.BackInOut:  return BackInOut(t);
                case EasingType.ElasticOut: return ElasticOut(t);
                case EasingType.BounceOut:  return BounceOut(t);
                default:                    return t;
            }
        }

        private static float BackInOut(float t)
        {
            return t < 0.5f
                ? Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2) * 0.5f
                : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (2f * t - 2f) + c2) + 2f) * 0.5f;
        }

        private static float ElasticOut(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        private static float BounceOut(float t)
        {
            if (t < 1f / d1)      return n1 * t * t;
            if (t < 2f / d1)      return n1 * (t -= 1.5f   / d1) * t + 0.75f;
            if (t < 2.5f / d1)    return n1 * (t -= 2.25f  / d1) * t + 0.9375f;
            return                       n1 * (t -= 2.625f  / d1) * t + 0.984375f;
        }
    }
}
