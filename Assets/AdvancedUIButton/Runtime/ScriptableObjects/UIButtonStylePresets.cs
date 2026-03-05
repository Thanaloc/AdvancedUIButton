using UnityEngine;

namespace AdvancedUI
{
    /// <summary>
    /// Factory for the 5 built-in button style presets.
    /// Call any method to get a pre-configured UIButtonStyle instance.
    /// These are also available as .asset files via Tools → AdvancedUI → Generate Style Presets.
    /// </summary>
    public static class UIButtonStylePresets
    {
        // ── Primary ───────────────────────────────────────────────────────────
        // Strong filled button — main call to action.

        public static UIButtonStyle Primary()
        {
            var s = ScriptableObject.CreateInstance<UIButtonStyle>();
            s.name = "Style_Primary";

            s.background.colors.normal              = HexColor("2563EB");
            s.background.colors.highlighted         = HexColor("1D4ED8");
            s.background.colors.pressed             = HexColor("1E40AF");
            s.background.colors.selected            = HexColor("1E40AF");
            s.background.colors.selectedHighlighted = HexColor("1E3A8A");
            s.background.colors.focused             = HexColor("3B82F6");
            s.background.colors.disabled            = HexColor("93C5FD", 0.5f);

            s.label.colors.normal              = Color.white;
            s.label.colors.highlighted         = Color.white;
            s.label.colors.pressed             = new Color(1f, 1f, 1f, 0.9f);
            s.label.colors.selected            = Color.white;
            s.label.colors.selectedHighlighted = Color.white;
            s.label.colors.focused             = Color.white;
            s.label.colors.disabled            = new Color(1f, 1f, 1f, 0.5f);

            s.icon.colors.CopyFrom(s.label.colors);

            ApplyDefaultAnimation(s);
            return s;
        }

        // ── Secondary ─────────────────────────────────────────────────────────
        // Neutral filled button — secondary actions.

        public static UIButtonStyle Secondary()
        {
            var s = ScriptableObject.CreateInstance<UIButtonStyle>();
            s.name = "Style_Secondary";

            s.background.colors.normal              = HexColor("4B5563");
            s.background.colors.highlighted         = HexColor("374151");
            s.background.colors.pressed             = HexColor("1F2937");
            s.background.colors.selected            = HexColor("1F2937");
            s.background.colors.selectedHighlighted = HexColor("111827");
            s.background.colors.focused             = HexColor("6B7280");
            s.background.colors.disabled            = HexColor("9CA3AF", 0.5f);

            s.label.colors.normal              = Color.white;
            s.label.colors.highlighted         = Color.white;
            s.label.colors.pressed             = new Color(1f, 1f, 1f, 0.9f);
            s.label.colors.selected            = Color.white;
            s.label.colors.selectedHighlighted = Color.white;
            s.label.colors.focused             = Color.white;
            s.label.colors.disabled            = new Color(1f, 1f, 1f, 0.5f);

            s.icon.colors.CopyFrom(s.label.colors);

            ApplyDefaultAnimation(s);
            return s;
        }

        // ── Danger ────────────────────────────────────────────────────────────
        // Destructive actions — delete, confirm irreversible operation.

        public static UIButtonStyle Danger()
        {
            var s = ScriptableObject.CreateInstance<UIButtonStyle>();
            s.name = "Style_Danger";

            s.background.colors.normal              = HexColor("DC2626");
            s.background.colors.highlighted         = HexColor("B91C1C");
            s.background.colors.pressed             = HexColor("991B1B");
            s.background.colors.selected            = HexColor("991B1B");
            s.background.colors.selectedHighlighted = HexColor("7F1D1D");
            s.background.colors.focused             = HexColor("EF4444");
            s.background.colors.disabled            = HexColor("FCA5A5", 0.5f);

            s.label.colors.normal              = Color.white;
            s.label.colors.highlighted         = Color.white;
            s.label.colors.pressed             = new Color(1f, 1f, 1f, 0.9f);
            s.label.colors.selected            = Color.white;
            s.label.colors.selectedHighlighted = Color.white;
            s.label.colors.focused             = Color.white;
            s.label.colors.disabled            = new Color(1f, 1f, 1f, 0.5f);

            s.icon.colors.CopyFrom(s.label.colors);

            ApplyDefaultAnimation(s);
            return s;
        }

