// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdvancedUI.Demo;

namespace AdvancedUI.Editor
{
    public static class DemoSceneBuilder
    {
        // ── Layout constants ─────────────────────────────────────────────
        private const float CW = 1920f, CH = 1080f;
        private const float SecW = 1760f;
        private const float SecH = 190f, SecGap = 12f;
        private const float BW = 174f, BH = 54f, BG = 22f;
        private const float CardRadius = 16f;
        private const float ContentOffY = -16f;

        // ── Palette ──────────────────────────────────────────────────────
        private static Color Hex(string h, float a = 1f)
        {
            ColorUtility.TryParseHtmlString("#" + h, out Color c);
            c.a = a;
            return c;
        }

        private static readonly Color BgMain  = Hex("0B0F1A");
        private static readonly Color BgCard  = Hex("111827");
        private static readonly Color BgCard2 = Hex("0D1117");
        private static readonly Color CTitle  = Hex("E2E8F0");
        private static readonly Color CSub    = Hex("94A3B8");
        private static readonly Color CCap    = Hex("64748B");
        private static readonly Color CSep    = Hex("1E293B");

        // ── Entry point ──────────────────────────────────────────────────

        [MenuItem("Tools/AdvancedUI/Create Demo Scene", priority = 2)]
        public static void Build()
        {
            if (!File.Exists(Path.GetFullPath("Assets/AdvancedUIButton/Presets/Style_Primary.asset")))
                UIButtonStylePresetsGenerator.GenerateSilent();

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = BgMain;
            cam.orthographic = true;

            var canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<RectTransform>();
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(CW, CH);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            var bgGO = new GameObject("BG");
            var bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.SetParent(canvasGO.transform, false);
            bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
            bgGO.AddComponent<Image>().color = BgMain;

            var ctrlGO = new GameObject("DemoController");
            var ctrl = ctrlGO.AddComponent<DemoController>();

            float totalH = 5 * SecH + 4 * SecGap;
            float startY = totalH * 0.5f - SecH * 0.5f;

            Sec1_Styles(canvasGO.transform,      startY - 0 * (SecH + SecGap));
            Sec2_Interaction(canvasGO.transform,  startY - 1 * (SecH + SecGap), ctrl);
            Sec3_States(canvasGO.transform,       startY - 2 * (SecH + SecGap));
            Sec4_Ripple(canvasGO.transform,       startY - 3 * (SecH + SecGap));
            Sec5_Easing(canvasGO.transform,       startY - 4 * (SecH + SecGap));

            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            string dir = "Assets/AdvancedUIButton/Demo";
            if (!AssetDatabase.IsValidFolder(dir))
                AssetDatabase.CreateFolder("Assets/AdvancedUIButton", "Demo");
            string path = dir + "/AdvancedUIButton_Demo.unity";
            EditorSceneManager.SaveScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene(), path);
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(path);
        }

        // ── Section 1 : Styles ───────────────────────────────────────────
        static void Sec1_Styles(Transform root, float y)
        {
            var c = Section(root, "S1", "Styles", "5 built-in presets \u2014 or create your own", y, BgCard);
            string[] names = { "Primary", "Secondary", "Danger", "Outline", "Ghost" };
            float x0 = -(names.Length * BW + (names.Length - 1) * BG) * 0.5f + BW * 0.5f;
            for (int i = 0; i < names.Length; i++)
            {
                float x = x0 + i * (BW + BG);
                Btn(c, names[i], names[i], x, 6, BW, BH, Preset(names[i]));
                Lbl(c, names[i], x, -BH * 0.5f - 18, BW, 20, 11, CCap);
            }
        }

