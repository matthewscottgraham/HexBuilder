using System;
using App.Events;
using App.Services;
using UnityEngine;

namespace App.Config
{
    public class ConfigController : IDisposable
    {
        private const string ConfigFileName = "config";
        private const string ConfigDirectoryName = "UserData";
        
        public Config Config { get; private set; }

        public ConfigController()
        {
            LoadConfig();
        }

        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(this);
        }

        private void LoadConfig()
        {
            ServiceLocator.Instance.Get<EventBus<FileLoadEvent>>().Raise(new FileLoadEvent());
            var config =  ServiceLocator.Instance.Get<IOController>()
                .ReadJson<Config>(ConfigDirectoryName, ConfigFileName);

            if (!config.HasValue)
            {
                Config = CreateNewConfig();
                SaveConfig(Config);
            }
            else
            {
                Config = config.Value;
            }
        }

        private static Config CreateNewConfig()
        {
            var config = new Config
            {
                AppName = Application.productName,
                AppVersion = Application.version,
                IsFullScreen = Screen.fullScreen,
                ScreenWidth = Screen.width,
                ScreenHeight = Screen.height,
            };
            return config;
        }

        private static async void SaveConfig(Config config)
        {
            ServiceLocator.Instance.Get<EventBus<FileSaveEvent>>().Raise(new FileSaveEvent());
            await ServiceLocator.Instance.Get<IOController>()
                .WriteJson(config, ConfigDirectoryName, ConfigFileName);
        }
    }
}