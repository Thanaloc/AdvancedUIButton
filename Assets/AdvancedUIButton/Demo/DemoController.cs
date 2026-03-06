// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using UnityEngine;
using TMPro;

namespace AdvancedUI.Demo
{
    /// <summary>
    /// Central controller for the demo scene.
    /// Holds serialized references to UI labels and the hold button.
    /// The DemoSceneBuilder wires all fields via SerializedObject at build time.
    /// Hold events are subscribed at runtime via <see cref="AdvancedUIButton.HoldConfig"/>.
    /// </summary>
    [AddComponentMenu("AdvancedUI/Demo/Demo Controller")]
    public sealed class DemoController : MonoBehaviour
    {
        [Header("Standard")]
        [Tooltip("Label that displays the running click count.")]
        [SerializeField] private TextMeshProUGUI clickLabel;

        [Header("Toggle")]
        [Tooltip("Label that shows ON/OFF state of the toggle button.")]
        [SerializeField] private TextMeshProUGUI toggleLabel;

        [Header("Hold")]
        [Tooltip("The Hold button whose HoldConfig events are subscribed to at runtime.")]
        [SerializeField] private AdvancedUIButton holdButton;

        [Tooltip("RectTransform of the progress bar fill image.")]
        [SerializeField] private RectTransform holdFill;

        [Tooltip("Full width of the progress bar in pixels, used to compute fill offsetMax.")]
        [SerializeField] private float holdFillWidth = 160f;

        private static readonly Color OnColor = new Color(0.29f, 0.84f, 0.44f);
        private static readonly Color OffColor = new Color(0.93f, 0.27f, 0.27f);
        private int _clicks;

        private void Start()
        {
            if (toggleLabel != null)
            {
                toggleLabel.text = "OFF";
                toggleLabel.color = OffColor;
            }

            // Subscribe to hold events at runtime via the public HoldConfig property.
            // This avoids UnityEvent<float> cross-assembly serialization issues.
            if (holdButton != null)
            {
                holdButton.HoldConfig.OnHoldProgress.AddListener(OnHoldProgress);
                holdButton.HoldConfig.OnHoldComplete.AddListener(OnHoldComplete);
            }
        }

        /// <summary>Called by the Standard button onClick (wired via SerializedObject).</summary>
        public void OnClick()
        {
            _clicks++;
            if (clickLabel != null) clickLabel.text = "Clicks: " + _clicks;
        }

        /// <summary>Called by the Toggle button _onSelected (wired via SerializedObject).</summary>
        public void OnToggleSelected()
        {
            if (toggleLabel == null) return;
            toggleLabel.text = "ON";
            toggleLabel.color = OnColor;
        }

        /// <summary>Called by the Toggle button _onDeselected (wired via SerializedObject).</summary>
        public void OnToggleDeselected()
        {
            if (toggleLabel == null) return;
            toggleLabel.text = "OFF";
            toggleLabel.color = OffColor;
        }

        private void OnHoldProgress(float t)
        {
            if (holdFill != null)
                holdFill.offsetMax = new Vector2(holdFillWidth * t, 0f);
        }

        private void OnHoldComplete()
        {
            if (holdFill != null)
                holdFill.offsetMax = Vector2.zero;
        }
    }
}