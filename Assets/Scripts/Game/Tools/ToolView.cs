using System;
using App.Events;
using App.Services;
using App.Utils;
using Game.Menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools
{
    public class ToolView : MonoBehaviour
    {
        private ToolController _toolController;
        private SliderInt _radiusSlider;
        private ToggleButtonGroup _buttonGroup;

        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;
        
        private void Start()
        {
            _toolController = ServiceLocator.Instance.Get<ToolController>();
            var menuBarController = GetComponent<MenuBarController>();
            
            _radiusSlider = menuBarController.RegisterCustomElement<SliderInt>(new SliderInt());
            _radiusSlider.lowValue = 0;
            _radiusSlider.highValue = 2;
            _radiusSlider.RegisterValueChangedCallback(HandleRadiusChanged);
            
            _buttonGroup = menuBarController.RegisterCustomElement<ToggleButtonGroup>(new ToggleButtonGroup());
            
            foreach (var tool in _toolController.Tools)
            {
                var button = _buttonGroup.AddNew(new Button());
                button.iconImage = tool.Icon?.texture;
            }
            
            _buttonGroup.RegisterValueChangedCallback(HandleToolChanged);
            
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