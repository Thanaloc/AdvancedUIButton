// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using System;
using UnityEngine;

namespace AdvancedUI
{
    /// <summary>
    /// ScriptableObject that bundles all visual and animation settings for a button.
    /// Assign to an AdvancedUIButton and click Apply Style to push all values at once.
    /// Any future change to the asset is applied by clicking Apply Style again.
    /// </summary>
    [CreateAssetMenu(fileName = "UIButtonStyle", menuName = "AdvancedUI/Button Style", order = 1)]
    public class UIButtonStyle : ScriptableObject
    {
        /// <summary>Color settings for one graphic role (Background, Label, or Icon).</summary>
        [Serializable]
        public sealed class GraphicStyle
        {
            public StateColors colors = new StateColors();
        }

        /// <summary>Scale and color animation settings stored inside a style asset.</summary>
        [Serializable]
        public sealed class AnimationStyle
        {
            [Tooltip("Enable scale animation on the button RectTransform.")]
            public bool enableScale = true;

            [Tooltip("Per-state target scale values.")]
            public StateTransform transform = new StateTransform();

            [Tooltip("Duration in seconds for the scale transition.")]
            [Min(0f)] public float scaleDuration = 0.12f;

            [Tooltip("Easing function applied to the scale transition. BackOut gives a springy feel.")]
            public EasingType scaleEasing = EasingType.BackOut;

            [Tooltip("Duration in seconds for color transitions on all graphic entries.")]
            [Min(0f)] public float colorDuration = 0.10f;

            [Tooltip("Easing function applied to color transitions.")]
            public EasingType colorEasing = EasingType.EaseOut;

            [Tooltip("When enabled, all animations use unscaledDeltaTime and stay responsive when the game is paused.")]
            public bool ignoreTimeScale = true;
        }

        /// <summary>Audio clip and playback settings stored inside a style asset.</summary>
        [Serializable]
        public sealed class AudioStyle
        {
            [Tooltip("Clip played when the pointer enters the button.")]
            public AudioClip hoverEnter;

            [Tooltip("Clip played when the button is pressed.")]
            public AudioClip press;

            [Tooltip("Clip played when the button becomes selected (Toggle/Radio on).")]
            public AudioClip select;

            [Tooltip("Clip played when the button becomes deselected (Toggle/Radio off).")]
            public AudioClip deselect;

            [Tooltip("Clip played when the button enters the Disabled state.")]
            public AudioClip disabled;

            [Tooltip("Playback volume multiplier.")]
            [Range(0f, 1f)] public float volume = 1f;

            [Tooltip("Base pitch for all clips.")]
            [Range(0.5f, 2f)] public float pitch = 1f;

            [Tooltip("Adds a small random pitch offset on each play to prevent repetition fatigue.")]
            public bool randomizePitch = false;

            [Tooltip("Maximum random pitch offset applied in either direction.")]
            [Range(0f, 0.3f)] public float pitchVariance = 0.1f;
        }

        [Tooltip("Colors applied to the Background graphic entry.")]
        public GraphicStyle background = new GraphicStyle();

        [Tooltip("Colors applied to the Label graphic entry.")]
        public GraphicStyle label = new GraphicStyle();

        [Tooltip("Colors applied to the Icon graphic entry.")]
        public GraphicStyle icon = new GraphicStyle();

        [Tooltip("Scale and color animation settings.")]
        public AnimationStyle animation = new AnimationStyle();

        [Tooltip("Audio clip and playback settings.")]
        public AudioStyle audio = new AudioStyle();

#if UNITY_EDITOR
        private void OnValidate()
        {
            animation.scaleDuration = Mathf.Max(0f, animation.scaleDuration);
            animation.colorDuration = Mathf.Max(0f, animation.colorDuration);
        }
#endif
    }
}