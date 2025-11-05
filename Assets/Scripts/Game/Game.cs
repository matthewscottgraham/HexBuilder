using App;
using App.Events;
using App.Scenes;
using App.Services;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        private EventBinding<SceneLoadedEvent> _sceneLoadedEventBinding;
        private EventBinding<SceneUnloadedEvent> _sceneUnloadedEventBinding;
        private EventBinding<MoveEvent> _moveEventBinding;
        private EventBinding<InteractEvent> _interactEventBinding;
        
        private void Start()
        {
            ServiceLocator.Instance.Register(this);
            
            _sceneLoadedEventBinding = new EventBinding<SceneLoadedEvent>(HandleSceneLoaded);
            _sceneUnloadedEventBinding = new EventBinding<SceneUnloadedEvent>(HandleSceneUnLoaded);

            _moveEventBinding = new EventBinding<MoveEvent>(HandleMoveEvent);
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            
            ServiceLocator.Instance.Get<EventBus<SceneLoadedEvent>>().Register(_sceneLoadedEventBinding);
            ServiceLocator.Instance.Get<EventBus<SceneUnloadedEvent>>().Register(_sceneUnloadedEventBinding);
            
            ServiceLocator.Instance.Get<EventBus<MoveEvent>>().Register(_moveEventBinding);
            ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Register(_interactEventBinding);
            
            ServiceLocator.Instance.Get<SceneController>().LoadScene("Game", false);
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
            {
                ServiceLocator.Instance.Deregister(this);
                ServiceLocator.Instance.Get<EventBus<SceneLoadedEvent>>().Deregister(_sceneLoadedEventBinding);
                ServiceLocator.Instance.Get<EventBus<SceneUnloadedEvent>>().Deregister(_sceneUnloadedEventBinding);
                ServiceLocator.Instance.Get<EventBus<MoveEvent>>().Deregister(_moveEventBinding);
                ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Deregister(_interactEventBinding);
            }

            _sceneLoadedEventBinding = null;
            _sceneUnloadedEventBinding = null;
        }

        private void HandleSceneLoaded(SceneLoadedEvent arg)
        {
            Debug.Log("Loaded: " + arg.SceneName);
        }

        private void HandleSceneUnLoaded(SceneUnloadedEvent arg)
        {
            Debug.Log("Unloaded: " + arg.SceneName);
        }

        private void HandleMoveEvent(MoveEvent arg)
        {
            Debug.Log("Moved: " + arg.Delta);
        }

        private void HandleInteractEvent()
        {
            Debug.Log("Interact");
        }
    }
}