using System;
using System.Linq;
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
        private const int SaveSlotCount = 3; // TODO: the max save slots should be added to the config
        private readonly bool _isNewGame;
        private MapType _mapType = MapType.Random;

        public SaveGameChooser(bool isNewGame)
        {
            _isNewGame = isNewGame;
            
            CreateHeader();
            CreateSlots();
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
            
            var dropdownField = header.AddNew<DropdownField>(new DropdownField(), "map-type-selector");
            dropdownField.label = "Map Type";
            dropdownField.choices = Enum.GetNames(typeof(MapType)).ToList();
            dropdownField.value = _mapType.ToString();
            dropdownField.RegisterValueChangedCallback((evt)=>HandleMapTypeChanged(evt.newValue));

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

            if (string.IsNullOrEmpty(labelText)) labelText = "New Game";
            saveSlot.AddNew<Label>(new Label(labelText), "save-info-label");

            return  saveSlot;
        }

        private void HandleMapTypeChanged(string mapTypeName)
        {
            _mapType = Enum.Parse<MapType>(mapTypeName);
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