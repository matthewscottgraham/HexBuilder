using System.Linq;
using App.Events;
using App.Services;
using App.UIComponents;
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
        private RadioBar _toolSelector;
        private RadioBar _toolModeSelector;
        
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

            var modeIcons = new []
            {
                Resources.Load<Sprite>("Sprites/toggle"),
                Resources.Load<Sprite>("Sprites/add"),
                Resources.Load<Sprite>("Sprites/subtract")
            };
            _toolModeSelector = menuBarController.RegisterCustomElement<RadioBar>(new RadioBar(modeIcons));
            _toolModeSelector.RegisterValueChangedCallback(HandleToolModeChanged);
            
            var spacer = new VisualElement();
            spacer.AddToClassList("spacer");
            menuBarController.RegisterCustomElement<VisualElement>(spacer);

            var toolIcons = _toolController.Tools.Select(tool => tool.Icon).ToArray();
            _toolSelector = menuBarController.RegisterCustomElement<RadioBar>(new RadioBar(toolIcons));
            _toolSelector.RegisterValueChangedCallback(HandleToolChanged);

            SetModeButtonsVisibility();
            
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void SetModeButtonsVisibility()
        {
            var availableModes = _toolController.CurrentTool.GetModes();
            if (availableModes == null || availableModes.Length == 0)
            {
                _toolModeSelector.SetVisibility(false);
                return;
            }
            else
            {
                _toolModeSelector.SetVisibility(true);
            }
            
            _toolModeSelector.SetButtonVisibility(0, availableModes.Contains(ToolMode.Toggle));
            _toolModeSelector.SetButtonVisibility(1, availableModes.Contains(ToolMode.Add));
            _toolModeSelector.SetButtonVisibility(2, availableModes.Contains(ToolMode.Subtract));

            var currentMode = _toolController.CurrentTool.CurrentMode;
            _toolModeSelector.SetValueWithoutNotify((int)currentMode);
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

        private void HandleToolChanged(ChangeEvent<int> evt)
        {
            var toolIndex = evt.newValue;
            _toolController.SetActiveTool(toolIndex);
            _radiusSlider.visible = _toolController.CurrentTool.UseRadius;
            SetModeButtonsVisibility();
        }

        private void HandleToolModeChanged(ChangeEvent<int> evt)
        {
            var modeIndex = evt.newValue;
            _toolController.SetToolMode((ToolMode)modeIndex);
        }
        
        private void HandlePauseEvent()
        {
            _radiusSlider.SetEnabled(false);
            _toolModeSelector.SetEnabled(false);
            _toolSelector.SetEnabled(false);
        }

        private void HandleResumeEvent()
        {
            _radiusSlider.SetEnabled(true);
            _toolModeSelector.SetEnabled(true);
            _toolSelector.SetEnabled(true);
        }
    }
}