#nullable enable
using System;

namespace App.SaveData
{
    public struct SaveData<T>
    {
        public int SaveID;
        public string AppName;
        public string AppVersion;
        public DateTime SaveTime;
        public T Data;
    }
}