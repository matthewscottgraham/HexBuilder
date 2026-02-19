using System.Threading.Tasks;
using UnityEngine;

namespace App.IO
{
    public interface IOController
    {
        public abstract string SaveFile(byte[] data, string fileName, string fileExtension, string relativePath);
        public abstract Texture2D LoadJpg(string relativePath, string fileName);
        public abstract string GetFileSaveTime(string relativePath, string fileName);
        public abstract void DeleteDirectory(string relativePath);
        public abstract T? ReadJson<T>(string relativePath, string fileName) where T : struct;
        public abstract Task<bool> WriteJson<T>(T obj, string relativePath, string fileName);
    }
}