// AdvancedUIButton — Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using System.Collections;
using UnityEngine;

namespace AdvancedUI
{
    // ITweenValue is intentionally internal — it is an implementation detail
    // of the tween system and not part of the public API surface.

    internal interface ITweenValue
    {
        float Duration { get; }
        EasingType Easing { get; }
        bool IgnoreTimeScale { get; }
        bool IsValid { get; }
        void TweenValue(float normalizedTime);
    }

    internal struct ColorTween : ITweenValue
    {
        public delegate void ColorTweenCallback(Color color);

        private ColorTweenCallback _callback;
        private Color _start;
        private Color _target;

        public float Duration { get; private set; }
        public EasingType Easing { get; private set; }
        public bool IgnoreTimeScale { get; private set; }
        public bool IsValid => _callback != null;

        public void Set(
            Color start, Color target,
            float duration, EasingType easing,
            bool ignoreTimeScale,
            ColorTweenCallback callback)
        {
            _start = start;
            _target = target;
            Duration = duration;
            Easing = easing;
            IgnoreTimeScale = ignoreTimeScale;
            _callback = callback;
        }

        public void TweenValue(float t)
        {
            _callback(Color.LerpUnclamped(_start, _target, EasingFunctions.Evaluate(Easing, t)));
        }
    }

    internal struct Vector3Tween : ITweenValue
    {
        public delegate void Vector3TweenCallback(Vector3 value);

        private Vector3TweenCallback _callback;
        private Vector3 _start;
        private Vector3 _target;

        public float Duration { get; private set; }
        public EasingType Easing { get; private set; }
        public bool IgnoreTimeScale { get; private set; }
        public bool IsValid => _callback != null;

        public void Set(
            Vector3 start, Vector3 target,
            float duration, EasingType easing,
            bool ignoreTimeScale,
            Vector3TweenCallback callback)
        {
            _start = start;
            _target = target;
            Duration = duration;
            Easing = easing;
            IgnoreTimeScale = ignoreTimeScale;
            _callback = callback;
        }

        public void TweenValue(float t)
        {
            _callback(Vector3.LerpUnclamped(_start, _target, EasingFunctions.Evaluate(Easing, t)));
        }
    }

    /// <summary>
    /// Lightweight coroutine-based tween engine.
    /// Runs a single tween at a time; starting a new tween automatically stops the previous one.
    /// Uses struct-based tween values to avoid GC allocations in steady state.
    /// </summary>
    internal sealed class TweenRunner<T> where T : struct, ITweenValue
    {
        private MonoBehaviour _host;
        private IEnumerator _runningTween;

        /// <summary>Must be called once before starting any tween.</summary>
        public void Init(MonoBehaviour host)
        {
            _host = host;
        }

        /// <summary>
        /// Starts a tween. If a tween is already running it is stopped first.
        /// If the host is inactive or duration is zero, the final value is applied immediately.
        /// </summary>
        public void StartTween(T value)
        {
            if (_host == null) return;

            StopTween();

            if (!value.IsValid) return;

            if (value.Duration <= 0f || !_host.gameObject.activeInHierarchy)
            {
                value.TweenValue(1f);
                return;
            }

            _runningTween = Run(value);
            _host.StartCoroutine(_runningTween);
        }

        /// <summary>Stops the running tween without snapping to the target value.</summary>
        public void StopTween()
        {
            if (_runningTween == null) return;
            _host?.StopCoroutine(_runningTween);
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