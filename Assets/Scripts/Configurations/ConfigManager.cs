using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileIO;
using UnityEngine;

namespace Configurations
{
    public class ConfigManager
    {
        #region Konstanten
        public const string GameFolderName = "BeFIT";
        public const string GameConfigFileName = "BeFIT.cfg";
        #endregion

        #region Felder
        private static ConfigManager current;

        private string gameDirectoryPath;

        private string gameConfigPath;

        private GameConfig gameConfig;

        #endregion

        #region Eigenschaften
        public static ConfigManager Current
        {
            get
            {
                if (current == null)
                {
                    current = new ConfigManager();
                }
                return current;
            }
        }

        public string GameDirectoryPath
        {
            get
            {
                return this.gameDirectoryPath;
            }
            private set
            {
                if (value != this.gameDirectoryPath)
                {
                    this.gameDirectoryPath = value;
                }
            }
        }

        public string GameConfigPath
        {
            get
            {
                return this.gameConfigPath;
            }
            private set
            {
                if (value != this.gameConfigPath)
                {
                    this.gameConfigPath = value;
                }
            }
        }

        public GameConfig GameConfig
        {
            get
            {
                return this.gameConfig;
            }
            private set
            {
                if (value != this.gameConfig)
                {
                    this.gameConfig = value;
                }
            }
        }
        #endregion

        #region Konstruktor
        private ConfigManager()
        {
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.GameDirectoryPath = Path.Combine(myDocuments, GameFolderName);
            this.GameConfigPath = Path.Combine(GameDirectoryPath, GameConfigFileName);
        }
        #endregion

        #region Implementierungen
        public void LoadGameConfig()
        {
            if (!Directory.Exists(this.GameDirectoryPath))
            {
                FileManager.CreateDirectory(this.GameDirectoryPath);
            }

            if (!File.Exists(this.GameConfigPath))
            {
                CreateGameConfig(this.GameConfigPath);
                this.GameConfig = new GameConfig();
            }
            else
            {
                string content = FileManager.Read(this.GameConfigPath);
                this.GameConfig = JsonUtility.FromJson<GameConfig>(content);
            }
        }

        private void CreateGameConfig(string path)
        {
            FileManager.Create(path);
        }

        public void SaveGameConfig()
        {
            string content = JsonUtility.ToJson(this.GameConfig);
            FileManager.Write(this.GameConfigPath, content);
        }
        #endregion
    }
}
