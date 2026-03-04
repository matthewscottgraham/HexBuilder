using App.Events;
using App.Services;
using App.Utils;
using Game.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools.UI
{
    public class ToolOptionsController : MonoBehaviour
    {
        private EventBinding<SelectToolEvent> _selectToolEventBinding;
        private VisualElement _container;
        private SliderInt _radiusSlider;
        private ToolController _toolController;

        private void Start()
        {
            _toolController = ServiceLocator.Instance.Get<ToolController>();
            
            var toolMenuController = GetComponent<ToolMenuController>();
            _container = toolMenuController.ToolOptionsContainer;
            _radiusSlider = _container.AddNew(new SliderInt());
            _radiusSlider.label = "Radius";
            _radiusSlider.lowValue = 0;
            _radiusSlider.highValue = 2;
            _radiusSlider.RegisterValueChangedCallback(HandleRadiusChanged);
            
            _selectToolEventBinding = new EventBinding<SelectToolEvent>(HandleSelectToolEvent);
            EventBus<SelectToolEvent>.Register(_selectToolEventBinding);
        }

        private void OnDestroy()
        {
            EventBus<SelectToolEvent>.Deregister(_selectToolEventBinding);
        }
        
        private void HandleRadiusChanged(ChangeEvent<int> evt)
        {
            _toolController.SetToolRadius(evt.newValue);
        }
        
        private void HandleSelectToolEvent(SelectToolEvent evt)
        {
            if (_toolController.CurrentTool.UseRadius)
                _container.RemoveFromClassList("model-shelf__closed");
            else
                _container.AddToClassList("model-shelf__closed"); 
        }
    }
}