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
                EnqueueSave(SaveDirectoryName, Config, CurrentSaveSlot);
            }
        }

        public static int CurrentSaveSlot { get; set; }
        public ConfigData Config { get; private set; }

        public void SetConfig(ConfigData config)
        {
            Config = config;
            EnqueueSave(SaveDirectoryName, Config, CurrentSaveSlot);
        }

        private static ConfigData CreateNewConfig()
        {
            return new ConfigData()
            {
                MusicVolume = 0.3f,
                SfxVolume = 0.5f,
                IsFullScreen = Screen.fullScreen,
                ScreenWidth = Screen.width,
                ScreenHeight = Screen.height
            };
        }
        
    }
}