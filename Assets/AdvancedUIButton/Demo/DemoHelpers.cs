// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using UnityEngine;
using TMPro;

namespace AdvancedUI.Demo
{
    [AddComponentMenu("AdvancedUI/Demo/Demo Controller")]
    public sealed class DemoController : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI clickLabel;
        [SerializeField] public TextMeshProUGUI toggleLabel;
        [SerializeField] public RectTransform holdFill;
        [SerializeField] public float holdFillWidth = 160f;

        private static readonly Color OnColor = new Color(0.29f, 0.84f, 0.44f);
        private static readonly Color OffColor = new Color(0.93f, 0.27f, 0.27f);
        private int _clicks;

        private void Start()
        {
            if (toggleLabel != null) { toggleLabel.text = "OFF"; toggleLabel.color = OffColor; }
        }

        public void OnClick()
        {
            _clicks++;
            if (clickLabel != null) clickLabel.text = "Clicks: " + _clicks;
        }

        public void OnToggleSelected()
        {
            if (toggleLabel == null) return;
            toggleLabel.text = "ON";
            toggleLabel.color = OnColor;
        }

        public void OnToggleDeselected()
        {
            if (toggleLabel == null) return;
            toggleLabel.text = "OFF";
            toggleLabel.color = OffColor;
        }

        public void OnHoldProgress(float t)
        {
            if (holdFill != null)
                holdFill.offsetMax = new Vector2(holdFillWidth * t, 0f);
        }

        public void OnHoldComplete()
        {
            if (holdFill != null)
                holdFill.offsetMax = Vector2.zero;
        }
    }
}