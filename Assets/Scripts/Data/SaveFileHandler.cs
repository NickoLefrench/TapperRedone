using System.IO;

using UnityEngine;

namespace FMS.TapperRedone.Data
{
    public interface ISaveFileHandler
    {
        bool SaveFile(string filename, string content);
        public bool LoadFile(string filename, out string data);
        bool DeleteFile(string filename);
        bool FileExists(string filename);
    }

    // A synchronous-time filesystem save handler
    public class PersistentSaveFileHandler : ISaveFileHandler
    {
        private readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, "Saves");

        public PersistentSaveFileHandler()
        {
            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
            }
        }

        private string GetFullPath(string filename) => Path.Combine(SAVE_PATH, filename);

        public bool LoadFile(string filename, out string data)
        {
            bool fileExists = FileExists(filename);
            data = fileExists ? File.ReadAllText(GetFullPath(filename)) : default;
            return fileExists;
        }

        public bool SaveFile(string filename, string content)
        {
            File.WriteAllText(GetFullPath(filename), content);
            return FileExists(filename);
        }

        public bool DeleteFile(string filename)
        {
            if (!FileExists(filename))
            {
                return true;
            }

            File.Delete(GetFullPath(filename));
            return !FileExists(filename);
        }

        public bool FileExists(string filename)
        {
            return File.Exists(GetFullPath(filename));
        }
    }
}