        // ── Section 2 : Interaction Modes ────────────────────────────────
        static void Sec2_Interaction(Transform root, float y, DemoController ctrl)
        {
            var c = Section(root, "S2", "Interaction Modes",
                "Standard \u00b7 Toggle \u00b7 Radio Group \u00b7 Hold", y, BgCard);
            var prim = Preset("Primary");
            float col = BW + BG + 56f, x0 = -col * 1.5f;

            // Standard
            var stdBtn = Btn(c, "Btn_Standard", "Click Me", x0, 6, BW, BH, prim);
            var cntLbl = Lbl(c, "Clicks: 0", x0, -BH * 0.5f - 18, BW + 20, 20, 11, CCap);
            Wire(ctrl, "clickLabel", cntLbl.GetComponent<TextMeshProUGUI>());
            WireClick(stdBtn, ctrl, "OnClick");
            Lbl(c, "Standard", x0, BH * 0.5f + 14, BW, 18, 10, CSub);

            // Toggle
            float tx = x0 + col;
            var togBtn = Btn(c, "Btn_Toggle", "Toggle", tx, 6, BW, BH, prim, InteractionMode.Toggle);
            var togLbl = Lbl(c, "OFF", tx, -BH * 0.5f - 18, 80, 20, 12, Hex("EF4444"));
            Wire(ctrl, "toggleLabel", togLbl.GetComponent<TextMeshProUGUI>());
            var togSO = new SerializedObject(togBtn.GetComponent<AdvancedUIButton>());
            WireVoid(togSO.FindProperty("_onSelected"), ctrl, "OnToggleSelected");
            WireVoid(togSO.FindProperty("_onDeselected"), ctrl, "OnToggleDeselected");
            togSO.ApplyModifiedProperties();
            Lbl(c, "Toggle", tx, BH * 0.5f + 14, BW, 18, 10, CSub);

            // Radio
            float rx = x0 + col * 2f;
            var grpGO = new GameObject("RadioGroup");
            grpGO.AddComponent<RectTransform>();
            Place(grpGO, c, rx, 6, BW * 3, BH);
            var tg = grpGO.AddComponent<UIButtonToggleGroup>();
            var tgSO = new SerializedObject(tg);
            tgSO.FindProperty("_buttons").arraySize = 0;
            foreach (var rl in new[] { "A", "B", "C" })
            {
                int idx = rl[0] - 'A';
                var rb = Btn(grpGO.transform, "Radio_" + rl, rl, (idx - 1) * 84f, 0, 78, BH, prim, InteractionMode.Radio);
                var rso = new SerializedObject(rb.GetComponent<AdvancedUIButton>());
                rso.FindProperty("_toggleGroup").objectReferenceValue = tg;
                rso.ApplyModifiedProperties();
                int sz = tgSO.FindProperty("_buttons").arraySize;
                tgSO.FindProperty("_buttons").arraySize = sz + 1;
                tgSO.FindProperty("_buttons").GetArrayElementAtIndex(sz).objectReferenceValue =
                    rb.GetComponent<AdvancedUIButton>();
            }
            tgSO.ApplyModifiedProperties();
            Lbl(c, "Radio Group", rx, BH * 0.5f + 14, BW + 40, 18, 10, CSub);

            // Hold
            float hx = x0 + col * 3f;
            var holdBtn = Btn(c, "Btn_Hold", "Hold (1s)", hx, 6, BW, BH, Preset("Danger"), InteractionMode.Hold);
            var pbBg = RoundedBar(c, hx, -BH * 0.5f - 18, BW, 6, Hex("1E293B"));
            var fillRT = RoundedFill(pbBg.transform, Hex("EF4444"));
            Wire(ctrl, "holdButton", holdBtn.GetComponent<AdvancedUIButton>());
            Wire(ctrl, "holdFill", fillRT);
            WireFloat(ctrl, "holdFillWidth", BW);
            Lbl(c, "Hold (1s)", hx, BH * 0.5f + 14, BW, 18, 10, CSub);
        }

