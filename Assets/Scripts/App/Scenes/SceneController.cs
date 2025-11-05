using System;
using App.Events;
using App.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Scenes
{
    public class SceneController : IDisposable
    {
        private readonly HashSet<string> _loadedScenes = new();
        
        private EventBinding<SceneLoadedEvent> _sceneLoaded;
        private EventBinding<SceneUnloadedEvent> _sceneUnloaded;
        
        public SceneController()
        {
            ServiceLocator.Instance.Register(new EventBus<SceneLoadedEvent>());
            ServiceLocator.Instance.Register(new EventBus<SceneUnloadedEvent>());
        }
        
        public void LoadScene(string sceneName, bool isAdditive = true)
        {
            if (_loadedScenes.Contains(sceneName)) return;
            LoadSceneAsync(sceneName, isAdditive);
        }

        public void UnloadScene(string sceneName)
        {
            if (!_loadedScenes.Contains(sceneName)) return;
            UnloadSceneAsync(sceneName);
        }

        public void Dispose()
        {
            _loadedScenes.Clear();
            ServiceLocator.Instance.Deregister(this);
            ServiceLocator.Instance.Deregister(typeof(EventBus<SceneLoadedEvent>));
            ServiceLocator.Instance.Deregister(typeof(EventBus<SceneUnloadedEvent>));
        }

        private async void LoadSceneAsync(string sceneName, bool isAdditive)
        {
            if (!isAdditive) _loadedScenes.Clear();
            _loadedScenes.Add(sceneName);
            await SceneManager.LoadSceneAsync(sceneName, isAdditive? LoadSceneMode.Additive : LoadSceneMode.Single);
            ServiceLocator.Instance.Get<EventBus<SceneLoadedEvent>>().Raise(new SceneLoadedEvent(sceneName));
        }

        private async void UnloadSceneAsync(string sceneName)
        {
            _loadedScenes.Remove(sceneName);
            await SceneManager.UnloadSceneAsync(sceneName);
            ServiceLocator.Instance.Get<EventBus<SceneUnloadedEvent>>().Raise(new SceneUnloadedEvent(sceneName));
        }
    }
}