using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace App
{
    public class IOController
    {
        private readonly string _appDataPath = Application.persistentDataPath;

        public void DeleteDirectory(string relativePath)
        {
            if (!DoesRelativeDirectoryExist(relativePath)) return;
            Directory.Delete(Path.Combine(_appDataPath, relativePath), true);
        }

        public async Task<bool> WriteJson<T>(T obj, string relativePath, string fileName)
        {
            CreateDirectory(relativePath);

            if (DoesFileExist(relativePath, fileName)) DeleteFile(relativePath, fileName);

            var serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
            await File.WriteAllTextAsync(Path.Combine(_appDataPath, relativePath, fileName + ".json"),
                serializedObject);

            return true;
        }
#nullable enable
        public T? ReadJson<T>(string relativePath, string fileName) where T : struct
        {
            if (!DoesFileExist(relativePath, fileName + ".json")) return null;

            var json = File.ReadAllText(Path.Combine(_appDataPath, relativePath, fileName + ".json"));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string GetFileSaveTime(string relativePath, string fileName)
        {
            if (!DoesFileExist(relativePath, fileName + ".json")) return string.Empty;

            var fileInfo = new FileInfo(Path.Combine(_appDataPath, relativePath, fileName));
            return fileInfo.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture);
        }

        public Texture2D LoadJpg(string relativePath, string fileName)
        {
            var tex = new Texture2D(2, 2);
            if (!DoesFileExist(relativePath, fileName + ".jpg")) return tex;

            var bytes = File.ReadAllBytes(Path.Combine(_appDataPath, relativePath, fileName + ".jpg"));
            tex.LoadImage(bytes);
            return tex;
        }

        public string SaveImage(byte[] imageData, string relativePath, string fileName, string fileExtension)
        {
            CreateDirectory(relativePath);
            if (DoesFileExist(relativePath, $"{fileName}.{fileExtension}"))
            {
                fileName = GetUniqueFilename(Path.Combine(_appDataPath, relativePath), fileName, fileExtension);
            }

            var fullFileName = Path.Combine(Path.Combine(_appDataPath, relativePath, $"{fileName}.{fileExtension}"));
            File.WriteAllBytes(fullFileName, imageData);
            return fullFileName;
        }

        private bool DoesRelativeDirectoryExist(string relativePath)
        {
            return Directory.Exists(Path.Combine(_appDataPath, relativePath));
        }

        private void CreateDirectory(string relativePath)
        {
            if (DoesRelativeDirectoryExist(relativePath)) return;
            var absolutePath = Path.Combine(_appDataPath, relativePath);
            Directory.CreateDirectory(absolutePath);
        }

        private void DeleteFile(string relativePath, string fileName)
        {
            Assert.IsTrue(DoesRelativeDirectoryExist(relativePath));
            Assert.IsTrue(DoesFileExist(relativePath, fileName));
            File.Delete(Path.Combine(_appDataPath, relativePath, fileName));
        }

        private bool DoesFileExist(string relativePath, string fileName)
        {
            return File.Exists(Path.Combine(_appDataPath, relativePath, fileName));
        }

        private string GetUniqueFilename(string folderPath, string fileName, string extension)
        {
            var fullPath = Path.Combine(folderPath, $"{fileName}.{extension}");
            var uniqueFileName = $"{fileName}";
            var counter = 1;

            while (File.Exists(fullPath))
            {
                var newFileName = $"{fileName}_{counter}";
                fullPath = Path.Combine(folderPath, $"{newFileName}.{extension}");
                uniqueFileName = $"{fileName}_{counter}";
                counter++;
            }

            return uniqueFileName;
        }
    }
}