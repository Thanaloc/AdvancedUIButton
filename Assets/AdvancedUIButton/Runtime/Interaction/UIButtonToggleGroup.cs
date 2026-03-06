// AdvancedUIButton -- Advanced UI Button System for Unity
// Copyright (c) 2025 AdvancedUI. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace AdvancedUI
{
    /// <summary>
    /// Manages mutual exclusion between a set of AdvancedUIButton components
    /// in Toggle or Radio mode. Only one button can be selected at a time.
    /// Supports optional "allow none" mode and dynamic registration.
    /// </summary>
    [AddComponentMenu("AdvancedUI/UI Button Toggle Group")]
    [DisallowMultipleComponent]
    public sealed class UIButtonToggleGroup : MonoBehaviour
    {
        [Tooltip("When enabled, clicking the selected button deselects it without selecting another.")]
        [SerializeField] private bool _allowNone;

        [Tooltip("Pre-registered buttons in this group. Buttons can also register themselves at runtime.")]
        [SerializeField] private List<AdvancedUIButton> _buttons = new List<AdvancedUIButton>();

        private AdvancedUIButton _current;

        // Lifecycle

        private void Start()
        {
            // Resolve initial selection: keep the first selected button,
            // deselect any others, and enforce a default if none is selected.
            for (int i = 0; i < _buttons.Count; i++)
            {
                AdvancedUIButton btn = _buttons[i];
                if (btn == null) continue;

                if (btn.IsToggleOn)
                {
                    if (_current == null)
                        _current = btn;
                    else
                        btn.SetSelected(false); // deselect duplicates
                }
            }

            if (_current == null && !_allowNone && _buttons.Count > 0)
                SelectButton(_buttons[0]);
        }

        // Public API

        /// <summary>
        /// Called by a button when it becomes selected.
        /// Deselects the previously selected button.
        /// </summary>
        public void NotifySelected(AdvancedUIButton button)
        {
            if (_current == button) return;
            _current?.SetSelected(false);
            _current = button;
        }

        /// <summary>Selects a specific button in the group.</summary>
        public void SelectButton(AdvancedUIButton button)
        {
            if (!_buttons.Contains(button))
            {
                Debug.LogWarning($"[UIButtonToggleGroup] Button '{button.name}' is not registered in this group.", this);
                return;
            }
            button.SetSelected(true);
        }

        /// <summary>Selects the button at the given index.</summary>
        public void SelectIndex(int index)
        {
            if (index < 0 || index >= _buttons.Count)
            {
                Debug.LogWarning($"[UIButtonToggleGroup] Index {index} is out of range (count: {_buttons.Count}).", this);
                return;
            }
            SelectButton(_buttons[index]);
        }

        /// <summary>
        /// Deselects all buttons. Only works when AllowNone is enabled.
        /// </summary>
        public void DeselectAll()
        {
            if (!_allowNone) return;
            _current?.SetSelected(false);
            _current = null;
        }

        /// <summary>
        /// Registers a button into this group at runtime.
        /// Safe to call multiple times with the same button.
        /// </summary>
        public void Register(AdvancedUIButton button)
        {
            if (button == null || _buttons.Contains(button)) return;
            _buttons.Add(button);
        }

        /// <summary>
        /// Unregisters a button from this group.
        /// If it was the current selection, the current is cleared.
        /// </summary>
        public void Unregister(AdvancedUIButton button)
        {
            if (!_buttons.Remove(button)) return;
            if (_current == button) _current = null;
        }

        // Properties

        /// <summary>The currently selected button, or null if none is selected.</summary>
        public AdvancedUIButton Current => _current;

        /// <summary>Whether the group can have no button selected.</summary>
        public bool AllowNone => _allowNone;

        /// <summary>Number of buttons registered in this group.</summary>
        public int Count => _buttons.Count;
    }
}