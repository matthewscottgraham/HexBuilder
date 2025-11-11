using App.SaveData;
using App.Services;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.UIElements;

namespace Game.Menu
{
    public class SaveGameChooser : VisualElement
    {
        private bool _isNewGame;
        public SaveGameChooser(bool isNewGame)
        {
            CreateHeader();
            // TODO: the max save slots should be added to the config
            const int saveSlotCount = 3;
            for (var i = 0; i < saveSlotCount; i++)
            {
                var index = i;
                CreateSlotButton(index);
            }
        }

        private void CreateHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("header-bar");
            Add(header);

            var label = new Label(_isNewGame ? "Choose Slot for New Game" : "Choose Game to Load");
            label.AddToClassList("header-bar-label");
            header.Add(label);
            
            var spacer = new VisualElement();
            spacer.AddToClassList("spacer");
            header.Add(spacer);

            var cancelButton = new Button();
            cancelButton.AddToClassList("cancel-button");
            cancelButton.text = "Cancel";
            header.Add(cancelButton);
        }

        private void CreateSlotButton(int index)
        {
            var saveData = ServiceLocator.Instance.Get<SaveDataController>().GetMetaData(index);
            
            if (!saveData.HasValue)
            {
                CreateSlotButton(index, null, null);
            }
            else
            {
                CreateSlotButton(index, saveData.Value.IconPath, saveData.Value.SaveTime.ToString());
            }
        }

        private void CreateSlotButton(int index, string iconPath, string labelText)
        {
            var saveSlot = new Button();
            Add(saveSlot);
            saveSlot.AddToClassList("save-slot");

            var icon = new Image();
            icon.image = null; // TODO Load Image from save file
            icon.AddToClassList("save-icon");
            saveSlot.Add(icon);
            
            if (string.IsNullOrEmpty(labelText)) labelText = "New Game";
            var infoLabel = new Label(labelText);
            infoLabel.AddToClassList("save-info-label");
            saveSlot.Add(infoLabel);
        }
    }
}