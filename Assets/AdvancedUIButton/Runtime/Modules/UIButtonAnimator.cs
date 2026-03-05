using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedUI
{
    [Serializable]
    public class GraphicEntry
    {
        [Tooltip("The UI Graphic (Image, Text, etc.) to animate.")]
        public Graphic target;

        [Tooltip("Role of this graphic. Background, Label and Icon are mapped automatically when applying a Style asset. Custom entries are never overwritten by styles.")]
        public GraphicRole role = GraphicRole.Custom;

        public StateColors colors = new StateColors();

        [Tooltip("Duration of the color transition in seconds.")]
        public float duration = 0.10f;

        [Tooltip("Easing function applied to the color transition.")]
        public EasingType easing = EasingType.EaseOut;

        [NonSerialized] internal TweenRunner<ColorTween> colorTween;
        [NonSerialized] internal ColorTween colorTweenValue;
    }

    [Serializable]
    public class UIButtonAnimator : IButtonModule
    {
        [Serializable]
        public class TransformSettings
        {
            [Tooltip("Enable scale animation on state changes.")]
            public bool enabled = true;

            public StateTransform states = new StateTransform();

            [Tooltip("Duration of the scale transition in seconds.")]
            public float duration = 0.12f;

            [Tooltip("Easing function applied to the scale transition.")]
            public EasingType easing = EasingType.BackOut;
        }

        public List<GraphicEntry> graphicEntries = new List<GraphicEntry>();
        public TransformSettings transformSettings = new TransformSettings();

        private AdvancedUIButton _button;
        private RectTransform _rectTransform;
        private TweenRunner<Vector3Tween> _scaleTween;
        private Vector3Tween _scaleTweenValue;
        private bool _initialized;

        public void Initialize(AdvancedUIButton button)
        {
            _button = button;
            _rectTransform = button.GetComponent<RectTransform>();
            _scaleTween = new TweenRunner<Vector3Tween>();
            _scaleTween.Init(button);

            for (int i = 0; i < graphicEntries.Count; i++)
            {
                graphicEntries[i].colorTween = new TweenRunner<ColorTween>();
                graphicEntries[i].colorTween.Init(button);
            }

            _initialized = true;
        }

        public void OnStateChanged(ButtonState previous, ButtonState next, bool immediate)
        {
            if (!_initialized) return;

            bool ignoreTime = _button.IgnoreTimeScale;

            for (int i = 0; i < graphicEntries.Count; i++)
            {
                var entry = graphicEntries[i];
                if (entry.target == null) continue;

                Color targetColor = entry.colors.ForState(next);

                if (immediate || entry.duration <= 0f || entry.colorTween == null)
                {
                    entry.colorTween?.StopTween();
                    entry.target.color = targetColor;
                }
                else
                {
                    entry.colorTweenValue.Set(
                        entry.target.color,
                        targetColor,
                        entry.duration,
                        entry.easing,
                        ignoreTime,
                        c => entry.target.color = c
                    );
                    entry.colorTween.StartTween(entry.colorTweenValue);
                }
            }

            if (!transformSettings.enabled || _rectTransform == null) return;

            Vector3 targetScale = transformSettings.states.ScaleForState(next);

            if (immediate || transformSettings.duration <= 0f || _scaleTween == null)
            {
                _scaleTween?.StopTween();
                _rectTransform.localScale = targetScale;
            }
            else
            {
                _scaleTweenValue.Set(
                    _rectTransform.localScale,
                    targetScale,
                    transformSettings.duration,
                    transformSettings.easing,
                    ignoreTime,
                    v => _rectTransform.localScale = v
                );
                _scaleTween.StartTween(_scaleTweenValue);
            }
        }

        /// <summary>
        /// Applies a UIButtonStyle asset. Maps Background, Label and Icon
        /// roles to the corresponding style colors. Animation settings
        /// (duration, easing, scale) are always applied to all entries.
        /// </summary>
        public void ApplyStyle(UIButtonStyle style)
        {
            if (style == null) return;

            var anim = style.animation;
            transformSettings.states = anim.transform;
            transformSettings.duration = anim.scaleDuration;
            transformSettings.easing = anim.scaleEasing;
            transformSettings.enabled = anim.enableScale;

            for (int i = 0; i < graphicEntries.Count; i++)
            {
                var entry = graphicEntries[i];

                entry.duration = anim.colorDuration;
                entry.easing = anim.colorEasing;

                switch (entry.role)
                {
                    case GraphicRole.Background:
                        entry.colors.CopyFrom(style.background.colors);
                        break;
                    case GraphicRole.Label:
                        entry.colors.CopyFrom(style.label.colors);
                        break;
                    case GraphicRole.Icon:
                        entry.colors.CopyFrom(style.icon.colors);
                        break;
                }
            }
        }

        public void StopAll()
        {
            _scaleTween?.StopTween();
            for (int i = 0; i < graphicEntries.Count; i++)
                graphicEntries[i].colorTween?.StopTween();
        }
    }
}