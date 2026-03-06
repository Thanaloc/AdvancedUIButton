// AdvancedUIButton Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdvancedUI.Editor
{
    [CustomEditor(typeof(AdvancedUIButton))]
    public class AdvancedUIButtonEditor : ButtonEditor
    {
        private static readonly string[] Tabs = { "Style", "Graphics", "Transform", "Audio", "Interaction", "Events", "Ripple" };

        private static readonly Color ColorStyle = new Color(0.30f, 0.60f, 1.00f, 0.18f);
        private static readonly Color ColorGraphics = new Color(0.30f, 0.85f, 0.45f, 0.18f);
        private static readonly Color ColorTransform = new Color(1.00f, 0.80f, 0.20f, 0.18f);
        private static readonly Color ColorAudio = new Color(0.75f, 0.35f, 1.00f, 0.18f);
        private static readonly Color ColorInteraction = new Color(1.00f, 0.45f, 0.35f, 0.18f);
        private static readonly Color ColorEvents = new Color(0.50f, 0.50f, 0.50f, 0.18f);
        private static readonly Color ColorRipple = new Color(0.20f, 0.80f, 1.00f, 0.18f);
        private static readonly Color ColorStateBar = new Color(0.12f, 0.12f, 0.18f, 0.95f);

        private static readonly Color[] TabColors =
        {
            ColorStyle, ColorGraphics, ColorTransform, ColorAudio, ColorInteraction, ColorEvents, ColorRipple
        };

        private static readonly string[] StateLabels =
        {
            "Normal", "Hover", "Pressed", "Selected", "Sel+Hover", "Focused", "Disabled"
        };

        private static readonly string[] StateFieldNames =
        {
            "normal", "highlighted", "pressed", "selected", "selectedHighlighted", "focused", "disabled"
        };

        private int _tab;
        private static int _savedTab;

        private SerializedProperty _style;
        private SerializedProperty _animator;
        private SerializedProperty _animatorEntries;
        private SerializedProperty _animatorTransform;
        private SerializedProperty _audio;
        private SerializedProperty _mode;
        private SerializedProperty _holdSettings;
        private SerializedProperty _ignoreTimeScale;
        private SerializedProperty _isSelected;
        private SerializedProperty _toggleGroup;
        private SerializedProperty _ripple;
        private SerializedProperty _onHoverEnter;
        private SerializedProperty _onHoverExit;
        private SerializedProperty _onPress;
        private SerializedProperty _onRelease;
        private SerializedProperty _onSelected;
        private SerializedProperty _onDeselected;
        private SerializedProperty _onHoldComplete;

        private readonly List<bool> _entryFoldouts = new List<bool>();

        // clipboard for copy/paste colors between entries
        private static StateColors _copiedColors;

        protected override void OnEnable()
        {
            base.OnEnable();
            _tab = _savedTab;

            _style = serializedObject.FindProperty("_style");
            _animator = serializedObject.FindProperty("_animator");
            _animatorEntries = _animator.FindPropertyRelative("_graphicEntries");
            _animatorTransform = _animator.FindPropertyRelative("_transformSettings");
            _audio = serializedObject.FindProperty("_audio");
            _mode = serializedObject.FindProperty("_mode");
            _holdSettings = serializedObject.FindProperty("_holdSettings");
            _ignoreTimeScale = serializedObject.FindProperty("_ignoreTimeScale");
            _isSelected = serializedObject.FindProperty("_isSelected");
            _toggleGroup = serializedObject.FindProperty("_toggleGroup");
            _ripple = serializedObject.FindProperty("_ripple");
            _onHoverEnter = serializedObject.FindProperty("_onHoverEnter");
            _onHoverExit = serializedObject.FindProperty("_onHoverExit");
            _onPress = serializedObject.FindProperty("_onPress");
            _onRelease = serializedObject.FindProperty("_onRelease");
            _onSelected = serializedObject.FindProperty("_onSelected");
            _onDeselected = serializedObject.FindProperty("_onDeselected");
            _onHoldComplete = serializedObject.FindProperty("_onHoldComplete");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawStateBar();
            EditorGUILayout.Space(4);
            DrawTabs();
            EditorGUILayout.Space(4);

            switch (_tab)
            {
                case 0: DrawStyleTab(); break;
                case 1: DrawGraphicsTab(); break;
                case 2: DrawTransformTab(); break;
                case 3: DrawAudioTab(); break;
                case 4: DrawInteractionTab(); break;
                case 5: DrawEventsTab(); break;
                case 6: DrawRippleTab(); break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStateBar()
        {
            var btn = (AdvancedUIButton)target;
            Rect rect = GUILayoutUtility.GetRect(0, 32, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, ColorStateBar);

            GUI.Label(new Rect(rect.x + 8, rect.y + 4, rect.width * 0.55f, 22), "AdvancedUI  --  Button", EditorStyles.boldLabel);

            if (Application.isPlaying)
                GUI.Label(new Rect(rect.x + rect.width * 0.55f, rect.y + 4, rect.width * 0.45f - 8, 22),
                    $"State : {btn.CurrentState}", EditorStyles.miniLabel);
        }

        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < Tabs.Length; i++)
            {
                bool active = _tab == i;
                Color bg = TabColors[i];
                bg.a = active ? 0.55f : 0.18f;

                Rect r = GUILayoutUtility.GetRect(new GUIContent(Tabs[i]), EditorStyles.miniButtonMid, GUILayout.ExpandWidth(true));
                EditorGUI.DrawRect(r, bg);

                if (GUI.Button(r, Tabs[i], EditorStyles.miniButtonMid))
                {
                    _tab = i;
                    _savedTab = i;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // STYLE

        private void DrawStyleTab()
        {
            DrawSection("Style Asset", ColorStyle, () =>
            {
                EditorGUILayout.PropertyField(_style, new GUIContent("Button Style",
                    "Drag a UIButtonStyle asset here. Click Apply Style to push colors and animation settings to all graphic entries."));

                var btn = (AdvancedUIButton)target;

                if (_style.objectReferenceValue != null)
                {
                    EditorGUILayout.Space(2);
                    if (GUILayout.Button(new GUIContent("Apply Style", "Overwrites colors on Background/Label/Icon entries and updates animation settings.")))
                        btn.ApplyStyle((UIButtonStyle)_style.objectReferenceValue);
                }

                EditorGUILayout.Space(6);
                EditorGUILayout.PropertyField(_ignoreTimeScale, new GUIContent("Ignore Time Scale",
                    "When enabled, all animations use unscaledDeltaTime. The button stays responsive when the game is paused."));
                EditorGUILayout.PropertyField(_isSelected, new GUIContent("Initially Selected",
                    "Sets the button as selected when it first starts. Useful for Toggle/Radio modes."));
            });
        }

        // GRAPHICS

        private void DrawGraphicsTab()
        {
            DrawSection("Graphic Entries", ColorGraphics, () =>
            {
                EditorGUILayout.HelpBox(
                    "Add one entry per Graphic to animate (e.g. Background image, Label text, Icon).\n" +
                    "Set the Role so Apply Style knows which colors to map from the Style asset.",
                    MessageType.None);

                EditorGUILayout.Space(2);

                while (_entryFoldouts.Count < _animatorEntries.arraySize)
                    _entryFoldouts.Add(true);

                for (int i = 0; i < _animatorEntries.arraySize; i++)
                {
                    SerializedProperty entry = _animatorEntries.GetArrayElementAtIndex(i);
                    SerializedProperty tgt = entry.FindPropertyRelative("_target");
                    SerializedProperty role = entry.FindPropertyRelative("_role");

                    string label = tgt.objectReferenceValue != null
                        ? $"{tgt.objectReferenceValue.name}  [{(GraphicRole)role.enumValueIndex}]"
                        : $"Entry {i}  [{(GraphicRole)role.enumValueIndex}]";

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    // header row
                    EditorGUILayout.BeginHorizontal();
                    _entryFoldouts[i] = EditorGUILayout.Foldout(_entryFoldouts[i], label, true, EditorStyles.foldoutHeader);

                    if (GUILayout.Button(new GUIContent("C", "Copy colors from this entry"), GUILayout.Width(22), GUILayout.Height(18)))
                        CopyColors(entry.FindPropertyRelative("_colors"));

                    GUI.enabled = _copiedColors != null;
                    if (GUILayout.Button(new GUIContent("P", "Paste copied colors into this entry"), GUILayout.Width(22), GUILayout.Height(18)))
                        PasteColors(entry.FindPropertyRelative("_colors"));
                    GUI.enabled = true;

                    if (GUILayout.Button(new GUIContent("X", "Remove this entry"), GUILayout.Width(22), GUILayout.Height(18)))
                    {
                        _animatorEntries.DeleteArrayElementAtIndex(i);
                        _entryFoldouts.RemoveAt(i);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (_entryFoldouts[i])
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(tgt, new GUIContent("Target Graphic",
                            "The Graphic component (Image, Text, TMP_Text, etc.) to colorize."));

                        EditorGUILayout.PropertyField(role, new GUIContent("Role",
                            "Background / Label / Icon entries receive their colors from the Style asset when Apply Style is clicked. Custom entries are never overwritten."));

                        EditorGUILayout.Space(4);
                        DrawStateColorsCompact(entry.FindPropertyRelative("_colors"));

                        EditorGUILayout.Space(4);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("_duration"), new GUIContent("Duration",
                            "Color transition duration in seconds. Set to 0 for instant."));
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("_easing"), new GUIContent("Easing",
                            "Easing function used for the color transition."));
                        EditorGUILayout.EndHorizontal();

                        if (GUILayout.Button(new GUIContent("Reset Colors to Default", "Resets all state colors to the built-in defaults.")))
                            ResetColors(entry.FindPropertyRelative("_colors"));

                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }

                if (GUILayout.Button("+ Add Graphic Entry"))
                {
                    _animatorEntries.arraySize++;
                    _entryFoldouts.Add(true);
                }
            });
        }

        // TRANSFORM

        private void DrawTransformTab()
        {
            DrawSection("Scale Animation", ColorTransform, () =>
            {
                SerializedProperty enabled = _animatorTransform.FindPropertyRelative("_enabled");
                EditorGUILayout.PropertyField(enabled, new GUIContent("Enabled",
                    "Animates the RectTransform localScale on state changes."));

                if (enabled.boolValue)
                {
                    EditorGUILayout.Space(4);
                    SerializedProperty states = _animatorTransform.FindPropertyRelative("_states");

                    DrawScaleRow("Normal", states.FindPropertyRelative("normalScale"));
                    DrawScaleRow("Hover", states.FindPropertyRelative("highlightedScale"));
                    DrawScaleRow("Pressed", states.FindPropertyRelative("pressedScale"));
                    DrawScaleRow("Selected", states.FindPropertyRelative("selectedScale"));
                    DrawScaleRow("Focused", states.FindPropertyRelative("focusedScale"));
                    DrawScaleRow("Disabled", states.FindPropertyRelative("disabledScale"));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(_animatorTransform.FindPropertyRelative("_duration"), new GUIContent("Duration",
                        "Scale transition duration in seconds."));
                    EditorGUILayout.PropertyField(_animatorTransform.FindPropertyRelative("_easing"), new GUIContent("Easing",
                        "Easing function used for the scale transition. BackOut gives a nice springy feel."));
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        // AUDIO

        private void DrawAudioTab()
        {
            DrawSection("Audio", ColorAudio, () =>
            {
                SerializedProperty enabled = _audio.FindPropertyRelative("_enabled");
                EditorGUILayout.PropertyField(enabled, new GUIContent("Enabled",
                    "Enable audio feedback on state changes."));

                if (enabled.boolValue)
                {
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_source"), new GUIContent("Audio Source",
                        "The AudioSource used to play button sounds. Place it on a persistent GameObject."));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_onHoverEnter"), new GUIContent("Hover Enter"));
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_onPress"), new GUIContent("Press"));
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_onSelect"), new GUIContent("Select"));
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_onDeselect"), new GUIContent("Deselect"));
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_onDisabled"), new GUIContent("Disabled"));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_volume"), new GUIContent("Volume"));
                    EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_pitch"), new GUIContent("Pitch"));
                    EditorGUILayout.EndHorizontal();

                    SerializedProperty randPitch = _audio.FindPropertyRelative("_randomizePitch");
                    EditorGUILayout.PropertyField(randPitch, new GUIContent("Randomize Pitch",
                        "Adds a small random offset to the pitch on each play to avoid repetition."));

                    if (randPitch.boolValue)
                        EditorGUILayout.PropertyField(_audio.FindPropertyRelative("_pitchVariance"), new GUIContent("Variance",
                            "Max pitch offset in either direction (e.g. 0.1 -> pitch +/- 0.1)."));
                }
            });
        }

        // INTERACTION

        private void DrawInteractionTab()
        {
            DrawSection("Interaction", ColorInteraction, () =>
            {
                EditorGUILayout.PropertyField(_mode, new GUIContent("Mode",
                    "Standard: normal click button.\nToggle: stays on/off on click.\nRadio: stays on, cannot deselect by clicking again (use ToggleGroup).\nHold: fires OnHoldComplete after holding for the set duration."));

                InteractionMode mode = (InteractionMode)_mode.enumValueIndex;

                if (mode == InteractionMode.Toggle || mode == InteractionMode.Radio)
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.PropertyField(_isSelected, new GUIContent("Initially Selected"));
                    EditorGUILayout.PropertyField(_toggleGroup, new GUIContent("Toggle Group",
                        "Assign a UIButtonToggleGroup to ensure only one button is selected at a time (tab bar, radio group)."));
                }

                if (mode == InteractionMode.Hold)
                {
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Hold Settings", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(_holdSettings.FindPropertyRelative("_duration"), new GUIContent("Hold Duration",
                        "How long in seconds the user must hold the button to trigger OnHoldComplete."));
                    EditorGUILayout.PropertyField(_holdSettings.FindPropertyRelative("_cancelOnExit"), new GUIContent("Cancel On Exit",
                        "If enabled, moving the pointer outside the button cancels the hold."));

                    EditorGUILayout.Space(2);
                    EditorGUILayout.PropertyField(_holdSettings.FindPropertyRelative("_onHoldComplete"), new GUIContent("On Hold Complete"));
                    EditorGUILayout.PropertyField(_holdSettings.FindPropertyRelative("_onHoldProgress"), new GUIContent("On Hold Progress",
                        "Fires every frame with a 0-1 progress value. Use it to drive a progress bar fill."));
                }

                if (Application.isPlaying)
                {
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
                    var btn = (AdvancedUIButton)target;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select")) btn.SetSelected(true);
                    if (GUILayout.Button("Deselect")) btn.SetSelected(false);
                    if (GUILayout.Button("Force Refresh")) btn.ForceRefresh();
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        // EVENTS

        private void DrawEventsTab()
        {
            DrawSection("C# Events", ColorEvents, () =>
            {
                EditorGUILayout.HelpBox(
                    "Subscribe to C# events directly in code for better performance:\n" +
                    "btn.OnHoverEnterEvent += MyMethod;\n" +
                    "btn.OnSelectionChangedEvent += isOn => { };",
                    MessageType.None);
            });

            DrawSection("Unity Events", ColorEvents, () =>
            {
                EditorGUILayout.PropertyField(_onHoverEnter, new GUIContent("On Hover Enter"));
                EditorGUILayout.PropertyField(_onHoverExit, new GUIContent("On Hover Exit"));
                EditorGUILayout.PropertyField(_onPress, new GUIContent("On Press"));
                EditorGUILayout.PropertyField(_onRelease, new GUIContent("On Release"));
                EditorGUILayout.PropertyField(_onSelected, new GUIContent("On Selected"));
                EditorGUILayout.PropertyField(_onDeselected, new GUIContent("On Deselected"));
                EditorGUILayout.PropertyField(_onHoldComplete, new GUIContent("On Hold Complete"));

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Base Button OnClick", EditorStyles.boldLabel);
                base.OnInspectorGUI();
            });
        }

        // RIPPLE

        private void DrawRippleTab()
        {
            DrawSection("Ripple Effect", ColorRipple, () =>
            {
                EditorGUILayout.PropertyField(_ripple, new GUIContent("Ripple Component",
                    "Assign the UIButtonRipple component here, or click Auto Setup to create and configure it automatically."));

                if (_ripple.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(
                        "No UIButtonRipple assigned. Click Auto Setup to create one automatically, " +
                        "or add UIButtonRipple to a child GameObject and drag it here.",
                        MessageType.Info);

                    EditorGUILayout.Space(2);

                    if (GUILayout.Button(new GUIContent("[+] Auto Setup Ripple",
                        "Creates a child GameObject, adds UIButtonRipple, configures its RectTransform and assigns it automatically.")))
                    {
                        AutoSetupRipple();
                    }
                }
                else
                {
                    var ripple = (UIButtonRipple)_ripple.objectReferenceValue;
                    EditorGUILayout.Space(4);

                    EditorGUILayout.HelpBox(
                        "Sync With Style: when enabled, Apply Style automatically sets the ripple color\n" +
                        "to a tinted version of the highlighted background color.",
                        MessageType.None);

                    EditorGUILayout.Space(2);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Remove Ripple", "Destroys the ripple component and clears the reference.")))
                    {
                        RemoveRipple(ripple);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        private void AutoSetupRipple()
        {
            var btn = (AdvancedUIButton)target;

            // Check if a UIButtonRipple already exists as a child
            UIButtonRipple existing = btn.GetComponentInChildren<UIButtonRipple>();
            if (existing != null)
            {
                _ripple.objectReferenceValue = existing;
                serializedObject.ApplyModifiedProperties();
                Debug.Log("[AdvancedUI] Found existing UIButtonRipple and assigned it.");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(btn.gameObject, "Auto Setup Ripple");

            GameObject go = new GameObject("Ripple");
            Undo.RegisterCreatedObjectUndo(go, "Create Ripple GameObject");
            go.transform.SetParent(btn.transform, false);

            // Place at end of hierarchy so it renders on top of background, under label
            go.transform.SetAsLastSibling();

            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;

            UIButtonRipple ripple = go.AddComponent<UIButtonRipple>();
            _ripple.objectReferenceValue = ripple;
            serializedObject.ApplyModifiedProperties();

            // Sync color if a style is already applied
            if (btn.EditorStyle != null)
                ripple.ApplyStyle(btn.EditorStyle);

            Debug.Log("[AdvancedUI] Ripple created and configured. Move it between Background and Label in the hierarchy, then add RectMask2D to the button root.");
        }

        private void RemoveRipple(UIButtonRipple ripple)
        {
            if (ripple == null) return;
            Undo.RegisterFullObjectHierarchyUndo(((AdvancedUIButton)target).gameObject, "Remove Ripple");
            _ripple.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
            Undo.DestroyObjectImmediate(ripple.gameObject);
        }

        // HELPERS

        private static void DrawStateColorsCompact(SerializedProperty colors)
        {
            EditorGUILayout.LabelField("State Colors", EditorStyles.boldLabel);

            for (int i = 0; i < StateLabels.Length; i++)
            {
                SerializedProperty c = colors.FindPropertyRelative(StateFieldNames[i]);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(StateLabels[i]), GUILayout.Width(90));
                c.colorValue = EditorGUILayout.ColorField(GUIContent.none, c.colorValue, true, true, false);
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawScaleRow(string label, SerializedProperty prop)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(80));
            prop.vector3Value = EditorGUILayout.Vector3Field(GUIContent.none, prop.vector3Value);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawSection(string title, Color color, System.Action content)
        {
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.DrawRect(rect, color);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(2);
            content?.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);
        }

        private void CopyColors(SerializedProperty colorsProp)
        {
            _copiedColors = new StateColors();
            for (int i = 0; i < StateFieldNames.Length; i++)
            {
                var field = typeof(StateColors).GetField(StateFieldNames[i]);
                if (field != null)
                    field.SetValue(_copiedColors, colorsProp.FindPropertyRelative(StateFieldNames[i]).colorValue);
            }
        }

        private void PasteColors(SerializedProperty colorsProp)
        {
            if (_copiedColors == null) return;
            for (int i = 0; i < StateFieldNames.Length; i++)
            {
                var field = typeof(StateColors).GetField(StateFieldNames[i]);
                if (field != null)
                    colorsProp.FindPropertyRelative(StateFieldNames[i]).colorValue = (Color)field.GetValue(_copiedColors);
            }
        }

        private static void ResetColors(SerializedProperty colorsProp)
        {
            var defaults = new StateColors();
            for (int i = 0; i < StateFieldNames.Length; i++)
            {
                var field = typeof(StateColors).GetField(StateFieldNames[i]);
                if (field != null)
                    colorsProp.FindPropertyRelative(StateFieldNames[i]).colorValue = (Color)field.GetValue(defaults);
            }
        }
    }
}
#endif