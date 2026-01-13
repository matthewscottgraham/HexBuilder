using App.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Events;
using UnityEngine;

namespace App.SaveData
{
    public class FileDataController : IDisposable
    {
        protected string SaveDirectoryName = "UserData";
        protected const string SaveDataFileName = "data";

        private Dictionary<string, SaveData<object>> _enqueuedSaveData = new();
        
        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(this);
        }

        protected async void EnqueueSave(string relativeSavePath, object objectToSave, int saveId)
        {
            // Allow the enqueued data to be overridden if it has not already been written to disk.
            var saveData = CreateSaveData(saveId, objectToSave);
            if (!_enqueuedSaveData.ContainsKey(relativeSavePath))
            {
                _enqueuedSaveData.Add(relativeSavePath, saveData);
            }
            else
            {
                _enqueuedSaveData[relativeSavePath] = saveData;
            }

            await Save(relativeSavePath);
        }
        
        private async Task Save(string key)
        {
            await Task.Delay(1000);
            if (!_enqueuedSaveData.ContainsKey(key)) return;
            
            var ioController = ServiceLocator.Instance.Get<IOController>();
            var saveData = _enqueuedSaveData[key];
            await ioController.WriteJson(saveData, key, SaveDataFileName);
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