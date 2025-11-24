#nullable enable
using System;
using System.IO;
using App.Config;
using App.Events;
using App.Services;
using UnityEngine;

namespace App.SaveData
{
    public class SaveDataController : FileDataController
    {
        private const string SaveImageFileName = "gameScreenshot";

        public (Texture2D, string) GetMetaData(int saveID)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            var tex = ioController.LoadPng(Path.Combine(SaveDirectoryName, saveID.ToString()), SaveImageFileName);
            var saveTime =
                ioController.GetFileSaveTime(Path.Combine(SaveDirectoryName, saveID.ToString()), SaveDataFileName);
            return (tex, saveTime);
        }

        public void DeleteSaveData(int saveId)
        {
            Delete(Path.Combine(SaveDirectoryName, saveId.ToString()));
        }

        public void SaveWithScreenshot(MonoBehaviour monoBehaviour, object gameData)
        {
            var ioController = ServiceLocator.Instance.Get<IOController>();
            var relativeSavePath = Path.Combine(SaveDirectoryName, ConfigController.CurrentSaveSlot.ToString());
            monoBehaviour.StartCoroutine(ioController.SavePng(relativeSavePath, SaveImageFileName));
            
            Save(relativeSavePath, gameData, ConfigController.CurrentSaveSlot);
            
        }

        public SaveData<T>? LoadSaveSlot<T>(int saveId)
        {
            var relativePath = Path.Combine(SaveDirectoryName, saveId.ToString());
            return Load<T>(relativePath);
        }
    }
}