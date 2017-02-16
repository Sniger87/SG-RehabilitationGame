using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configurations;
using FileIO;
using UnityEngine;

namespace Profiles
{
    public class ProfileManager
    {
        #region Konstanten
        private const string PlayerFolderPrefix = "Player";
        private const string BalanceBoardMovementsFileName = "BalanceBoardMovements";
        private const string PlayerPositionsFileName = "PlayerPositions";
        private const string LevelPositionsFileName = "LevelPositions";
        #endregion

        #region Felder
        private static ProfileManager current;

        private List<Player> players;
        private Player currentPlayer;
        #endregion

        #region Eigenschaften
        public static ProfileManager Current
        {
            get
            {
                if (current == null)
                {
                    current = new ProfileManager();
                }
                return current;
            }
        }

        public List<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                if (value != players)
                {
                    players = value;
                }
            }
        }

        public Player CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }
            set
            {
                if (value != currentPlayer)
                {
                    currentPlayer = value;
                }
            }
        }
        #endregion

        #region Konstruktor
        private ProfileManager()
        {
            this.Players = new List<Player>();
        }
        #endregion

        #region Implementierungen
        public Player CreatePlayer(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name, "Name des Spielers darf nicht leer sein.");
            }

            int playerId = this.Players.Count;
            string playerFolderName = string.Format("{0}_{1}", PlayerFolderPrefix, playerId);

            string directoryPath = Path.Combine(ConfigManager.Current.GameDirectoryPath, playerFolderName);

            FileManager.CreateDirectory(directoryPath);

            string playerConfigName = string.Format("{0}_{1}.cfg", PlayerFolderPrefix, playerId);
            string configFilePath = Path.Combine(directoryPath, playerConfigName);

            FileManager.Create(configFilePath);

            Player p = new Player(playerId, name, directoryPath, configFilePath);

            this.Players.Add(p);

            ConfigManager.Current.GameConfig.PlayersConfigPath.Add(configFilePath);

            // Save new Player
            SavePlayers();
            ConfigManager.Current.SaveGameConfig();

            return p;
        }

        public void LoadPlayers()
        {
            this.Players.Clear();

            foreach (string playerConfigPath in ConfigManager.Current.GameConfig.PlayersConfigPath)
            {
                string content = FileManager.Read(playerConfigPath);
                Player p = JsonUtility.FromJson<Player>(content);
                this.Players.Add(p);
            }
        }

        public bool DeletePlayer(Player player)
        {
            if (this.Players.Contains(player))
            {
                FileManager.DeleteDirectory(player.DirectoryPath, true);
                int index = ConfigManager.Current.GameConfig.PlayersConfigPath.IndexOf(player.ConfigFilePath);
                if (index >= 0 && index < ConfigManager.Current.GameConfig.PlayersConfigPath.Count)
                {
                    ConfigManager.Current.GameConfig.PlayersConfigPath.RemoveAt(index);
                    ConfigManager.Current.SaveGameConfig();
                }
                return this.Players.Remove(player);
            }
            return false;
        }

        private void CreateCurrentGameFolder()
        {
            if (this.CurrentPlayer != null)
            {
                string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                string path = Path.Combine(this.CurrentPlayer.DirectoryPath, timeStamp);
                if (FileManager.CreateDirectory(path))
                {
                    this.CurrentPlayer.CurrentGameFolderPath = path;
                }
            }
        }

        private void CreateMovementFile()
        {
            if (this.CurrentPlayer != null)
            {
                string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                string fileName = string.Format("{0}_{1}.csv", BalanceBoardMovementsFileName, timeStamp);
                string filePath = Path.Combine(this.CurrentPlayer.CurrentGameFolderPath, fileName);
                if (FileManager.Create(filePath))
                {
                    this.CurrentPlayer.CurrentMovementFilePath = filePath;
                }
            }
        }

        public void WriteMovementFile(string content)
        {
            if (this.CurrentPlayer != null && !string.IsNullOrEmpty(this.CurrentPlayer.CurrentMovementFilePath))
            {
                FileManager.Append(this.CurrentPlayer.CurrentMovementFilePath, content);
            }
        }

        private void CreatePlayerPositionFile()
        {
            if (this.CurrentPlayer != null)
            {
                string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                string fileName = string.Format("{0}_{1}.csv", PlayerPositionsFileName, timeStamp);
                string filePath = Path.Combine(this.CurrentPlayer.CurrentGameFolderPath, fileName);
                if (FileManager.Create(filePath))
                {
                    this.CurrentPlayer.CurrentPlayerPositionFilePath = filePath;
                }
            }
        }

        public void WritePlayerPositionFile(string content)
        {
            if (this.CurrentPlayer != null && !string.IsNullOrEmpty(this.CurrentPlayer.CurrentPlayerPositionFilePath))
            {
                FileManager.Append(this.CurrentPlayer.CurrentPlayerPositionFilePath, content);
            }
        }

        private void CreateLevelPositionFile()
        {
            if (this.CurrentPlayer != null)
            {
                string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                string fileName = string.Format("{0}_{1}.csv", LevelPositionsFileName, timeStamp);
                string filePath = Path.Combine(this.CurrentPlayer.CurrentGameFolderPath, fileName);
                if (FileManager.Create(filePath))
                {
                    this.CurrentPlayer.CurrentLevelPostionFilePath = filePath;
                }
            }
        }

        public void WriteLevelPositionFile(string content)
        {
            if (this.CurrentPlayer != null && !string.IsNullOrEmpty(this.CurrentPlayer.CurrentLevelPostionFilePath))
            {
                FileManager.Append(this.CurrentPlayer.CurrentLevelPostionFilePath, content);
            }
        }

        public void CreateFilesForGame()
        {
            CreateCurrentGameFolder();
            CreateMovementFile();
            CreatePlayerPositionFile();
            CreateLevelPositionFile();
        }

        public void SavePlayers()
        {
            foreach (Player player in this.Players)
            {
                string content = JsonUtility.ToJson(player);
                FileManager.Write(player.ConfigFilePath, content);
            }
        }
        #endregion
    }
}
