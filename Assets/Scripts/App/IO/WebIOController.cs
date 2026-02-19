using System.Threading.Tasks;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace App.IO
{
    public class WebIOController : IOController
    {
        private IOController _ioControllerImplementation;
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void DownloadFile(string fileName, byte[] data, int dataLength);
#endif

        public string SaveFile(byte[] data, string fileName, string fileExtension, string relativePath)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            DownloadFile($"{fileName}.{fileExtension}", data, data.Length);
#endif
            return $"{fileName}.{fileExtension}";
        }

        public Texture2D LoadJpg(string relativePath, string fileName)
        {
            return null;
        }

        public string GetFileSaveTime(string relativePath, string fileName)
        {
            return null;
        }

        public void DeleteDirectory(string relativePath)
        {
            // NOOP
        }

        public T? ReadJson<T>(string relativePath, string fileName) where T : struct
        {
            return null;
        }

        public Task<bool> WriteJson<T>(T obj, string relativePath, string fileName)
        {
            return Task.FromResult(false);
        }
    }
}