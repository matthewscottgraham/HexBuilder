using App.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools
{
    public class ToolView : MonoBehaviour
    {
        private UIDocument _document;
        private ToggleButtonGroup _buttonGroup;

        private void Start()
        {
            _document = GetComponent<UIDocument>();
            _buttonGroup = _document.rootVisualElement.Q<ToggleButtonGroup>();
            _buttonGroup.RegisterValueChangedCallback(HandleToolChanged);
        }

        private static void HandleToolChanged(ChangeEvent<ToggleButtonGroupState> evt)
        {
            var toolIndex = evt.newValue.GetActiveOptions(stackalloc int[evt.newValue.length]);
            ServiceLocator.Instance.Get<ToolController>().SetActiveTool(toolIndex[0]);
        }
    }
}
