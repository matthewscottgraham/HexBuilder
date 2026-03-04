using System;
using App.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace App.UIComponents
{
    public class RadioBar : VisualElement, INotifyValueChanged<int>
    {
        private int _value;
        private Button[] _buttons;
        
        public int value
        {
            get => _value;
            set => SetValue(value);
        }
        
        public RadioBar(string[] labels)
        {
            Setup(labels, CreateTextButton);
        }

        public RadioBar(Sprite[] icons)
        {
            Setup(icons, CreateIconButton);
        }

        public void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, _buttons.Length - 1);
            if (value == newValue) return;
            var previous = _value;
            _value = newValue;
            UpdateVisualState();

            using (var evt = ChangeEvent<int>.GetPooled(previous, _value))
            {
                evt.target = this;
                SendEvent(evt);
            }
        }

        public void SetValueWithoutNotify(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, _buttons.Length - 1);
            if (value == newValue) return;
            _value = newValue;
            UpdateVisualState();
        }

        public void SetButtonVisibility(int index, bool isVisible)
        {
            index = Mathf.Clamp(index, 0, _buttons.Length - 1);
            _buttons[index].SetVisibility(isVisible);
            if (_value == index) SetFirstAvailableValue();
        }

        public void SetButtonInteractable(int index, bool isInteractable)
        {
            index = Mathf.Clamp(index, 0, _buttons.Length - 1);
            _buttons[index].SetEnabled(isInteractable);
            if (_value == index) SetFirstAvailableValue();
        }

        private void Setup<T>(T[] array, Func<T, Button> createButton)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array cannot be null or empty.");
            
            AddToClassList("radio-bar");
            _buttons = new Button[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                var button = createButton(array[i]);
                button.AddToClassList(typeof(T) == typeof(Sprite) ? "radio-bar__button-icon" : "radio-bar__button-text");
                button.RegisterCallback<ClickEvent>(OnButtonClicked);
                _buttons[i] = button;
                Add(button);
                
                if (i == 0) button.AddToClassList("radio-bar__button-first");
                if (i == array.Length - 1) button.AddToClassList("radio-bar__button-last");
            }
            
            SetValueWithoutNotify(0);
            UpdateVisualState();
            RegisterCallback<DetachFromPanelEvent>(CleanUp);
        }

        private void CleanUp(DetachFromPanelEvent evt)
        {
            foreach (var button in _buttons)
            {
                button?.UnregisterCallback<ClickEvent>(OnButtonClicked);
            }

            UnregisterCallback<DetachFromPanelEvent>(CleanUp);
        }
        
        private static Button CreateTextButton(string text)
        {
            return new Button { text = text };
        }

        private static Button CreateIconButton(Sprite sprite)
        {
            var btn = new Button();

            if (sprite != null)
                btn.iconImage = sprite.texture;

            return btn;
        }

        private void OnButtonClicked(ClickEvent evt)
        {
            var button = evt.target as Button;
            for (var i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != button) continue;
                value = i;
                return;
            }
        }

        private void SetFirstAvailableValue()
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                var button = _buttons[i];
                if (button.ClassListContainsAny(new string[] { "disabled", "hidden", "hidden-and-collapsed" }))
                {
                    continue;
                }

                value = i;
                return;
            }
            throw new ArgumentException("Must have at least one button available");
        }

        private void UpdateVisualState()
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                if (i == _value) _buttons[i].AddToClassList("checked");
                else _buttons[i].RemoveFromClassList("checked");
            }
        }
    }
}
