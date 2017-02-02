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
        public const string GameFolderName = "SG_Game";
        public const string GameConfigFileName = "SG_Game.cfg";
        #endregion

        #region Felder
        private static ConfigManager current;

        private string gameDirectoryPath;

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
        }
        #endregion

        #region Implementierungen
        public void LoadGameConfig()
        {
            if (!Directory.Exists(this.GameDirectoryPath))
            {
                FileManager.CreateDirectory(this.GameDirectoryPath);
            }

            string configPath = Path.Combine(this.GameDirectoryPath, GameConfigFileName);

            if (!File.Exists(configPath))
            {
                CreateGameConfig(configPath);
            }
            else
            {
                string content = FileManager.Read(configPath);
                this.GameConfig = JsonUtility.FromJson<GameConfig>(content);
            }
        }

        private void CreateGameConfig(string path)
        {
            FileManager.Create(path);
        }
        #endregion
    }
}