        // ── Section 3 : States ───────────────────────────────────────────
        static void Sec3_States(Transform root, float y)
        {
            var c = Section(root, "S3", "All States",
                "Hover the Normal button to see live transitions", y, BgCard);
            var prim = Preset("Primary");
            string[] states = { "Normal", "Hover", "Pressed", "Selected", "Focused", "Disabled" };
            float x0 = -(states.Length * BW + (states.Length - 1) * BG) * 0.5f + BW * 0.5f;

            for (int i = 0; i < states.Length; i++)
            {
                float x = x0 + i * (BW + BG);
                var btn = Btn(c, "State_" + states[i], states[i], x, 6, BW, BH, prim);
                var adv = btn.GetComponent<AdvancedUIButton>();
                var bgImg = btn.transform.Find("Background").GetComponent<Image>();
                var lblTmp = btn.transform.Find("Label").GetComponent<TextMeshProUGUI>();

                switch (states[i])
                {
                    case "Normal": break;
                    case "Disabled": adv.SetInteractable(false); break;
                    case "Selected": adv.SetSelected(true); break;
                    default:
                        adv.enabled = false;
                        bgImg.color = StateColor(prim.background.colors, states[i]);
                        lblTmp.color = StateColor(prim.label.colors, states[i]);
                        break;
                }
                Lbl(c, states[i], x, -BH * 0.5f - 18, BW, 20, 11, CCap);
            }
        }

        // ── Section 4 : Ripple ───────────────────────────────────────────
        static void Sec4_Ripple(Transform root, float y)
        {
            var c = Section(root, "S4", "Ripple Effect",
                "Click to see ripple \u2014 customizable color, duration, easing", y, BgCard2);
            Color[] cols = { new(1, 1, 1, 0.30f), new(0.4f, 0.7f, 1, 0.35f), new(0, 0.9f, 1, 0.30f) };
            string[] caps = { "White", "Blue", "Cyan" };
            float x0 = -(3 * BW + 2 * BG) * 0.5f + BW * 0.5f;

            for (int i = 0; i < 3; i++)
            {
                float x = x0 + i * (BW + BG);
                var btn = Btn(c, "RippleBtn_" + i, "Click Me", x, 6, BW, BH, Preset("Primary"));
                btn.AddComponent<RectMask2D>();

                var rGO = new GameObject("Ripple");
                var rRT = rGO.AddComponent<RectTransform>();
                rRT.SetParent(btn.transform, false);
                rRT.SetSiblingIndex(1);
                Stretch(rRT);
                var rip = rGO.AddComponent<UIButtonRipple>();
                var rso = new SerializedObject(rip);
                rso.FindProperty("_rippleColor").colorValue = cols[i];
                rso.FindProperty("_syncWithStyle").boolValue = false;
                rso.ApplyModifiedProperties();
                var bso = new SerializedObject(btn.GetComponent<AdvancedUIButton>());
                bso.FindProperty("_ripple").objectReferenceValue = rip;
                bso.ApplyModifiedProperties();
                Lbl(c, caps[i] + " Ripple", x, -BH * 0.5f - 18, BW + 20, 20, 11, CCap);
            }
        }

