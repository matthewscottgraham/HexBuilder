using App.Events;
using App.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools
{
    public class ToolView : MonoBehaviour
    {
        private SliderInt _radiusSlider;
        private ToggleButtonGroup _buttonGroup;
        private UIDocument _document;

        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;
        
        private void Start()
        {
            _document = GetComponent<UIDocument>();

            _buttonGroup = _document.rootVisualElement.Q<ToggleButtonGroup>();
            _buttonGroup.RegisterValueChangedCallback(HandleToolChanged);

            _radiusSlider = _document.rootVisualElement.Q<SliderInt>();
            _radiusSlider.RegisterValueChangedCallback(HandleRadiusChanged);
            
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void OnDestroy()
        {
            EventBus<GamePauseEvent>.Deregister(_pauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeEventBinding);
        }

        private static void HandleRadiusChanged(ChangeEvent<int> evt)
        {
            ServiceLocator.Instance.Get<ToolController>().SetToolRadius(evt.newValue);
        }

        private void HandleToolChanged(ChangeEvent<ToggleButtonGroupState> evt)
        {
            var toolIndex = evt.newValue.GetActiveOptions(stackalloc int[evt.newValue.length]);
            var toolController = ServiceLocator.Instance.Get<ToolController>();
            toolController.SetActiveTool(toolIndex[0]);

            _radiusSlider.visible = toolController.CurrentTool.UseRadius;
        }
        
        private void HandlePauseEvent()
        {
            _buttonGroup.SetEnabled(false);
            _radiusSlider.SetEnabled(false);
        }

        private void HandleResumeEvent()
        {
            _buttonGroup.SetEnabled(true);
            _radiusSlider.SetEnabled(true);
        }
    }
}