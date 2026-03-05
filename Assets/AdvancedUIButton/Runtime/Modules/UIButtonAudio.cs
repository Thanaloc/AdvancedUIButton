using System;
using UnityEngine;

namespace AdvancedUI
{
    [Serializable]
    public class UIButtonAudio : IButtonModule
    {
        public bool         enabled         = false;
        public AudioSource  source;

        [Space]
        public AudioClip    onHoverEnter;
        public AudioClip    onPress;
        public AudioClip    onSelect;
        public AudioClip    onDeselect;
        public AudioClip    onDisabled;

        [Space]
        [Range(0f, 1f)]  public float volume          = 1f;
        [Range(0.5f, 2f)] public float pitch          = 1f;
        public bool randomizePitch                    = false;
        [Range(0f, 0.3f)] public float pitchVariance  = 0.1f;

        private AdvancedUIButton _button;

        public void Initialize(AdvancedUIButton button)
        {
            _button = button;
        }

        public void OnStateChanged(ButtonState previous, ButtonState next, bool immediate)
        {
            if (!enabled || immediate) return;

            switch (next)
            {
                case ButtonState.Highlighted:
                case ButtonState.SelectedHighlighted:
                    if (previous != ButtonState.Highlighted && previous != ButtonState.SelectedHighlighted)
                        Play(onHoverEnter);
                    break;

                case ButtonState.Pressed:
                    Play(onPress);
                    break;

                case ButtonState.Selected:
                    Play(previous == ButtonState.Selected ? onDeselect : onSelect);
                    break;

                case ButtonState.Disabled:
                    Play(onDisabled);
                    break;
            }
        }

        public void Play(AudioClip clip)
        {
            if (!enabled || source == null || clip == null) return;

            float p = randomizePitch
                ? pitch + UnityEngine.Random.Range(-pitchVariance, pitchVariance)
                : pitch;

            source.pitch = p;
            source.PlayOneShot(clip, volume);
        }

        public void ApplyStyle(UIButtonStyle style)
        {
            if (style == null) return;

            var a        = style.audio;
            onHoverEnter = a.hoverEnter;
            onPress      = a.press;
            onSelect     = a.select;
            onDisabled   = a.disabled;
            volume       = a.volume;
            pitch        = a.pitch;
            randomizePitch   = a.randomizePitch;
            pitchVariance    = a.pitchVariance;
        }
    }
}
