// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace AdvancedUI.Demo
{
    /// <summary>
    /// Central controller for the demo scene.
    /// Holds references to all interactive labels and wires hold events at runtime.
    /// </summary>
    [AddComponentMenu("AdvancedUI/Demo/Demo Controller")]
    public sealed class DemoController : MonoBehaviour
    {
        [Header("Standard")]
        public TextMeshProUGUI clickLabel;

        [Header("Toggle")]
        public TextMeshProUGUI toggleLabel;

        [Header("Hold")]
        public AdvancedUIButton holdButton;
        public RectTransform holdFill;
        public float holdFillWidth = 160f;

        private static readonly Color OnColor = new Color(0.29f, 0.84f, 0.44f);
        private static readonly Color OffColor = new Color(0.93f, 0.27f, 0.27f);
        private int _clicks;

        private void Start()
        {
            // Toggle label initial state
            if (toggleLabel != null)
            {
                toggleLabel.text = "OFF";
                toggleLabel.color = OffColor;
            }

            // Wire hold events at runtime via the public HoldConfig property
            if (holdButton != null)
            {
                holdButton.HoldConfig.OnHoldProgress.AddListener(OnHoldProgress);
                holdButton.HoldConfig.OnHoldComplete.AddListener(OnHoldComplete);
            }
        }

        // Called by Standard button onClick (wired via SerializedObject)
        public void OnClick()
        {
            _clicks++;
            if (clickLabel != null) clickLabel.text = "Clicks: " + _clicks;
        }

        // Called by Toggle button _onSelected (wired via SerializedObject)
        public void OnToggleSelected()
        {
            if (toggleLabel == null) return;
            toggleLabel.text = "ON";
            toggleLabel.color = OnColor;
        }

        // Called by Toggle button _onDeselected (wired via SerializedObject)
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
