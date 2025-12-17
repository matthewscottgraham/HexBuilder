using System;
using System.Collections;
using System.Linq;
using App.Audio;
using App.Config;
using App.Events;
using App.Input;
using App.SaveData;
using App.Scenes;
using App.Services;
using App.Tweens;
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

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _exitEventBinding = new EventBinding<AppExitEvent>(HandleAppExit);
            EventBus<AppExitEvent>.Register(_exitEventBinding);

            var serviceLocator = new ServiceLocator();
            serviceLocator.Register(new IOController());

            var inputController = gameObject.AddComponent<InputController>();
            var tweenController = gameObject.AddComponent<TweenController>();
            var audioController = gameObject.AddComponent<AudioController>();

            _resources = new IDisposable[]
            {
                new SceneController(),
                new ConfigController(),
                new SaveDataController(),
                tweenController,
                inputController,
                audioController,
                serviceLocator
            };
            
            audioController.Initialize();
            tweenController.Initialize();
            inputController.Initialize();

            foreach (var resource in _resources)
            {
                if (resource.GetType() == typeof(ServiceLocator)) continue;
                serviceLocator.Register(resource);
            }

            ServiceLocator.Instance.Get<SceneController>().LoadGameScene();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
            if (SceneManager.GetSceneByName(MainSceneName).IsValid()) return;
            SceneManager.LoadScene(MainSceneName);
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

            foreach (var t in _resources.ToList())
            {
                if (t.GetType() != typeof(ServiceLocator)) ServiceLocator.Instance.Deregister(t);
                t.Dispose();
            }

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}