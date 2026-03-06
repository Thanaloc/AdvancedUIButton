// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.
// Version 1.0.0

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdvancedUI
{
    /// <summary>
    /// Drop-in replacement for Unity's Button component with extended state machine,
    /// multi-graphic transitions, ripple effect, and ScriptableObject-based styling.
    /// Supports Standard, Toggle, Radio, and Hold interaction modes.
    /// </summary>
    [AddComponentMenu("AdvancedUI/Advanced UI Button")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class AdvancedUIButton : Button
    {
        // Nested types

        /// <summary>Settings for Hold interaction mode.</summary>
        [Serializable]
        public sealed class HoldSettings
        {
            [Tooltip("Duration in seconds the user must hold to trigger OnHoldComplete.")]
            [SerializeField, Min(0.1f)] private float _duration = 1f;

            [Tooltip("If enabled, moving the pointer outside the button cancels the hold.")]
            [SerializeField] private bool _cancelOnExit = true;

            /// <summary>Fired every frame while holding. Value is normalized progress 0-1.</summary>
            [SerializeField] private UnityEvent<float> _onHoldProgress = new UnityEvent<float>();

            /// <summary>Fired once when the hold duration is fully elapsed.</summary>
            [SerializeField] private UnityEvent _onHoldComplete = new UnityEvent();

            public float Duration => _duration;
            public bool CancelOnExit => _cancelOnExit;
            public UnityEvent<float> OnHoldProgress => _onHoldProgress;
            public UnityEvent OnHoldComplete => _onHoldComplete;
        }

        // Serialized fields

        [Tooltip("Optional ScriptableObject style asset. Click Apply Style after assigning.")]
        [SerializeField] private UIButtonStyle _style;

        [SerializeField] private UIButtonAnimator _animator = new UIButtonAnimator();
        [SerializeField] private UIButtonAudio _audio = new UIButtonAudio();

        [Tooltip("Determines how the button responds to clicks.")]
        [SerializeField] private InteractionMode _mode = InteractionMode.Standard;

        [SerializeField] private HoldSettings _holdSettings = new HoldSettings();

        [Tooltip("When enabled, all animations use unscaledDeltaTime and remain responsive while the game is paused.")]
        [SerializeField] private bool _ignoreTimeScale = true;

        [Tooltip("Initial selection state. Used by Toggle and Radio modes.")]
        [SerializeField] private bool _isSelected;

        [Tooltip("The toggle group this button belongs to. Used with Radio and Toggle modes.")]
        [SerializeField] private UIButtonToggleGroup _toggleGroup;

        [Tooltip("Optional ripple effect component on a child GameObject.")]
        [SerializeField] private UIButtonRipple _ripple;

        [Space]
        [SerializeField] private UnityEvent _onHoverEnter = new UnityEvent();
        [SerializeField] private UnityEvent _onHoverExit = new UnityEvent();
        [SerializeField] private UnityEvent _onPress = new UnityEvent();
        [SerializeField] private UnityEvent _onRelease = new UnityEvent();
        [SerializeField] private UnityEvent _onSelected = new UnityEvent();
        [SerializeField] private UnityEvent _onDeselected = new UnityEvent();
        [SerializeField] private UnityEvent _onHoldComplete = new UnityEvent();

        // C# events

        /// <summary>Raised when the pointer enters the button.</summary>
        public event Action OnHoverEnterEvent;

        /// <summary>Raised when the pointer exits the button.</summary>
        public event Action OnHoverExitEvent;

        /// <summary>Raised when the button is pressed.</summary>
        public event Action OnPressEvent;

        /// <summary>Raised when the button is released.</summary>
        public event Action OnReleaseEvent;

        /// <summary>Raised when the selection state changes. Argument is the new state.</summary>
        public event Action<bool> OnSelectionChangedEvent;

        // Public properties

        /// <summary>The current resolved button state.</summary>
        public ButtonState CurrentState { get; private set; } = ButtonState.Normal;

        /// <summary>Whether the button is currently in a selected/toggled-on state.</summary>
        public bool IsToggleOn => _isSelected;

        /// <summary>Whether animations use unscaledDeltaTime.</summary>
        public bool IgnoreTimeScale => _ignoreTimeScale;

        /// <summary>The active interaction mode.</summary>
        public InteractionMode Mode => _mode;

        // Private state

        private bool _isPointerOver;
        private bool _isPressed;
        private bool _isFocused;
        private Coroutine _holdCoroutine;
        private bool _modulesInitialized;

        // Lifecycle

        protected override void Awake()
        {
            base.Awake();
            // Disable Unity's built-in transition system -- we handle everything.
            transition = Transition.None;
        }

        protected override void Start()
        {
            base.Start();
            InitializeModules();
            _modulesInitialized = true;
            ApplyStyle(_style);
            RefreshState(immediate: true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_modulesInitialized) return;
            RefreshState(immediate: true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!_modulesInitialized) return;
            _isPointerOver = false;
            _isPressed = false;
            _isFocused = false;
            StopHold();
            RefreshState(immediate: true);
        }

        // Public API

        /// <summary>
        /// Applies a UIButtonStyle asset, pushing its colors and animation
        /// settings to all graphic entries and the ripple component.
        /// </summary>
        /// <param name="style">The style asset to apply. Pass null to clear.</param>
        public void ApplyStyle(UIButtonStyle style)
        {
            _style = style;
            if (style == null) return;
            _animator.ApplyStyle(style);
            _audio.ApplyStyle(style);
            _ripple?.ApplyStyle(style);
            RefreshState(immediate: true);
        }

        /// <summary>Sets the selected state programmatically.</summary>
        /// <param name="selected">The desired selection state.</param>
        public void SetSelected(bool selected)
        {
            if (_isSelected == selected) return;
            _isSelected = selected;
            RefreshState(immediate: false);

            if (selected)
            {
                _onSelected.Invoke();
                _toggleGroup?.NotifySelected(this);
            }
            else
            {
                _onDeselected.Invoke();
            }

            OnSelectionChangedEvent?.Invoke(_isSelected);
        }

        /// <summary>Toggles the current selection state.</summary>
        public void ToggleSelection() => SetSelected(!_isSelected);

        /// <summary>Sets the button's interactable state and refreshes its visual state.</summary>
        public void SetInteractable(bool value)
        {
            interactable = value;
            RefreshState(immediate: false);
        }

        /// <summary>Forces an immediate visual refresh without animation.</summary>
        public void ForceRefresh() => RefreshState(immediate: true);

        // Pointer events

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _isPointerOver = true;
            RefreshState(immediate: false);
            if (!interactable) return;
            _onHoverEnter.Invoke();
            OnHoverEnterEvent?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isPointerOver = false;
            RefreshState(immediate: false);

            if (_mode == InteractionMode.Hold && _holdSettings.CancelOnExit)
                StopHold();

            _onHoverExit.Invoke();
            OnHoverExitEvent?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!interactable) return;

            _isPressed = true;
            RefreshState(immediate: false);
            _ripple?.Spawn(eventData.position);
            _onPress.Invoke();
            OnPressEvent?.Invoke();

            if (_mode == InteractionMode.Hold)
                StartHold();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _isPressed = false;
            RefreshState(immediate: false);
            StopHold();
            _onRelease.Invoke();
            OnReleaseEvent?.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;

            if (_mode == InteractionMode.Toggle || _mode == InteractionMode.Radio)
            {
                if (_mode == InteractionMode.Radio && _isSelected) return;
                SetSelected(!_isSelected);
                ClearPointerFocus(eventData);
                return;
            }

            if (_mode == InteractionMode.Hold) return;

            base.OnPointerClick(eventData);
            ClearPointerFocus(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _isFocused = true;
            RefreshState(immediate: false);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _isFocused = false;
            RefreshState(immediate: false);
        }

        // Private implementation

        private void InitializeModules()
        {
            _animator.Initialize(this);
            _audio.Initialize(this);
        }

        private ButtonState ResolveState()
        {
            if (!interactable) return ButtonState.Disabled;
            if (_isSelected && _isPointerOver) return ButtonState.SelectedHighlighted;
            if (_isSelected) return ButtonState.Selected;
            if (_isPressed) return ButtonState.Pressed;
            if (_isPointerOver) return ButtonState.Highlighted;
            if (_isFocused) return ButtonState.Focused;
            return ButtonState.Normal;
        }

        private void RefreshState(bool immediate)
        {
            ButtonState next = ResolveState();
            if (CurrentState == next && !immediate) return;

            ButtonState previous = CurrentState;
            CurrentState = next;
            _animator.OnStateChanged(previous, next, immediate);
            _audio.OnStateChanged(previous, next, immediate);
        }

        private void StartHold()
        {
            StopHold();
            _holdCoroutine = StartCoroutine(HoldRoutine());
        }

        private void StopHold()
        {
            if (_holdCoroutine == null) return;
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
            _holdSettings.OnHoldProgress?.Invoke(0f);
        }

        private IEnumerator HoldRoutine()
        {
            float elapsed = 0f;
            float duration = _holdSettings.Duration;

            while (elapsed < duration)
            {
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                _holdSettings.OnHoldProgress?.Invoke(Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            _holdSettings.OnHoldComplete?.Invoke();
            _onHoldComplete.Invoke();
        }

        // Clears EventSystem focus after a mouse/touch click.
        // Keyboard and gamepad navigation are not affected (they use pointerId < -1).
        private static void ClearPointerFocus(PointerEventData eventData)
        {
            if (eventData.pointerId >= -1)
                EventSystem.current?.SetSelectedGameObject(null);
        }

        // Editor support

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            transition = Transition.None;
        }

        // Editor-only accessors for the custom Inspector.
        // Prefixed with Editor to make clear they are not part of the runtime API.
        public UIButtonStyle EditorStyle => _style;
        public UIButtonAnimator EditorAnimator => _animator;
        public UIButtonAudio EditorAudio => _audio;
        public InteractionMode EditorMode => _mode;
        public HoldSettings EditorHoldSettings => _holdSettings;
#endif
    }
}