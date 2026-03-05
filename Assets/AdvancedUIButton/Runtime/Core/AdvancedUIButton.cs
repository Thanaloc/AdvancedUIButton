using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdvancedUI
{
    [AddComponentMenu("AdvancedUI/Advanced UI Button")]
    [RequireComponent(typeof(RectTransform))]
    public class AdvancedUIButton : Button
    {
        [Serializable]
        public class HoldSettings
        {
            public float duration = 1f;
            public bool cancelOnExit = true;
            public UnityEvent OnHoldComplete = new UnityEvent();
            public UnityEvent<float> OnHoldProgress = new UnityEvent<float>();
        }

        [SerializeField] private UIButtonStyle _style;
        [SerializeField] private UIButtonAnimator _animator = new UIButtonAnimator();
        [SerializeField] private UIButtonAudio _audio = new UIButtonAudio();
        [SerializeField] private InteractionMode _mode = InteractionMode.Standard;
        [SerializeField] private HoldSettings _holdSettings = new HoldSettings();
        [SerializeField] private bool _ignoreTimeScale = true;
        [SerializeField] private bool _isSelected = false;
        [SerializeField] private UIButtonToggleGroup _toggleGroup;

        [Tooltip("Optional ripple effect component. Assign a UIButtonRipple on a child GameObject.")]
        [SerializeField] private UIButtonRipple _ripple;

        [Space]
        [SerializeField] private UnityEvent _onHoverEnter = new UnityEvent();
        [SerializeField] private UnityEvent _onHoverExit = new UnityEvent();
        [SerializeField] private UnityEvent _onPress = new UnityEvent();
        [SerializeField] private UnityEvent _onRelease = new UnityEvent();
        [SerializeField] private UnityEvent _onSelected = new UnityEvent();
        [SerializeField] private UnityEvent _onDeselected = new UnityEvent();
        [SerializeField] private UnityEvent _onHoldComplete = new UnityEvent();

        public event Action OnHoverEnterEvent;
        public event Action OnHoverExitEvent;
        public event Action OnPressEvent;
        public event Action OnReleaseEvent;
        public event Action<bool> OnSelectionChangedEvent;

        public ButtonState CurrentState { get; private set; } = ButtonState.Normal;
        public bool IsToggleOn => _isSelected;
        public bool IgnoreTimeScale => _ignoreTimeScale;
        public InteractionMode Mode => _mode;

        private bool _isPointerOver = false;
        private bool _isPressed = false;
        private bool _isFocused = false;
        private Coroutine _holdCoroutine;

        private bool _modulesInitialized;

        protected override void Awake()
        {
            base.Awake();
            transition = Transition.None;
        }

        protected override void Start()
        {
            base.Start();
            InitializeModules();
            _modulesInitialized = true;
            ApplyStyle(_style);
            RefreshState(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_modulesInitialized) return;
            RefreshState(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!_modulesInitialized) return;
            _isPointerOver = false;
            _isPressed = false;
            _isFocused = false;
            StopHold();
            RefreshState(true);
        }

        private void InitializeModules()
        {
            _animator.Initialize(this);
            _audio.Initialize(this);
        }

        public void ApplyStyle(UIButtonStyle style)
        {
            _style = style;
            if (style == null) return;
            _animator.ApplyStyle(style);
            _audio.ApplyStyle(style);
            RefreshState(true);
        }

        public void SetSelected(bool selected)
        {
            if (_isSelected == selected) return;
            _isSelected = selected;
            RefreshState(false);

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

        public void ToggleSelection() => SetSelected(!_isSelected);

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _isPointerOver = true;
            RefreshState(false);

            if (!interactable) return;
            _onHoverEnter.Invoke();
            OnHoverEnterEvent?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isPointerOver = false;
            RefreshState(false);

            if (_mode == InteractionMode.Hold && _holdSettings.cancelOnExit)
                StopHold();

            _onHoverExit.Invoke();
            OnHoverExitEvent?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!interactable) return;

            _isPressed = true;
            RefreshState(false);
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
            RefreshState(false);
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

        // Clears EventSystem focus after a mouse/touch click so the button
        // does not stay in Focused state. Keyboard and gamepad navigation
        // are not affected because they never trigger pointer events.
        private static void ClearPointerFocus(PointerEventData eventData)
        {
            if (eventData.pointerId >= -1)
                UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _isFocused = true;
            RefreshState(false);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _isFocused = false;
            RefreshState(false);
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
            ButtonState previous = CurrentState;
            ButtonState next = ResolveState();

            if (previous == next && !immediate) return;

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
            float duration = _holdSettings.duration;

            while (elapsed < duration)
            {
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                _holdSettings.OnHoldProgress?.Invoke(Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            _holdSettings.OnHoldComplete?.Invoke();
            _onHoldComplete.Invoke();
        }

        public void SetInteractable(bool value)
        {
            interactable = value;
            RefreshState(false);
        }

        public void ForceRefresh() => RefreshState(true);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            transition = Transition.None;
        }

        public UIButtonStyle EditorStyle => _style;
        public UIButtonAnimator EditorAnimator => _animator;
        public UIButtonAudio EditorAudio => _audio;
        public InteractionMode EditorMode => _mode;
        public HoldSettings EditorHoldSettings => _holdSettings;
#endif
    }
}