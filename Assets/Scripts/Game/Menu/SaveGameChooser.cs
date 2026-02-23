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
            CreateHeader();
            _contentContainer = this.AddNew<VisualElement>(new VisualElement(), "new-game-container");
            CreateMapChooser();
            CreateSingleSlot();
            HandleMapTypeChanged(_mapType);
        }

        private void CreateMapChooser()
        {
            var container = _contentContainer.AddNew<VisualElement>(new VisualElement(), "map-chooser-container");
            container.AddNew(new Label("Map Type: "));
            
            for (var i = 0; i < Enum.GetValues(typeof(MapType)).Length; i++)
            {
                var mapType = (MapType)i;
                var button = container.AddButton(Enum.GetName(typeof(MapType), i), () => HandleMapTypeChanged(mapType));
                _mapTypeButtons.Add(mapType, button);
            }
        }

        private void SetMapTypeImage()
        {
            _mapTypeImage.image = Resources.Load<Texture2D>("MapPreviews/" + _mapType);
        }

        private void CreateSingleSlot()
        {
            var slotContainer = _contentContainer.AddNew<VisualElement>(new VisualElement(), "save-slot-container");
            _mapTypeImage = slotContainer.AddNew<Image>(new Image(), "save-slot-image");
            var button = slotContainer.AddButton("Create Map", CreateNewMap);
        }

        private void CreateHeader()
        {
            var header = this.AddNew<VisualElement>(new VisualElement(), "header-bar");
            header.AddNew<Label>(new Label("Choose Map Type for New Game"), "header-bar-label");
            header.AddSpacer();

            var cancelButton = header.AddNew<Button>(new Button(CloseWindow), "exit-button");
            cancelButton.text = "X";
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