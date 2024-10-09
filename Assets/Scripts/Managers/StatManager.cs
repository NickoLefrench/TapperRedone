using System;

using FMS.TapperRedone.Data;

using UnityEngine;

namespace FMS.TapperRedone.Managers
{
    public class StatManager : MonoBehaviour
    {
        public static event Action<int> OnScoreChanged;

        public SavedData savedData { get; private set; } = new();

        private const string SAVED_DATA_FILENAME = "save.json";

        private ISaveFileHandler _saveFileHandler;

        public int Score
        {
            get
            {
                return savedData.CurrentScore;
            }
            set
            {
                savedData.CurrentScore = value;
                OnScoreChanged?.Invoke(Score);
            }
        }

        public int CurrentNight
        {
            get
            {
                return savedData.RunNight;
            }
            set
            {
                savedData.RunNight = value;
            }
        }

        public void RequestStatSave()
        {
            SavePersistentStats(_saveFileHandler);
        }

        private void Start()
        {
            // Set up file handler
            _saveFileHandler = new PersistentSaveFileHandler();

            // Reload any stat values from deep storage
            LoadPersistentStats(_saveFileHandler);
        }

        private void LoadPersistentStats(ISaveFileHandler saveFileHandler)
        {
            if (!saveFileHandler.FileExists(SAVED_DATA_FILENAME))
                return;

            if (!saveFileHandler.LoadFile(SAVED_DATA_FILENAME, out string jsonified))
                return;

            SavedData parsedStats = JsonUtility.FromJson<SavedData>(jsonified);
            if (parsedStats == null)
                return;

            savedData = parsedStats;
        }

        private void SavePersistentStats(ISaveFileHandler saveFileHandler)
        {
            saveFileHandler.SaveFile(SAVED_DATA_FILENAME, JsonUtility.ToJson(savedData));
        }
    }
}
