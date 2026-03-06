// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using UnityEngine;

namespace AdvancedUI
{
    /// <summary>
    /// Factory for the 5 built-in button style presets.
    /// Use via code or generate .asset files via Tools -> AdvancedUI -> Generate Style Presets.
    /// </summary>
    public static class UIButtonStylePresets
    {
        // --- Primary --------------------------------------------------
        // Main call-to-action. Indigo filled, clear state progression.

        public static UIButtonStyle Primary()
        {
            var s = Create("Style_Primary");

            // Background: normal indigo -> lighter hover -> deep press
            s.background.colors.normal = Hex("4F46E5");
            s.background.colors.highlighted = Hex("818CF8"); // lighter, clearly hover
            s.background.colors.pressed = Hex("312E81"); // deep navy, clearly pressed
            s.background.colors.selected = Hex("7C3AED"); // violet = toggled on
            s.background.colors.selectedHighlighted = Hex("A78BFA");
            s.background.colors.focused = Hex("38BDF8"); // cyan = keyboard focus
            s.background.colors.disabled = Hex("334155", 0.7f);

            // Label: white base, dark on light backgrounds
            s.label.colors.normal = Color.white;
            s.label.colors.highlighted = Hex("1E1B4B"); // dark on light hover
            s.label.colors.pressed = Color.white;
            s.label.colors.selected = Color.white;
            s.label.colors.selectedHighlighted = Hex("1E1B4B");
            s.label.colors.focused = Hex("0C4A6E"); // dark on cyan
            s.label.colors.disabled = Hex("64748B");

            s.icon.colors.CopyFrom(s.label.colors);
            Anim(s);
            return s;
        }

        // --- Secondary ------------------------------------------------
        // Neutral secondary action. Slate filled.

        public static UIButtonStyle Secondary()
        {
            var s = Create("Style_Secondary");

            s.background.colors.normal = Hex("334155");
            s.background.colors.highlighted = Hex("475569");
            s.background.colors.pressed = Hex("1E293B");
            s.background.colors.selected = Hex("0F172A");
            s.background.colors.selectedHighlighted = Hex("1E293B");
            s.background.colors.focused = Hex("64748B");
            s.background.colors.disabled = Hex("1E293B", 0.7f);

            s.label.colors.normal = Hex("E2E8F0");
            s.label.colors.highlighted = Color.white;
            s.label.colors.pressed = Hex("94A3B8");
            s.label.colors.selected = Hex("94A3B8");
            s.label.colors.selectedHighlighted = Hex("64748B");
            s.label.colors.focused = Color.white;
            s.label.colors.disabled = Hex("475569");

            s.icon.colors.CopyFrom(s.label.colors);
            Anim(s);
            return s;
        }

        // --- Danger ---------------------------------------------------
        // Destructive actions.

        public static UIButtonStyle Danger()
        {
            var s = Create("Style_Danger");

            s.background.colors.normal = Hex("E11D48");
            s.background.colors.highlighted = Hex("FB7185"); // lighter pink hover
            s.background.colors.pressed = Hex("9F1239"); // deep crimson
            s.background.colors.selected = Hex("9F1239");
            s.background.colors.selectedHighlighted = Hex("881337");
            s.background.colors.focused = Hex("FDA4AF");
            s.background.colors.disabled = Hex("334155", 0.7f);

            s.label.colors.normal = Color.white;
            s.label.colors.highlighted = Hex("4C0519"); // dark on light pink
            s.label.colors.pressed = Color.white;
            s.label.colors.selected = Color.white;
            s.label.colors.selectedHighlighted = Color.white;
            s.label.colors.focused = Hex("4C0519");
            s.label.colors.disabled = Hex("64748B");

            s.icon.colors.CopyFrom(s.label.colors);
            Anim(s);
            return s;
        }

        // --- Outline --------------------------------------------------
        // Transparent with a tinted background on hover.
        // Uses a visible low-alpha base so it reads on dark backgrounds.

