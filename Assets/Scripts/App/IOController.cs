using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace App
{
    public class IOController
    {
        private readonly string _appDataPath = Application.persistentDataPath;
        
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
            if (!DoesFileExist(relativePath, fileName + ".json")) return null;
            
            var json = File.ReadAllText(Path.Combine(_appDataPath, relativePath, fileName + ".json"));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string GetFileSaveTime(string relativePath, string fileName)
        {
            if (!DoesFileExist(relativePath, fileName + ".json")) return string.Empty;
            
            var fileInfo = new FileInfo(Path.Combine(_appDataPath, relativePath, fileName));
            return fileInfo.LastWriteTimeUtc.ToString();
        }

        public Texture2D LoadPng(string relativePath, string fileName)
        {
            if (!DoesFileExist(relativePath, fileName + ".png")) return null;
            
            byte[] bytes = File.ReadAllBytes(Path.Combine(_appDataPath, relativePath, fileName + ".png"));
            var tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return tex;
        }

        public IEnumerator SavePng(string relativePath, string fileName)
        {
            if (DoesFileExist(relativePath, fileName + ".png"))
            {
                DeleteFile(relativePath, fileName + ".png");
            }

            var fullFileName = Path.Combine(Path.Combine(_appDataPath, relativePath, fileName + ".png"));

            yield return new WaitForEndOfFrame();
            
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
            var tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            
            tex = ResizeTexture(tex, tex.width / 4, tex.height / 4);
            
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(fullFileName, bytes);
        }
        
        private static Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            RenderTexture.active = rt;

            // Copy texture to RT (GPU scaling)
            Graphics.Blit(source, rt);

            // Read RT back into a new Texture2D
            Texture2D result = new Texture2D(width, height, source.format, false);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return result;
        }
    }
}
