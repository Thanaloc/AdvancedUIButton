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
        private const float CW = 1920f, CH = 1080f;
        private const float SecH = 178f, SecGap = 14f;
        private const float BW = 160f, BH = 50f, BG = 18f;

        private static Color Hex(string h) { ColorUtility.TryParseHtmlString("#" + h, out Color c); return c; }
        private static readonly Color BgMain = new Color(0.102f, 0.102f, 0.180f);
        private static readonly Color BgSec = new Color(0.086f, 0.129f, 0.243f);
        private static readonly Color BgDark = new Color(0.051f, 0.051f, 0.102f);
        private static readonly Color CTitle = new Color(0.886f, 0.910f, 0.941f);
        private static readonly Color CCap = new Color(0.580f, 0.635f, 0.722f);
        private static readonly Color CSep = new Color(0.200f, 0.255f, 0.333f);

        [MenuItem("Tools/AdvancedUI/Create Demo Scene", priority = 2)]
        public static void Build()
        {
            if (!File.Exists(Path.GetFullPath("Assets/AdvancedUIButton/Presets/Style_Primary.asset")))
                UIButtonStylePresetsGenerator.GenerateSilent();

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = BgMain;
            cam.orthographic = true;

            // Canvas
            var canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<RectTransform>();
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(CW, CH);
            scaler.matchWidthOrHeight = 0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Background panel
            var bgGO = new GameObject("BG");
            var bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.SetParent(canvasGO.transform, false);
            bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
            bgGO.AddComponent<Image>().color = BgMain;

            // DemoController -- on its own root GO, NOT on any button
            var ctrlGO = new GameObject("DemoController");
            var ctrl = ctrlGO.AddComponent<DemoController>();

            float totalH = 5 * SecH + 4 * SecGap;
            float startY = totalH * 0.5f - SecH * 0.5f;
            float secW = CW - 80f;

            Sec1_Styles(canvasGO.transform, startY - 0 * (SecH + SecGap), secW);
            Sec2_Interaction(canvasGO.transform, startY - 1 * (SecH + SecGap), secW, ctrl);
            Sec3_States(canvasGO.transform, startY - 2 * (SecH + SecGap), secW);
            Sec4_Ripple(canvasGO.transform, startY - 3 * (SecH + SecGap), secW);
            Sec5_Easing(canvasGO.transform, startY - 4 * (SecH + SecGap), secW);

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

        // ---- Section 1 : Styles ----
        static void Sec1_Styles(Transform root, float y, float w)
        {
            var c = Section(root, "S1", "Styles", y, w, BgSec);
            string[] names = { "Primary", "Secondary", "Danger", "Outline", "Ghost" };
            float x0 = -(names.Length * BW + (names.Length - 1) * BG) * 0.5f + BW * 0.5f;
            for (int i = 0; i < names.Length; i++)
            {
                float x = x0 + i * (BW + BG);
                Btn(c, names[i], names[i], x, 8, BW, BH, Preset(names[i]));
                Lbl(c, names[i], x, -BH * 0.5f - 16, BW, 22, 12, CCap);
            }
        }

        // ---- Section 2 : Interaction ----
        static void Sec2_Interaction(Transform root, float y, float w, DemoController ctrl)
        {
            var c = Section(root, "S2", "Interaction Modes", y, w, BgSec);
            var prim = Preset("Primary");
            float col = BW + BG + 60f, x0 = -col * 1.5f;

            // Standard
            var stdBtn = Btn(c, "Btn_Standard", "Click Me", x0, 8, BW, BH, prim);
            var cntLbl = Lbl(c, "Clicks: 0", x0, -BH * 0.5f - 16, BW + 20, 22, 12, CCap);
            var cso = new SerializedObject(ctrl);
            cso.FindProperty("clickLabel").objectReferenceValue = cntLbl.GetComponent<TextMeshProUGUI>();
            cso.ApplyModifiedProperties();
            var sso = new SerializedObject(stdBtn.GetComponent<AdvancedUIButton>());
            WireVoid(sso.FindProperty("m_OnClick"), ctrl, "OnClick");
            sso.ApplyModifiedProperties();
            Lbl(c, "Standard", x0, BH * 0.5f + 16, BW, 20, 11, CCap);

            // Toggle
            float tx = x0 + col;
            var togBtn = Btn(c, "Btn_Toggle", "Toggle", tx, 8, BW, BH, prim, InteractionMode.Toggle);
            var togLbl = Lbl(c, "OFF", tx, -BH * 0.5f - 16, 80, 22, 13, Hex("EF4444"));
            var tso = new SerializedObject(ctrl);
            tso.FindProperty("toggleLabel").objectReferenceValue = togLbl.GetComponent<TextMeshProUGUI>();
            tso.ApplyModifiedProperties();
            var togSO = new SerializedObject(togBtn.GetComponent<AdvancedUIButton>());
            WireVoid(togSO.FindProperty("_onSelected"), ctrl, "OnToggleSelected");
            WireVoid(togSO.FindProperty("_onDeselected"), ctrl, "OnToggleDeselected");
            togSO.ApplyModifiedProperties();
            Lbl(c, "Toggle", tx, BH * 0.5f + 16, BW, 20, 11, CCap);

            // Radio group
            float rx = x0 + col * 2f;
            var grpGO = new GameObject("RadioGroup");
            grpGO.AddComponent<RectTransform>();
            Place(grpGO, c, rx, 8, BW * 3, BH);
            var tg = grpGO.AddComponent<UIButtonToggleGroup>();
            var tgSO = new SerializedObject(tg);
            tgSO.FindProperty("_buttons").arraySize = 0;
            string[] rl = { "A", "B", "C" };
            for (int r = 0; r < rl.Length; r++)
            {
                var rb = Btn(grpGO.transform, "Radio_" + rl[r], rl[r], (r - 1) * (80f + 8f), 0, 78, BH, prim, InteractionMode.Radio);
                var rso = new SerializedObject(rb.GetComponent<AdvancedUIButton>());
                rso.FindProperty("_toggleGroup").objectReferenceValue = tg;
                rso.ApplyModifiedProperties();
                int sz = tgSO.FindProperty("_buttons").arraySize;
                tgSO.FindProperty("_buttons").arraySize = sz + 1;
                tgSO.FindProperty("_buttons").GetArrayElementAtIndex(sz).objectReferenceValue = rb.GetComponent<AdvancedUIButton>();
            }
            tgSO.ApplyModifiedProperties();
            Lbl(c, "Radio Group", rx, BH * 0.5f + 16, BW + 40, 20, 11, CCap);

            // Hold
            float hx = x0 + col * 3f;
            var holdBtn = Btn(c, "Btn_Hold", "Hold (1s)", hx, 8, BW, BH, Preset("Danger"), InteractionMode.Hold);
            var pbBgGO = new GameObject("ProgressBg");
            pbBgGO.AddComponent<RectTransform>();
            Place(pbBgGO, c, hx, -BH * 0.5f - 16, BW, 8);
            pbBgGO.AddComponent<Image>().color = Hex("1E293B");
            var fillGO = new GameObject("Fill");
            var fillRT = fillGO.AddComponent<RectTransform>();
            fillRT.SetParent(pbBgGO.transform, false);
            fillRT.anchorMin = new Vector2(0, 0); fillRT.anchorMax = new Vector2(0, 1);
            fillRT.pivot = new Vector2(0, 0.5f);
            fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;
            fillGO.AddComponent<Image>().color = Hex("EF4444");

            // Wire hold references on DemoController -- events subscribed at runtime via HoldConfig
            var hso = new SerializedObject(ctrl);
            hso.FindProperty("holdButton").objectReferenceValue = holdBtn.GetComponent<AdvancedUIButton>();
            hso.FindProperty("holdFill").objectReferenceValue = fillRT;
            hso.FindProperty("holdFillWidth").floatValue = BW;
            hso.ApplyModifiedProperties();
            Lbl(c, "Hold (1s)", hx, BH * 0.5f + 16, BW, 20, 11, CCap);
        }

        // ---- Section 3 : All States ----
        static void Sec3_States(Transform root, float y, float w)
        {
            var c = Section(root, "S3", "All States  (hover to explore)", y, w, BgSec);
            var prim = Preset("Primary");
            string[] states = { "Normal", "Hover", "Pressed", "Selected", "Focused", "Disabled" };
            float x0 = -(states.Length * BW + (states.Length - 1) * BG) * 0.5f + BW * 0.5f;
            for (int i = 0; i < states.Length; i++)
            {
                float x = x0 + i * (BW + BG);
                var btn = Btn(c, "State_" + states[i], states[i], x, 8, BW, BH, prim);
                var adv = btn.GetComponent<AdvancedUIButton>();
                var bgImg = btn.transform.Find("Background").GetComponent<Image>();
                var lblTmp = btn.transform.Find("Label").GetComponent<TextMeshProUGUI>();

                switch (states[i])
                {
                    case "Normal":
                        // Leave fully interactive -- visitor can hover to see transitions
                        break;

                    case "Disabled":
                        adv.SetInteractable(false);
                        break;

                    case "Selected":
                        adv.SetSelected(true);
                        break;

                    default:
                        {
                            // Freeze in target state: disable AdvancedUIButton so no
                            // state machine runs, then set colors directly on the Graphics.
                            // No extra component needed -- nothing to serialize, nothing to break.
                            adv.enabled = false;
                            bgImg.color = states[i] switch
                            {
                                "Hover" => prim.background.colors.highlighted,
                                "Pressed" => prim.background.colors.pressed,
                                "Focused" => prim.background.colors.focused,
                                _ => prim.background.colors.normal
                            };
                            lblTmp.color = states[i] switch
                            {
                                "Hover" => prim.label.colors.highlighted,
                                "Pressed" => prim.label.colors.pressed,
                                "Focused" => prim.label.colors.focused,
                                _ => prim.label.colors.normal
                            };
                            break;
                        }
                }
                Lbl(c, states[i], x, -BH * 0.5f - 16, BW, 22, 12, CCap);
            }
        }

        // ---- Section 4 : Ripple ----
        static void Sec4_Ripple(Transform root, float y, float w)
        {
            var c = Section(root, "S4", "Ripple Effect", y, w, BgDark);
            Color[] cols = { new Color(1, 1, 1, 0.30f), new Color(0.4f, 0.7f, 1, 0.35f), new Color(0, 0.9f, 1, 0.30f) };
            string[] caps = { "White", "Blue", "Cyan" };
            float x0 = -(3 * BW + 2 * BG) * 0.5f + BW * 0.5f;
            for (int i = 0; i < 3; i++)
            {
                float x = x0 + i * (BW + BG);
                var btn = Btn(c, "RippleBtn_" + i, "Click Me", x, 8, BW, BH, Preset("Primary"));
                btn.AddComponent<RectMask2D>();
                var rGO = new GameObject("Ripple");
                var rRT = rGO.AddComponent<RectTransform>();
                rRT.SetParent(btn.transform, false);
                rRT.SetSiblingIndex(1);
                rRT.anchorMin = Vector2.zero; rRT.anchorMax = Vector2.one;
                rRT.offsetMin = Vector2.zero; rRT.offsetMax = Vector2.zero;
                var rip = rGO.AddComponent<UIButtonRipple>();
                var rso = new SerializedObject(rip);
                rso.FindProperty("_rippleColor").colorValue = cols[i];
                rso.FindProperty("_syncWithStyle").boolValue = false;
                rso.ApplyModifiedProperties();
                var bso = new SerializedObject(btn.GetComponent<AdvancedUIButton>());
                bso.FindProperty("_ripple").objectReferenceValue = rip;
                bso.ApplyModifiedProperties();
                Lbl(c, caps[i] + " Ripple", x, -BH * 0.5f - 16, BW + 20, 22, 12, CCap);
            }
        }

        // ---- Section 5 : Easing ----
        static void Sec5_Easing(Transform root, float y, float w)
        {
            var c = Section(root, "S5", "Easing Showcase", y, w, BgSec);
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
                s.animation.transform.highlightedScale = new Vector3(1.14f, 1.14f, 1);
                s.animation.transform.pressedScale = new Vector3(0.87f, 0.87f, 1);
                Btn(c, "Ease_" + labels[i], labels[i], x, 8, BW, BH, s);
                Lbl(c, labels[i], x, -BH * 0.5f - 16, BW, 22, 12, CCap);
            }
        }

        // ---- Primitives ----

        static Transform Section(Transform root, string id, string title,
            float y, float w, Color bg)
        {
            var go = new GameObject(id);
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(root, false);
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, y); rt.sizeDelta = new Vector2(w, SecH);
            go.AddComponent<Image>().color = bg;

            var tGO = new GameObject("Title");
            var tRT = tGO.AddComponent<RectTransform>();
            tRT.SetParent(go.transform, false);
            tRT.anchorMin = new Vector2(0, 1); tRT.anchorMax = new Vector2(1, 1);
            tRT.pivot = new Vector2(0.5f, 1);
            tRT.anchoredPosition = new Vector2(0, -4); tRT.sizeDelta = new Vector2(-40, 28);
            var ttmp = tGO.AddComponent<TextMeshProUGUI>();
            ttmp.text = title; ttmp.fontSize = 15; ttmp.color = CTitle;
            ttmp.fontStyle = FontStyles.Bold; ttmp.alignment = TextAlignmentOptions.Left;
            ttmp.raycastTarget = false;

            var sGO = new GameObject("Sep");
            var sRT = sGO.AddComponent<RectTransform>();
            sRT.SetParent(go.transform, false);
            sRT.anchorMin = new Vector2(0, 1); sRT.anchorMax = new Vector2(1, 1);
            sRT.pivot = new Vector2(0.5f, 1);
            sRT.anchoredPosition = new Vector2(0, -34); sRT.sizeDelta = new Vector2(-40, 1);
            sGO.AddComponent<Image>().color = CSep;

            var cGO = new GameObject("Content");
            var cRT = cGO.AddComponent<RectTransform>();
            cRT.SetParent(go.transform, false);
            cRT.anchorMin = new Vector2(0.5f, 0.5f); cRT.anchorMax = new Vector2(0.5f, 0.5f);
            cRT.pivot = new Vector2(0.5f, 0.5f);
            cRT.anchoredPosition = new Vector2(0, -18); cRT.sizeDelta = new Vector2(w - 60, SecH - 52);
            return cGO.transform;
        }

        static GameObject Btn(Transform parent, string name, string lbl,
            float x, float y, float w, float h, UIButtonStyle style,
            InteractionMode mode = InteractionMode.Standard)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            Place(go, parent, x, y, w, h);

            var bgGO = new GameObject("Background");
            var bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.SetParent(go.transform, false);
            Stretch(bgRT);
            bgGO.AddComponent<Image>().color = Color.white;

            var lGO = new GameObject("Label");
            var lRT = lGO.AddComponent<RectTransform>();
            lRT.SetParent(go.transform, false);
            Stretch(lRT);
            var tmp = lGO.AddComponent<TextMeshProUGUI>();
            tmp.text = lbl; tmp.fontSize = 15; tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center; tmp.raycastTarget = false;

            var adv = go.AddComponent<AdvancedUIButton>();
            var so = new SerializedObject(adv);
            so.FindProperty("_mode").enumValueIndex = (int)mode;
            var entries = so.FindProperty("_animator").FindPropertyRelative("_graphicEntries");
            entries.arraySize = 2;
            var e0 = entries.GetArrayElementAtIndex(0);
            e0.FindPropertyRelative("_target").objectReferenceValue = bgGO.GetComponent<Image>();
            e0.FindPropertyRelative("_role").enumValueIndex = (int)GraphicRole.Background;
            var e1 = entries.GetArrayElementAtIndex(1);
            e1.FindPropertyRelative("_target").objectReferenceValue = tmp;
            e1.FindPropertyRelative("_role").enumValueIndex = (int)GraphicRole.Label;
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
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.color = col;
            tmp.alignment = TextAlignmentOptions.Center; tmp.raycastTarget = false;
            return go;
        }

        static void Place(GameObject go, Transform parent, float x, float y, float w, float h)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h);
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
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
            call.FindPropertyRelative("m_Mode").enumValueIndex = 1; // Void
            call.FindPropertyRelative("m_CallState").enumValueIndex = 2; // RuntimeOnly
            call.FindPropertyRelative("m_Arguments.m_ObjectArgumentAssemblyTypeName").stringValue = "UnityEngine.Object, UnityEngine";
        }

        static void WireFloat(SerializedProperty evt, Object target, string method)
        {
            var calls = evt.FindPropertyRelative("m_PersistentCalls.m_Calls");
            int i = calls.arraySize++;
            var call = calls.GetArrayElementAtIndex(i);
            call.FindPropertyRelative("m_Target").objectReferenceValue = target;
            call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue =
                target.GetType().AssemblyQualifiedName;
            call.FindPropertyRelative("m_MethodName").stringValue = method;
            call.FindPropertyRelative("m_Mode").enumValueIndex = 5; // Float
            call.FindPropertyRelative("m_CallState").enumValueIndex = 2; // RuntimeOnly
            // Float argument must be initialized even if value is 0
            call.FindPropertyRelative("m_Arguments.m_FloatArgument").floatValue = 0f;
            call.FindPropertyRelative("m_Arguments.m_ObjectArgumentAssemblyTypeName").stringValue = "UnityEngine.Object, UnityEngine";
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