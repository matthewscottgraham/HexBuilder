using System;
using System.Collections.Generic;
using App.Config;
using App.Events;
using App.SaveData;
using App.Services;
using App.Utils;
using Game.Hexes;
using Game.Map;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Menu
{
    public class SaveGameChooser : VisualElement
    {
        private static MapType _mapType = MapType.Random;
        private readonly Dictionary<MapType, Button> _mapTypeButtons = new();
        private VisualElement _contentContainer;
        private Image _mapTypeImage;
        
        public SaveGameChooser()
        {
            _contentContainer = this.AddNew<VisualElement>(new VisualElement(), "new-game-container");
            CreateMapChooser();
            CreateSingleSlot();
            HandleMapTypeChanged(_mapType);
            EventBus<HideUIEvent>.Raise(new HideUIEvent());
        }

        private void CreateMapChooser()
        {
            var container = _contentContainer.AddNew<VisualElement>(new VisualElement(), "map-chooser-container");
            container.AddNew(new Label("New Game"));
            
            for (var i = 0; i < Enum.GetValues(typeof(MapType)).Length; i++)
            {
                var mapType = (MapType)i;
                var button = container.AddButton(Enum.GetName(typeof(MapType), i), () => HandleMapTypeChanged(mapType));
                _mapTypeButtons.Add(mapType, button);
            }
        }

        private void SetMapTypeImage()
        {
            _mapTypeImage.style.backgroundImage = Resources.Load<Texture2D>("MapPreviews/" + _mapType);
        }

        private void CreateSingleSlot()
        {
            var slotContainer = _contentContainer.AddNew<VisualElement>(new VisualElement(), "save-slot-container");
            _mapTypeImage = slotContainer.AddNew<Image>(new Image(), "save-slot-image");
            
            var buttonContainer = slotContainer.AddNew<VisualElement>(new VisualElement(), "save-slot-buttons");
            var cancelButton = buttonContainer.AddNew<Button>(new Button(CloseWindow), "exit-button");
            cancelButton.text = "Cancel";
            buttonContainer.AddNew<VisualElement>(new VisualElement(), "expander");
            var button = buttonContainer.AddButton("Create Map", CreateNewMap);
        }

        private void HandleMapTypeChanged(MapType mapType)
        {
            _mapType = mapType;
            SetMapTypeImage();
            foreach (var pair in _mapTypeButtons)
            {
                if (pair.Key == _mapType) pair.Value.AddToClassList("active-tab");
                else pair.Value.RemoveFromClassList("active-tab");
            }
        }

        private void CloseWindow()
        {
            RemoveFromHierarchy();
            EventBus<ShowUIEvent>.Raise(new ShowUIEvent());
        }

        private void CreateNewMap()
        {
#if !UNITY_WEBGL
            ServiceLocator.Instance.Get<SaveDataController>().DeleteSaveData(0);
#endif

            HexController.SetNewMapType(_mapType);
            ConfigController.CurrentSaveSlot = 0;
            
            CloseWindow();
            EventBus<GameReloadEvent>.Raise(new GameReloadEvent());
        }
    }
}