// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedUI
{
    /// <summary>
    /// Describes a single Graphic that the button animates.
    /// Assign a role to enable automatic color mapping from a UIButtonStyle asset.
    /// </summary>
    [Serializable]
    public sealed class GraphicEntry
    {
        [Tooltip("The Graphic component to colorize. Accepts any Graphic subclass (Image, TMP_Text, etc.).")]
        [SerializeField] private Graphic _target;

        [Tooltip("Background, Label and Icon entries receive their colors from the Style asset on Apply Style. Custom entries are never overwritten.")]
        [SerializeField] private GraphicRole _role = GraphicRole.Custom;

        [SerializeField] private StateColors _colors = new StateColors();

        [Tooltip("Color transition duration in seconds. Set to 0 for an instant transition.")]
        [SerializeField, Min(0f)] private float _duration = 0.10f;

        [Tooltip("Easing function applied to the color transition.")]
        [SerializeField] private EasingType _easing = EasingType.EaseOut;

        // Public read-only accessors
        public Graphic Target => _target;
        public GraphicRole Role => _role;
        public StateColors Colors => _colors;
        public float Duration => _duration;
        public EasingType Easing => _easing;

        // Internal tween state -- not serialized, initialized at runtime.
        [NonSerialized] internal TweenRunner<ColorTween> ColorTween;
        [NonSerialized] internal ColorTween ColorTweenValue;

        /// <summary>
        /// Applies style-derived values. Called only by UIButtonAnimator.ApplyStyle.
        /// Kept internal to prevent accidental modification from user code.
        /// </summary>
        internal void ApplyStyleValues(float duration, EasingType easing)
        {
            _duration = duration;
            _easing = easing;
        }
    }

    /// <summary>
    /// Handles color and scale animations for all registered graphics.
    /// Supports independent per-graphic transitions with individual durations and easings.
    /// </summary>
    [Serializable]
    public sealed class UIButtonAnimator : IButtonModule
    {
        /// <summary>Settings for the scale animation applied to the button's RectTransform.</summary>
        [Serializable]
        public sealed class TransformSettings
        {
            [Tooltip("Enable scale animation on state changes.")]
            [SerializeField] private bool _enabled = true;

            [SerializeField] private StateTransform _states = new StateTransform();

            [Tooltip("Scale transition duration in seconds.")]
            [SerializeField, Min(0f)] private float _duration = 0.12f;

            [Tooltip("Easing function applied to the scale transition. BackOut gives a springy feel.")]
            [SerializeField] private EasingType _easing = EasingType.BackOut;

            public bool Enabled => _enabled;
            public StateTransform States => _states;
            public float Duration => _duration;
            public EasingType Easing => _easing;

            // Writable from ApplyStyle only
            internal void Apply(bool enabled, StateTransform states, float duration, EasingType easing)
            {
                _enabled = enabled;
                _states = states;
                _duration = duration;
                _easing = easing;
            }
        }

        [SerializeField] private List<GraphicEntry> _graphicEntries = new List<GraphicEntry>();
        [SerializeField] private TransformSettings _transformSettings = new TransformSettings();

        private AdvancedUIButton _button;
        private RectTransform _rectTransform;
        private TweenRunner<Vector3Tween> _scaleTween;
        private Vector3Tween _scaleTweenValue;
        private bool _initialized;

        // IButtonModule

        public void Initialize(AdvancedUIButton button)
        {
            _button = button;
            _rectTransform = button.GetComponent<RectTransform>();

            _scaleTween = new TweenRunner<Vector3Tween>();
            _scaleTween.Init(button);

            for (int i = 0; i < _graphicEntries.Count; i++)
            {
                _graphicEntries[i].ColorTween = new TweenRunner<ColorTween>();
                _graphicEntries[i].ColorTween.Init(button);
            }

            _initialized = true;
        }

        public void OnStateChanged(ButtonState previous, ButtonState next, bool immediate)
        {
            if (!_initialized) return;

            bool ignoreTime = _button.IgnoreTimeScale;

            AnimateGraphics(next, immediate, ignoreTime);
            AnimateScale(next, immediate, ignoreTime);
        }

        // Public API

        /// <summary>
        /// Applies animation settings and role-based colors from a UIButtonStyle asset.
        /// Entries with role Custom are never modified.
        /// </summary>
        public void ApplyStyle(UIButtonStyle style)
        {
            if (style == null) return;

            UIButtonStyle.AnimationStyle anim = style.animation;

            _transformSettings.Apply(
                anim.enableScale,
                anim.transform,
                anim.scaleDuration,
                anim.scaleEasing
            );

            for (int i = 0; i < _graphicEntries.Count; i++)
            {
                GraphicEntry entry = _graphicEntries[i];

                // Animation timing is applied to all entries regardless of role.
                // We use reflection-free direct field access via a helper to keep
                // the serialized fields private while allowing ApplyStyle to write them.
                ApplyEntryStyle(entry, anim.colorDuration, anim.colorEasing, style);
            }
        }

        /// <summary>Stops all running tween animations immediately.</summary>
        public void StopAll()
        {
            _scaleTween?.StopTween();
            for (int i = 0; i < _graphicEntries.Count; i++)
                _graphicEntries[i].ColorTween?.StopTween();
        }

        // Private implementation

        private void AnimateGraphics(ButtonState next, bool immediate, bool ignoreTime)
        {
            for (int i = 0; i < _graphicEntries.Count; i++)
            {
                GraphicEntry entry = _graphicEntries[i];
                if (entry.Target == null) continue;

                Color targetColor = entry.Colors.ForState(next);

                if (immediate || entry.Duration <= 0f || entry.ColorTween == null)
                {
                    entry.ColorTween?.StopTween();
                    entry.Target.color = targetColor;
                    continue;
                }

                entry.ColorTweenValue.Set(
                    entry.Target.color,
                    targetColor,
                    entry.Duration,
                    entry.Easing,
                    ignoreTime,
                    c => entry.Target.color = c
                );
                entry.ColorTween.StartTween(entry.ColorTweenValue);
            }
        }

        private void AnimateScale(ButtonState next, bool immediate, bool ignoreTime)
        {
            if (!_transformSettings.Enabled || _rectTransform == null) return;

            Vector3 targetScale = _transformSettings.States.ScaleForState(next);

            if (immediate || _transformSettings.Duration <= 0f)
            {
                _scaleTween?.StopTween();
                _rectTransform.localScale = targetScale;
                return;
            }

            _scaleTweenValue.Set(
                _rectTransform.localScale,
                targetScale,
                _transformSettings.Duration,
                _transformSettings.Easing,
                ignoreTime,
                v => _rectTransform.localScale = v
            );
            _scaleTween.StartTween(_scaleTweenValue);
        }

        private static void ApplyEntryStyle(GraphicEntry entry, float colorDuration, EasingType colorEasing, UIButtonStyle style)
        {
            entry.ApplyStyleValues(colorDuration, colorEasing);

            switch (entry.Role)
            {
                case GraphicRole.Background: entry.Colors.CopyFrom(style.background.colors); break;
                case GraphicRole.Label: entry.Colors.CopyFrom(style.label.colors); break;
                case GraphicRole.Icon: entry.Colors.CopyFrom(style.icon.colors); break;
                    // GraphicRole.Custom: no color override
            }
        }
    }
}