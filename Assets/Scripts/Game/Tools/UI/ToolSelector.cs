using System.Linq;
using App.Events;
using App.Services;
using App.UIComponents;
using App.Utils;
using Game.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools.UI
{
    public class ToolSelector : MonoBehaviour
    {
        private EventBinding<SelectToolEvent> _selectToolEventBinding;
        private ToolController _toolController;
        private RadioBar _toolSelector;
        
        private void Start()
        {
            _toolController = ServiceLocator.Instance.Get<ToolController>();

            var toolIcons = _toolController.Tools.Select(tool => tool.Icon).ToArray();
            var toolMenuController = GetComponent<ToolMenuController>();
            _toolSelector = toolMenuController.ToolsContainer.AddNew(new RadioBar(toolIcons));
            _toolSelector.RegisterValueChangedCallback(HandlePlayerSelectTool);
            
            _selectToolEventBinding = new EventBinding<SelectToolEvent>(HandleSelectToolEvent);
            EventBus<SelectToolEvent>.Register(_selectToolEventBinding);
        }
        
        private void HandlePlayerSelectTool(ChangeEvent<int> evt)
        {
            _toolController.SetActiveTool(evt.newValue);
        }
        
        private void HandleSelectToolEvent(SelectToolEvent evt)
        {
            _toolSelector.SetValueWithoutNotify(_toolController.GetCurrentToolIndex());
        }
        
    }
}