        // ── Outline ───────────────────────────────────────────────────────────
        // Transparent background with visible border — secondary/ghost variant.
        // Background alpha drives the fill on hover.

        public static UIButtonStyle Outline()
        {
            var s = ScriptableObject.CreateInstance<UIButtonStyle>();
            s.name = "Style_Outline";

            s.background.colors.normal              = new Color(0.15f, 0.39f, 0.92f, 0f);
            s.background.colors.highlighted         = new Color(0.15f, 0.39f, 0.92f, 0.12f);
            s.background.colors.pressed             = new Color(0.15f, 0.39f, 0.92f, 0.22f);
            s.background.colors.selected            = new Color(0.15f, 0.39f, 0.92f, 0.22f);
            s.background.colors.selectedHighlighted = new Color(0.15f, 0.39f, 0.92f, 0.30f);
            s.background.colors.focused             = new Color(0.15f, 0.39f, 0.92f, 0.08f);
            s.background.colors.disabled            = new Color(0.5f,  0.5f,  0.5f,  0f);

            s.label.colors.normal              = HexColor("2563EB");
            s.label.colors.highlighted         = HexColor("1D4ED8");
            s.label.colors.pressed             = HexColor("1E40AF");
            s.label.colors.selected            = HexColor("1E40AF");
            s.label.colors.selectedHighlighted = HexColor("1E3A8A");
            s.label.colors.focused             = HexColor("2563EB");
            s.label.colors.disabled            = HexColor("93C5FD", 0.5f);

            s.icon.colors.CopyFrom(s.label.colors);

            ApplyDefaultAnimation(s);
            return s;
        }

        // ── Ghost ─────────────────────────────────────────────────────────────
        // Fully transparent — minimal, text-only style.

        public static UIButtonStyle Ghost()
        {
            var s = ScriptableObject.CreateInstance<UIButtonStyle>();
            s.name = "Style_Ghost";

            s.background.colors.normal              = new Color(0f, 0f, 0f, 0f);
            s.background.colors.highlighted         = new Color(0f, 0f, 0f, 0.06f);
            s.background.colors.pressed             = new Color(0f, 0f, 0f, 0.12f);
            s.background.colors.selected            = new Color(0f, 0f, 0f, 0.12f);
            s.background.colors.selectedHighlighted = new Color(0f, 0f, 0f, 0.18f);
            s.background.colors.focused             = new Color(0f, 0f, 0f, 0.04f);
            s.background.colors.disabled            = new Color(0f, 0f, 0f, 0f);

            s.label.colors.normal              = HexColor("374151");
            s.label.colors.highlighted         = HexColor("111827");
            s.label.colors.pressed             = HexColor("030712");
            s.label.colors.selected            = HexColor("111827");
            s.label.colors.selectedHighlighted = HexColor("030712");
            s.label.colors.focused             = HexColor("374151");
            s.label.colors.disabled            = HexColor("9CA3AF", 0.5f);

            s.icon.colors.CopyFrom(s.label.colors);

            s.animation.enableScale    = false;
            s.animation.colorDuration  = 0.08f;
            s.animation.colorEasing    = EasingType.EaseOut;
            return s;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void ApplyDefaultAnimation(UIButtonStyle s)
        {
            s.animation.enableScale    = true;
            s.animation.colorDuration  = 0.10f;
            s.animation.colorEasing    = EasingType.EaseOut;
            s.animation.scaleDuration  = 0.12f;
            s.animation.scaleEasing    = EasingType.BackOut;
            s.animation.ignoreTimeScale = true;

            s.animation.transform.normalScale      = Vector3.one;
            s.animation.transform.highlightedScale = new Vector3(1.04f, 1.04f, 1f);
            s.animation.transform.pressedScale     = new Vector3(0.96f, 0.96f, 1f);
            s.animation.transform.selectedScale    = Vector3.one;
            s.animation.transform.focusedScale     = new Vector3(1.02f, 1.02f, 1f);
            s.animation.transform.disabledScale    = Vector3.one;
        }

        private static Color HexColor(string hex, float alpha = 1f)
        {
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color c))
            {
                c.a = alpha;
                return c;
            }
            return Color.white;
        }
    }
}
