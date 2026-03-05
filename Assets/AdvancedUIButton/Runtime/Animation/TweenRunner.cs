using System.Collections;
using UnityEngine;

namespace AdvancedUI
{
    internal interface ITweenValue
    {
        float Duration       { get; }
        EasingType Easing    { get; }
        bool IgnoreTimeScale { get; }
        bool IsValid         { get; }
        void TweenValue(float t);
    }

    internal struct ColorTween : ITweenValue
    {
        public delegate void ColorTweenCallback(Color c);

        private ColorTweenCallback _callback;
        private Color _start;
        private Color _target;

        public float Duration       { get; private set; }
        public EasingType Easing    { get; private set; }
        public bool IgnoreTimeScale { get; private set; }
        public bool IsValid         => _callback != null;

        public void Set(Color start, Color target, float duration, EasingType easing, bool ignoreTimeScale, ColorTweenCallback callback)
        {
            _start       = start;
            _target      = target;
            Duration       = duration;
            Easing         = easing;
            IgnoreTimeScale = ignoreTimeScale;
            _callback    = callback;
        }

        public void TweenValue(float t)
        {
            _callback(Color.LerpUnclamped(_start, _target, EasingFunctions.Evaluate(Easing, t)));
        }
    }

    internal struct Vector3Tween : ITweenValue
    {
        public delegate void Vector3TweenCallback(Vector3 v);

        private Vector3TweenCallback _callback;
        private Vector3 _start;
        private Vector3 _target;

        public float Duration       { get; private set; }
        public EasingType Easing    { get; private set; }
        public bool IgnoreTimeScale { get; private set; }
        public bool IsValid         => _callback != null;

        public void Set(Vector3 start, Vector3 target, float duration, EasingType easing, bool ignoreTimeScale, Vector3TweenCallback callback)
        {
            _start       = start;
            _target      = target;
            Duration       = duration;
            Easing         = easing;
            IgnoreTimeScale = ignoreTimeScale;
            _callback    = callback;
        }

        public void TweenValue(float t)
        {
            _callback(Vector3.LerpUnclamped(_start, _target, EasingFunctions.Evaluate(Easing, t)));
        }
    }

    internal class TweenRunner<T> where T : struct, ITweenValue
    {
        private MonoBehaviour _host;
        private IEnumerator _runningTween;

        public void Init(MonoBehaviour host)
        {
            _host = host;
        }

        public void StartTween(T value, bool immediate = false)
        {
            if (_host == null) return;

            StopTween();

            if (!value.IsValid) return;

            if (immediate || value.Duration <= 0f || !_host.gameObject.activeInHierarchy)
            {
                value.TweenValue(1f);
                return;
            }

            _runningTween = Run(value);
            _host.StartCoroutine(_runningTween);
        }

        public void StopTween()
        {
            if (_runningTween == null) return;
            _host.StopCoroutine(_runningTween);
            _runningTween = null;
        }

        private static IEnumerator Run(T value)
        {
            float elapsed = 0f;
            float duration = value.Duration;

            while (elapsed < duration)
            {
                elapsed += value.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                value.TweenValue(Mathf.Min(elapsed / duration, 1f));
                yield return null;
            }

            value.TweenValue(1f);
        }
    }
}
