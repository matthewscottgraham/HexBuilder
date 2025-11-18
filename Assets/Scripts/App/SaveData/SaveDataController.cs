#nullable enable
using App.Config;
using App.Events;
using App.Services;
using System;
using System.IO;
using UnityEngine;

namespace App.SaveData
{
    public class SaveDataController : IDisposable
    {
        private const string SaveDataFileName = "gameData";
        private const string SaveImageFileName = "gameScreenshot";
        private const string SaveDirectoryName = "UserData";

        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(this);
        }

        public (Texture2D, string) GetMetaData(int saveID)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            Texture2D tex = ioController.LoadPng(Path.Combine(SaveDirectoryName, saveID.ToString()), SaveImageFileName);
            var saveTime = ioController.GetFileSaveTime(Path.Combine(SaveDirectoryName, saveID.ToString()), SaveDataFileName);
            return (tex, saveTime);
        }

        public void DeleteSaveData(int saveId)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            ioController.DeleteDirectory(Path.Combine(SaveDirectoryName, saveId.ToString()));
        }
        
        public async void Save(MonoBehaviour monoBehaviour, object gameData)
        {
            var saveId = ServiceLocator.Instance.Get<ConfigController>().Config.SaveId;
            var saveData = CreateSaveData(saveId, gameData);
            var ioController = ServiceLocator.Instance.Get<IOController>();
            monoBehaviour.StartCoroutine(ioController.SavePng(Path.Combine(SaveDirectoryName, saveId.ToString()), SaveImageFileName));
            await ioController.WriteJson(saveData, Path.Combine(SaveDirectoryName, saveId.ToString()), SaveDataFileName);
            EventBus<FileSaveEvent>.Raise(new FileSaveEvent());
        }
        
        public SaveData<T>? Load<T>()
        {
            var saveId = ServiceLocator.Instance.Get<ConfigController>().Config.SaveId;
            var ioController = ServiceLocator.Instance.Get<IOController>();
            return ioController.ReadJson<SaveData<T>>(Path.Combine(SaveDirectoryName, saveId.ToString()), SaveDataFileName);
        }
        
        private static SaveData<T> CreateSaveData<T>(int saveId, T gameData)
        {
            var saveData = new SaveData<T>();
            saveData.SaveID = saveId;
            saveData.AppName = Application.productName;
            saveData.AppVersion = Application.version;
            saveData.SaveTime = DateTime.Now;
            saveData.Data = gameData;
            return saveData;
        }
    }
}