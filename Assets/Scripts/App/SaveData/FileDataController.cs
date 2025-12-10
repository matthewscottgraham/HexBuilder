using App.Services;
using System;
using App.Events;
using UnityEngine;

namespace App.SaveData
{
    public class FileDataController : IDisposable
    {
        protected string SaveDirectoryName = "UserData";
        protected const string SaveDataFileName = "data";
        
        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(this);
        }

        protected static async void Save(string relativeSavePath, object data, int saveId)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            var saveData = CreateSaveData(saveId, data);
            await ioController.WriteJson(saveData, relativeSavePath, SaveDataFileName);
            EventBus<FileSaveEvent>.Raise(new FileSaveEvent());
        }

        protected static SaveData<T>? Load<T>(string relativePath)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            return ioController.ReadJson<SaveData<T>>(relativePath, SaveDataFileName);
        }

        protected static void Delete(string path)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            ioController.DeleteDirectory(path);
        }
        
        private static SaveData<T> CreateSaveData<T>(int saveId, T data)
        {
            var saveData = new SaveData<T>
            {
                SaveID = saveId,
                AppName = Application.productName,
                AppVersion = Application.version,
                SaveTime = DateTime.Now,
                Data = data
            };
            return saveData;
        }
    }
}