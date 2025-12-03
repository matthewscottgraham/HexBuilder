using System;
using System.Collections;
using System.Linq;
using App.Events;
using App.Services;
using Game.Cameras;
using Game.Features;
using Game.Grid;
using Game.Hexes;
using Game.Tools;
using Game.Tools.Paths;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GridPreset gridPreset;
        [SerializeField] private Transform ground;

        private EventBinding<GameExitEvent> _gameExitEventBinding;
        private IDisposable[] _resources;

        private void Awake()
        {
            if (ServiceLocator.Instance == null) return;
            Initialize();
        }

        private void Initialize()
        {
            _gameExitEventBinding = new EventBinding<GameExitEvent>(HandleGameExit);
            EventBus<GameExitEvent>.Register(_gameExitEventBinding);

            ServiceLocator.Instance.Register(this);

            var grid = gridPreset.CreateGrid();
            ServiceLocator.Instance.Register(grid);

            var featureFactory = new FeatureFactory();

            var hexController = gameObject.AddComponent<HexController>();
            var toolController = gameObject.AddComponent<ToolController>();
            var pathController = gameObject.AddComponent<PathController>();

            _resources = new IDisposable[]
            {
                featureFactory,
                hexController,
                toolController,
                pathController,
                new CameraController(Camera.main)
            };

            foreach (var resource in _resources) ServiceLocator.Instance.Register(resource);
            
            pathController.Initialize();
            hexController.Initialize();
            toolController.Initialize();

            ground.transform.localScale = new Vector3(grid.WorldWidth + 3, 1, grid.WorldHeight + 3);
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
            
            EventBus<GameExitEvent>.Deregister(_gameExitEventBinding);

            ServiceLocator.Instance.Deregister(typeof(HexGrid));
            ServiceLocator.Instance.Deregister(typeof(FeatureFactory));

            foreach (var t in _resources.ToList())
            {
                ServiceLocator.Instance.Deregister(t);
                t.Dispose();
            }

            ServiceLocator.Instance.Deregister(this);

            EventBus<AppExitEvent>.Raise(new AppExitEvent());
        }
    }
}