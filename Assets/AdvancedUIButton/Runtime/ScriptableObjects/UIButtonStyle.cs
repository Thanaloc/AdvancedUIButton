using System;
using UnityEngine;

namespace AdvancedUI
{
    [CreateAssetMenu(fileName = "UIButtonStyle", menuName = "AdvancedUI/Button Style", order = 1)]
    public class UIButtonStyle : ScriptableObject
    {
        [Serializable]
        public class GraphicStyle
        {
            public StateColors colors = new StateColors();
        }

        [Serializable]
        public class AnimationStyle
        {
            public bool  enableScale          = true;
            public StateTransform transform   = new StateTransform();
            public float scaleDuration        = 0.12f;
            public EasingType scaleEasing     = EasingType.BackOut;

            public float colorDuration        = 0.10f;
            public EasingType colorEasing     = EasingType.EaseOut;

            public bool ignoreTimeScale       = true;
        }

        [Serializable]
        public class AudioStyle
        {
            public AudioClip hoverEnter;
            public AudioClip press;
            public AudioClip select;
            public AudioClip disabled;

            [Range(0f, 1f)] public float volume         = 1f;
            [Range(0.8f, 1.2f)] public float pitch      = 1f;
            public bool randomizePitch                  = false;
            [Range(0f, 0.2f)] public float pitchVariance = 0.1f;
        }

        public GraphicStyle   background  = new GraphicStyle();
        public GraphicStyle   label       = new GraphicStyle();
        public GraphicStyle   icon        = new GraphicStyle();
        public AnimationStyle animation   = new AnimationStyle();
        public AudioStyle     audio       = new AudioStyle();

#if UNITY_EDITOR
        private void OnValidate()
        {
            animation.scaleDuration = Mathf.Max(0f, animation.scaleDuration);
            animation.colorDuration = Mathf.Max(0f, animation.colorDuration);
        }
#endif
    }
}
