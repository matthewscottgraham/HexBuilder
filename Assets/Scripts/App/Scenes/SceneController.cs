using System;
using App.Events;
using App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Scenes
{
    public class SceneController : IDisposable
    {
        private const string GameSceneName = "Game";
        private readonly HashSet<string> _loadedScenes = new();
        
        private EventBinding<SceneLoadedEvent> _sceneLoaded;
        private EventBinding<SceneUnloadedEvent> _sceneUnloaded;
        
        public SceneController()
        {
            
        }

        public void Dispose()
        {
            _loadedScenes.Clear();
            ServiceLocator.Instance.Deregister(this);
        }

        public async void LoadGameScene()
        {
            await UnloadSceneAsync(GameSceneName);
            await LoadSceneAsync(GameSceneName, true);
        }

        public async Task LoadSceneAsync(string sceneName, bool isAdditive)
        {
            if (_loadedScenes.Contains(sceneName)) return;
            if (!isAdditive) _loadedScenes.Clear();
            _loadedScenes.Add(sceneName);
            await SceneManager.LoadSceneAsync(sceneName, isAdditive? LoadSceneMode.Additive : LoadSceneMode.Single);
            EventBus<SceneLoadedEvent>.Raise(new SceneLoadedEvent(sceneName));
        }

        public async Task UnloadSceneAsync(string sceneName)
        {
            if (!_loadedScenes.Contains(sceneName)) return;
            _loadedScenes.Remove(sceneName);
            await SceneManager.UnloadSceneAsync(sceneName);
            EventBus<SceneUnloadedEvent>.Raise(new SceneUnloadedEvent(sceneName));
        }
    }
}