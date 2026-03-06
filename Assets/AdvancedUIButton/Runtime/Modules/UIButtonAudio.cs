// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using UnityEngine;

namespace AdvancedUI
{
    /// <summary>
    /// Handles audio feedback for button state changes.
    /// Attach an AudioSource to any persistent GameObject and assign it here.
    /// </summary>
    [System.Serializable]
    public sealed class UIButtonAudio : IButtonModule
    {
        // Serialized fields

        [Tooltip("Enable audio feedback on state changes.")]
        [SerializeField] private bool _enabled;

        [Tooltip("AudioSource used to play clips. Can live on any persistent GameObject.")]
        [SerializeField] private AudioSource _source;

        [Header("Clips")]
        [Tooltip("Played when the pointer enters the button.")]
        [SerializeField] private AudioClip _onHoverEnter;

        [Tooltip("Played when the button is pressed.")]
        [SerializeField] private AudioClip _onPress;

        [Tooltip("Played when the button is selected (Toggle/Radio on).")]
        [SerializeField] private AudioClip _onSelect;

        [Tooltip("Played when the button is deselected (Toggle/Radio off).")]
        [SerializeField] private AudioClip _onDeselect;

        [Tooltip("Played when the button becomes disabled while hovered.")]
        [SerializeField] private AudioClip _onDisabled;

        [Header("Settings")]
        [Tooltip("Playback volume multiplier applied to all clips.")]
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;

        [Tooltip("Base pitch applied to all clips.")]
        [SerializeField, Range(0.5f, 2f)] private float _pitch = 1f;

        [Tooltip("Adds a random offset to pitch on each play, preventing repetition fatigue.")]
        [SerializeField] private bool _randomizePitch;

        [Tooltip("Maximum random pitch offset applied in either direction.")]
        [SerializeField, Range(0f, 0.3f)] private float _pitchVariance = 0.1f;

        // IButtonModule

        public void Initialize(AdvancedUIButton button) { }

        public void OnStateChanged(ButtonState previous, ButtonState next, bool immediate)
        {
            if (!_enabled || immediate) return;

            switch (next)
            {
                case ButtonState.SelectedHighlighted:
                    // Only play hover sound when entering hover from a non-hover state.
                    if (previous != ButtonState.Highlighted &&
                        previous != ButtonState.SelectedHighlighted)
                        Play(_onHoverEnter);
                    break;

                case ButtonState.Pressed:
                    Play(_onPress);
                    break;

                case ButtonState.Selected:
                    // next == Selected always means we just became selected.
                    // Deselect sound is handled in the Normal/Highlighted cases below.
                    Play(_onSelect);
                    break;

                case ButtonState.Normal:
                    // Transitioning from Selected or SelectedHighlighted to Normal = deselection.
                    if (previous == ButtonState.Selected ||
                        previous == ButtonState.SelectedHighlighted)
                        Play(_onDeselect);
                    break;

                case ButtonState.Highlighted:
                    // Pointer re-entered after click that deselected from SelectedHighlighted.
                    // previous == SelectedHighlighted means the user clicked while hovering,
                    // which deselected the button and left the pointer still over it.
                    if (previous == ButtonState.SelectedHighlighted)
                        Play(_onDeselect);
                    else if (previous != ButtonState.Highlighted)
                        Play(_onHoverEnter);
                    break;

                case ButtonState.Disabled:
                    Play(_onDisabled);
                    break;
            }
        }

        // Public API

        /// <summary>Plays a clip through the assigned AudioSource.</summary>
        public void Play(AudioClip clip)
        {
            if (!_enabled || _source == null || clip == null) return;

            _source.pitch = _randomizePitch
                ? _pitch + Random.Range(-_pitchVariance, _pitchVariance)
                : _pitch;

            _source.PlayOneShot(clip, _volume);
        }

        /// <summary>Applies audio settings from a UIButtonStyle asset.</summary>
        public void ApplyStyle(UIButtonStyle style)
        {
            if (style == null) return;

            UIButtonStyle.AudioStyle a = style.audio;
            _onHoverEnter = a.hoverEnter;
            _onPress = a.press;
            _onSelect = a.select;
            _onDeselect = a.deselect;
            _onDisabled = a.disabled;
            _volume = a.volume;
            _pitch = a.pitch;
            _randomizePitch = a.randomizePitch;
            _pitchVariance = a.pitchVariance;
        }
    }
}