using System.IO;
using App.SaveData;
using UnityEngine;

namespace App.Config
{
    public class ConfigController : FileDataController
    {
        public ConfigController()
        {
            var saveData = Load<ConfigData>(Path.Combine(SaveDirectoryName));
            
            if (saveData != null)
            {
                CurrentSaveSlot = saveData.Value.SaveID;
                Config = saveData.Value.Data;
            }
            else
            {
                Config = CreateNewConfig();
                Save(SaveDirectoryName, Config, CurrentSaveSlot);
            }
        }

        public static int CurrentSaveSlot { get; set; }
        public ConfigData Config { get; private set; }

        private ConfigData CreateNewConfig()
        {
            return new ConfigData()
            {
                IsFullScreen = Screen.fullScreen,
                ScreenWidth = Screen.width,
                ScreenHeight = Screen.height
            };
        }
        
    }
}