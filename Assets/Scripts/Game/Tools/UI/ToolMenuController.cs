using App.Events;
using App.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tools.UI
{
    public class ToolMenuController : MonoBehaviour
    {
        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;
        
        private VisualElement _mainContainer;
        public VisualElement ToolsContainer {get; private set;}
        public VisualElement ToolOptionsContainer {get; private set;}
        public VisualElement ModelsContainer {get; private set;}
        
        private void Awake()
        {
            var uiDocument = GetComponentInParent<UIDocument>();
            var root = uiDocument.rootVisualElement;
            _mainContainer = root.AddNew<VisualElement>(new VisualElement(), "tool-bar");
            ToolsContainer = _mainContainer.AddNew<VisualElement>(new VisualElement(), "menu-bar-header");
            ToolOptionsContainer = _mainContainer.AddNew<VisualElement>(new VisualElement(),"tool-bar-content");
            ModelsContainer = _mainContainer.AddNew<VisualElement>(new VisualElement(), "model-shelf-content");
            
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
        
        private void HandlePauseEvent()
        {
            _mainContainer.SetEnabled(false);
        }

        private void HandleResumeEvent()
        {
            _mainContainer.SetEnabled(true);
        }
    }
}
