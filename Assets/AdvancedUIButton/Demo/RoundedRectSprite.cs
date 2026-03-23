// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using UnityEngine;

namespace AdvancedUI
{
    /// <summary>
    /// Generates rounded rectangle sprites at runtime.
    /// Zero external dependencies — the texture is created procedurally.
    /// Used by the demo scene and available for user code.
    /// </summary>
    public static class RoundedRectSprite
    {
        private static Sprite s_default;
        private static Sprite s_pill;

        /// <summary>
        /// Returns a cached 128x128 rounded rect sprite with 24px corner radius.
        /// Suitable for 9-slice (Image.Type.Sliced) on any button size.
        /// </summary>
        public static Sprite Default
        {
            get
            {
                if (s_default == null)
                    s_default = Create(128, 128, 24, 2f);
                return s_default;
            }
        }

        /// <summary>
        /// Returns a cached 128x64 pill-shaped sprite (radius = half height).
        /// Great for small rounded buttons and tags.
        /// </summary>
        public static Sprite Pill
        {
            get
            {
                if (s_pill == null)
                    s_pill = Create(128, 64, 32, 2f);
                return s_pill;
            }
        }

        /// <summary>
        /// Creates a rounded rectangle sprite with the given dimensions and corner radius.
        /// The sprite is configured for 9-slice via its border property.
        /// </summary>
        /// <param name="width">Texture width in pixels.</param>
        /// <param name="height">Texture height in pixels.</param>
        /// <param name="radius">Corner radius in pixels.</param>
        /// <param name="softEdge">Edge softness in pixels for anti-aliasing.</param>
        /// <returns>A new Sprite ready for use with Image.Type.Sliced.</returns>
        public static Sprite Create(int width, int height, int radius, float softEdge = 1.5f)
        {
            radius = Mathf.Clamp(radius, 0, Mathf.Min(width, height) / 2);

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                hideFlags  = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode   = TextureWrapMode.Clamp
            };

            Color[] pixels = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float alpha = ComputeAlpha(x, y, width, height, radius, softEdge);
                    pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            // Border for 9-slice: radius + 1 on each side
            float border = radius + 1f;
            Vector4 borders = new Vector4(border, border, border, border);

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                borders
            );
            sprite.hideFlags = HideFlags.HideAndDontSave;

            return sprite;
        }

        private static float ComputeAlpha(int px, int py, int w, int h, int r, float soft)
        {
            // Determine the closest corner center
            float cx, cy;

            if (px < r)
                cx = r;
            else if (px >= w - r)
                cx = w - r - 1;
            else
                return 1f; // Not in a corner column -> fully inside

            if (py < r)
                cy = r;
            else if (py >= h - r)
                cy = h - r - 1;
            else
                return 1f; // Not in a corner row -> fully inside

            // Distance from corner center
            float dx = px - cx;
            float dy = py - cy;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);

            // Soft edge anti-aliasing
            if (dist <= r - soft)
                return 1f;
            if (dist >= r)
                return 0f;

            return 1f - (dist - (r - soft)) / soft;
        }
    }
}
