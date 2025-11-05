using System;
using App.Events;
using App.Services;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace App
{
    public class IOController : IDisposable
    {
        private readonly string _appDataPath = Application.persistentDataPath;

        public IOController()
        {
            ServiceLocator.Instance.Register(new EventBus<FileSaveEvent>());
            ServiceLocator.Instance.Register(new EventBus<FileLoadEvent>());
        }

        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(typeof(EventBus<FileSaveEvent>));
            ServiceLocator.Instance.Deregister(typeof(EventBus<FileLoadEvent>));
        }
        
        public bool DoesFileExist(string relativePath, string fileName)
        {
            return File.Exists(Path.Combine(_appDataPath, relativePath, fileName));
        }

        public bool DoesRelativeDirectoryExist(string relativePath)
        {
            return Directory.Exists(Path.Combine(_appDataPath, relativePath));
        }

        public void DeleteFile(string relativePath, string fileName)
        {
            Assert.IsTrue(DoesRelativeDirectoryExist(relativePath));
            Assert.IsTrue(DoesFileExist(relativePath, fileName));
            File.Delete(Path.Combine(_appDataPath, relativePath, fileName));
        }

        public void DeleteDirectory(string relativePath)
        {
            Assert.IsTrue(DoesRelativeDirectoryExist(relativePath));
            Directory.Delete(Path.Combine(_appDataPath, relativePath), true);
        }

        public void CreateDirectory(string relativePath)
        {
            if (DoesRelativeDirectoryExist(relativePath)) return;
            var absolutePath = Path.Combine(_appDataPath, relativePath);
            Directory.CreateDirectory(absolutePath);
        }
        
        public async Task<bool> WriteJson<T>(T obj, string relativePath, string fileName)
        {
            CreateDirectory(relativePath);
            
            if (DoesFileExist(relativePath, fileName))
            {
                DeleteFile(relativePath, fileName);
            }
            
            var serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
            await File.WriteAllTextAsync(Path.Combine(_appDataPath, relativePath, fileName + ".json"), serializedObject);
            
            return true;
        }
        
        #nullable enable
        public T? ReadJson<T>(string relativePath, string fileName) where T : struct
        {
            if (!DoesFileExist(relativePath, fileName))
            {
                return null;
            }
            
            var json = File.ReadAllText(Path.Combine(_appDataPath, relativePath, fileName + ".json"));
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
