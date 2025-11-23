using System;
using System.Collections;
using App.Events;
using App.Services;
using Game.Cameras;
using Game.Features;
using Game.Grid;
using Game.Hexes;
using Game.Tools;
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
            var hexSelector = new GameObject("Selector").AddComponent<HexSelector>();
            
            _resources = new IDisposable[]
            {
                featureFactory,
                hexSelector, 
                hexController, 
                toolController, 
                new CameraController(Camera.main)
            };

            foreach (var resource in _resources)
            {
                ServiceLocator.Instance.Register(resource);    
            }
            
            hexController.Initialize();
            toolController.Initialize();
            hexSelector.Initialize();

            ground.transform.localScale = new Vector3(grid.WorldWidth() + 3, 1, grid.WorldHeight() + 3);
        }

        private void HandleGameExit(GameExitEvent gameExitEvent)
        {
            StartCoroutine(ExitGame());
        }

        private IEnumerator ExitGame()
        {
            Debug.Log("Exiting Game");
            yield return new WaitForEndOfFrame();
            
            EventBus<GameExitEvent>.Register(_gameExitEventBinding);
            
            ServiceLocator.Instance.Deregister(typeof(HexGrid));
            ServiceLocator.Instance.Deregister(typeof(FeatureFactory));

            for (var i = 0; i < _resources.Length; i++)
            {
                ServiceLocator.Instance.Deregister(_resources[i]);
                _resources[i].Dispose();
            }

            ServiceLocator.Instance.Deregister(this);
            
            EventBus<AppExitEvent>.Raise(new AppExitEvent());
        }
    }
}