// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using System;
using UnityEngine;

namespace AdvancedUI
{
    public enum ButtonState
    {
        Normal              = 0,
        Highlighted         = 1,
        Pressed             = 2,
        Selected            = 3,
        SelectedHighlighted = 4,
        Focused             = 5,
        Disabled            = 6
    }

    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BackIn,
        BackOut,
        BackInOut,
        ElasticOut,
        BounceOut,
        ExpoOut,
        CircOut
    }

    public enum InteractionMode
    {
        Standard,
        Toggle,
        Radio,
        Hold
    }

    /// <summary>
    /// Defines the role of a graphic entry. Background, Label and Icon
    /// are automatically mapped when applying a UIButtonStyle asset.
    /// Custom entries are not affected by style application.
    /// </summary>
    public enum GraphicRole
    {
        Custom      = 0,
        Background  = 1,
        Label       = 2,
        Icon        = 3
    }

    public interface IButtonModule
    {
        void Initialize(AdvancedUIButton button);
        void OnStateChanged(ButtonState previous, ButtonState next, bool immediate);
    }

    [Serializable]
    public class StateColors
    {
        [Tooltip("Color when the button is in its default resting state.")]
        public Color normal              = Color.white;

        [Tooltip("Color when the pointer is over the button.")]
        public Color highlighted         = new Color(0.92f, 0.92f, 0.92f, 1f);

        [Tooltip("Color while the button is being pressed.")]
        public Color pressed             = new Color(0.75f, 0.75f, 0.75f, 1f);

        [Tooltip("Color when the button is in a selected/toggled-on state.")]
        public Color selected            = new Color(0.78f, 0.85f, 1f,    1f);

        [Tooltip("Color when the button is selected AND the pointer is over it.")]
        public Color selectedHighlighted = new Color(0.68f, 0.76f, 0.98f, 1f);

        [Tooltip("Color when the button has keyboard or gamepad focus.")]
        public Color focused             = new Color(0.88f, 0.92f, 1f,    1f);

        [Tooltip("Color when the button is not interactable.")]
        public Color disabled            = new Color(0.50f, 0.50f, 0.50f, 0.5f);

        public Color ForState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Highlighted:         return highlighted;
                case ButtonState.Pressed:             return pressed;
                case ButtonState.Selected:            return selected;
                case ButtonState.SelectedHighlighted: return selectedHighlighted;
                case ButtonState.Focused:             return focused;
                case ButtonState.Disabled:            return disabled;
                default:                              return normal;
            }
        }

        public void CopyFrom(StateColors other)
        {
            normal              = other.normal;
            highlighted         = other.highlighted;
            pressed             = other.pressed;
            selected            = other.selected;
            selectedHighlighted = other.selectedHighlighted;
            focused             = other.focused;
            disabled            = other.disabled;
        }
    }

    [Serializable]
    public class StateTransform
    {
        [Tooltip("Scale when the button is in its default resting state.")]
        public Vector3 normalScale      = Vector3.one;

        [Tooltip("Scale when the pointer is over the button.")]
        public Vector3 highlightedScale = new Vector3(1.05f, 1.05f, 1f);

        [Tooltip("Scale while the button is being pressed.")]
        public Vector3 pressedScale     = new Vector3(0.95f, 0.95f, 1f);

        [Tooltip("Scale when the button is selected.")]
        public Vector3 selectedScale    = Vector3.one;

        [Tooltip("Scale when the button has keyboard or gamepad focus.")]
        public Vector3 focusedScale     = new Vector3(1.02f, 1.02f, 1f);

        [Tooltip("Scale when the button is not interactable.")]
        public Vector3 disabledScale    = Vector3.one;

        public Vector3 ScaleForState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Highlighted:
                case ButtonState.SelectedHighlighted: return highlightedScale;
                case ButtonState.Pressed:             return pressedScale;
                case ButtonState.Selected:            return selectedScale;
                case ButtonState.Focused:             return focusedScale;
                case ButtonState.Disabled:            return disabledScale;
                default:                              return normalScale;
            }
        }
    }
}