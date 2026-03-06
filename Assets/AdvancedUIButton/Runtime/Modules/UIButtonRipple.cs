// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedUI
{
    /// <summary>
    /// Ripple effect for AdvancedUIButton.
    /// Uses a pre-allocated pool of Image components -- works in any render pipeline,
    /// no custom shader required, zero GC allocation at runtime after initialization.
    /// </summary>
    [AddComponentMenu("AdvancedUI/UI Button Ripple")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIButtonRipple : MonoBehaviour
    {
        // Configuration

        [Tooltip("Color of the ripple. Keep alpha between 0.15 and 0.35.")]
        [SerializeField] private Color _rippleColor = new Color(1f, 1f, 1f, 0.25f);

        [Tooltip("Sprite used for the ripple circle. Leave empty to use a plain white circle generated at runtime.")]
        [SerializeField] private Sprite _sprite;

        [Tooltip("Duration of a single ripple in seconds.")]
        [SerializeField] private float _duration = 0.45f;

        [Tooltip("Easing applied to the ripple expansion.")]
        [SerializeField] private EasingType _expandEasing = EasingType.EaseOut;

        [Tooltip("Maximum number of simultaneous ripples. Pre-allocated at startup.")]
        [SerializeField, Range(1, 6)] private int _poolSize = 3;

        [Tooltip("When enabled, animations use unscaledDeltaTime.")]
        [SerializeField] private bool _ignoreTimeScale = true;

        [Tooltip("When enabled, ApplyStyle() on the parent button automatically derives the ripple color from the style's highlighted background color.")]
        [SerializeField] private bool _syncWithStyle = true;

        // Pool

        private struct RippleEntry
        {
            public RectTransform rectTransform;
            public Image image;
            public bool active;
            public float elapsed;
            public float targetSize;
        }

        private RippleEntry[] _pool;
        private int _activeCount;
        private Coroutine _tickCoroutine;
        private float _maxSize;

        // Lifecycle

        private void Awake()
        {
            BuildPool();
        }

        private void OnEnable()
        {
            RecalculateSize();
        }

        private void OnDisable()
        {
            StopTick();
            HideAll();
        }

        private void OnRectTransformDimensionsChange()
        {
            RecalculateSize();
        }

        // Public API

        public bool SyncWithStyle => _syncWithStyle;

        /// <summary>
        /// Called by AdvancedUIButton.ApplyStyle when SyncWithStyle is enabled.
        /// Derives ripple color from the style highlighted background color.
        /// </summary>
        public void ApplyStyle(UIButtonStyle style)
        {
            if (!_syncWithStyle || style == null) return;
            Color highlighted = style.background.colors.highlighted;
            _rippleColor = new Color(highlighted.r, highlighted.g, highlighted.b, 0.30f);
        }

        /// <summary>Spawns a ripple at the given screen position.</summary>
        public void Spawn(Vector2 screenPosition)
        {
            if (!isActiveAndEnabled) return;
            if (_maxSize <= 0f) RecalculateSize();
            if (_maxSize <= 0f) return;

            Canvas canvas = GetComponentInParent<Canvas>();
            Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    GetComponent<RectTransform>(), screenPosition, cam, out Vector2 local))
                return;

            int slot = AcquireSlot();
            if (slot < 0) return;

            // Compute the radius needed to fully cover the rect from this click point.
            // This ensures the ripple always reaches every corner regardless of click origin.
            Rect rect = GetComponent<RectTransform>().rect;
            float left = local.x - rect.xMin;
            float right = rect.xMax - local.x;
            float bottom = local.y - rect.yMin;
            float top = rect.yMax - local.y;
            float maxDist = Mathf.Sqrt(
                Mathf.Max(left, right) * Mathf.Max(left, right) +
                Mathf.Max(bottom, top) * Mathf.Max(bottom, top)
            ) * 2f; // *2 because sizeDelta is diameter, not radius

            ref RippleEntry entry = ref _pool[slot];
            entry.elapsed = 0f;
            entry.targetSize = maxDist;
            entry.active = true;
            entry.rectTransform.anchoredPosition = local;
            entry.rectTransform.sizeDelta = Vector2.zero;
            entry.image.color = _rippleColor;
            entry.image.gameObject.SetActive(true);
            _activeCount++;

            StartTick();
        }

        // Pool construction

        // Generates a soft circle sprite at runtime -- no external asset required.
        // Texture is 64x64, created once and shared across the pool.
        private static Sprite s_circleSprite;

        private static Sprite GetOrCreateCircleSprite()
        {
            if (s_circleSprite != null) return s_circleSprite;

            const int size = 64;
            const float half = size * 0.5f;
            const float r = half - 1f;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - half + 0.5f;
                    float dy = y - half + 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    // soft edge over 2 pixels
                    float alpha = Mathf.Clamp01(1f - (dist - (r - 2f)) / 2f);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            s_circleSprite = Sprite.Create(
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
            s_circleSprite.hideFlags = HideFlags.HideAndDontSave;

            return s_circleSprite;
        }

        private void BuildPool()
        {
            _pool = new RippleEntry[_poolSize];

            Sprite circle = _sprite != null ? _sprite : GetOrCreateCircleSprite();

            for (int i = 0; i < _poolSize; i++)
            {
                GameObject go = new GameObject($"Ripple_{i}", typeof(RectTransform), typeof(Image))
                {
                    hideFlags = HideFlags.HideInHierarchy
                };

                go.transform.SetParent(transform, false);

                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = Vector2.zero;

                Image img = go.GetComponent<Image>();
                img.color = _rippleColor;
                img.raycastTarget = false;
                img.sprite = circle;

                go.SetActive(false);

                _pool[i] = new RippleEntry
                {
                    rectTransform = rt,
                    image = img,
                    active = false,
                    elapsed = 0f,
                    targetSize = 0f
                };
            }
        }

        // Animation tick

        private void StartTick()
        {
            if (_tickCoroutine != null) return;
            _tickCoroutine = StartCoroutine(Tick());
        }

        private void StopTick()
        {
            if (_tickCoroutine == null) return;
            StopCoroutine(_tickCoroutine);
            _tickCoroutine = null;
        }

        private IEnumerator Tick()
        {
            while (_activeCount > 0)
            {
                float dt = _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

                for (int i = 0; i < _pool.Length; i++)
                {
                    if (!_pool[i].active) continue;

                    _pool[i].elapsed += dt;
                    float t = Mathf.Clamp01(_pool[i].elapsed / _duration);
                    float eased = EasingFunctions.Evaluate(_expandEasing, t);

                    float size = _pool[i].targetSize * eased;
                    _pool[i].rectTransform.sizeDelta = new Vector2(size, size);

                    Color c = _rippleColor;
                    c.a = _rippleColor.a * (1f - t);
                    _pool[i].image.color = c;

                    if (_pool[i].elapsed >= _duration)
                    {
                        _pool[i].active = false;
                        _pool[i].image.gameObject.SetActive(false);
                        _activeCount--;
                    }
                }

                yield return null;
            }

            _tickCoroutine = null;
        }

        // Helpers

        private void RecalculateSize()
        {
            Rect r = GetComponent<RectTransform>().rect;
            float w = r.width;
            float h = r.height;
            _maxSize = Mathf.Sqrt(w * w + h * h) * 1.15f;
        }

        private int AcquireSlot()
        {
            for (int i = 0; i < _pool.Length; i++)
                if (!_pool[i].active) return i;

            // Pool full: recycle oldest
            int oldest = 0;
            float maxElapsed = -1f;
            for (int i = 0; i < _pool.Length; i++)
            {
                if (_pool[i].elapsed <= maxElapsed) continue;
                maxElapsed = _pool[i].elapsed;
                oldest = i;
            }
            _pool[oldest].active = false;
            _pool[oldest].image.gameObject.SetActive(false);
            _activeCount = Mathf.Max(0, _activeCount - 1);
            return oldest;
        }

        private void HideAll()
        {
            if (_pool == null) return;
            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i].active = false;
                if (_pool[i].image != null)
                    _pool[i].image.gameObject.SetActive(false);
            }
            _activeCount = 0;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Rebuild pool if pool size changed in editor
            if (_pool != null && _pool.Length != _poolSize)
            {
                HideAll();
                // Destroy old pool GameObjects
                for (int i = 0; i < _pool.Length; i++)
                {
                    if (_pool[i].image != null)
                        DestroyImmediate(_pool[i].image.gameObject);
                }
                BuildPool();
            }
        }
#endif
    }
}