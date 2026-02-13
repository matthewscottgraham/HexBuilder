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
        private const int SaveSlotCount = 3; // TODO: the max save slots should be added to the config
        private readonly bool _isNewGame;
        private readonly Dictionary<MapType, Button> _mapTypeButtons = new();
        
        public SaveGameChooser(bool isNewGame)
        {
            _isNewGame = isNewGame;
            
            CreateHeader();
            CreateMapChooser();
            CreateSlots();
        }

        private void CreateMapChooser()
        {
            var container = this.AddNew<VisualElement>(new VisualElement(), "save-slot-container");
            
            container.AddNew(new Label("Map Type: "));
            
            for (var i = 0; i < Enum.GetValues(typeof(MapType)).Length; i++)
            {
                var mapType = (MapType)i;
                var button = container.AddButton(Enum.GetName(typeof(MapType), i), () => HandleMapTypeChanged(mapType));
                _mapTypeButtons.Add(mapType, button);
            }
            HandleMapTypeChanged(_mapType);
        }

        private void CreateSlots()
        {
            var slotContainer = this.AddNew<VisualElement>(new VisualElement(), "save-slot-container");
            for (var i = 0; i < SaveSlotCount; i++)
            {
                var index = i;
                CreateSlotButton(slotContainer, index);
            }
        }

        private void CreateHeader()
        {
            var header = this.AddNew<VisualElement>(new VisualElement(), "header-bar");

            var labelText = _isNewGame ? "Choose Slot for New Game" : "Choose Game to Load";
            header.AddNew<Label>(new Label(labelText), "header-bar-label");

            header.AddSpacer();

            var cancelButton = header.AddNew<Button>(new Button(CloseWindow), "exit-button");
            cancelButton.text = "X";
        }

        private void CreateSlotButton(VisualElement parentContainer, int index)
        {
            var saveData = ServiceLocator.Instance.Get<SaveDataController>().GetMetaData(index);
            parentContainer.Add(CreateSlotButton(index, saveData.Item1, saveData.Item2));
        }

        private VisualElement CreateSlotButton(int index, Texture2D screenshot, string labelText)
        {
            var saveSlot = new Button();
            saveSlot.clicked += () => HandleSlotClicked(index);
            saveSlot.AddToClassList("save-slot");

            var icon = saveSlot.AddNew<Image>(new Image(), "save-icon");
            icon.image = screenshot;

            labelText = string.IsNullOrEmpty(labelText) ? $"Slot {index + 1} - New Game" : $"Slot {index + 1} - {labelText}";
            saveSlot.AddNew<Label>(new Label(labelText), "save-info-label");

            return  saveSlot;
        }

        private void HandleMapTypeChanged(MapType mapType)
        {
            _mapType = mapType;
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

        private void HandleSlotClicked(int index)
        {
            if (ConfigController.CurrentSaveSlot != index)
                ServiceLocator.Instance.Get<HexController>().SaveData();

            if (_isNewGame)
            {
                ServiceLocator.Instance.Get<SaveDataController>().DeleteSaveData(index);
                HexController.SetNewMapType(_mapType);
            }

            ConfigController.CurrentSaveSlot = index;
            
            CloseWindow();
            EventBus<GameReloadEvent>.Raise(new GameReloadEvent());
        }
    }
}