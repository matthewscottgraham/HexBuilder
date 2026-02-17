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
        private ToggleButtonGroup _toolButtonGroup;
        private ToggleButtonGroup _toolModeButtonGroup;
        private Button _toggleModeButton;
        private Button _addModeButton;
        
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
            
            _toolModeButtonGroup = menuBarController.RegisterCustomElement<ToggleButtonGroup>(new ToggleButtonGroup());
            _toggleModeButton = _toolModeButtonGroup.AddNew(new Button());
            _toggleModeButton.iconImage = Resources.Load<Sprite>("Sprites/toggle").texture;
            _addModeButton = _toolModeButtonGroup.AddNew(new Button());
            _addModeButton.iconImage = Resources.Load<Sprite>("Sprites/add").texture;
            var subtractButton = _toolModeButtonGroup.AddNew(new Button());
            subtractButton.iconImage = Resources.Load<Sprite>("Sprites/subtract").texture;
            SetToggleButtonVisibility();
            _toolModeButtonGroup.RegisterValueChangedCallback(HandleToolModeChanged);

            var spacer = new VisualElement();
            spacer.AddToClassList("spacer");
            
            menuBarController.RegisterCustomElement<VisualElement>(spacer);
            
            _toolButtonGroup = menuBarController.RegisterCustomElement<ToggleButtonGroup>(new ToggleButtonGroup());
            
            foreach (var tool in _toolController.Tools)
            {
                var button = _toolButtonGroup.AddNew(new Button());
                button.iconImage = tool.Icon?.texture;
            }
            
            _toolButtonGroup.RegisterValueChangedCallback(HandleToolChanged);
            
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void SetToggleButtonVisibility()
        {
            var isVisible = _toolController.CurrentTool.UseToggleMode;
            _toggleModeButton.SetVisibility(isVisible);
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
            _toolController.SetActiveTool(toolIndex[0]);
            _radiusSlider.visible = _toolController.CurrentTool.UseRadius;
            _toggleModeButton.SetVisibility(_toolController.CurrentTool.UseToggleMode);
        }

        private void HandleToolModeChanged(ChangeEvent<ToggleButtonGroupState> evt)
        {
            var modeIndex = evt.newValue.GetActiveOptions(stackalloc int[evt.newValue.length]);
            _toolController.SetToolMode((ToolMode)modeIndex[0]);
            _toolModeButtonGroup.SetVisibility(_toolController.CurrentTool.UseMode);
        }
        
        private void HandlePauseEvent()
        {
            _toolButtonGroup.SetEnabled(false);
            _radiusSlider.SetEnabled(false);
        }

        private void HandleResumeEvent()
        {
            _toolButtonGroup.SetEnabled(true);
            _radiusSlider.SetEnabled(true);
        }
    }
}