        // ── Section 5 : Easing ───────────────────────────────────────────
        static void Sec5_Easing(Transform root, float y)
        {
            var c = Section(root, "S5", "Easing Showcase",
                "Hover each button to feel the difference", y, BgCard);
            EasingType[] easings = { EasingType.BackOut, EasingType.ElasticOut, EasingType.EaseOut, EasingType.Linear };
            string[] labels = { "BackOut", "ElasticOut", "EaseOut", "Linear" };
            float x0 = -(easings.Length * BW + (easings.Length - 1) * BG) * 0.5f + BW * 0.5f;

            for (int i = 0; i < easings.Length; i++)
            {
                float x = x0 + i * (BW + BG);
                var s = Object.Instantiate(Preset("Primary"));
                s.animation.scaleEasing = easings[i];
                s.animation.scaleDuration = 0.22f;
                s.animation.enableScale = true;
                s.animation.transform.highlightedScale = new Vector3(1.12f, 1.12f, 1);
                s.animation.transform.pressedScale = new Vector3(0.88f, 0.88f, 1);
                Btn(c, "Ease_" + labels[i], labels[i], x, 6, BW, BH, s);
                Lbl(c, labels[i], x, -BH * 0.5f - 18, BW, 20, 11, CCap);
            }
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // Primitives
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        static Transform Section(Transform root, string id, string title, string subtitle,
            float y, Color bg)
        {
            var go = new GameObject(id);
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(root, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, y);
            rt.sizeDelta = new Vector2(SecW, SecH);

            var cardImg = go.AddComponent<Image>();
            cardImg.color = bg;
            cardImg.sprite = RoundedRectSprite.Create(128, 128, (int)CardRadius, 2f);
            cardImg.type = Image.Type.Sliced;

            // Title
            MakeText(go.transform, "Title", title, 16, CTitle, FontStyles.Bold,
                new Vector2(0, -10), new Vector2(-48, 24), TextAlignmentOptions.Left,
                anchorTop: true);

            // Subtitle
            MakeText(go.transform, "Subtitle", subtitle, 11, CSub, FontStyles.Italic,
                new Vector2(0, -32), new Vector2(-48, 18), TextAlignmentOptions.Left,
                anchorTop: true);

            // Separator
            var sGO = new GameObject("Sep");
            var sRT = sGO.AddComponent<RectTransform>();
            sRT.SetParent(go.transform, false);
            sRT.anchorMin = new Vector2(0, 1); sRT.anchorMax = new Vector2(1, 1);
            sRT.pivot = new Vector2(0.5f, 1);
            sRT.anchoredPosition = new Vector2(0, -52);
            sRT.sizeDelta = new Vector2(-40, 1);
            sGO.AddComponent<Image>().color = CSep;

            // Content
            var cGO = new GameObject("Content");
            var cRT = cGO.AddComponent<RectTransform>();
            cRT.SetParent(go.transform, false);
            cRT.anchorMin = cRT.anchorMax = new Vector2(0.5f, 0.5f);
            cRT.pivot = new Vector2(0.5f, 0.5f);
            cRT.anchoredPosition = new Vector2(0, ContentOffY);
            cRT.sizeDelta = new Vector2(SecW - 60, SecH - 70);

            return cGO.transform;
        }

        static GameObject Btn(Transform parent, string name, string lbl,
            float x, float y, float w, float h, UIButtonStyle style,
            InteractionMode mode = InteractionMode.Standard)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            Place(go, parent, x, y, w, h);

            // Rounded background
            var bgGO = new GameObject("Background");
            var bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.SetParent(go.transform, false);
            Stretch(bgRT);
            var bgImg = bgGO.AddComponent<Image>();
            bgImg.color = Color.white;
            bgImg.sprite = RoundedRectSprite.Default;
            bgImg.type = Image.Type.Sliced;

            // Label
            var lGO = new GameObject("Label");
            var lRT = lGO.AddComponent<RectTransform>();
            lRT.SetParent(go.transform, false);
            Stretch(lRT);
            var tmp = lGO.AddComponent<TextMeshProUGUI>();
            tmp.text = lbl;
            tmp.fontSize = 14;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;

            var adv = go.AddComponent<AdvancedUIButton>();
            var so = new SerializedObject(adv);
            so.FindProperty("_mode").enumValueIndex = (int)mode;
            var entries = so.FindProperty("_animator").FindPropertyRelative("_graphicEntries");
            entries.arraySize = 2;
            entries.GetArrayElementAtIndex(0).FindPropertyRelative("_target").objectReferenceValue = bgImg;
            entries.GetArrayElementAtIndex(0).FindPropertyRelative("_role").enumValueIndex = (int)GraphicRole.Background;
            entries.GetArrayElementAtIndex(1).FindPropertyRelative("_target").objectReferenceValue = tmp;
            entries.GetArrayElementAtIndex(1).FindPropertyRelative("_role").enumValueIndex = (int)GraphicRole.Label;
            so.ApplyModifiedProperties();
            if (style != null) adv.ApplyStyle(style);
            return go;
        }

