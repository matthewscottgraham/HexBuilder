using System;
using System.Collections;
using App.Config;
using App.Events;
using App.Input;
using App.SaveData;
using App.Scenes;
using App.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace App
{
    public class Main : MonoBehaviour
    {
        private const string MainSceneName = "App";
        private EventBinding<AppExitEvent> _exitEventBinding;
        private IDisposable[] _resources;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
            if (SceneManager.GetSceneByName(MainSceneName).IsValid()) return;
            SceneManager.LoadScene(MainSceneName);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _exitEventBinding = new EventBinding<AppExitEvent>(HandleAppExit);
            EventBus<AppExitEvent>.Register(_exitEventBinding);

            var serviceLocator = new ServiceLocator();
            serviceLocator.Register(new IOController());
            
            var inputController = gameObject.AddComponent<InputController>();
            
            _resources = new IDisposable[]
            {
                new SceneController(),
                new ConfigController(),
                new SaveDataController(),
                inputController,
                serviceLocator
            };
            
            inputController.Initialize();

            foreach (var resource in _resources)
            {
                if (resource.GetType() == typeof(ServiceLocator)) continue;
                serviceLocator.Register(resource);
            }

            ServiceLocator.Instance.Get<SceneController>().LoadGameScene();
        }

        private void HandleAppExit(AppExitEvent exitEvent)
        {
            StartCoroutine(ExitApp());
        }

        private IEnumerator ExitApp()
        {
            Debug.Log("Exiting App");
            yield return new WaitForEndOfFrame();
            
            EventBus<AppExitEvent>.Deregister(_exitEventBinding);

            for (var i = 0; i < _resources.Length; i++)
            {
                if (_resources[i].GetType() != typeof(ServiceLocator))
                {
                    ServiceLocator.Instance.Deregister(_resources[i]);
                }
                _resources[i].Dispose();
            }
            
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}