using System;
using System.IO;
using App.Events;
using App.Services;
using UnityEngine;

namespace App.SaveData
{
    public class SaveDataController : IDisposable
    {
        private const string SaveFileName = "saveData";
        private const string SaveDirectoryName = "UserData";
        
        public SaveData Data { get; private set; }

        public SaveDataController()
        {
            LoadConfig(0);
        }

        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(this);
        }

        public SaveData? GetMetaData(int id)
        {
            return ServiceLocator.Instance.Get<IOController>()
                .ReadJson<SaveData>(Path.Combine(SaveDirectoryName, id.ToString()), SaveFileName);
        }

        private void LoadConfig(int id)
        {
            ServiceLocator.Instance.Get<EventBus<FileLoadEvent>>().Raise(new FileLoadEvent());
            var data =  ServiceLocator.Instance.Get<IOController>()
                .ReadJson<SaveData>(Path.Combine(SaveDirectoryName, id.ToString()), SaveFileName);

            if (!data.HasValue)
            {
                Data = CreateNewConfig();
                Save(Data);
            }
            else
            {
                Data = data.Value;
            }
        }

        private static SaveData CreateNewConfig()
        {
            var data = new SaveData()
            {
                AppName = Application.productName,
                AppVersion = Application.version,
                Id = 0,
                SaveTime = DateTime.Now
            };
            return data;
        }

        private static async void Save(SaveData data)
        {
            ServiceLocator.Instance.Get<EventBus<FileSaveEvent>>().Raise(new FileSaveEvent());
            await ServiceLocator.Instance.Get<IOController>()
                .WriteJson(data, Path.Combine(SaveDirectoryName, data.Id.ToString()), SaveFileName);
        }
    }
}