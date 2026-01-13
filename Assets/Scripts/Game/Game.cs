using System;
using System.Collections;
using System.Linq;
using App.Events;
using App.Scenes;
using App.Services;
using App.Utils;
using Game.Cameras;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using Game.Tools;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GridPreset gridPreset;

        private EventBinding<GameReloadEvent> _gameReloadEventBinding;
        private EventBinding<GameExitEvent> _gameExitEventBinding;
        private IDisposable[] _resources;

        private void Awake()
        {
            if (ServiceLocator.Instance == null) return;
            Initialize();
        }

        private void Initialize()
        {
            _gameReloadEventBinding = new EventBinding<GameReloadEvent>(HandleGameReload);
            EventBus<GameReloadEvent>.Register(_gameReloadEventBinding);
            
            _gameExitEventBinding = new EventBinding<GameExitEvent>(HandleGameExit);
            EventBus<GameExitEvent>.Register(_gameExitEventBinding);

            ServiceLocator.Instance.Register(this);
            
            ServiceLocator.Instance.Register(gridPreset.CreateGrid());

            var featureFactory = new FeatureFactory();

            var hexController = gameObject.AddChild<HexController>("Hexes");
            var toolController = gameObject.AddChild<ToolController>("Tools");

            _resources = new IDisposable[]
            {
                featureFactory,
                hexController,
                toolController,
                new CameraController(Camera.main)
            };

            foreach (var resource in _resources) ServiceLocator.Instance.Register(resource);
            
            hexController.Initialize();
            toolController.Initialize();
        }

        private void HandleGameReload(GameReloadEvent gameReloadEvent)
        {
            StartCoroutine(RestartGame());
        }

        private void HandleGameExit(GameExitEvent gameExitEvent)
        {
            StartCoroutine(ExitGame());
        }

        private IEnumerator ExitGame()
        {
            Debug.Log("Exiting Game");
            yield return new WaitForEndOfFrame();
            ServiceLocator.Instance.Get<HexController>().SaveData();
            ReleaseResources();
            EventBus<AppExitEvent>.Raise(new AppExitEvent());
        }

        private IEnumerator RestartGame()
        {
            Debug.Log("Restarting Game");
            yield return new WaitForEndOfFrame();
            ReleaseResources();
            ServiceLocator.Instance.Get<SceneController>().LoadGameScene();
        }

        private void ReleaseResources()
        {
            EventBus<GameReloadEvent>.Deregister(_gameReloadEventBinding);            
            EventBus<GameExitEvent>.Deregister(_gameExitEventBinding);

            foreach (var t in _resources.ToList())
            {
                ServiceLocator.Instance.Deregister(t);
                t.Dispose();
            }
            ServiceLocator.Instance.Deregister(typeof(HexGrid));
            ServiceLocator.Instance.Deregister(this);
        }
    }
}