        public static UIButtonStyle Outline()
        {
            var s = Create("Style_Outline");

            // Subtle always-visible tint so button is readable on dark bg
            s.background.colors.normal = new Color(0.31f, 0.27f, 0.90f, 0.15f);
            s.background.colors.highlighted = new Color(0.51f, 0.55f, 0.97f, 0.30f);
            s.background.colors.pressed = new Color(0.19f, 0.18f, 0.51f, 0.45f);
            s.background.colors.selected = new Color(0.49f, 0.23f, 0.93f, 0.35f);
            s.background.colors.selectedHighlighted = new Color(0.49f, 0.23f, 0.93f, 0.50f);
            s.background.colors.focused = new Color(0.22f, 0.74f, 0.98f, 0.20f);
            s.background.colors.disabled = new Color(0.2f, 0.2f, 0.2f, 0.10f);

            s.label.colors.normal = Hex("818CF8");
            s.label.colors.highlighted = Hex("C7D2FE");
            s.label.colors.pressed = Hex("4F46E5");
            s.label.colors.selected = Hex("A78BFA");
            s.label.colors.selectedHighlighted = Hex("C4B5FD");
            s.label.colors.focused = Hex("38BDF8");
            s.label.colors.disabled = Hex("475569");

            s.icon.colors.CopyFrom(s.label.colors);
            Anim(s);
            return s;
        }

        // --- Ghost ----------------------------------------------------
        // Minimal text-only. Readable on dark backgrounds.

        public static UIButtonStyle Ghost()
        {
            var s = Create("Style_Ghost");

            s.background.colors.normal = new Color(1f, 1f, 1f, 0.00f);
            s.background.colors.highlighted = new Color(1f, 1f, 1f, 0.08f);
            s.background.colors.pressed = new Color(1f, 1f, 1f, 0.16f);
            s.background.colors.selected = new Color(1f, 1f, 1f, 0.12f);
            s.background.colors.selectedHighlighted = new Color(1f, 1f, 1f, 0.20f);
            s.background.colors.focused = new Color(1f, 1f, 1f, 0.06f);
            s.background.colors.disabled = new Color(1f, 1f, 1f, 0.00f);

            // Bright labels so text is readable on dark bg
            s.label.colors.normal = Hex("CBD5E1");
            s.label.colors.highlighted = Color.white;
            s.label.colors.pressed = Hex("94A3B8");
            s.label.colors.selected = Color.white;
            s.label.colors.selectedHighlighted = Hex("E2E8F0");
            s.label.colors.focused = Hex("BAE6FD");
            s.label.colors.disabled = Hex("475569");

            s.icon.colors.CopyFrom(s.label.colors);

            // Faster, no scale
            s.animation.enableScale = false;
            s.animation.colorDuration = 0.08f;
            s.animation.colorEasing = EasingType.EaseOut;
            return s;
        }

        // --- Helpers --------------------------------------------------

        private static UIButtonStyle Create(string name)
        {
            var s = ScriptableObject.CreateInstance<UIButtonStyle>();
            s.name = name;
            return s;
        }

        private static void Anim(UIButtonStyle s)
        {
            s.animation.enableScale = true;
            s.animation.colorDuration = 0.12f;
            s.animation.colorEasing = EasingType.EaseOut;
            s.animation.scaleDuration = 0.14f;
            s.animation.scaleEasing = EasingType.BackOut;
            s.animation.ignoreTimeScale = true;

            s.animation.transform.normalScale = Vector3.one;
            s.animation.transform.highlightedScale = new Vector3(1.05f, 1.05f, 1f);
            s.animation.transform.pressedScale = new Vector3(0.95f, 0.95f, 1f);
            s.animation.transform.selectedScale = Vector3.one;
            s.animation.transform.focusedScale = new Vector3(1.03f, 1.03f, 1f);
            s.animation.transform.disabledScale = Vector3.one;
        }

        private static Color Hex(string hex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out Color c);
            c.a = alpha;
            return c;
        }
    }
}