        static GameObject Lbl(Transform parent, string text,
            float x, float y, float w, float h, float size, Color col)
        {
            var go = new GameObject("Lbl_" + text.Replace(" ", "_"));
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.color = col;
            tmp.alignment = TextAlignmentOptions.Center; tmp.raycastTarget = false;
            return go;
        }

        static void MakeText(Transform parent, string name, string text,
            float size, Color color, FontStyles fontStyle,
            Vector2 pos, Vector2 sizeDelta, TextAlignmentOptions align,
            bool anchorTop = false)
        {
            var go = new GameObject(name);
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            if (anchorTop)
            {
                rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 1);
            }
            else
            {
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
            }
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.color = color;
            tmp.fontStyle = fontStyle; tmp.alignment = align;
            tmp.raycastTarget = false;
        }

        static GameObject RoundedBar(Transform parent, float x, float y, float w, float h, Color col)
        {
            var go = new GameObject("ProgressBg");
            go.AddComponent<RectTransform>();
            Place(go, parent, x, y, w, h);
            var img = go.AddComponent<Image>();
            img.color = col;
            img.sprite = RoundedRectSprite.Create(64, 8, 4, 1f);
            img.type = Image.Type.Sliced;
            return go;
        }

        static RectTransform RoundedFill(Transform parent, Color col)
        {
            var go = new GameObject("Fill");
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 0.5f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = col;
            img.sprite = RoundedRectSprite.Create(64, 8, 4, 1f);
            img.type = Image.Type.Sliced;
            return rt;
        }

        static void Place(GameObject go, Transform parent, float x, float y, float w, float h)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        static Color StateColor(StateColors colors, string state) => state switch
        {
            "Hover"   => colors.highlighted,
            "Pressed" => colors.pressed,
            "Focused" => colors.focused,
            _         => colors.normal
        };

        // ── Wiring helpers ───────────────────────────────────────────────

        static void Wire(DemoController ctrl, string field, Object value)
        {
            var so = new SerializedObject(ctrl);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedProperties();
        }

        static void WireFloat(DemoController ctrl, string field, float value)
        {
            var so = new SerializedObject(ctrl);
            so.FindProperty(field).floatValue = value;
            so.ApplyModifiedProperties();
        }

        static void WireClick(GameObject btnGO, Object target, string method)
        {
            var so = new SerializedObject(btnGO.GetComponent<AdvancedUIButton>());
            WireVoid(so.FindProperty("m_OnClick"), target, method);
            so.ApplyModifiedProperties();
        }

        static void WireVoid(SerializedProperty evt, Object target, string method)
        {
            var calls = evt.FindPropertyRelative("m_PersistentCalls.m_Calls");
            int i = calls.arraySize++;
            var call = calls.GetArrayElementAtIndex(i);
            call.FindPropertyRelative("m_Target").objectReferenceValue = target;
            call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue =
                target.GetType().AssemblyQualifiedName;
            call.FindPropertyRelative("m_MethodName").stringValue = method;
            call.FindPropertyRelative("m_Mode").enumValueIndex = 1;
            call.FindPropertyRelative("m_CallState").enumValueIndex = 2;
            call.FindPropertyRelative("m_Arguments.m_ObjectArgumentAssemblyTypeName").stringValue =
                "UnityEngine.Object, UnityEngine";
        }

        static UIButtonStyle Preset(string name)
        {
            var s = AssetDatabase.LoadAssetAtPath<UIButtonStyle>(
                $"Assets/AdvancedUIButton/Presets/Style_{name}.asset");
            if (s == null) Debug.LogWarning("[DemoSceneBuilder] Missing preset: " + name);
            return s;
        }
    }
}
#endif
