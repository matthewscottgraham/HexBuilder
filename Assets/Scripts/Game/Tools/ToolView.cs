using App.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools
{
    public class ToolView : MonoBehaviour
    {
        private ToggleButtonGroup _buttonGroup;
        private SliderInt _aoeSlider;
        private UIDocument _document;

        private void Start()
        {
            _document = GetComponent<UIDocument>();
            
            _buttonGroup = _document.rootVisualElement.Q<ToggleButtonGroup>();
            _buttonGroup.RegisterValueChangedCallback(HandleToolChanged);
            
            _aoeSlider = _document.rootVisualElement.Q<SliderInt>();
            _aoeSlider.RegisterValueChangedCallback(HandleAreaOfEffectChanged);
        }

        private void HandleAreaOfEffectChanged(ChangeEvent<int> evt)
        {
            ServiceLocator.Instance.Get<ToolController>().SetAreaOfEffect(evt.newValue);
        }

        private static void HandleToolChanged(ChangeEvent<ToggleButtonGroupState> evt)
        {
            var toolIndex = evt.newValue.GetActiveOptions(stackalloc int[evt.newValue.length]);
            ServiceLocator.Instance.Get<ToolController>().SetActiveTool(toolIndex[0]);
        }
    }
}