using System.Linq;
using App.Events;
using App.Services;
using App.UIComponents;
using Game.Events;
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
        
        private EventBinding<SelectToolEvent> _selectToolEventBinding;
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
            
            var spacer = new VisualElement();
            spacer.AddToClassList("spacer");
            menuBarController.RegisterCustomElement<VisualElement>(spacer);

            var toolIcons = _toolController.Tools.Select(tool => tool.Icon).ToArray();
            _toolSelector = menuBarController.RegisterCustomElement<RadioBar>(new RadioBar(toolIcons));
            _toolSelector.RegisterValueChangedCallback(HandlePlayerSelectTool);

            _selectToolEventBinding = new EventBinding<SelectToolEvent>(HandleSelectToolEvent);
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            
            EventBus<SelectToolEvent>.Register(_selectToolEventBinding);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void OnDestroy()
        {
            EventBus<SelectToolEvent>.Deregister(_selectToolEventBinding);
            EventBus<GamePauseEvent>.Deregister(_pauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeEventBinding);
        }

        private static void HandleRadiusChanged(ChangeEvent<int> evt)
        {
            ServiceLocator.Instance.Get<ToolController>().SetToolRadius(evt.newValue);
        }

        private void HandlePlayerSelectTool(ChangeEvent<int> evt)
        {
            _toolController.SetActiveTool(evt.newValue);
        }

        private void HandleSelectToolEvent(SelectToolEvent evt)
        {
            _radiusSlider.visible = _toolController.CurrentTool.UseRadius;
        }
        
        private void HandlePauseEvent()
        {
            _radiusSlider.SetEnabled(false);
            _toolSelector.SetEnabled(false);
        }

        private void HandleResumeEvent()
        {
            _radiusSlider.SetEnabled(true);
            _toolSelector.SetEnabled(true);
        }
    }
}