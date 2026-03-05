using System.Collections.Generic;
using UnityEngine;

namespace AdvancedUI
{
    [AddComponentMenu("AdvancedUI/UI Button Toggle Group")]
    public class UIButtonToggleGroup : MonoBehaviour
    {
        [SerializeField] private bool _allowNone = false;
        [SerializeField] private List<AdvancedUIButton> _buttons = new List<AdvancedUIButton>();

        private AdvancedUIButton _current;

        private void Awake()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] != null)
                    _buttons[i].OnSelectionChangedEvent += _ => { };
            }
        }

        private void Start()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                var btn = _buttons[i];
                if (btn == null) continue;

                if (btn.IsToggleOn && _current == null)
                    _current = btn;
                else if (btn.IsToggleOn)
                    btn.SetSelected(false);
            }

            if (_current == null && !_allowNone && _buttons.Count > 0)
                SelectButton(_buttons[0]);
        }

        public void NotifySelected(AdvancedUIButton button)
        {
            if (_current == button) return;

            if (_current != null)
                _current.SetSelected(false);

            _current = button;
        }

        public void SelectButton(AdvancedUIButton button)
        {
            if (!_buttons.Contains(button)) return;
            button.SetSelected(true);
        }

        public void SelectIndex(int index)
        {
            if (index < 0 || index >= _buttons.Count) return;
            SelectButton(_buttons[index]);
        }

        public void Deselect()
        {
            if (!_allowNone) return;
            _current?.SetSelected(false);
            _current = null;
        }

        public void Register(AdvancedUIButton button)
        {
            if (button == null || _buttons.Contains(button)) return;
            _buttons.Add(button);
        }

        public void Unregister(AdvancedUIButton button)
        {
            _buttons.Remove(button);
            if (_current == button) _current = null;
        }

        public AdvancedUIButton Current => _current;
    }
}
